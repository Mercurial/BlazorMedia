interface BlazorMediaVideoElement extends HTMLVideoElement {
    mediaRecorder: MediaRecorder;
    mediaStream: MediaStream;
    fpsIntervalId: number;
    lastFPSTime: number;
    lastFPS: number;
    bmWidth: number;
    bmHeight: number;
}
namespace BlazorMedia {
    export class BlazorMediaInterop {

        /// Defaults
        private static constraints = {
            audio: {
                deviceId: ""
            },
            video: {
                width: {
                    ideal: 640
                },
                height: {
                    ideal: 480
                },
                frameRate: {
                    ideal: 60
                },
                deviceId: ""
            },

        }


        static async Initialize(width: number = 640, height: number = 480, frameRate = 60, canCaptureAudio: boolean = true, cameraDeviceId: string = "", microphoneDeviceId: string = "", timeslice: number = 0, videoElement: BlazorMediaVideoElement, componentRef: any) {
            videoElement.bmWidth = width;
            videoElement.bmHeight = height;
            BlazorMediaInterop.constraints = {
                audio: {
                    deviceId: microphoneDeviceId,
                },
                video: {
                    width: {
                        ideal: width
                    },
                    height: {
                        ideal: height
                    },
                    frameRate: {
                        ideal: frameRate
                    },
                    deviceId: cameraDeviceId,
                }
            };

            if (canCaptureAudio == false) {
                BlazorMediaInterop.constraints.audio = false as any;
            }

            BlazorMediaInterop.Destroy(videoElement);

            try {
                videoElement.mediaStream = await navigator.mediaDevices.getUserMedia(BlazorMediaInterop.constraints);
                videoElement.srcObject = videoElement.mediaStream;
                videoElement.mediaRecorder = new MediaRecorder(videoElement.mediaStream);
                videoElement.volume = 0;
                videoElement.mediaRecorder.ondataavailable = async (e) => {
                    let uintArr = new Uint8Array(await new Response(e.data).arrayBuffer());
                    let buffer = Array.from(uintArr);
                    componentRef.invokeMethodAsync("ReceiveData", buffer);
                };

                videoElement.mediaRecorder.onerror = async (e: MediaRecorderErrorEvent) => {
                    let mediaError = { Type: 1, Message: "" }
                    componentRef.invokeMethodAsync("ReceiveError", mediaError);
                };
                videoElement.mediaRecorder.onstart = () => {
                    componentRef.invokeMethodAsync("ReceiveStart", videoElement.videoWidth, videoElement.videoHeight);
                };
                videoElement.mediaRecorder.start(timeslice);
                BlazorMediaInterop.DetectMediaDeviceUsedDisconnection(videoElement, componentRef);
            }
            catch (exception) {
                let mediaError = { Type: 0, Message: exception.message }
                componentRef.invokeMethodAsync("ReceiveError", mediaError);

            }
        }

        static async Destroy(videoElement: BlazorMediaVideoElement) {
            if (videoElement && videoElement.mediaRecorder && videoElement.mediaRecorder.state != 'inactive') {
                videoElement.mediaRecorder.stop();
            }
            if (videoElement.mediaStream) {
                let stream = videoElement.mediaStream;
                let tracks = stream.getTracks();
                let track: MediaStreamTrack | undefined;
                while (track = tracks.pop()) {
                    track.stop();
                    stream.removeTrack(track);
                }
            }
            BlazorMediaInterop.RemoveBlazorFPSListener(videoElement);
        }

        static async StartBlazorDeviceListener(componentRef: any) {
            navigator.mediaDevices.ondevicechange = async (e) => {
                let newDevices = await navigator.mediaDevices.enumerateDevices();
                componentRef.invokeMethodAsync("OnDeviceChange", newDevices);
            }
        }

        static async StopBlazorDeviceListener(componentRef: any) {
            navigator.mediaDevices.ondevicechange = null;
        }

        static async AddBlazorFPSListener(videoElement: BlazorMediaVideoElement, componentRef: any) {
            videoElement.lastFPS = 0;
            // FPS Counter
            videoElement.fpsIntervalId = setInterval(() => {
                if (videoElement) {
                    let frameRate = videoElement.getVideoPlaybackQuality().totalVideoFrames - videoElement.lastFPS;
                    videoElement.lastFPS = videoElement.getVideoPlaybackQuality().totalVideoFrames;
                    componentRef.invokeMethodAsync("ReceiveFPS", frameRate);
                }
            }, 1000);
        }

        static async RemoveBlazorFPSListener(videoElement: BlazorMediaVideoElement) {
            if (videoElement && videoElement.fpsIntervalId)
                clearInterval(videoElement.fpsIntervalId);
        }

        static async CaptureImage(videoElement: BlazorMediaVideoElement) {
            let canvas = document.createElement("canvas") as HTMLCanvasElement;
            let context = canvas.getContext("2d") as CanvasRenderingContext2D;

            canvas.width = videoElement.bmWidth;
            canvas.height = videoElement.bmHeight;
            context.drawImage(videoElement, 0, 0, videoElement.bmWidth, videoElement.bmHeight);
            return canvas.toDataURL('image/png');
        }

        static async DetectMediaDeviceUsedDisconnection(videoElement: BlazorMediaVideoElement, componentRef: any) {
            if (videoElement.mediaStream) {
                let stream = videoElement.mediaStream;
                let tracks = stream.getTracks();
                for (let x = 0; x < tracks.length; x++) {
                    const track = tracks[x];
                    track.onended = async (ev: Event) => {
                        let devices = await navigator.mediaDevices.enumerateDevices();
                        var videoIsStillConnected = false;
                        var audioIsStillConnected = false;
                        for (let y = 0; y < devices.length; y++) {
                            const device = devices[y];
                            if (device.deviceId == this.constraints.video.deviceId)
                                videoIsStillConnected = true;
                            if (device.deviceId == this.constraints.audio.deviceId)
                                audioIsStillConnected = true;
                            if (videoIsStillConnected && audioIsStillConnected)
                                break;
                        }
                        let mediaError = { Type: 2, Message: "Audio Device used is disconnected." }
                        if (!videoIsStillConnected)
                            mediaError.Message = "Video Device used is disconnected.";
                        if (!videoIsStillConnected && !audioIsStillConnected)
                            mediaError.Message = "Audio and Video Device used is disconnected.";
                        if (!audioIsStillConnected || !videoIsStillConnected) {
                            componentRef.invokeMethodAsync("ReceiveError", mediaError);
                        }
                        stream.removeTrack(track);
                    };
                }
            }
        }
    }
}

interface BlazorMediaVideoElement extends HTMLVideoElement {
    mediaRecorder: MediaRecorder;
    mediaStream: MediaStream;
    fpsIntervalId: number;
    lastFPSTime: number;
    lastFPS: number;
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
                    var mediaError = { Type: 1, Message: "" }
                    componentRef.invokeMethodAsync("ReceiveError", mediaError);
                };
                videoElement.mediaRecorder.start(timeslice);
            }
            catch (exception) {
                var mediaError = { Type: 0, Message: exception.message }
                componentRef.invokeMethodAsync("ReceiveError", mediaError);

            }
        }

        static async Destroy(videoElement: BlazorMediaVideoElement) {
            if (videoElement.mediaStream) {
                let stream = videoElement.mediaStream;
                let tracks = stream.getTracks();
                let track: MediaStreamTrack | undefined;
                while (track = tracks.pop()) {
                    track.stop();
                    stream.removeTrack(track);
                }
            }
            if (videoElement && videoElement.mediaRecorder) {
                videoElement.mediaRecorder.stop();
            }
        }

        static async StartBlazorDeviceListener(componentRef: any) {
            navigator.mediaDevices.ondevicechange = async (e) => {
                var newDevices = await navigator.mediaDevices.enumerateDevices();
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
                let frameRate = videoElement.getVideoPlaybackQuality().totalVideoFrames - videoElement.lastFPS;
                videoElement.lastFPS = videoElement.getVideoPlaybackQuality().totalVideoFrames;
                componentRef.invokeMethodAsync("ReceiveFPS", frameRate);
            }, 1000);
        }

        static async RemoveBlazorFPSListener(videoElement: BlazorMediaVideoElement) {
            if (videoElement.fpsIntervalId)
                clearInterval(videoElement.fpsIntervalId);
        }
    }
}

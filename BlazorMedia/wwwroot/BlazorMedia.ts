// This file is to show how a library package may provide JavaScript interop features
// wrapped in a .NET API

interface BlazorMediaVideoElement extends HTMLVideoElement {
    mediaRecorder: MediaRecorder;
    mediaStream: MediaStream;
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
                deviceId: ""
            },

        }


        static async InitializeMediaStream(width: number = 640, height: number = 480, canCaptureAudio: boolean = true, cameraDeviceId: string = "", microphoneDeviceId: string = "", timeslice: number = 0, videoElement: BlazorMediaVideoElement, componentRef: any) {

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
                    deviceId: cameraDeviceId,
                }
            };

            if (canCaptureAudio == false) {
                BlazorMediaInterop.constraints.audio = false as any;
            }

            BlazorMediaInterop.UninitializeMediaStream(videoElement);

            try {
                videoElement.mediaStream = await navigator.mediaDevices.getUserMedia(BlazorMediaInterop.constraints);
            }
            catch (exception) {
                var mediaError = { Type: 1, Message: exception.message }
                componentRef.invokeMethodAsync("ReceiveError", mediaError);
                throw exception;
            }
            videoElement.srcObject = videoElement.mediaStream;
            videoElement.mediaRecorder = new MediaRecorder(videoElement.mediaStream);
            videoElement.volume = 0;
            videoElement.mediaRecorder.ondataavailable = async (e) => {
                let uintArr = new Uint8Array(await new Response(e.data).arrayBuffer());
                let buffer = Array.from(uintArr);
                componentRef.invokeMethodAsync("ReceiveData", buffer);
            };

            videoElement.mediaRecorder.onerror = async (e: MediaRecorderErrorEvent) => {
                var mediaError = { Type: 1, Message: e.error.message }
                componentRef.invokeMethodAsync("ReceiveError", mediaError);
            };

            videoElement.mediaRecorder.start(timeslice);
        }

        static async UninitializeMediaStream(videoElement: BlazorMediaVideoElement) {
            if (videoElement.mediaStream) {
                let stream = videoElement.mediaStream;
                let tracks = stream.getTracks();
                let track: MediaStreamTrack | undefined;
                while (track = tracks.pop()) {
                    track.stop();
                    stream.removeTrack(track);
                }
            }
        }

        static async DeviceChange(componentRef: any) {
            navigator.mediaDevices.ondevicechange = async (e) => {
                var newDevices = await navigator.mediaDevices.enumerateDevices();

                componentRef.invokeMethodAsync("OnDeviceChange", newDevices);
            }
        }

        static async DisposeVideoElement(videoElement: BlazorMediaVideoElement) {
            if (videoElement && videoElement.mediaRecorder) {
                videoElement.mediaRecorder.stop();
            }
        }

        static async SetVideoRecorderTimeslice(videoElement: BlazorMediaVideoElement, timeslice: number = 0) {
            if (videoElement && videoElement.mediaRecorder) {
                videoElement.mediaRecorder.stop();
                videoElement.mediaRecorder.start(timeslice);
            }
        }
    }
}

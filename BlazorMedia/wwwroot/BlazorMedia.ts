// This file is to show how a library package may provide JavaScript interop features
// wrapped in a .NET API

interface BlazorMediaVideoElement extends HTMLVideoElement {
    mediaRecorder: MediaRecorder;
}

namespace BlazorMedia {
    export class BlazorMediaInterop {

        /// Defaults
        private static constraints = {
            audio: true,
            video: {
                width: {
                    min: 640,
                    ideal: 640,
                    max: 640
                },
                height: {
                    min: 480,
                    ideal: 480,
                    max: 480
                },
            }
        }

        private static MediaStream: MediaStream;

        static async InitializeMediaStream(width: number = 640, height: number = 480, canCaptureAudio: boolean = true) {

            BlazorMediaInterop.constraints = {
                audio: canCaptureAudio,
                video: {
                    width: {
                        min: width,
                        ideal: width,
                        max: width
                    },
                    height: {
                        min: height,
                        ideal: height,
                        max: height
                    },
                }
            };

            BlazorMediaInterop.UninitializeMediaStream();
            BlazorMediaInterop.MediaStream = await navigator.mediaDevices.getUserMedia(BlazorMediaInterop.constraints);
        }

        static async UninitializeMediaStream() {
            if (BlazorMediaInterop.MediaStream) {
                let tracks = BlazorMediaInterop.MediaStream.getTracks();
                let track: MediaStreamTrack | undefined;
                while (track = tracks.pop()) {
                    BlazorMediaInterop.MediaStream.removeTrack(track);
                }
            }
        }

        static async InitializeVideoElement(videoElement: BlazorMediaVideoElement, componentRef: any, timeslice: number = 0) {
            if (!BlazorMediaInterop.MediaStream) throw "MediaStream is not Initialized, please call InitializeMediaStream first.";

            videoElement.srcObject = BlazorMediaInterop.MediaStream;
            videoElement.muted = true;
            videoElement.volume = 0;
            videoElement.mediaRecorder = new MediaRecorder(BlazorMediaInterop.MediaStream);

            videoElement.mediaRecorder.ondataavailable = async (e) => {
                let uintArr = new Uint8Array(await new Response(e.data).arrayBuffer());
                let buffer = Array.from(uintArr);
                componentRef.invokeMethodAsync("ReceiveDataAsync", buffer);
            };
            videoElement.mediaRecorder.start(timeslice);
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

// This file is to show how a library package may provide JavaScript interop features
// wrapped in a .NET API

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
            if (BlazorMediaInterop.MediaStream) BlazorMediaInterop.MediaStream.stop();
            BlazorMediaInterop.MediaStream = await navigator.mediaDevices.getUserMedia(BlazorMediaInterop.constraints);
        }

        static async InitializeVideoElement(videoElement: HTMLVideoElement, componentRef: any) {
            console.log("componentRef", componentRef);
            if (!BlazorMediaInterop.MediaStream) throw "MediaStream is not Initialized, please call InitializeMediaStream first.";

            videoElement.srcObject = BlazorMediaInterop.MediaStream;
            let mediaRecorder = new MediaRecorder(BlazorMediaInterop.MediaStream);

            mediaRecorder.ondataavailable = async (e) => {
                let uintArr = new Uint8Array(await new Response(e.data).arrayBuffer());
                let buffer = Array.from(uintArr);
                console.log("data to send", buffer);
                componentRef.invokeMethodAsync("ReceiveData", buffer);
            };

            mediaRecorder.start(0);
        }
    }
}

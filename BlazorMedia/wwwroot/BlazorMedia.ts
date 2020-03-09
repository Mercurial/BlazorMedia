// This file is to show how a library package may provide JavaScript interop features
// wrapped in a .NET API

interface BlazorMediaVideoElement extends HTMLVideoElement {
    mediaRecorder: MediaRecorder;
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

        private static MediaStream: MediaStream;

        static async InitializeMediaStream(width: number = 640, height: number = 480, canCaptureAudio: boolean = true, cameraDeviceId: string = "", microphoneDeviceId: string = "") {

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
            BlazorMediaInterop.UninitializeMediaStream();

            try {
                BlazorMediaInterop.MediaStream = await navigator.mediaDevices.getUserMedia(BlazorMediaInterop.constraints);
            }
            catch (exception) {
                throw exception;
            }
        }

        static async UninitializeMediaStream() {
            try {
                if (BlazorMediaInterop.MediaStream) {
                    let tracks = BlazorMediaInterop.MediaStream.getTracks();
                    let track: MediaStreamTrack | undefined;
                    while (track = tracks.pop()) {
                        track.stop();
                        BlazorMediaInterop.MediaStream.removeTrack(track);
                    }
                }
            } catch (exception) {
                throw exception;
            }
        }

        static async OnDeviceChange(currentMediaDevices: any[], componentRef: any) {
            navigator.mediaDevices.ondevicechange = async (e) => {
                var newDevices = await navigator.mediaDevices.enumerateDevices();
                var oldDevicesLabel = currentMediaDevices.map(devices => devices.label);
                var newDevicesLabel = newDevices.map(devices => devices.label);

                var removedDevices = currentMediaDevices.filter(device => newDevicesLabel.indexOf(device.label) == -1);
                var addedDevices = newDevices.filter(device => oldDevicesLabel.indexOf(device.label) == -1);

                componentRef.invokeMethodAsync("OnDeviceChange", newDevices, removedDevices, addedDevices);
                currentMediaDevices = newDevices;
            }
        }

        static async InitializeVideoElement(videoElement: BlazorMediaVideoElement, componentRef: any, timeslice: number = 0) {
            if (!BlazorMediaInterop.MediaStream) throw "MediaStream is not Initialized, please call InitializeMediaStream first.";

            videoElement.srcObject = BlazorMediaInterop.MediaStream;
            videoElement.volume = 0;
            videoElement.mediaRecorder = new MediaRecorder(BlazorMediaInterop.MediaStream);

            videoElement.mediaRecorder.ondataavailable = async (e) => {
                try {
                    let uintArr = new Uint8Array(await new Response(e.data).arrayBuffer());
                    let buffer = Array.from(uintArr);
                    componentRef.invokeMethodAsync("ReceiveData", buffer);
                } catch (exception) {
                    var bmError = { Type: 2, Message: "Media Recorder error, unable to continue media stream." }
                    componentRef.invokeMethodAsync("MediaError", bmError);
                }
            };

            videoElement.mediaRecorder.onerror = async (e: MediaRecorderErrorEvent) => {
                var bmError = { Type: 2, Message: "Media Recorder error, unable to continue media stream." }
                componentRef.invokeMethodAsync("MediaError", bmError);
            };

            videoElement.mediaRecorder.start(timeslice);
        }

        static async DisposeVideoElement(videoElement: BlazorMediaVideoElement) {
            try {
                if (videoElement && videoElement.mediaRecorder) {
                    videoElement.mediaRecorder.stop();
                }
            } catch (exception) {
                throw exception;
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

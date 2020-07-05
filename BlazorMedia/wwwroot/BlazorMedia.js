"use strict";
var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
var __generator = (this && this.__generator) || function (thisArg, body) {
    var _ = { label: 0, sent: function() { if (t[0] & 1) throw t[1]; return t[1]; }, trys: [], ops: [] }, f, y, t, g;
    return g = { next: verb(0), "throw": verb(1), "return": verb(2) }, typeof Symbol === "function" && (g[Symbol.iterator] = function() { return this; }), g;
    function verb(n) { return function (v) { return step([n, v]); }; }
    function step(op) {
        if (f) throw new TypeError("Generator is already executing.");
        while (_) try {
            if (f = 1, y && (t = op[0] & 2 ? y["return"] : op[0] ? y["throw"] || ((t = y["return"]) && t.call(y), 0) : y.next) && !(t = t.call(y, op[1])).done) return t;
            if (y = 0, t) op = [op[0] & 2, t.value];
            switch (op[0]) {
                case 0: case 1: t = op; break;
                case 4: _.label++; return { value: op[1], done: false };
                case 5: _.label++; y = op[1]; op = [0]; continue;
                case 7: op = _.ops.pop(); _.trys.pop(); continue;
                default:
                    if (!(t = _.trys, t = t.length > 0 && t[t.length - 1]) && (op[0] === 6 || op[0] === 2)) { _ = 0; continue; }
                    if (op[0] === 3 && (!t || (op[1] > t[0] && op[1] < t[3]))) { _.label = op[1]; break; }
                    if (op[0] === 6 && _.label < t[1]) { _.label = t[1]; t = op; break; }
                    if (t && _.label < t[2]) { _.label = t[2]; _.ops.push(op); break; }
                    if (t[2]) _.ops.pop();
                    _.trys.pop(); continue;
            }
            op = body.call(thisArg, _);
        } catch (e) { op = [6, e]; y = 0; } finally { f = t = 0; }
        if (op[0] & 5) throw op[1]; return { value: op[0] ? op[1] : void 0, done: true };
    }
};
var BlazorMedia;
(function (BlazorMedia) {
    var BlazorMediaInterop = /** @class */ (function () {
        function BlazorMediaInterop() {
        }
        BlazorMediaInterop.Initialize = function (width, height, frameRate, canCaptureAudio, cameraDeviceId, microphoneDeviceId, timeslice, videoElement, componentRef) {
            if (width === void 0) { width = 640; }
            if (height === void 0) { height = 480; }
            if (frameRate === void 0) { frameRate = 60; }
            if (canCaptureAudio === void 0) { canCaptureAudio = true; }
            if (cameraDeviceId === void 0) { cameraDeviceId = ""; }
            if (microphoneDeviceId === void 0) { microphoneDeviceId = ""; }
            if (timeslice === void 0) { timeslice = 0; }
            return __awaiter(this, void 0, void 0, function () {
                var _a, exception_1, mediaError;
                var _this = this;
                return __generator(this, function (_b) {
                    switch (_b.label) {
                        case 0:
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
                                BlazorMediaInterop.constraints.audio = false;
                            }
                            BlazorMediaInterop.Destroy(videoElement);
                            _b.label = 1;
                        case 1:
                            _b.trys.push([1, 3, , 4]);
                            _a = videoElement;
                            return [4 /*yield*/, navigator.mediaDevices.getUserMedia(BlazorMediaInterop.constraints)];
                        case 2:
                            _a.mediaStream = _b.sent();
                            videoElement.srcObject = videoElement.mediaStream;
                            videoElement.mediaRecorder = new MediaRecorder(videoElement.mediaStream);
                            videoElement.volume = 0;
                            videoElement.mediaRecorder.ondataavailable = function (e) { return __awaiter(_this, void 0, void 0, function () {
                                var uintArr, _a, buffer;
                                return __generator(this, function (_b) {
                                    switch (_b.label) {
                                        case 0:
                                            _a = Uint8Array.bind;
                                            return [4 /*yield*/, new Response(e.data).arrayBuffer()];
                                        case 1:
                                            uintArr = new (_a.apply(Uint8Array, [void 0, _b.sent()]))();
                                            buffer = Array.from(uintArr);
                                            componentRef.invokeMethodAsync("ReceiveData", buffer);
                                            return [2 /*return*/];
                                    }
                                });
                            }); };
                            videoElement.mediaRecorder.onerror = function (e) { return __awaiter(_this, void 0, void 0, function () {
                                var mediaError;
                                return __generator(this, function (_a) {
                                    mediaError = { Type: 1, Message: "" };
                                    componentRef.invokeMethodAsync("ReceiveError", mediaError);
                                    return [2 /*return*/];
                                });
                            }); };
                            videoElement.mediaRecorder.onstart = function () {
                                componentRef.invokeMethodAsync("ReceiveStart", videoElement.videoWidth, videoElement.videoHeight);
                            };
                            videoElement.mediaRecorder.start(timeslice);
                            BlazorMediaInterop.DetectMediaDeviceUsedDisconnection(videoElement, componentRef);
                            return [3 /*break*/, 4];
                        case 3:
                            exception_1 = _b.sent();
                            mediaError = { Type: 0, Message: exception_1.message };
                            componentRef.invokeMethodAsync("ReceiveError", mediaError);
                            return [3 /*break*/, 4];
                        case 4: return [2 /*return*/];
                    }
                });
            });
        };
        BlazorMediaInterop.Destroy = function (videoElement) {
            return __awaiter(this, void 0, void 0, function () {
                var stream, tracks, track;
                return __generator(this, function (_a) {
                    if (videoElement && videoElement.mediaRecorder && videoElement.mediaRecorder.state != 'inactive') {
                        videoElement.mediaRecorder.stop();
                    }
                    if (videoElement.mediaStream) {
                        stream = videoElement.mediaStream;
                        tracks = stream.getTracks();
                        track = void 0;
                        while (track = tracks.pop()) {
                            track.stop();
                            stream.removeTrack(track);
                        }
                    }
                    BlazorMediaInterop.RemoveBlazorFPSListener(videoElement);
                    return [2 /*return*/];
                });
            });
        };
        BlazorMediaInterop.StartBlazorDeviceListener = function (componentRef) {
            return __awaiter(this, void 0, void 0, function () {
                var _this = this;
                return __generator(this, function (_a) {
                    navigator.mediaDevices.ondevicechange = function (e) { return __awaiter(_this, void 0, void 0, function () {
                        var newDevices;
                        return __generator(this, function (_a) {
                            switch (_a.label) {
                                case 0: return [4 /*yield*/, navigator.mediaDevices.enumerateDevices()];
                                case 1:
                                    newDevices = _a.sent();
                                    componentRef.invokeMethodAsync("OnDeviceChange", newDevices);
                                    return [2 /*return*/];
                            }
                        });
                    }); };
                    return [2 /*return*/];
                });
            });
        };
        BlazorMediaInterop.StopBlazorDeviceListener = function (componentRef) {
            return __awaiter(this, void 0, void 0, function () {
                return __generator(this, function (_a) {
                    navigator.mediaDevices.ondevicechange = null;
                    return [2 /*return*/];
                });
            });
        };
        BlazorMediaInterop.AddBlazorFPSListener = function (videoElement, componentRef) {
            return __awaiter(this, void 0, void 0, function () {
                return __generator(this, function (_a) {
                    videoElement.lastFPS = 0;
                    // FPS Counter
                    videoElement.fpsIntervalId = setInterval(function () {
                        if (videoElement) {
                            var frameRate = videoElement.getVideoPlaybackQuality().totalVideoFrames - videoElement.lastFPS;
                            videoElement.lastFPS = videoElement.getVideoPlaybackQuality().totalVideoFrames;
                            componentRef.invokeMethodAsync("ReceiveFPS", frameRate);
                        }
                    }, 1000);
                    return [2 /*return*/];
                });
            });
        };
        BlazorMediaInterop.RemoveBlazorFPSListener = function (videoElement) {
            return __awaiter(this, void 0, void 0, function () {
                return __generator(this, function (_a) {
                    if (videoElement && videoElement.fpsIntervalId)
                        clearInterval(videoElement.fpsIntervalId);
                    return [2 /*return*/];
                });
            });
        };
        BlazorMediaInterop.CaptureImage = function (videoElement) {
            return __awaiter(this, void 0, void 0, function () {
                var canvas, context;
                return __generator(this, function (_a) {
                    canvas = document.createElement("canvas");
                    context = canvas.getContext("2d");
                    canvas.width = videoElement.bmWidth;
                    canvas.height = videoElement.bmHeight;
                    context.drawImage(videoElement, 0, 0, videoElement.bmWidth, videoElement.bmHeight);
                    return [2 /*return*/, canvas.toDataURL('image/png')];
                });
            });
        };
        BlazorMediaInterop.DetectMediaDeviceUsedDisconnection = function (videoElement, componentRef) {
            return __awaiter(this, void 0, void 0, function () {
                var stream, tracks, x, track;
                var _this = this;
                return __generator(this, function (_a) {
                    if (videoElement.mediaStream) {
                        stream = videoElement.mediaStream;
                        tracks = stream.getTracks();
                        for (x = 0; x < tracks.length; x++) {
                            track = tracks[x];
                            track.onended = function (ev) { return __awaiter(_this, void 0, void 0, function () {
                                var devices, videoIsStillConnected, audioIsStillConnected, y, device, mediaError;
                                return __generator(this, function (_a) {
                                    switch (_a.label) {
                                        case 0: return [4 /*yield*/, navigator.mediaDevices.enumerateDevices()];
                                        case 1:
                                            devices = _a.sent();
                                            videoIsStillConnected = false;
                                            audioIsStillConnected = false;
                                            for (y = 0; y < devices.length; y++) {
                                                device = devices[y];
                                                if (device.deviceId == this.constraints.video.deviceId)
                                                    videoIsStillConnected = true;
                                                if (device.deviceId == this.constraints.audio.deviceId)
                                                    audioIsStillConnected = true;
                                                if (videoIsStillConnected && audioIsStillConnected)
                                                    break;
                                            }
                                            mediaError = { Type: 1, Message: "Audio Device used is disconnected." };
                                            if (!videoIsStillConnected)
                                                mediaError.Message = "Video Device used is disconnected.";
                                            if (!videoIsStillConnected && !audioIsStillConnected)
                                                mediaError.Message = "Audio and Video Device used is disconnected.";
                                            if (!audioIsStillConnected || !videoIsStillConnected) {
                                                componentRef.invokeMethodAsync("ReceiveError", mediaError);
                                            }
                                            return [2 /*return*/];
                                    }
                                });
                            }); };
                        }
                    }
                    return [2 /*return*/];
                });
            });
        };
        /// Defaults
        BlazorMediaInterop.constraints = {
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
        };
        return BlazorMediaInterop;
    }());
    BlazorMedia.BlazorMediaInterop = BlazorMediaInterop;
})(BlazorMedia || (BlazorMedia = {}));

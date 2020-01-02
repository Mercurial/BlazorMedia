"use strict";
// This file is to show how a library package may provide JavaScript interop features
// wrapped in a .NET API
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
        BlazorMediaInterop.InitializeMediaStream = function (width, height, canCaptureAudio) {
            if (width === void 0) { width = 640; }
            if (height === void 0) { height = 480; }
            if (canCaptureAudio === void 0) { canCaptureAudio = true; }
            return __awaiter(this, void 0, void 0, function () {
                var tracks, track, _a;
                return __generator(this, function (_b) {
                    switch (_b.label) {
                        case 0:
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
                            if (BlazorMediaInterop.MediaStream) {
                                tracks = BlazorMediaInterop.MediaStream.getTracks();
                                track = void 0;
                                while (track = tracks.pop()) {
                                    BlazorMediaInterop.MediaStream.removeTrack(track);
                                }
                            }
                            _a = BlazorMediaInterop;
                            return [4 /*yield*/, navigator.mediaDevices.getUserMedia(BlazorMediaInterop.constraints)];
                        case 1:
                            _a.MediaStream = _b.sent();
                            return [2 /*return*/];
                    }
                });
            });
        };
        BlazorMediaInterop.InitializeVideoElement = function (videoElement, componentRef, timeslice) {
            if (timeslice === void 0) { timeslice = 0; }
            return __awaiter(this, void 0, void 0, function () {
                var _this = this;
                return __generator(this, function (_a) {
                    if (!BlazorMediaInterop.MediaStream)
                        throw "MediaStream is not Initialized, please call InitializeMediaStream first.";
                    videoElement.srcObject = BlazorMediaInterop.MediaStream;
                    videoElement.muted = true;
                    videoElement.volume = 0;
                    videoElement.mediaRecorder = new MediaRecorder(BlazorMediaInterop.MediaStream);
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
                                    return [4 /*yield*/, componentRef.invokeMethodAsync("ReceiveDataAsync", buffer)];
                                case 2:
                                    _b.sent();
                                    return [2 /*return*/];
                            }
                        });
                    }); };
                    videoElement.mediaRecorder.start(timeslice);
                    return [2 /*return*/];
                });
            });
        };
        BlazorMediaInterop.DisposeVideoElement = function (videoElement) {
            return __awaiter(this, void 0, void 0, function () {
                return __generator(this, function (_a) {
                    if (videoElement && videoElement.mediaRecorder) {
                        videoElement.mediaRecorder.stop();
                    }
                    return [2 /*return*/];
                });
            });
        };
        BlazorMediaInterop.SetVideoRecorderTimeslice = function (videoElement, timeslice) {
            if (timeslice === void 0) { timeslice = 0; }
            return __awaiter(this, void 0, void 0, function () {
                return __generator(this, function (_a) {
                    if (videoElement && videoElement.mediaRecorder) {
                        videoElement.mediaRecorder.stop();
                        videoElement.mediaRecorder.start(timeslice);
                    }
                    return [2 /*return*/];
                });
            });
        };
        /// Defaults
        BlazorMediaInterop.constraints = {
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
        };
        return BlazorMediaInterop;
    }());
    BlazorMedia.BlazorMediaInterop = BlazorMediaInterop;
})(BlazorMedia || (BlazorMedia = {}));

using BlazorMedia.Model;
using Microsoft.JSInterop;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlazorMedia
{
    public class BlazorMediaAPI
    {
        public static async Task InitializeMediaStreamAsync(IJSRuntime JSRuntime, int width = 640, int height = 480, bool canCaptureAudio = true, string cameraDeviceId = "", string microphoneDeviceId = "")
        {
            await JSRuntime.InvokeVoidAsync("BlazorMedia.BlazorMediaInterop.InitializeMediaStream", width, height, canCaptureAudio, cameraDeviceId, microphoneDeviceId);
        } 

        public static async Task UnInitializeMediaStreamAsync(IJSRuntime JSRuntime)
        {
            await JSRuntime.InvokeVoidAsync("BlazorMedia.BlazorMediaInterop.UninitializeMediaStream");
        }

        public static async Task<List<MediaDeviceInfo>> EnumerateMediaDevices(IJSRuntime JSRuntime)
        {
            return await JSRuntime.InvokeAsync<List<MediaDeviceInfo>>("navigator.mediaDevices.enumerateDevices");
        }
    } 
}

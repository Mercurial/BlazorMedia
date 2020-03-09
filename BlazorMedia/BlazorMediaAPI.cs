using System;
using BlazorMedia.Model;
using Microsoft.JSInterop;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlazorMedia
{
    public class BlazorMediaAPI
    {
        public static List<MediaDeviceInfo> CurrentMediaDevices = new List<MediaDeviceInfo>();
        public static async Task InitializeMediaStreamAsync(IJSRuntime JSRuntime, int width = 640, int height = 480, bool canCaptureAudio = true, string cameraDeviceId = "", string microphoneDeviceId = "", object componentRef = null)
        {
            await JSRuntime.InvokeVoidAsync("BlazorMedia.BlazorMediaInterop.InitializeMediaStream", width, height, canCaptureAudio, cameraDeviceId, microphoneDeviceId, componentRef);
        } 

        public static async Task UnInitializeMediaStreamAsync(IJSRuntime JSRuntime)
        {
            await JSRuntime.InvokeVoidAsync("BlazorMedia.BlazorMediaInterop.UninitializeMediaStream");
        }

        public static async Task<List<MediaDeviceInfo>> EnumerateMediaDevices(IJSRuntime JSRuntime)
        {
            CurrentMediaDevices = await JSRuntime.InvokeAsync<List<MediaDeviceInfo>>("navigator.mediaDevices.enumerateDevices");
            return CurrentMediaDevices;
        }

        public static void OnDeviceChange(IJSRuntime JSRuntime, object componentRef)
        {
            JSRuntime.InvokeVoidAsync("BlazorMedia.BlazorMediaInterop.OnDeviceChange", CurrentMediaDevices ,componentRef);
        }
       
    } 
}

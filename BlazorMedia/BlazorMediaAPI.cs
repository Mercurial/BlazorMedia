using System;
using System.Linq;
using BlazorMedia.Model;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace BlazorMedia
{
    public class BlazorMediaAPI
    {
        public IJSRuntime JSRuntime { get; set; }
        public List<MediaDeviceInfo> CurrentMediaDevices { get; set; } = new List<MediaDeviceInfo>();

        public BlazorMediaAPI(IJSRuntime jsRuntime)
        {
            JSRuntime = jsRuntime;

        }

        public async Task InitializeMediaStreamAsync(int width = 640, int height = 480, bool canCaptureAudio = true, string cameraDeviceId = "", string microphoneDeviceId = "", int timeSlice = 100, object videoElementRef = null, object componentRef = null)
        {
            await JSRuntime.InvokeVoidAsync("BlazorMedia.BlazorMediaInterop.InitializeMediaStream", width, height, canCaptureAudio, cameraDeviceId, microphoneDeviceId, timeSlice, videoElementRef, componentRef);
        }

        public async Task UnInitializeMediaStreamAsync(ElementReference videoElementRef)
        {
            await JSRuntime.InvokeVoidAsync("BlazorMedia.BlazorMediaInterop.UninitializeMediaStream", videoElementRef);
            await JSRuntime.InvokeVoidAsync("BlazorMedia.BlazorMediaInterop.DisposeVideoElement", videoElementRef);
        }

        public async Task<List<MediaDeviceInfo>> EnumerateMediaDevices()
        {
            CurrentMediaDevices = await JSRuntime.InvokeAsync<List<MediaDeviceInfo>>("navigator.mediaDevices.enumerateDevices");
            await JSRuntime.InvokeVoidAsync("BlazorMedia.BlazorMediaInterop.DeviceChange", DotNetObjectReference.Create(this));
            return CurrentMediaDevices;
        }

        public event EventHandler<DeviceChangeEventArgs> DeviceChanged;

        [JSInvokable]
        public void OnDeviceChange(List<MediaDeviceInfo> newDevices)
        {
            var removedDevices = CurrentMediaDevices.Where(cmd => !newDevices.Any(nd => cmd.Label == nd.Label)).ToList();
            var addedDevices = newDevices.Where(nd => !CurrentMediaDevices.Any(cmd => cmd.Label == nd.Label)).ToList();
            DeviceChanged?.Invoke(this, new DeviceChangeEventArgs()
            {
                Devices = newDevices,
                RemovedDevices = removedDevices,
                AddedDevices = addedDevices
            });
            CurrentMediaDevices = newDevices;
        }

    }
}

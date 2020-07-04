using System;
using System.Linq;
using BlazorMedia.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
namespace BlazorMedia
{
    public class BlazorMediaAPI
    {
        public IJSRuntime JSRuntime { get; set; }
        public List<MediaDeviceInfo> CurrentMediaDevices { get; set; } = new List<MediaDeviceInfo>();
        public event EventHandler<DeviceChangeEventArgs> DeviceChanged;

        public BlazorMediaAPI(IJSRuntime jsRuntime)
        {
            JSRuntime = jsRuntime;
        }

        public async Task InitializeAsync(int width = 640, int height = 480, int framerate = 60, bool canCaptureAudio = true, string cameraDeviceId = "", string microphoneDeviceId = "", int timeSlice = 100, ElementReference videoElementRef = default(ElementReference), DotNetObjectReference<VideoMediaViewModel> componentRef = null)
        {
            await JSRuntime.InvokeVoidAsync("BlazorMedia.BlazorMediaInterop.Initialize", width, height, framerate, canCaptureAudio, cameraDeviceId, microphoneDeviceId, timeSlice, videoElementRef, componentRef);
        }

        public async Task StartDeviceChangeListenerAsync()
        {
            await JSRuntime.InvokeVoidAsync("BlazorMedia.BlazorMediaInterop.StartBlazorDeviceListener", DotNetObjectReference.Create(this));
        }

        public async Task StopDeviceChangeListenerAsync()
        {
            await JSRuntime.InvokeVoidAsync("BlazorMedia.BlazorMediaInterop.StopBlazorDeviceListener", DotNetObjectReference.Create(this));
        }

        public async Task AddBlazorFPSListenerAsync(ElementReference videoElementRef, DotNetObjectReference<VideoMediaViewModel> componentRef)
        {
            await JSRuntime.InvokeVoidAsync("BlazorMedia.BlazorMediaInterop.AddBlazorFPSListener", videoElementRef, componentRef);
        }

        public async Task RemoveBlazorFPSListenerAsync(ElementReference videoElementRef)
        {
            await JSRuntime.InvokeVoidAsync("BlazorMedia.BlazorMediaInterop.RemoveBlazorFPSListener", videoElementRef);
        }

        public async Task<string> CaptureImageAsync(ElementReference videoElementRef)
        {
            return await JSRuntime.InvokeAsync<string>("BlazorMedia.BlazorMediaInterop.CaptureImage", videoElementRef);
        }

        public async Task Destroy(ElementReference videoElementRef)
        {
            await JSRuntime.InvokeVoidAsync("BlazorMedia.BlazorMediaInterop.Destroy", videoElementRef);
        }

        public async Task<List<MediaDeviceInfo>> EnumerateMediaDevices()
        {
            // @TODO implement proper System.Text js
            var devicesJsonArray = await JSRuntime.InvokeAsync<object>("navigator.mediaDevices.enumerateDevices");
            CurrentMediaDevices = JsonConvert.DeserializeObject<List<MediaDeviceInfo>>(devicesJsonArray.ToString());
            return CurrentMediaDevices;
        }

        [JSInvokable]
        public void OnDeviceChange(object newDevicesObject)
        {
            var newDevices = JsonConvert.DeserializeObject<List<MediaDeviceInfo>>(newDevicesObject.ToString());
            var removedDevices = CurrentMediaDevices.Where(cmd => !newDevices.Any(nd => cmd.Name == nd.Name)).ToList();
            var addedDevices = newDevices.Where(nd => !CurrentMediaDevices.Any(cmd => cmd.Name == nd.Name)).ToList();
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

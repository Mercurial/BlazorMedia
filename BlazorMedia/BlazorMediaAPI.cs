using System;
using System.Linq;
using BlazorMedia.Model;
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
        public event EventHandler<DeviceChangeEventArgs> OnDeviceChanged;

        public BlazorMediaAPI(IJSRuntime jsRuntime)
        {
            JSRuntime = jsRuntime;
        }

        public async Task Initialize(int width = 640, int height = 480, bool canCaptureAudio = true, string cameraDeviceId = "", string microphoneDeviceId = "", int timeSlice = 100, object videoElementRef = null, object componentRef = null)
        {
            await JSRuntime.InvokeVoidAsync("BlazorMedia.BlazorMediaInterop.Initialize", width, height, canCaptureAudio, cameraDeviceId, microphoneDeviceId, timeSlice, videoElementRef, componentRef);
        }

        public async Task StartListeningToDeviceChange()
        {
            await JSRuntime.InvokeVoidAsync("BlazorMedia.BlazorMediaInterop.DeviceChange", DotNetObjectReference.Create(this));
        }

        public async Task Uninitialize(ElementReference videoElementRef)
        {
            await JSRuntime.InvokeVoidAsync("BlazorMedia.BlazorMediaInterop.Uninitialize", videoElementRef);
            await JSRuntime.InvokeVoidAsync("BlazorMedia.BlazorMediaInterop.DisposeVideoElement", videoElementRef);
        }

        public async Task<List<MediaDeviceInfo>> EnumerateMediaDevices()
        {
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
            OnDeviceChanged?.Invoke(this, new DeviceChangeEventArgs()
            {
                Devices = newDevices,
                RemovedDevices = removedDevices,
                AddedDevices = addedDevices
            });
            CurrentMediaDevices = newDevices;
        }

    }
}

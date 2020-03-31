using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BlazorMedia.Model;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorMedia.Demo
{
    public class IndexViewModel : ComponentBase
    {

        [Inject]
        protected IJSRuntime JSRuntime { get; set; }
        protected bool IsRecording = false;
        protected List<MediaDeviceInfo> Cameras = new List<MediaDeviceInfo>();
        protected List<MediaDeviceInfo> Microphones = new List<MediaDeviceInfo>();
        protected string SelectedCamera = string.Empty;
        protected string SelectedMicrophone = string.Empty;
        protected BlazorMediaAPI BlazorMediaAPI { get; set; }
        protected override async Task OnInitializedAsync()
        {
            BlazorMediaAPI = new BlazorMediaAPI(JSRuntime);
            await base.OnInitializedAsync();
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                // Should call this before listening to device changes.
                await BlazorMediaAPI.StartListeningToDeviceChange();
                await FetchDeviceListAsync();
                BlazorMediaAPI.OnDeviceChanged += BlazorMedia_DeviceChanged;
            }
            await base.OnAfterRenderAsync(firstRender);
            await InvokeAsync(StateHasChanged);
        }

        protected void OnDataReceived(byte[] data)
        {
            Console.WriteLine(data.Length);
        }

        protected void OnError(MediaError error)
        {
            Console.WriteLine(error.Message);
        }

        protected async Task FetchDeviceListAsync()
        {
            var Devices = await BlazorMediaAPI.EnumerateMediaDevices();
            
            foreach (MediaDeviceInfo mdi in Devices)
            {
                if (mdi.Kind == MediaDeviceKind.AudioInput)
                {
                    Microphones.Add(mdi);
                }
                if (mdi.Kind == MediaDeviceKind.VideoInput)
                {
                    Cameras.Add(mdi);
                }
            }

            if (Microphones.Count > 0)
            {
                SelectedMicrophone = Microphones[0].DeviceId;
            }
            if (Cameras.Count > 0)
            {
                SelectedCamera = Cameras[0].DeviceId;
            }
        }

        protected async Task OnCameraSelected(ChangeEventArgs e)
        {
            SelectedCamera = e.Value.ToString();
            await InvokeAsync(StateHasChanged);
        }
        protected async Task OnMicrophoneSelected(ChangeEventArgs e)
        {
            SelectedMicrophone = e.Value.ToString();
            await InvokeAsync(StateHasChanged);
        }
        protected void OnToggleRecordingPressed()
        {
            IsRecording = !IsRecording;
        }

        public void BlazorMedia_DeviceChanged(object sender, DeviceChangeEventArgs e)
        {
            // Console.WriteLine("Total :" + e.Devices.Count);
            // foreach (var device in e.Devices)
            // {
            //     Console.WriteLine(device.Label);
            // }

            Console.WriteLine("Total removedDevices:" + e.RemovedDevices.Count);
            foreach (var device in e.RemovedDevices)
            {
                Console.WriteLine(device.Name);
            }

            Console.WriteLine("Total AddedDevices:" + e.AddedDevices.Count);
            foreach (var device in e.AddedDevices)
            {
                Console.WriteLine(device.Name);
            }
        }

    }

}

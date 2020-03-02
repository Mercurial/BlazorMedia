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

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await FetchDeviceListAsync();
            }
            await base.OnAfterRenderAsync(firstRender);
            await InvokeAsync(StateHasChanged);
        }

        protected void OnDataReceived(byte[] data)
        {
            Console.WriteLine(data.Length);
        }

        protected async Task FetchDeviceListAsync()
        {
            var Devices = await BlazorMediaAPI.EnumerateMediaDevices(JSRuntime);

            foreach (MediaDeviceInfo mdi in Devices)
            {
                if (mdi.kind == "audioinput")
                {
                    Microphones.Add(mdi);
                }
                if (mdi.kind == "videoinput")
                {
                    Cameras.Add(mdi);
                }
            }

            if(Microphones.Count > 0)
            {
                SelectedMicrophone = Microphones[0].deviceId;
            }
            if(Cameras.Count > 0)
            {
                SelectedCamera = Cameras[0].deviceId;
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

    }
 
}
 
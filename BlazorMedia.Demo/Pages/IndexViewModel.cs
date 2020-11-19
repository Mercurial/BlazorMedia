using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BlazorMedia.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorMedia.Demo
{
	public class IndexViewModel : ComponentBase, IDisposable
	{

		[Inject]
		protected IJSRuntime JSRuntime { get; set; }
		protected VideoMedia VideoMediaComponent { get; set; }
		protected bool IsRecording = false;
		protected List<MediaDeviceInfo> Cameras = new List<MediaDeviceInfo>();
		protected List<MediaDeviceInfo> Microphones = new List<MediaDeviceInfo>();
		protected string SelectedCamera { get; set; } = string.Empty;
		protected string SelectedMicrophone { get; set; } = string.Empty;
		protected string SelectedQuality { get; set; } = "1080p";
		protected BlazorMediaAPI BlazorMediaAPI { get; set; }
		protected int Width { get; set; } = 1920;
		protected int EffectiveWidth { get; set; }
		protected int Height { get; set; } = 1080;
		protected int EffectiveHeight { get; set; }
		protected int Framerate { get; set; } = 30;
		protected int FPS { get; set; }
		protected int KBps { get; set; }
		protected int BytesInSecond { get; set; }
		protected DateTime lastBitRateData { get; set; } = DateTime.Now;
		protected string PictureData { get; set; }

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
				await BlazorMediaAPI.StartDeviceChangeListenerAsync();
				await FetchDeviceListAsync();
				BlazorMediaAPI.DeviceChanged += BlazorMedia_DeviceChanged;
				await InvokeAsync(StateHasChanged);
			}
			await base.OnAfterRenderAsync(firstRender);
		}

		protected void OnData(byte[] data)
		{
			if (DateTime.Now - lastBitRateData >= TimeSpan.FromSeconds(1))
			{
				lastBitRateData = DateTime.Now;
				KBps = (KBps + (BytesInSecond / 1000)) / 2;
				BytesInSecond = 0;
			}
			BytesInSecond += data.Length;
		}

		protected void OnError(MediaError error)
		{
			Console.WriteLine(error.Message);
		}

		protected async void OnFPS(int fps)
		{
			FPS = fps;
			await InvokeAsync(StateHasChanged);
		}

		protected async void OnInitialize(MediaStartEventArgs e)
		{
			EffectiveWidth = e.Width;
			EffectiveHeight = e.Height;
			await InvokeAsync(StateHasChanged);
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

		protected async void OnCameraSelected(ChangeEventArgs e)
		{
			SelectedCamera = e.Value.ToString();
			await InvokeAsync(StateHasChanged);
		}
		protected async void OnMicrophoneSelected(ChangeEventArgs e)
		{
			SelectedMicrophone = e.Value.ToString();
			await InvokeAsync(StateHasChanged);
		}
		protected async void OnQualitySelected(ChangeEventArgs e)
		{
			SelectedQuality = e.Value.ToString();
			Width = int.Parse(SelectedQuality.Split("x")[0]);
			Height = int.Parse(SelectedQuality.Split("x")[1]);
			await InvokeAsync(StateHasChanged);
		}
		protected async void OnToggleRecordingPressed()
		{
			await VideoMediaComponent.ReloadAsync();
		}

		protected async void OnTakePhotoPressed()
		{
			PictureData = await VideoMediaComponent.CaptureImageAsync();
			await InvokeAsync(StateHasChanged);
		}

		public void BlazorMedia_DeviceChanged(object sender, DeviceChangeEventArgs e)
		{
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

		public void Dispose()
		{
			BlazorMediaAPI.DeviceChanged -= BlazorMedia_DeviceChanged;
		}
	}

}

using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;
using System.Linq;
using BlazorMedia.Model;

namespace BlazorMedia
{
    public class VideoMediaViewModel : ComponentBase, IDisposable
    {
        [Inject]
        IJSRuntime JS { get; set; }

        protected ElementReference VideoElementRef { get; set; }

        [Parameter]
        public EventCallback<byte[]> OnDataReceived { get; set; }

        [Parameter]
        public EventCallback<MediaError> OnError { get; set; }

        private int _timeslice = 0;

        [Parameter]
        public int Timeslice
        {
            get
            {
                return _timeslice;
            }
            set
            {
                if (_timeslice != value)
                {
                    _timeslice = value;
                }
            }
        }

        [Parameter]
        public int Width { get; set; } = 640;

        [Parameter]
        public int Height { get; set; } = 480;

        [Parameter]
        public bool RecordAudio { get; set; } = false;

        [Parameter]
        public string CameraDeviceId { get; set; } = string.Empty;

        [Parameter]
        public string MicrophoneDeviceId { get; set; } = string.Empty;

        [Parameter]
        public string Id { get; set; } = string.Empty;

        [Parameter]
        public string Class { get; set; } = string.Empty;

        protected bool IsInitialized { get; set; } = false;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await InitializeComponentAsync();
            }
            await base.OnAfterRenderAsync(firstRender);
        }

        protected BlazorMediaAPI BlazorMediaAPI { get; set; }

        public async Task InitializeComponentAsync()
        {
            if (!IsInitialized)
            {
                BlazorMediaAPI = new BlazorMediaAPI(JS);
                await BlazorMediaAPI.Initialize(Width, Height, RecordAudio, CameraDeviceId, MicrophoneDeviceId, Timeslice, VideoElementRef, DotNetObjectReference.Create(this));
                IsInitialized = true;
            }
        }

        [JSInvokable]
        public void ReceiveData(int[] data)
        {
            /// @TODO: C# Blazor wont accept ArrayUint8 from JS so we pass the binary data as int[] and convert to byte[]
            byte[] buffer = data.Cast<int>().Select(i => (byte)i).ToArray();
            if (OnDataReceived.HasDelegate)
                OnDataReceived.InvokeAsync(buffer);
        }

        [JSInvokable]
        public void ReceiveError(MediaError mediaError)
        {
            if (OnError.HasDelegate)
                OnError.InvokeAsync(mediaError);
        }

        public async void Dispose()
        {
            if (IsInitialized)
            {
                await BlazorMediaAPI.Uninitialize(VideoElementRef);
                IsInitialized = false;
            }
        }
    }
}

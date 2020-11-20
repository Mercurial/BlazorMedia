using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;
using System.Linq;
using BlazorMedia.Models;
using System.Text;

namespace BlazorMedia
{
    public class VideoMediaViewModel : ComponentBase, IDisposable
    {
        [Inject]
        IJSRuntime JS { get; set; }

        protected ElementReference VideoElementRef { get; set; }

        [Parameter]
        public EventCallback<byte[]> OnData { get; set; }

        [Parameter]
        public EventCallback<MediaError> OnError { get; set; }


        [Parameter]
        public EventCallback<int> OnFPS { get; set; }

        [Parameter]
        public EventCallback<MediaStartEventArgs> OnInitialize { get; set; }

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
        public int Framerate { get; set; } = 30;

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

        [Parameter]
        public string Style { get; set; } = string.Empty;

        protected bool IsInitialized { get; set; } = false;

        protected BlazorMediaAPI BlazorMediaAPI { get; set; }

        private Encoding ANSIEncoder { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await InitializeComponentAsync();
            }
            await base.OnAfterRenderAsync(firstRender);
        }

        public async Task InitializeComponentAsync()
        {
            if (!IsInitialized)
            {
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                ANSIEncoder = Encoding.GetEncoding(1252);
                BlazorMediaAPI = new BlazorMediaAPI(JS);
                await ReloadAsync();
                IsInitialized = true;
            }
        }

        [JSInvokable]
        public void ReceiveData(string data)
        {
            if (OnData.HasDelegate)
            {
                byte[] byteArray = ANSIEncoder.GetBytes(data);
                OnData.InvokeAsync(byteArray);
            }
        }

        [JSInvokable]
        public void ReceiveError(MediaError mediaError)
        {
            if (OnError.HasDelegate)
                OnError.InvokeAsync(mediaError);
        }

        [JSInvokable]
        public void ReceiveFPS(int fps)
        {
            if (OnFPS.HasDelegate)
                OnFPS.InvokeAsync(fps);
        }

        [JSInvokable]
        public void ReceiveStart(int width, int height)
        {
            if (OnInitialize.HasDelegate)
                OnInitialize.InvokeAsync(new MediaStartEventArgs { Width = width, Height = height });
        }

        public async Task ReloadAsync()
        {
            var componentRef = DotNetObjectReference.Create<VideoMediaViewModel>(this);
            await BlazorMediaAPI.RemoveBlazorFPSListenerAsync(VideoElementRef);
            await BlazorMediaAPI.InitializeAsync(Width, Height, Framerate, RecordAudio, CameraDeviceId, MicrophoneDeviceId, Timeslice, VideoElementRef, componentRef);
            await BlazorMediaAPI.AddBlazorFPSListenerAsync(VideoElementRef, componentRef);
        }

        public async Task<string> CaptureImageAsync()
        {
            return await BlazorMediaAPI.CaptureImageAsync(VideoElementRef);
        }

        public async void Dispose()
        {
            if (IsInitialized)
            {
                IsInitialized = false;
                try
                {
                    await BlazorMediaAPI.Destroy(VideoElementRef);
                }
                catch (Exception exception)
                {
                    // Exception occurs when a task is cancelled
                    Console.WriteLine($"Error in VideoMedia Dispose: {exception}");
                }
            }
        }
    }
}

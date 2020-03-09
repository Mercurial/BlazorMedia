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
        public EventCallback<BMError> OnError { get; set; }

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
                    JS.InvokeAsync<dynamic>("BlazorMedia.BlazorMediaInterop.SetVideoRecorderTimeslice", VideoElementRef, value);
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

        public async Task InitializeComponentAsync()
        {
            try
            {
                if (!IsInitialized)
                {
                    await BlazorMediaAPI.InitializeMediaStreamAsync(JS, Width, Height, RecordAudio, CameraDeviceId, MicrophoneDeviceId);
                    IsInitialized = true;
                }

                await JS.InvokeAsync<dynamic>(
                    "BlazorMedia.BlazorMediaInterop.InitializeVideoElement",
                    VideoElementRef,
                    DotNetObjectReference.Create(this),
                    Timeslice);
            }
            catch(JSException exception)
            {
                var bmError = new BMError()
                {
                    Type = BMErrorType.Initialization,
                    Message = "Unable to initialize video stream. Please check Media Device Permissions."
                };

                if (OnError.HasDelegate)
                    await OnError.InvokeAsync(bmError);
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
        public void MediaError(BMError bmError)
        {
            if (OnError.HasDelegate)
                OnError.InvokeAsync(bmError);
            
        }

        public async void Dispose()
        {
            try
            {
                if (IsInitialized)
                {
                    await JS.InvokeAsync<dynamic>(
                        "BlazorMedia.BlazorMediaInterop.DisposeVideoElement",
                        VideoElementRef);

                    await BlazorMediaAPI.UnInitializeMediaStreamAsync(JS);

                    IsInitialized = false;
                }
            }
            catch
            {
                // Page has been reloaded, API is not available
                var bmError = new BMError()
                {
                    Type = BMErrorType.Recorder,
                    Message = "Error occured while stopping media stream."
                };
                if (OnError.HasDelegate)
                    OnError.InvokeAsync(bmError);
            }
        }

    }
}

using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace BlazorMedia
{
    public class VideoMediaViewModel : ComponentBase, IDisposable
    {
        [Inject]
        IJSRuntime JS { get; set; }
        protected ElementReference VideoElementRef { get; set; }
        [Parameter]
        public EventCallback<byte[]> OnDataReceived { get; set; }
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

        protected bool IsInitialized { get; set; } = false;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await InitializeComponentAsync();
            }

            await base.OnAfterRenderAsync(firstRender);
        }

        protected async Task InitializeComponentAsync()
        {
            if (!IsInitialized)
            {
                await BlazorMediaAPI.InitializeMediaStreamAsync(JS, Width, Height, RecordAudio);
                IsInitialized = true;
            }

            await JS.InvokeAsync<dynamic>(
                "BlazorMedia.BlazorMediaInterop.InitializeVideoElement",
                VideoElementRef,
                DotNetObjectReference.Create(this),
                Timeslice);
        }

        [JSInvokable]
        public async void ReceiveDataAsync(int[] data)
        {
            /// @TODO: C# Blazor wont accept ArrayUint8 from JS so we pass the binary data as int[] and convert to byte[]
            byte[] buffer = data.Cast<int>().Select(i => (byte)i).ToArray();
            if (OnDataReceived.HasDelegate)
                await OnDataReceived.InvokeAsync(buffer);
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
            }
        }
    }
}

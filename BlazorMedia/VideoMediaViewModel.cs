using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BlazorMedia
{
    public class VideoMediaViewModel : ComponentBase
    {
        [Inject]
        IJSRuntime JSRuntime { get; set; }
        [Inject]
        IComponentContext ComponentContext { get; set; }
        bool IsInitialized { get; set; }
        protected ElementRef VideoElementRef { get; set; }
        [Parameter]
        private EventCallback<byte[]> OnDataReceived { get; set; }

        protected override void OnInit()
        {
            IsInitialized = false;
            base.OnInit();
        }

        protected override async Task OnAfterRenderAsync()
        {
            if (!ComponentContext.IsConnected) return;

            if(!IsInitialized)
            {
                await InitializeComponent();
                IsInitialized = true;
            }

            await base.OnAfterRenderAsync();
        }

        protected async Task InitializeComponent()
        {
            await JSRuntime.InvokeAsync<dynamic>(
                "BlazorMedia.BlazorMediaInterop.InitializeVideoElement",
                VideoElementRef,
                new DotNetObjectRef(this));
        }

        [JSInvokable]
        public async void ReceiveData(byte[] data)
        {
            await OnDataReceived.InvokeAsync(data);
        }
    }
}

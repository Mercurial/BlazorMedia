using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace BlazorMedia
{
    public class VideoMediaViewModel : ComponentBase
    {
        [Inject]
        IJSRuntime JS { get; set; }
        [Inject]
        IComponentContext ComponentContext { get; set; }
        bool IsInitialized { get; set; }
        protected ElementRef VideoElementRef { get; set; }
        [Parameter]
        private EventCallback<byte[]> OnDataReceived { get; set; }
        private int _timeslice = 0;
        [Parameter]
        private int Timeslice
        {
            get
            {
                return _timeslice;
            }
            set
            {
                if(_timeslice != value)
                {
                    _timeslice = value;
                    JS.InvokeAsync<dynamic>("BlazorMedia.BlazorMediaInterop.SetVideoRecorderTimeslice", VideoElementRef, value);
                }
            }
        }

        #region Hack to fix https://github.com/aspnet/AspNetCore/issues/11159

        public static object CreateDotNetObjectRefSyncObj = new object();

        protected DotNetObjectRef<T> CreateDotNetObjectRef<T>(T value) where T : class
        {
            lock (CreateDotNetObjectRefSyncObj)
            {
                JSRuntime.SetCurrentJSRuntime(JS);
                return DotNetObjectRef.Create(value);
            }
        }

        protected void DisposeDotNetObjectRef<T>(DotNetObjectRef<T> value) where T : class
        {
            if (value != null)
            {
                lock (CreateDotNetObjectRefSyncObj)
                {
                    JSRuntime.SetCurrentJSRuntime(JS);
                    value.Dispose();
                }
            }
        }

        #endregion

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
            await JS.InvokeAsync<dynamic>(
                "BlazorMedia.BlazorMediaInterop.InitializeVideoElement",
                VideoElementRef,
                CreateDotNetObjectRef(this),
                Timeslice);
        }

        [JSInvokable]
        public async void ReceiveData(int[] data)
        {
            /// @TODO: C# Blazor wont accept ArrayUint8 from JS so we pass the binary data as int[] and convert to byte[]
            byte[] buffer = data.Cast<int>().Select(i => (byte)i).ToArray();
            if (OnDataReceived.HasDelegate)
                await OnDataReceived.InvokeAsync(buffer);
        }

        [JSInvokable]
        public void Test(string message)
        {
            Console.WriteLine(message);
        }
    }
}

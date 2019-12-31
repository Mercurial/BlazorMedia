using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BlazorMedia
{
    public class BlazorMediaAPI
    {
        public static bool Initialized { get; set; }
        public static async Task InitializeMediaStream(IJSRuntime JSRuntime, int width = 640, int height = 480, bool canCaptureAudio = true)
        {
            await JSRuntime.InvokeAsync<dynamic>("BlazorMedia.BlazorMediaInterop.InitializeMediaStream", width, height, canCaptureAudio);
            BlazorMediaAPI.Initialized = true;
        } 
    }
}

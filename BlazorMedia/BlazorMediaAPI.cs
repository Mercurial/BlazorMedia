using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BlazorMedia
{
    public class BlazorMediaAPI
    {
        public static async Task InitializeMediaStreamAsync(IJSRuntime JSRuntime, int width = 640, int height = 480, bool canCaptureAudio = true)
        {
            await JSRuntime.InvokeVoidAsync("BlazorMedia.BlazorMediaInterop.InitializeMediaStream", width, height, canCaptureAudio);
        } 

        public static async Task UnInitializeMediaStreamAsync(IJSRuntime JSRuntime)
        {
            await JSRuntime.InvokeVoidAsync("BlazorMedia.BlazorMediaInterop.UninitializeMediaStream");
        }
    } 
}

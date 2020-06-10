using System;

namespace BlazorMedia.Models
{
    public class MediaStartEventArgs : EventArgs
    {
        public int Width { get; set; }
        public int Height { get; set; }
    }
}
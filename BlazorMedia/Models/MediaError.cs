using System;

namespace BlazorMedia.Model
{
    public enum ErrorType
    {
        Initialization,
        Runtime
    }
    
    public class MediaError
    {
        public ErrorType Type { get; set; } = ErrorType.Initialization;
        public string Message { get; set; } = string.Empty;
    }
}

using System;

namespace BlazorMedia.Model
{
    public enum BMErrorType
    {
        Error,
        Initialization,
        Recorder,
        VideoElement
    }
    public class BMError
    {
        public BMErrorType Type { get; set; } = BMErrorType.Error;
        public string Message { get; set; } = string.Empty;
    }
}

using System;

namespace BlazorMedia.Model
{
    public class MediaDeviceInfo
    {
        public string deviceId { get; set; } = string.Empty;
        public string kind { get; set; } = string.Empty;
        public string label { get; set; } = string.Empty;
        public string groupId { get; set; } = string.Empty;
    }
}

using System;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
namespace BlazorMedia.Models
{
    public enum MediaDeviceKind
    {
        AudioInput,
        VideoInput,
        AudioOutput,
        VideoOutput
    }

    public class MediaDeviceInfo
    {
        [JsonProperty("label")]
        public string Name { get; set; } = string.Empty;
        [JsonConverter(typeof(StringEnumConverter))]
        public MediaDeviceKind Kind { get; set; }
        public string DeviceId { get; set; } = string.Empty;

       
    }
}

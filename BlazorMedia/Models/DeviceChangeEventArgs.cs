using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorMedia.Model
{
    public class DeviceChangeEventArgs : EventArgs
    {
        public List<MediaDeviceInfo> Devices { get; set; }
        public List<MediaDeviceInfo> RemovedDevices { get; set; }
        public List<MediaDeviceInfo> AddedDevices { get; set; }
    }
}

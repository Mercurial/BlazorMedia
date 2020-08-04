using System;

namespace BlazorMedia.Models
{
	public enum ErrorType
	{
		Initialization,
		Runtime,
		MediaDevice,
		NotAllowed,
		NotReadable,
		NotFound,
		OverConstrained,
	}

	public class MediaError
	{
		public ErrorType Type { get; set; } = ErrorType.Initialization;
		public string Message { get; set; } = string.Empty;
	}
}

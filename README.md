[![Build Status](https://dev.azure.com/rawriclark/BlazorMedia/_apis/build/status/BlazorMedia-CI?branchName=master)](https://dev.azure.com/rawriclark/BlazorMedia/_build/latest?definitionId=1&branchName=master) [![Nuget](https://img.shields.io/nuget/v/BlazorMedia)](https://www.nuget.org/packages/BlazorMedia/)

# BlazorMedia
Open Source Media Capture API and Components for Blazor

A Blazor Library for Interacting with [Browser Media Streaming APIs](https://developer.mozilla.org/en-US/docs/Web/API/Media_Streams_API)

This Library allows you to record your browsers Camera / Screen Sharing Data and save it to a file or live stream it to a remote server.

![Architecture](https://raw.githubusercontent.com/Mercurial/BlazorMedia/master/docs/blazormedia.png)

# How to use

Command Line
```
dotnet add package BlazorMedia
```

In your Component
```C#
@using BlazorMedia
```
```C#
<VideoMedia OnDataReceived="OnDataReceived" Timeslice="100" Width="640" Height="480" RecordAudio="true"></VideoMedia>
```
**Code Behind**

```C#
protected void OnDataReceived(byte[] data)
{
    Console.WriteLine($"Data Recieved of length: {data.Length}");
}
```



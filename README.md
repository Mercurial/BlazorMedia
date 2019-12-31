[![Build Status](https://dev.azure.com/rawriclark/BlazorMedia/_apis/build/status/BlazorMedia-CI?branchName=master)](https://dev.azure.com/rawriclark/BlazorMedia/_build/latest?definitionId=1&branchName=master)

[![Nuget](https://www.nuget.org/packages/BlazorMedia/)](https://img.shields.io/nuget/v/BlazorMedia)

# BlazorMedia
Blazor Library for Interacting with Browser Media Streaming APIs

# How to use

In your Component
```C#
@using BlazorMedia
```
```C#
@if (BlazorMediaAPI.Initialized)
{
    <VideoMedia OnDataReceived="@OnDataReceived" />
}
```
**Code Behind**

```C#
if (!BlazorMediaAPI.Initialized)
{
    await BlazorMediaAPI.InitializeMediaStream(JSRuntime, 640, 480, true);
}
```

```C#
protected void OnDataReceived(byte[] data)
{
    Console.WriteLine($"Data Recieved of length: {data.Length}");
}
```

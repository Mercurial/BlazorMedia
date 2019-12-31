[![Build Status](https://dev.azure.com/rawriclark/BlazorMedia/_apis/build/status/BlazorMedia-CI?branchName=master)](https://dev.azure.com/rawriclark/BlazorMedia/_build/latest?definitionId=1&branchName=master) [![Nuget](https://img.shields.io/nuget/v/BlazorMedia)](https://www.nuget.org/packages/BlazorMedia/)

# BlazorMedia
Open Source Media Capture API and Components for Blazor

Blazor Library for Interacting with Browser Media Streaming APIs

# How to use

Command Line
```
dotnet add package BlazorMedia --version 0.2.7.1
```

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

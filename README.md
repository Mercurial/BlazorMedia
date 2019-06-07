# BlazorMedia
Blazor Library for Interacting with Browser Media Streaming APIs

# How to use

In your Component
```C#
@using BlazorMedia;
```
...
```C#
@if (BlazorMediaAPI.Initialized)
{
    <VideoMedia OnDataReceived="@OnDataReceived" />
}
```
**Code Behind**

...
```C#
if (!BlazorMediaAPI.Initialized)
{
    await BlazorMediaAPI.InitializeMediaStream(JSRuntime, 640, 480, true);
}
```

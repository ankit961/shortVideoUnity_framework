
# 🎮 Short Video Player SDK for Unity (iOS)

This SDK bridges native Swift-based video playback functionality with Unity using P/Invoke, offering a seamless experience for playing, seeking, and controlling video content in your Unity applications.

## 📦 Installation

- Clone the repo or drag the framework into your Xcode Unity project.
- Ensure the `.framework` is embedded & signed in the iOS target's **Frameworks, Libraries, and Embedded Content** section.
- Enable Objective-C and C++ support if needed in Unity's iOS Build Settings.
- Link the framework using Unity’s `Plugins/iOS` folder structure.
- **Set the minimum deployment target to iOS 15.0** for the framework.
  
## 🎯 Key Features

- ✅ Supports video formats: HLS, MP4, MOV
- ✅ Auto-hide controls
- ✅ Exit/Back button support
- ✅ Swift → Unity callback on control events

---

## 🔁 Swift-to-Unity Callback Integration

This SDK supports calling back into Unity when video-related events occur (e.g., play, pause, exit, error).  
It enables Unity to react to player events—useful for UI updates, analytics, or in-game logic.

### 🔁 Triggering the Callback in Swift

You can trigger Unity callbacks from Swift using:

```swift
sendUnityCallback("event:play")
sendUnityCallback("event:pause")
sendUnityCallback("event:exit,total:300,watched:240")
```

### 📘 Example Usage in Unity

To play a video using the SDK:

```csharp
using System.Runtime.InteropServices;
using UnityEngine;

public class VideoController : MonoBehaviour
{
    [DllImport("__Internal")] private static extern void Sh_loadVideo(string url);
    [DllImport("__Internal")] private static extern void Sh_play();

    public void LoadAndPlay()
    {
    ##if UNITY_IOS && !UNITY_EDITOR
        Sh_loadVideo("https://example.com/video.m3u8");
        Sh_play();
    ##endif
    }
}
```

## 🧹 Unity Integration

1. Create a C# MonoBehaviour Script (if not using the one provided).

2. Use the `VideoPlayerBridge.cs` in your Unity project:


## Usage Guidelines

### Mandatory Initialization

Before calling `Sh_play()`, you **must** call `Sh_setURLs()` with an array of valid video URLs. Failing to do so may cause a native crash.

Example in Unity MonoBehaviour:

```csharp
public class VideoPlayerController : MonoBehaviour
{
    public string[] videoURLs;
    private VideoPlayerBridge playerBridge;

    void Start()
    {
        playerBridge = GetComponent<VideoPlayerBridge>();
        playerBridge.Sh_setURLs(videoURLs);
        playerBridge.Sh_play()();
    }

    void OnApplicationQuit()
    {
        playerBridge.Sh_cleanup();
    }
    
    void OnApplicationPause ( bool pause )
    {
        if ( !pause )    playerBridge.Sh_play();
    }
}
```

### Cleaning Up

Always call `Sh_cleanup()` in `OnApplicationQuit()` to release native resources and prevent background playback.


## ⚠️ Usage Guidelines

- Set the video URLs using `Sh_setURLs()` **before** calling `Sh_play()`.
- Always call `Sh_cleanup()` in `OnApplicationQuit()` to avoid background playback.
- Avoid calling `Sh_play()` without setting URLs — it may result in a native crash.

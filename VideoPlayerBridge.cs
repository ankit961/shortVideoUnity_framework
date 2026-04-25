using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class VideoPlayerBridge : MonoBehaviour
{
    #region P/Invoke - Call Swift Functions

    [DllImport("__Internal")] private static extern void Sh_loadVideo(string url);
    [DllImport("__Internal")] private static extern void Sh_play();
    [DllImport("__Internal")] private static extern void Sh_pause();
    [DllImport("__Internal")] private static extern void Sh_stop();
    [DllImport("__Internal")] private static extern void Sh_seekTo(double value);
    [DllImport("__Internal")] private static extern void Sh_seekForward(double seconds);
    [DllImport("__Internal")] private static extern void Sh_seekBackward(double seconds);
    [DllImport("__Internal")] private static extern void Sh_cleanup();
    [DllImport("__Internal")] private static extern void Sh_setURLS(IntPtr urlArray, int count);

    [DllImport("__Internal")] private static extern void Sh_setShowPlayPauseButton([MarshalAs(UnmanagedType.I1)] bool visible);
    [DllImport("__Internal")] private static extern void Sh_setShowSeekbar([MarshalAs(UnmanagedType.I1)] bool visible);

    [DllImport("__Internal")] private static extern void Sh_setShowForwardButton([MarshalAs(UnmanagedType.I1)] bool visible);
    [DllImport("__Internal")] private static extern void Sh_setShowBackwordButton([MarshalAs(UnmanagedType.I1)] bool visible);
    [DllImport("__Internal")] private static extern void Sh_setShowBack10Button([MarshalAs(UnmanagedType.I1)] bool visible);
    [DllImport("__Internal")] private static extern void Sh_setShowFor10Button([MarshalAs(UnmanagedType.I1)] bool visible);
    [DllImport("__Internal")] private static extern void Sh_setShowBackButton([MarshalAs(UnmanagedType.I1)] bool visible);
    [DllImport("__Internal")] private static extern void Sh_setShowLogo([MarshalAs(UnmanagedType.I1)] bool visible);
    [DllImport("__Internal")] private static extern void Sh_setShowTimeDuration([MarshalAs(UnmanagedType.I1)] bool visible);

    [DllImport("__Internal")] private static extern void Sh_registerUnityCallback();

    #endregion

    #region Wrapper Functions

    public void LoadVideo(string url)
    {
#if UNITY_IOS && !UNITY_EDITOR
        Sh_loadVideo(url);
#endif
    }

    public void Play()
    {
#if UNITY_IOS && !UNITY_EDITOR
        Sh_play();
#endif
    }

    public void Pause()
    {
#if UNITY_IOS && !UNITY_EDITOR
        Sh_pause();
#endif
    }

    public void Stop()
    {
#if UNITY_IOS && !UNITY_EDITOR
        Sh_stop();
#endif
    }

    public void SeekForward(double seconds)
    {
#if UNITY_IOS && !UNITY_EDITOR
        Sh_seekForward(seconds);
#endif
    }

    public void SeekBackward(double seconds)
    {
#if UNITY_IOS && !UNITY_EDITOR
        Sh_seekBackward(seconds);
#endif
    }

    public void SeekTo(double value)
    {
#if UNITY_IOS && !UNITY_EDITOR
        Sh_seekTo(value);
#endif
    }

    public void Cleanup()
    {
#if UNITY_IOS && !UNITY_EDITOR
        Sh_cleanup();
#endif
    }

    public void SetURLs(string[] urls)
    {
        if (urls == null || urls.Length == 0) return;
#if UNITY_IOS && !UNITY_EDITOR
        IntPtr urlArray = MarshalArray(urls);
        try
        {
            Sh_setURLS(urlArray, urls.Length);
        }
        finally
        {
            FreeMarshalledArray(urlArray, urls.Length);
        }
#endif
    }

    public void ShowForwardButton(bool visible)
    {
#if UNITY_IOS && !UNITY_EDITOR
        Sh_setShowForwardButton(visible);
#endif
    }

    public void ShowBackwardButton(bool visible)
    {
#if UNITY_IOS && !UNITY_EDITOR
        Sh_setShowBackwordButton(visible);
#endif
    }

    public void ShowBack10Button(bool visible)
    {
#if UNITY_IOS && !UNITY_EDITOR
        Sh_setShowBack10Button(visible);
#endif
    }

    public void ShowFor10Button(bool visible)
    {
#if UNITY_IOS && !UNITY_EDITOR
        Sh_setShowFor10Button(visible);
#endif
    }

    public void ShowPlayPauseButton(bool visible)
    {
#if UNITY_IOS && !UNITY_EDITOR
        Sh_setShowPlayPauseButton(visible);
#endif
    }

    public void ShowBackButton(bool visible)
    {
#if UNITY_IOS && !UNITY_EDITOR
        Sh_setShowBackButton(visible);
#endif
    }

    public void ShowLogo(bool visible)
    {
#if UNITY_IOS && !UNITY_EDITOR
        Sh_setShowLogo(visible);
#endif
    }

    public void ShowSeekbar(bool visible)
    {
#if UNITY_IOS && !UNITY_EDITOR
        Sh_setShowSeekbar(visible);
#endif
    }

    public void ShowTimeDuration(bool visible)
    {
#if UNITY_IOS && !UNITY_EDITOR
        Sh_setShowTimeDuration(visible);
#endif
    }

    #endregion

    #region Helper Methods

    private IntPtr MarshalArray(string[] array)
    {
        IntPtr ptr = Marshal.AllocHGlobal(IntPtr.Size * array.Length);

        for (int i = 0; i < array.Length; i++)
        {
            IntPtr stringPtr = Marshal.StringToHGlobalAnsi(array[i]);
            Marshal.WriteIntPtr(ptr, i * IntPtr.Size, stringPtr);
        }

        return ptr;
    }

    // Free inner strings as well as the outer array — otherwise each call to
    // SetURLs leaked one heap allocation per URL.
    private void FreeMarshalledArray(IntPtr ptr, int length)
    {
        for (int i = 0; i < length; i++)
        {
            IntPtr strPtr = Marshal.ReadIntPtr(ptr, i * IntPtr.Size);
            if (strPtr != IntPtr.Zero) Marshal.FreeHGlobal(strPtr);
        }

        Marshal.FreeHGlobal(ptr);
    }

    #endregion
}

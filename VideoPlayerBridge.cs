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

    [DllImport("__Internal")] private static extern void Sh_setShowPlayPauseButton(bool visible);
    [DllImport("__Internal")] private static extern void Sh_setShowSeekbar(bool visible);

    [DllImport("__Internal")] private static extern void Sh_setShowForwardButton(bool visible);
    [DllImport("__Internal")] private static extern void Sh_setShowBackwordButton(bool visible);
    [DllImport("__Internal")] private static extern void Sh_setShowBack10Button(bool visible);
    [DllImport("__Internal")] private static extern void Sh_setShowFor10Button(bool visible);
    [DllImport("__Internal")] private static extern void Sh_setShowBackButton(bool visible);
    [DllImport("__Internal")] private static extern void Sh_setShowLogo(bool visible);
    [DllImport("__Internal")] private static extern void Sh_setShowTimeDuration(bool visible);

    [DllImport("__Internal")] private static extern void Sh_registerUnityCallback();

    #endregion

    #region Wrapper Functions

    public void LoadVideo(string url) => Sh_loadVideo(url);
    public void Play() => Sh_play();
    public void Pause() => Sh_pause();
    public void Stop() => Sh_stop();
    public void SeekForward(double seconds) => Sh_seekForward(seconds);
    public void SeekBackward(double seconds) => Sh_seekBackward(seconds);
    public void SeekTo(double value) => Sh_seekTo(value);
    public void Cleanup() => Sh_cleanup();

    public void SetURLs(string[] urls)
    {
        IntPtr urlArray = MarshalArray(urls);
        Sh_setURLS(urlArray, urls.Length);
        FreeMarshalledArray(urlArray, urls.Length); // ✅ FIXED memory leak
    }

    public void ShowForwardButton(bool visible) => Sh_setShowForwardButton(visible);
    public void ShowBackwordButton(bool visible) => Sh_setShowBackwordButton(visible);
    public void ShowBack10Button(bool visible) => Sh_setShowBack10Button(visible);
    public void ShowFor10Button(bool visible) => Sh_setShowFor10Button(visible);
    public void ShowPlayPauseButton(bool visible) => Sh_setShowPlayPauseButton(visible);
    public void ShowBackButton(bool visible) => Sh_setShowBackButton(visible);
    public void ShowLogo(bool visible) => Sh_setShowLogo(visible);
    public void ShowSeekbar(bool visible) => Sh_setShowSeekbar(visible);
    public void ShowTimeDuration(bool visible) => Sh_setShowTimeDuration(visible);

    #endregion

    #region Unity Callback

    public delegate void UnityCallback(string message);

    [AOT.MonoPInvokeCallback(typeof(UnityCallback))]
    public static void OnSwiftEvent(string message)
    {
        Debug.Log("Video Event: " + message);
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

    // ✅ IMPORTANT: Free inner strings also (you were leaking memory)
    private void FreeMarshalledArray(IntPtr ptr, int length)
    {
        for (int i = 0; i < length; i++)
        {
            IntPtr strPtr = Marshal.ReadIntPtr(ptr, i * IntPtr.Size);
            Marshal.FreeHGlobal(strPtr);
        }

        Marshal.FreeHGlobal(ptr);
    }

    #endregion
}
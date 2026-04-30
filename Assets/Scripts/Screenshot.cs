using UnityEngine;
using System.IO;
using System;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
public class Screenshot : MonoBehaviour
{
    public Camera captureCamera;

    void Update()
    {
        if (Keyboard.current.pKey.wasPressedThisFrame)
        {
            string pictures = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            string directory = Path.Combine(pictures, "OSOM_Screenhots");

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            string filename = "screenshot_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".png";
            string path = Path.Combine(directory, filename);

            ScreenCapture.CaptureScreenshot(path);
            Debug.Log("Saving to: " + path);
        }
    }

    public void TakeScreenshotVR()
    {
        // int width = 1920;
        // int height = 1080;

        // RenderTexture rt = new RenderTexture(width, height, 24);
        // captureCamera.targetTexture = rt;

        // Texture2D tex = new Texture2D(width, height, TextureFormat.RGB24, false);
        // captureCamera.Render();

        // RenderTexture.active = rt;
        // tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        // tex.Apply();

        // captureCamera.targetTexture = null;
        // RenderTexture.active = null;
        // Destroy(rt);

        // string directory = "/sdcard/Pictures/YourAppName/";
        // if (!Directory.Exists(directory))
        //     Directory.CreateDirectory(directory);

        // string filename = "screenshot_" + System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".png";
        // string fullPath = Path.Combine(directory, filename);

        // File.WriteAllBytes(fullPath, tex.EncodeToPNG());

        // Debug.Log("Saved to: " + fullPath);
        // ScanFile(fullPath);

        string pictures = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
        string directory = Path.Combine(pictures, "OSOM_Screenhots");

        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);

        string filename = "screenshot_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".png";
        string path = Path.Combine(directory, filename);

        ScreenCapture.CaptureScreenshot(path);
        Debug.Log("Saving to: " + path);
    }

    public void ScanFile(string path)
    {
        using (AndroidJavaClass mediaScanner = new AndroidJavaClass("android.media.MediaScannerConnection"))
        using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            AndroidJavaObject context = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            mediaScanner.CallStatic("scanFile", context, new string[] { path }, null, null);
        }
    }
}
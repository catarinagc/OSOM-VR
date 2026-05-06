using UnityEngine;
using System.IO;
using System;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System.Collections;
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

    // public void TakeScreenshotVR()
    // {
    //     // int width = 1920;
    //     // int height = 1080;

    //     // RenderTexture rt = new RenderTexture(width, height, 24);
    //     // captureCamera.targetTexture = rt;

    //     // Texture2D tex = new Texture2D(width, height, TextureFormat.RGB24, false);
    //     // captureCamera.Render();

    //     // RenderTexture.active = rt;
    //     // tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
    //     // tex.Apply();

    //     // captureCamera.targetTexture = null;
    //     // RenderTexture.active = null;
    //     // Destroy(rt);

    //     string directory = "/sdcard/Pictures";
    //     // if (!Directory.Exists(directory))
    //     //     Directory.CreateDirectory(directory);

    //     string filename = "screenshot_" + System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".png";
    //     string fullPath = Path.Combine(directory, filename);

    //     // File.WriteAllBytes(fullPath, tex.EncodeToPNG());

    //     // Debug.Log("Saved to: " + fullPath);
    //     // ScanFile(fullPath);

    //     // string pictures = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
    //     // string directory = Path.Combine(pictures, "OSOM_Screenhots");

    //     // if (!Directory.Exists(directory))
    //     //     Directory.CreateDirectory(directory);

    //     // string filename = "screenshot_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".png";
    //     // string path = Path.Combine(directory, filename);

    //     ScreenCapture.CaptureScreenshot(fullPath);
    //     // Debug.Log("Saving to: " + path);
    // }

    // public void TakeScreenshotVR()
    // {
    //     StartCoroutine(SaveScreenshotToGallery());
    // }

    //public Camera captureCamera;

    public void TakeScreenshotVR()
    {
        StartCoroutine(CaptureAndSave());
    }

    IEnumerator CaptureAndSave()
    {
        yield return new WaitForEndOfFrame();

        int width = 1024;   // you can increase this
        int height = 1024;

        RenderTexture rt = new RenderTexture(width, height, 24);
        captureCamera.targetTexture = rt;

        Texture2D tex = new Texture2D(width, height, TextureFormat.RGB24, false);

        captureCamera.Render();

        RenderTexture.active = rt;
        tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        tex.Apply();

        captureCamera.targetTexture = null;
        RenderTexture.active = null;
        Destroy(rt);

        byte[] bytes = tex.EncodeToPNG();
        Destroy(tex);

        yield return SaveToGallery(bytes);
    }

    // IEnumerator SaveScreenshotToGallery()
    // {
    //     string filename = "screenshot_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".png";

    //     string tempPath = Path.Combine(Application.persistentDataPath, filename);

    //     // Step 1: capture to temp location
    //     ScreenCapture.CaptureScreenshot(tempPath);

    //     // Wait until file is actually written
    //     yield return new WaitForSeconds(1.5f);

    // #if UNITY_ANDROID && !UNITY_EDITOR
    //     using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
    //     using (AndroidJavaObject context = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
    //     using (AndroidJavaObject contentResolver = context.Call<AndroidJavaObject>("getContentResolver"))
    //     using (AndroidJavaClass mediaStoreImages = new AndroidJavaClass("android.provider.MediaStore$Images$Media"))
    //     using (AndroidJavaClass contentValuesClass = new AndroidJavaClass("android.content.ContentValues"))
    //     {
    //         AndroidJavaObject values = new AndroidJavaObject("android.content.ContentValues");

    //         values.Call("put", "_display_name", filename);
    //         values.Call("put", "mime_type", "image/png");
    //         values.Call("put", "relative_path", "Pictures/OSOM_Screenshots");

    //         AndroidJavaObject uri = contentResolver.Call<AndroidJavaObject>(
    //             "insert",
    //             mediaStoreImages.GetStatic<AndroidJavaObject>("EXTERNAL_CONTENT_URI"),
    //             values
    //         );

    //         using (AndroidJavaObject outputStream = contentResolver.Call<AndroidJavaObject>("openOutputStream", uri))
    //         {
    //             byte[] bytes = File.ReadAllBytes(tempPath);
    //             outputStream.Call("write", bytes);
    //             outputStream.Call("close");
    //         }
    //     }
    // #endif

    //     Debug.Log("Saved to Quest Gallery: " + filename);
    // }~
    IEnumerator SaveToGallery(byte[] bytes)
    {
        string filename = "screenshot_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".png";

    #if UNITY_ANDROID && !UNITY_EDITOR
        using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        using (AndroidJavaObject context = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
        using (AndroidJavaObject resolver = context.Call<AndroidJavaObject>("getContentResolver"))
        using (AndroidJavaClass mediaStore = new AndroidJavaClass("android.provider.MediaStore$Images$Media"))
        {
            AndroidJavaObject values = new AndroidJavaObject("android.content.ContentValues");
            values.Call("put", "_display_name", filename);
            values.Call("put", "mime_type", "image/png");
            values.Call("put", "relative_path", "Pictures/OSOM_Screenshots");

            AndroidJavaObject uri = resolver.Call<AndroidJavaObject>(
                "insert",
                mediaStore.GetStatic<AndroidJavaObject>("EXTERNAL_CONTENT_URI"),
                values
            );

            using (AndroidJavaObject stream = resolver.Call<AndroidJavaObject>("openOutputStream", uri))
            {
                stream.Call("write", bytes);
                stream.Call("flush");
                stream.Call("close");
            }
        }
    #endif

        Debug.Log("Saved to gallery: " + filename);
        yield return null;
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
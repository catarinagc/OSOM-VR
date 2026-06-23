// using UnityEngine;
// using System.IO;
// using System;
// using UnityEngine.InputSystem;
// using UnityEngine.EventSystems;
// using System.Collections;
// public class Screenshot : MonoBehaviour
// {
//     public Camera captureCamera;

//     public void TakeScreenshot()
//     {
//         string pictures = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
//         string directory = Path.Combine(pictures, "OSOM_Screenhots");

//         if (!Directory.Exists(directory))
//             Directory.CreateDirectory(directory);

//         string filename = "screenshot_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".png";
//         string path = Path.Combine(directory, filename);

//         ScreenCapture.CaptureScreenshot(path);
//         Debug.Log("Saving to: " + path);
//     }

//     public void TakeScreenshotVR()
//     {
//         StartCoroutine(CaptureAndSave());
//         // sem build
//         // string pictures = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
//         // string directory = Path.Combine(pictures, "OSOM_Screenhots");

//         // if (!Directory.Exists(directory))
//         //     Directory.CreateDirectory(directory);

//         // string filename = "screenshot_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".png";
//         // string path = Path.Combine(directory, filename);

//         // ScreenCapture.CaptureScreenshot(path);
//         // Debug.Log("Saving to: " + path);
//     }

//     IEnumerator CaptureAndSave()
//     {
//         yield return new WaitForEndOfFrame();

//         int width = 1024;
//         int height = 1024;

//         RenderTexture rt = new RenderTexture(width, height, 24);
//         captureCamera.targetTexture = rt;

//         Texture2D tex = new Texture2D(width, height, TextureFormat.RGB24, false);

//         captureCamera.Render();

//         RenderTexture.active = rt;
//         tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
//         tex.Apply();

//         captureCamera.targetTexture = null;
//         RenderTexture.active = null;
//         Destroy(rt);

//         byte[] bytes = tex.EncodeToPNG();
//         Destroy(tex);

//         yield return SaveToGallery(bytes);
//     }

//     IEnumerator SaveToGallery(byte[] bytes)
//     {
//         string filename = "screenshot_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".png";

//     #if UNITY_ANDROID && !UNITY_EDITOR
//         using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
//         using (AndroidJavaObject context = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
//         using (AndroidJavaObject resolver = context.Call<AndroidJavaObject>("getContentResolver"))
//         using (AndroidJavaClass mediaStore = new AndroidJavaClass("android.provider.MediaStore$Images$Media"))
//         {
//             AndroidJavaObject values = new AndroidJavaObject("android.content.ContentValues");
//             values.Call("put", "_display_name", filename);
//             values.Call("put", "mime_type", "image/png");
//             values.Call("put", "relative_path", "Pictures/OSOM_Screenshots");

//             AndroidJavaObject uri = resolver.Call<AndroidJavaObject>(
//                 "insert",
//                 mediaStore.GetStatic<AndroidJavaObject>("EXTERNAL_CONTENT_URI"),
//                 values
//             );

//             using (AndroidJavaObject stream = resolver.Call<AndroidJavaObject>("openOutputStream", uri))
//             {
//                 stream.Call("write", bytes);
//                 stream.Call("flush");
//                 stream.Call("close");
//             }
//         }
//     #endif

//         Debug.Log("Saved to gallery: " + filename);
//         yield return null;
//     }

//     public void ScanFile(string path)
//     {
//         using (AndroidJavaClass mediaScanner = new AndroidJavaClass("android.media.MediaScannerConnection"))
//         using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
//         {
//             AndroidJavaObject context = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
//             mediaScanner.CallStatic("scanFile", context, new string[] { path }, null, null);
//         }
//     }
// }


using UnityEngine;
using System.IO;
using System;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System.Collections;

public class Screenshot : MonoBehaviour
{
    [Header("Capture Camera")]
    [Tooltip("Your actual VR/head camera. It already tracks the player's position and rotation, so we don't need a separate transform for that — we just need to temporarily pull it out of stereo mode to render a flat photo.")]
    public Camera captureCamera;

    [Header("Capture Settings")]
    [Tooltip("Replicating the headset's real per-eye FOV (often 90-110+) on a flat 2D photo looks heavily fisheye-distorted. A moderate value (70-90) reads as a 'normal' photo, closer to how a phone camera sees the world.")]
    [Range(50f, 110f)]
    public float captureFOV = 80f;

    public int captureWidth = 1920;
    public int captureHeight = 1080;

    [Tooltip("Multisample anti-aliasing for the capture. VR rendering is heavily anti-aliased by the XR runtime, so without this the screenshot looks noticeably rougher than what the user actually sees.")]
    [Range(1, 8)]
    public int antiAliasing = 4;

    public void TakeScreenshot()
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

    public void TakeScreenshotVR()
    {
        StartCoroutine(CaptureAndSave());
    }

    IEnumerator CaptureAndSave()
    {
        yield return new WaitForEndOfFrame();

        // --- Remember the camera's live VR state so we can put it back exactly. ---
        StereoTargetEyeMask originalStereoTarget = captureCamera.stereoTargetEye;
        float originalFOV = captureCamera.fieldOfView;
        RenderTexture originalTarget = captureCamera.targetTexture;

        // 1. Pull the camera OUT of stereo/XR rendering for this one render call.
        //    While stereoTargetEye is Left/Right/Both, the XR runtime drives this
        //    camera's projection using the headset's real lens-matched, asymmetric
        //    per-eye matrices -- NOT the fieldOfView/aspect you set below. Rendering
        //    into a plain RenderTexture while that's still active is what was
        //    producing the bad/distorted capture. Setting it to None for a moment
        //    makes this a normal flat camera for the duration of the screenshot.
        captureCamera.stereoTargetEye = StereoTargetEyeMask.None;

        // 2. Use a flattering, non-fisheye FOV and a normal photo aspect ratio.
        //    Replicating the headset's real per-eye FOV (often 90-110+) on a flat
        //    2D photo looks heavily fisheye-distorted -- a moderate value reads
        //    as a normal photo, closer to how a phone camera sees the world.
        captureCamera.fieldOfView = captureFOV;
        captureCamera.aspect = (float)captureWidth / captureHeight;

        int width = captureWidth;
        int height = captureHeight;

        bool linear = QualitySettings.activeColorSpace == ColorSpace.Linear;

        // 3. Render to a multisampled (anti-aliased) render texture.
        RenderTextureDescriptor msaaDesc = new RenderTextureDescriptor(width, height, RenderTextureFormat.ARGB32, 24)
        {
            msaaSamples = antiAliasing,
            sRGB = linear
        };

        RenderTexture msaaRT = RenderTexture.GetTemporary(msaaDesc);
        captureCamera.targetTexture = msaaRT;
        captureCamera.Render();

        // 4. Resolve the MSAA texture into a plain single-sample texture before reading pixels.
        //    (Texture2D.ReadPixels can't reliably read directly from a multisampled target.)
        RenderTextureDescriptor resolveDesc = msaaDesc;
        resolveDesc.msaaSamples = 1;
        RenderTexture resolvedRT = RenderTexture.GetTemporary(resolveDesc);
        Graphics.Blit(msaaRT, resolvedRT);

        Texture2D tex = new Texture2D(width, height, TextureFormat.RGB24, false);
        RenderTexture.active = resolvedRT;
        tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        tex.Apply();
        RenderTexture.active = null;

        RenderTexture.ReleaseTemporary(msaaRT);
        RenderTexture.ReleaseTemporary(resolvedRT);

        // --- Put the camera back exactly how the headset needs it. This matters: ---
        // skipping this would leave the live VR camera broken after the first screenshot.
        captureCamera.targetTexture = originalTarget;
        captureCamera.fieldOfView = originalFOV;
        captureCamera.ResetAspect(); // back to auto aspect driven by the XR display
        captureCamera.stereoTargetEye = originalStereoTarget;

        byte[] bytes = tex.EncodeToPNG();
        Destroy(tex);

        yield return SaveToGallery(bytes);
    }

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
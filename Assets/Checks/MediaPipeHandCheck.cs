using System.Collections.Generic;
using Mediapipe.Tasks.Core;
using Mediapipe.Tasks.Vision.Core;
using Mediapipe.Tasks.Vision.HandLandmarker;
using Mediapipe.Unity.Experimental;
using UnityEngine;

// Phase 0 check: does MediaPipe hand tracking run on this webcam in Unity 6.3?
// Shows the camera image and draws a dot on each of the 21 points of every hand it finds.
// Not a demo: the real Webcam_Hands scene replaces this.
public class MediaPipeHandCheck : MonoBehaviour
{
    public TextAsset handModel;   // hand_landmarker.bytes, from the MediaPipe package's PackageResources/MediaPipe folder
    public string cameraName = "";   // empty = the first camera Windows lists (virtual cameras such as OBS count too)
    public int maxHands = 2;

    WebCamTexture webcam;
    TextureFrame frame;           // a CPU copy of the camera image that MediaPipe can read
    HandLandmarker landmarker;
    List<Vector2> points = new List<Vector2>();   // 0..1, top-left origin, like MediaPipe gives them
    float detectMs;

    void Start()
    {
        string device = cameraName != "" ? cameraName : WebCamTexture.devices[0].name;
        webcam = new WebCamTexture(device, 640, 480, 30);
        webcam.Play();

        var options = new HandLandmarkerOptions(
            new BaseOptions(BaseOptions.Delegate.CPU, modelAssetBuffer: handModel.bytes),
            runningMode: RunningMode.VIDEO,         // we call it once per frame and wait for the answer
            numHands: maxHands);
        landmarker = HandLandmarker.CreateFromOptions(options);
    }

    void Update()
    {
        // Only run on new camera frames. Before the first one arrives the texture is 16x16.
        if (!webcam.didUpdateThisFrame || webcam.width < 100) return;

        if (frame == null) frame = new TextureFrame(webcam.width, webcam.height);

        // Unity stores image rows bottom-up, MediaPipe expects top-down, so flip vertically.
        frame.ReadTextureOnCPU(webcam, false, true);
        using var image = frame.BuildCPUImage();

        var started = Time.realtimeSinceStartupAsDouble;
        long timestampMs = (long)(started * 1000);   // VIDEO mode needs a time that always goes up
        var result = landmarker.DetectForVideo(image, timestampMs);
        detectMs = (float)((Time.realtimeSinceStartupAsDouble - started) * 1000);

        points.Clear();
        if (result.handLandmarks == null) return;   // null when no hands were found
        foreach (var hand in result.handLandmarks)
            foreach (var p in hand.landmarks)
                points.Add(new Vector2(p.x, p.y));
    }

    void OnGUI()
    {
        if (webcam == null) return;
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), webcam, ScaleMode.StretchToFill);

        // GUI coordinates also start top-left, so MediaPipe's 0..1 values map straight onto the screen.
        foreach (var p in points)
            GUI.DrawTexture(new Rect(p.x * Screen.width - 4, p.y * Screen.height - 4, 8, 8), Texture2D.whiteTexture);

        GUI.Label(new Rect(10, 10, 600, 25),
            $"Camera {webcam.width}x{webcam.height}   hands: {points.Count / 21}   detect: {detectMs:0} ms");
    }

    void OnDestroy()
    {
        landmarker?.Close();
        frame?.Dispose();
        if (webcam != null) webcam.Stop();
    }
}

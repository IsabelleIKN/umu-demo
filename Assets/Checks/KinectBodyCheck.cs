using UnityEngine;
using Windows.Kinect;

// Phase 0 check: does Microsoft's Kinect v2 plugin report bodies in Unity 6.3?
// Draws every joint of every tracked body as a dot and lists each body's ID and distance.
// Not a demo: the real Kinect_Body scene replaces this.
public class KinectBodyCheck : MonoBehaviour
{
    KinectSensor sensor;
    BodyFrameReader reader;
    Body[] bodies;

    void Start()
    {
        sensor = KinectSensor.GetDefault();
        reader = sensor.BodyFrameSource.OpenReader();
        sensor.Open();   // IsAvailable turns true a second or two later, once the sensor answers
        bodies = new Body[sensor.BodyFrameSource.BodyCount];   // always 6 on Kinect v2
    }

    void Update()
    {
        // Returns null when no new frame has arrived since the last call (Kinect runs at 30 fps).
        var frame = reader.AcquireLatestFrame();
        if (frame == null) return;
        frame.GetAndRefreshBodyData(bodies);
        frame.Dispose();   // release it straight away, or the Kinect stops sending new frames
    }

    void OnGUI()
    {
        string text = $"Kinect available: {sensor.IsAvailable}";

        foreach (var body in bodies)
        {
            if (body == null || !body.IsTracked) continue;

            // Joint positions are in metres from the sensor. To draw them, map each one
            // onto the 512x424 depth image, then stretch that to the screen.
            foreach (var joint in body.Joints.Values)
            {
                var p = sensor.CoordinateMapper.MapCameraPointToDepthSpace(joint.Position);
                GUI.DrawTexture(new Rect(p.X / 512 * Screen.width - 4, p.Y / 424 * Screen.height - 4, 8, 8), Texture2D.whiteTexture);
            }

            float distance = body.Joints[JointType.SpineBase].Position.Z;
            text += $"\nbody {body.TrackingId}: {distance:0.00} m away";
        }

        GUI.Label(new Rect(10, 10, 600, 200), text);
    }

    void OnApplicationQuit()
    {
        reader?.Dispose();
        if (sensor != null && sensor.IsOpen) sensor.Close();
    }
}

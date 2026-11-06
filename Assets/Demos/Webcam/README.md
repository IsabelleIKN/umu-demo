# Webcam demos (MediaPipe)

These demos track hands and bodies in an ordinary webcam image, using Google's MediaPipe through the [MediaPipe Unity Plugin](https://github.com/homuler/MediaPipeUnityPlugin). Everything runs on the computer itself: no internet connection, and no images are stored or sent anywhere.

Back to the [main README](../../../README.md). The Android AR demos are in [umu-demo-ar](https://github.com/IsabelleIKN/umu-demo-ar).

## 1. What you need

- **A webcam.** The laptop's built-in one is fine for trying things out. The demos ask for 640×480, so any HD webcam is plenty.
- **A reasonably fast Windows laptop or PC.** On Windows, MediaPipe runs on the processor (CPU), not the graphics card. On our test laptop, hand tracking took about 17 ms per frame with no hands in view and 30–50 ms with hands, so expect 20–30 frames per second. Slower laptops will be slower.
- For the museum: a separate USB webcam on a stand or mount, so you can choose its height and angle (see section 7).

## 2. Installing

Nothing beyond the main README's steps: `setup.ps1` downloads the MediaPipe plugin. Webcams don't need drivers.

Check one Windows setting: *Settings* → *Privacy & security* → *Camera*. **Camera access** and **Let desktop apps access your camera** must both be on, because Unity counts as a desktop app.

## 3. Check the webcam works outside Unity

Open the Windows **Camera** app. If you see yourself, the webcam works. **Close the Camera app afterwards:** most webcams can only be used by one program at a time.

No image? Look for a privacy shutter on the camera, or a camera key on the keyboard (on many laptops one of the F keys has a crossed-out camera icon), and try again.

## 4. Opening and running a demo

1. In Unity's Project window, open a scene from one of the folders here, for example `Hands/Webcam_Hands`.
2. Press Play. You should see the camera image with tracking drawn on top.
3. **Wrong camera, or more than one?** Unity also lists virtual cameras, such as OBS Virtual Camera or Meta Quest Link. Select the object with the Tracker script and type the camera's name into **Camera Name**. Leave it empty to use the first camera Unity finds.

## 5. How the code works

*Written together with the demos.*

## 6. Common problems

| Problem | What to try |
|---|---|
| Unity shows errors about `com.github.homuler.mediapipe` when opening the project | `setup.ps1` wasn't run or didn't finish. Close Unity, run it, open the project again. |
| Black image, or "no camera found" | Another program is using the webcam (Camera app, Teams, Zoom, a browser tab). Close it and press Play again. Also check the privacy setting in section 2. |
| The image comes from the wrong camera | Fill in **Camera Name** (section 4). |
| Low frame rate | Plug the laptop in (on battery, Windows slows the processor down), close other programs, and set *Settings* → *System* → *Power* → *Power mode* to *Best performance*. Tracking fewer hands or people also helps. |

## 7. Tips for the museum

- **Light the visitors, not the camera.** A window or bright lamp *behind* the visitors turns them into dark silhouettes and tracking gets much worse. Light from the front or the side works best.
- **Put the camera where children fit in the picture too.** A camera at adult eye height cuts off small children standing close by. Try around 1–1.2 m high, and test with real children's heights.
- **A calm background helps,** especially for the cut-out demo (`Webcam_Layers`). Busy patterns or other people walking past behind the visitors cause mistakes.
- **Know the limits of tracking several people with a webcam:**
  - MediaPipe doesn't remember who is who between frames, so when two people cross, their colours can swap.
  - Small children and people standing partly behind each other are detected less reliably.
  - Hands aren't linked to a person: you get "6 hands", not "Anna's left hand".

  The Kinect demos don't have these problems, which makes an interesting comparison.
- **Privacy:** the demos process the image on the computer and store nothing. Say so on a sign next to the installation, because visitors will wonder.

## 8. Ideas for taking it further

*Written together with the demos.*

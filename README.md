# umu-demo: webcam and Kinect demos for Unity

Small Unity demos showing how a museum installation can react to visitors through a **webcam** (hand and body tracking with MediaPipe) or a **Kinect v2** (bodies and real distance).

Each demo is one scene with two short scripts:

- **`…Tracker.cs`** talks to the hardware and gives you plain data: a list of points, a mask image, a distance.
- **`…Interaction.cs`** uses that data for one small interaction. Replace this file with your own idea.

The Android AR demos are in a separate project: **[umu-demo-ar](https://github.com/IsabelleIKN/umu-demo-ar)**.

> **Work in progress.** The demo scenes below are being built. For now the project holds two hardware checks in `Assets/Checks/`.

## The demos

| Scene | Hardware | What it shows |
|---|---|---|
| `Webcam_Hands` | Webcam | Pinch thumb and index finger to grab and move an object |
| `Webcam_Hands_Multi` | Webcam | Up to 6 hands touch floating objects to reveal them |
| `Webcam_Body` | Webcam | Raise both hands to light up an exhibit |
| `Webcam_Body_Multi` | Webcam | Counts up to 4 people; each person can trigger something |
| `Webcam_Layers` | Webcam | Visitors are cut out and placed *inside* a picture, between a background and a foreground |
| `Kinect_Body` | Kinect v2 | Up to 6 skeletons; picks out the person closest to the screen |
| `Kinect_DepthLayers` | Kinect v2 | Your real distance decides which layer you see; step closer to reveal |

Each hardware folder has its own README with setup steps, common problems and tips for the museum:

- [Assets/Demos/Webcam/README.md](Assets/Demos/Webcam/README.md)
- [Assets/Demos/Kinect/README.md](Assets/Demos/Kinect/README.md)

You don't need a Kinect to use the webcam demos, and the other way round.

## Getting started

1. **Install Unity 6.3 LTS, version 6000.3.26f1**, with Unity Hub. If Hub's *Install Editor* list doesn't offer that exact version, get it from the [Unity download archive](https://unity.com/releases/editor/archive). Other 6.3 versions will probably work, but this is the one we tested.
2. **Clone this repo**, with GitHub Desktop or `git clone https://github.com/IsabelleIKN/umu-demo.git`.
3. **Run `setup.ps1` before opening the project.** It downloads the two plugins that can't be stored in this repo (see below). In File Explorer, open the project folder, type `powershell` in the address bar and press Enter. Then run:

   ```
   powershell -ExecutionPolicy Bypass -File .\setup.ps1
   ```

   It downloads about 350 MB. Running it again is safe: it skips what's already there.
4. **Open the project:** in Unity Hub, click *Add* → *Add project from disk* and pick the project folder. The first time takes several minutes while Unity imports everything.
5. Open a scene in `Assets/Demos/…`, follow that folder's README, and press Play.

## What `setup.ps1` downloads, and why

| What | Where it goes | Why it isn't in the repo |
|---|---|---|
| [MediaPipe Unity Plugin v0.16.3](https://github.com/homuler/MediaPipeUnityPlugin) (MIT licence), the webcam tracking | `Packages/com.github.homuler.mediapipe-0.16.3.tgz` | It's 277 MB, and GitHub doesn't accept files over 100 MB. |
| Microsoft's Kinect v2 Unity plugin | `Assets/Plugins/x86_64/` and `Assets/Standard Assets/` | Microsoft's licence doesn't allow us to share it. Everyone downloads it from Microsoft. |

Both are listed in `.gitignore`, so they never end up in a commit.

**If `setup.ps1` doesn't work,** do it by hand:

1. Download [com.github.homuler.mediapipe-0.16.3.tgz](https://github.com/homuler/MediaPipeUnityPlugin/releases/download/v0.16.3/com.github.homuler.mediapipe-0.16.3.tgz) into the `Packages` folder. Keep the file name as it is.
2. Download [KinectForWindows_UnityPro_2.0.1410.zip](https://download.microsoft.com/download/F/8/1/F81BC66F-7AA8-4FE4-8317-26A0991162E9/KinectForWindows_UnityPro_2.0.1410.zip) and unzip it.
3. Open the project in Unity, choose *Assets* → *Import Package* → *Custom Package…*, pick `Kinect.2.0.1410.19000.unitypackage` from the zip and click *Import*.
4. In Unity's Project window, select `Assets/Plugins/x86_64/KinectUnityAddin.dll`. In the Inspector, tick **Editor**, set its OS to *Windows* and CPU to *x86_64*, and click *Apply*. Without this, Kinect scenes fail with `DllNotFoundException: KinectUnityAddin`.

## Project layout

```
README.md            this file
setup.ps1            downloads the two plugins
Packages/            the MediaPipe package lands here
Assets/Demos/
  Webcam/            README.md and one folder per webcam demo
  Kinect/            README.md and one folder per Kinect demo
Assets/Checks/       hardware checks used while building the demos (will be removed)
```

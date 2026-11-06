# Hardware demo project — plan

Goal: students clone the repo, install what a demo's README says, plug in the hardware, open a scene, press Play, and see a small working interaction. The code should be short enough to read in one sitting and easy to take apart.

Context we're designing for: museum installation, well-lit space, **families with kids interacting at the same time**, Windows laptops, Kinect v2, webcams and an Android phone/tablet.

---

## Decisions

| Topic | Decision | Why |
|---|---|---|
| Unity version | **Unity 6.3 LTS (6000.3.26f1)**, already installed here | Long-term support, works with the current AR Foundation (6.3) and MediaPipe plugin. |
| Kinect v2 | **Microsoft's free Kinect v2 Unity plugin** + Kinect for Windows SDK 2.0 | Free, still downloadable (checked 2026-11-06), and gives raw body/depth data we can wrap in very little code. Fallback if it fails on Unity 6.3: the "Kinect v2 Examples" Asset Store package (€23 per seat, can't be shared in the repo). |
| Webcam tracking | **MediaPipe Unity Plugin v0.16.3** (MIT licence) | One library covers hands, body pose (multi-person) and person cut-out. It runs on Windows on the CPU only; the plugin calls Windows support "experimental". Fallback: Unity's Inference Engine with the Blaze hand/pose models, which needs much more code. |
| "Depth layers" | **Webcam: person cut-out** placed between a background and foreground layer. **Kinect: real distance**, so stepping closer reveals content. | The cut-out handles a whole family in one mask and holds up well in good light. Webcam depth estimation (MiDaS) is slow and rough, and would add a second library. Kinect measures real distance, so it can do "near/far" layers properly. |
| Project layout | **Two Unity projects in two repos**: `umu-demo` for the Windows demos (webcam + Kinect), and `umu-demo-ar` for Android AR | You said "a demo project", so this departs from that. Splitting means the AR team doesn't download 290 MB of MediaPipe or install the Kinect SDK, nobody has to switch build platform (which re-imports everything), and the Android build doesn't pull in Windows-only plugins. Separate repos keep git simple. |
| Getting MediaPipe into the project | A **`setup.ps1` script** downloads the 290 MB package into `Packages/`, with a manual download link in the README as backup | Too big for GitHub (100 MB file limit). Git LFS would run out of free bandwidth with a whole class cloning. |
| Claude ↔ Unity connection | **Unity CLI** (free, official) with its MCP server, plus the `com.unity.pipeline` package in each project | Unity deprecated the MCP server in its AI Assistant package and replaced it with this. The pipeline package is only for building the demos: **remove it from both projects before handing them to students.** |
| MediaPipe calling style | **Synchronous "video" mode**, called from `Update()` | No threads or callbacks for students to untangle. Costs some frame rate, which is fine for a demo. |
| Rendering | Universal Render Pipeline (URP), placeholder shapes and images we make ourselves | No art licensing questions, and students swap in their own content. |

## How the code is shaped

Every demo uses the same two-part pattern so students learn it once:

- **`…Tracker.cs`** talks to the hardware and exposes plain data (lists of points, a mask texture, a distance). It shows an on-screen status line such as *"Kinect not found: is it powered, on USB 3, and does the Kinect Configuration Verifier pass?"*
- **`…Interaction.cs`** reads that data and does the small museum interaction. This is the file students replace with their own idea.

Rules for all scripts:
- About 100 lines or fewer, plain C# with `Start()`/`Update()`, settings as public Inspector fields.
- No shared framework, base classes, events, singletons or async code. A few duplicated lines across demos are fine if each file reads on its own.
- Comments explain *why*, aimed at someone new to the hardware.
- We don't use MediaPipe's own sample scenes. They're built for flexibility, not readability.

## The demos

### DesktopDemos (Windows)

| Scene | Hardware | What you see | Interaction |
|---|---|---|---|
| `Webcam_Hands` | Webcam | Your hand drawn as dots over the camera image | Pinch (thumb + index finger) to grab and move an object |
| `Webcam_Hands_Multi` | Webcam | Up to 6 hands (about 3 people) | Touch floating objects to reveal or pop them |
| `Webcam_Body` | Webcam | Stick figure over one person | Raise both hands to trigger something (light up an exhibit) |
| `Webcam_Body_Multi` | Webcam | Up to 4 people, each with their own colour | Counts people in front of the camera; each person can trigger |
| `Webcam_Layers` | Webcam | Visitors cut out and placed *inside* a scene, between a background and a foreground layer | Walk into frame to "enter" the picture |
| `Kinect_Body` | Kinect v2 | Up to 6 skeletons with stable IDs; the closest person is highlighted | Hand above head triggers; shows how to pick "the person in front" |
| `Kinect_DepthLayers` | Kinect v2 | Depth image coloured in near / middle / far bands | Each visitor's real distance decides which layer they see; step closer to reveal |

### ARDemos (Android)

| Scene | Hardware | What you see | Interaction |
|---|---|---|---|
| `AR_ImageMarker` | Android phone/tablet + printed marker | A 3D object and info label appear on the printed image | Point the camera at the marker; a printable marker PDF is included |
| `AR_PlaceObject` | Android phone/tablet | The floor or a table is detected | Tap to place an object (e.g. a statue), tap again to move it |

## Documentation

- **Top-level `README.md`**: what's in the repo, which Unity version to install, running `setup.ps1`, which project to open for which hardware.
- **One `README.md` per hardware folder** (inside `Assets/Demos/…`, so it shows in Unity's Project window and on GitHub). Each one has:
  1. What you need (hardware, cables, adapters)
  2. Installing drivers/SDK, step by step
  3. Checking the hardware works *outside* Unity first (Windows Camera app; Kinect Configuration Verifier; "Google Play Services for AR" on the device)
  4. Opening and running the scene
  5. How the code works: a walkthrough of the two scripts
  6. Common problems
  7. Installation tips for the museum: camera height for kids, avoiding windows behind visitors (webcam), avoiding direct sunlight and staying within 0.5–4.5 m (Kinect), and processing on-device with nothing recorded
  8. Ideas for taking it further

Known limits we'll state up front rather than hide:
- Webcam multi-person tracking doesn't keep people's identity between frames, so colours can swap when people cross. Small children and overlapping people are detected less reliably. Hands aren't linked to a person. Kinect has none of these problems, which is a useful comparison for students.
- MediaPipe runs on the CPU on Windows, so slower laptops get lower frame rates. We'll use the "lite" models.

## Build order

**Phase 0: Check the risky parts first** (before writing any demo code)
1. Kinect v2 plugin imports into Unity 6.3 and reports bodies on this machine (Windows 11). *You plug in the Kinect.* Also check the plugin's licence to see whether its files can go in the repo; if not, students import it themselves as a README step.
2. MediaPipe v0.16.3 hand tracking runs on the webcam in Unity 6.3.
3. An empty AR Foundation scene builds and runs on your Android device.

If any of these fails, we switch to its fallback before going further.

Results:
- **MediaPipe: passed (2026-11-06).** v0.16.3 imports into 6000.3.26f1 with no errors or warnings. Hand tracking on the laptop's integrated webcam finds one and two hands, and the dots line up with the hand. Detection takes about 17 ms per frame with no hands and 32–53 ms with hands (640×480, CPU, in the editor). Things learned for the demos:
  - The model (`hand_landmarker.bytes`, in the package's `PackageResources/MediaPipe`) can be loaded through a plain `TextAsset` Inspector field, so no file-loading code is needed.
  - The plugin's `TextureFrame.ReadTextureOnCPU(webcam, false, true)` does the vertical flip MediaPipe needs in one call.
  - Unity lists virtual cameras (OBS, Meta Quest) alongside real ones, so each webcam script gets a camera-name field.
  - The check lives in `Assets/Checks/`; delete it once `Webcam_Hands` exists.
- **Kinect: passed (2026-11-06).** Microsoft's plugin (`KinectForWindows_UnityPro_2.0.1410.zip`, only its `Kinect.2.0.1410.19000.unitypackage`) imports into 6000.3.26f1 and compiles with no errors or warnings. It tracked one body at 1.18 m with a stable ID: 15 of 25 joints tracked, 10 inferred (legs behind a desk). The laptop's AMD USB 3.1 controller works with the sensor. Things learned:
  - **The plugin can't go in the repo.** The SDK EULA only lets you redistribute files listed in `REDIST.TXT` (this SDK ships none) or code from its Samples folder. The Unity plugin is a separate download and isn't covered. So `setup.ps1` will also download the zip from Microsoft and unpack the package into `Assets/`, and those files are git-ignored. This also keeps the project compiling for everyone, so students without a Kinect can still open the webcam scenes.
  - **The 64-bit DLL arrives disabled for the editor**, which would make Play mode fail with `DllNotFoundException`. `setup.ps1` has to switch on Editor (Windows, x86_64) for `Assets/Plugins/x86_64/KinectUnityAddin.dll`.
  - **For the students' museum product:** the EULA requires a Kinect app that is handed to others to show a privacy statement and Microsoft's telemetry notice, and to say it is for use only with the Kinect v2 sensor. That goes in the Kinect README.
  - **Tracking freezes for a few seconds every few seconds**, in Unity and in the SDK's own app alike, so it isn't our code. Windows logs no disconnects, so the sensor stays connected and its stream stalls. Measured over 30 s of depth stream: 495 frames instead of ~900, in a steady rhythm of about 6 s at full speed and 6.5 s of nothing. Kinect Configuration Verifier shows a warning on the USB controller row, and its own stream test hangs. Cause: this laptop's AMD USB 3.1 controllers (Microsoft asks for Intel or Renesas). Lenovo Performance mode and turning off USB selective suspend didn't help (506 frames, same rhythm). Moving to the neighbouring port didn't either (505), because both ports sit behind a USB 3.2 hub built into the laptop (Genesys Logic 05E3:0620), and Microsoft says the Kinect shouldn't go through a hub. Four measurements, same ~12 s cycle every time. **Accepted for now:** we build on this laptop, since tracking is correct between stalls, and the Kinect demos must cope with tracking dropping out for a few seconds (visitors walking out of view cause the same thing). Not tried: a USB-C port with a USB-C-to-A adapter, or the sensor on a PC with an Intel processor. Do the second before buying hardware for the museum. If every port stalls, test the Kinect on a PC with an Intel USB controller to rule out the sensor itself. This goes in the Kinect README's "Common problems", and the museum PC should have an Intel or Renesas USB 3 controller (on a desktop, a Renesas PCIe USB 3 card).
  - The check lives in `Assets/Checks/`; delete it once `Kinect_Body` exists.
- **Android AR: builds, not yet run on a device (2026-11-06).** In `umu-demo-ar`: AR Foundation and Google ARCore XR Plugin **6.3.5** (Unity's recommended version for 6000.3; 6.7.1 is newer but not verified for this editor), build target Android, ARCore loader enabled in XR Plug-in Management. The check scene `Assets/Checks/ARCheck.unity` (AR Session, XR Origin, plane detection, status line) builds to a 44 MB APK with no errors. The first IL2CPP build took about 27 minutes. Things learned:
  - XR Project Validation flags that ARCore with Vulkan needs **minimum Android 10 (API 29)**; its automatic fix sets that. Fine for AR-capable devices.
  - **URP needs the AR Background Renderer Feature** on the Android renderer (`Assets/Settings/Mobile_Renderer.asset`), or the camera image is black. Validation does *not* flag this, so the AR README must mention it.
  - The template's app ID was replaced with `com.umudemo.ardemos`.
  - **Still to do:** install on a device and check the camera image, the `SessionTracking` state and planes appearing.

**Phase 1: Skeleton.** Both Unity projects, folder structure, `.gitignore` adjusted (the current one only covers a project at the repo root), `setup.ps1`, top-level README.

**Phase 2: Webcam demos** (5 scenes + README)

**Phase 3: Kinect demos** (2 scenes + README)

**Phase 4: AR demos** (2 scenes + printable marker + README)

**Phase 5: Fresh-eyes pass.** A clean clone, following only the READMEs, ideally on a student-type laptop.

**Testing split:** I make sure everything compiles and builds (Windows player and Android APK). You check each interaction in front of the hardware using a short checklist I'll add per scene, since I can't stand in front of the camera.

## Repo layout

```
umu-demo/                   repo 1, Unity project (Windows): webcam + Kinect
  README.md                 start here
  setup.ps1                 downloads the MediaPipe package
  Packages/                 MediaPipe .tgz lands here (git-ignored)
  Assets/Demos/
    Webcam/   README.md, Hands/, HandsMulti/, Body/, BodyMulti/, Layers/
    Kinect/   README.md, Body/, DepthLayers/

umu-demo-ar/                repo 2, Unity project (Android)
  README.md                 start here
  Assets/Demos/
    ImageMarker/, PlaceObject/
  Markers/marker.pdf
```

Each README links to the other repo, so students can find both.

Before handing out: remove the template's `TutorialInfo/`, `Readme.asset` and `SampleScene`, and the `com.unity.pipeline` package, from both projects.

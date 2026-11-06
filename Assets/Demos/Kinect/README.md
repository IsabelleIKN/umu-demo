# Kinect v2 demos

These demos use Microsoft's **Kinect v2** sensor. Unlike a webcam, it measures how far away everything is, and it tracks up to 6 people with a stable ID each, so it always knows who is who. It uses Microsoft's free Kinect v2 Unity plugin, which `setup.ps1` downloads.

Back to the [main README](../../../README.md). The Android AR demos are in [umu-demo-ar](https://github.com/IsabelleIKN/umu-demo-ar).

## 1. What you need

- **A Kinect v2 sensor.** This is the Kinect for Xbox One, or the identical "Kinect for Windows v2".
- **The Kinect Adapter for Windows.** The Xbox sensor has a special plug; the adapter turns it into a power supply plus a normal USB 3 cable. The Kinect for Windows v2 comes with one.
- **A free USB 3 port directly on the computer.** USB 3 ports are often blue or marked *SS*. Not through a USB hub, docking station or extension cable.
- **A 64-bit Windows 10 or 11 PC.** Microsoft asks for a USB 3 controller made by **Intel or Renesas**. Others may work, or may not: see "Tracking freezes" in section 6.

## 2. Installing

1. Download and install the [Kinect for Windows SDK 2.0](https://www.microsoft.com/en-us/download/details.aspx?id=44561) from Microsoft, and restart the computer.
2. Plug the adapter's power supply into the wall and its USB cable into the computer, then plug the sensor into the adapter. Windows installs the driver by itself. In Device Manager it shows up under *Kinect sensor devices*.
3. The Unity plugin comes from `setup.ps1` (see the main README).

## 3. Check the Kinect works outside Unity

1. From the Start menu, open **SDK Browser v2.0 (Kinect for Windows)**.
2. Run **Kinect Configuration Verifier**. Every row should turn green. An orange warning on the USB row means the USB controller may cause trouble (section 6).
3. In the SDK Browser, run the **Body Basics-WPF** sample and stand 1.5–3 m in front of the sensor. You should see your skeleton. The white light on the sensor turns on whenever a program is using it.

If this doesn't work, the Unity demos won't either, so fix it here first.

## 4. Opening and running a demo

1. In Unity's Project window, open a scene from one of the folders here, for example `Body/Kinect_Body`.
2. Press Play and step in front of the sensor. The status line at the top tells you whether Unity found the Kinect.

## 5. How the code works

*Written together with the demos.*

## 6. Common problems

| Problem | What to try |
|---|---|
| `DllNotFoundException: KinectUnityAddin` when pressing Play | Run `setup.ps1`. If you imported the plugin by hand, select `Assets/Plugins/x86_64/KinectUnityAddin.dll`, tick **Editor** in the Inspector (OS *Windows*, CPU *x86_64*) and click *Apply*. |
| The status line says the Kinect isn't available | Is the adapter's power supply plugged in (it has a small light)? Is the USB cable in a USB 3 port, directly on the computer? Does the Configuration Verifier pass (section 3)? |
| No body is found | Stand 0.5–4.5 m away, with your whole body in view. Up close, the sensor only sees part of you. |
| Arms or legs jump around | The sensor can't see those joints (behind a table, another person, or out of view) and is guessing them. The code can tell these "inferred" joints apart from tracked ones. |
| **Tracking freezes for a few seconds, every few seconds** | Almost always the USB controller. The SDK's own Body Basics sample freezes too, so it isn't your code. Try every USB 3 port, including a USB-C port with a USB-C-to-A adapter. On a desktop PC, a PCIe USB 3 card with a Renesas chip is the usual fix. Changing the Windows power mode or turning off USB power saving didn't help on our test laptop. |

About that last one: our test laptop has an AMD USB controller, and its ports sit behind a USB hub built into the laptop. Tracking runs for about 6 seconds and then stops for about 6 seconds, over and over. The demos are written to cope with tracking dropping out, because a visitor walking out of view causes the same thing. **For the museum, choose a PC with an Intel or Renesas USB 3 controller and test the Kinect on it before you buy.**

## 7. Tips for the museum

- **Distance:** bodies are tracked from 0.5 m to 4.5 m. Plan the floor so visitors naturally stand in that range, for example with a floor marking.
- **Height:** place the sensor somewhere between knee and head height (about 0.6–1.8 m), pointing at where people stand, so it sees both children and adults.
- **No direct sunlight** on the visitors or the sensor. The Kinect measures distance with infrared light, and sunlight drowns it out. Ordinary indoor lighting is fine, and so is darkness.
- **Up to 6 people at once,** each with their own ID that stays the same while they're in view. A seventh person is ignored.
- **Privacy:** the demos store nothing and send nothing over the network. Say so on a sign. (Microsoft's own software does send anonymous usage data; see below.)

### If you hand your Kinect app to someone else (for example the museum)

Microsoft's licence (installed with the SDK as `C:\Program Files\Microsoft SDKs\Kinect\v2.0_1409\SDKEula.rtf`) sets conditions for a Kinect app that you give to others. In short, your app must:

- show a **privacy statement** saying how your app collects and uses data,
- include **Microsoft's note about telemetry**: when the app is used with a Kinect, Microsoft collects anonymous technical data (operating system, processor, graphics chip, memory and so on). Copy the exact required wording from the licence file, and fill in your app's name,
- say in its documentation that it is **intended for use only with the Kinect v2 sensor**, and
- show **your own copyright notice**.

The museum PC needs the Kinect driver too: install the [Kinect for Windows Runtime 2.0](https://www.microsoft.com/en-us/download/details.aspx?id=44559) (or the full SDK) on it.

## 8. Ideas for taking it further

*Written together with the demos.*

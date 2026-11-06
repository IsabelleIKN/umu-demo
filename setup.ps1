# setup.ps1: run this once after cloning, BEFORE opening the project in Unity.
#
# It downloads the two things this repo can't contain:
#   1. The MediaPipe Unity Plugin (277 MB, too big for GitHub)   -> Packages\
#   2. Microsoft's Kinect v2 Unity plugin (its licence doesn't
#      allow us to share it)                                     -> Assets\Plugins and Assets\Standard Assets
#
# Run it from a PowerShell window in this folder:
#   powershell -ExecutionPolicy Bypass -File .\setup.ps1
#
# Running it again is safe: it skips anything that is already in place.

$ErrorActionPreference = 'Stop'
Set-Location $PSScriptRoot

# True if the file exists and its SHA-256 fingerprint matches, so we know the download is complete and unchanged.
function Test-Download($path, $sha256) {
    (Test-Path $path) -and ((Get-FileHash $path -Algorithm SHA256).Hash -eq $sha256)
}

# --- 1. MediaPipe Unity Plugin ---------------------------------------------------------------
# Packages\manifest.json points at this exact file name.
$mediaPipeFile = 'Packages\com.github.homuler.mediapipe-0.16.3.tgz'
$mediaPipeUrl  = 'https://github.com/homuler/MediaPipeUnityPlugin/releases/download/v0.16.3/com.github.homuler.mediapipe-0.16.3.tgz'
$mediaPipeHash = 'CC3E77A219E0B99618AE3BE64C31A566197DEEDC69C1E136ACF52D65D7CF2E79'

if (Test-Download $mediaPipeFile $mediaPipeHash) {
    Write-Host 'MediaPipe: already downloaded.'
} else {
    Write-Host 'MediaPipe: downloading 277 MB from GitHub. On a slow connection this takes a few minutes...'
    curl.exe --location --fail --output $mediaPipeFile $mediaPipeUrl
    if (-not (Test-Download $mediaPipeFile $mediaPipeHash)) {
        throw "MediaPipe download failed or is incomplete. Delete $mediaPipeFile and run setup.ps1 again."
    }
}

# --- 2. Kinect v2 Unity plugin ---------------------------------------------------------------
$kinectDll  = 'Assets\Plugins\x86_64\KinectUnityAddin.dll'
$kinectUrl  = 'https://download.microsoft.com/download/F/8/1/F81BC66F-7AA8-4FE4-8317-26A0991162E9/KinectForWindows_UnityPro_2.0.1410.zip'
$kinectHash = '1FFBE982AE8B405AABE3ACB576D45D699613FC1A3A9097F2AEA079745E7FE21A'
$package    = 'KinectForWindows_UnityPro_2.0.1410/Kinect.2.0.1410.19000.unitypackage'

if (Test-Path $kinectDll) {
    Write-Host 'Kinect plugin: already installed.'
} else {
    $temp = Join-Path $env:TEMP 'umu-demo-setup'
    New-Item -ItemType Directory -Force "$temp\unpacked" | Out-Null
    $zip = "$temp\KinectForWindows_UnityPro_2.0.1410.zip"

    Write-Host 'Kinect plugin: downloading 72 MB from Microsoft...'
    curl.exe --location --fail --output $zip $kinectUrl
    if (-not (Test-Download $zip $kinectHash)) { throw 'Kinect plugin download failed or is incomplete. Run setup.ps1 again.' }

    # The zip holds three Unity packages (body, face, gestures). We only need the body one.
    tar -xf $zip -C $temp $package

    # A .unitypackage is a .tar.gz with one folder per file inside. In each folder,
    # 'pathname' says where the file belongs in the project and 'asset' is the file itself.
    tar -xzf "$temp\$package" -C "$temp\unpacked"
    foreach ($entry in Get-ChildItem "$temp\unpacked" -Directory) {
        $asset = Join-Path $entry.FullName 'asset'
        if (-not (Test-Path $asset)) { continue }   # folders have no 'asset' file
        $target = Get-Content (Join-Path $entry.FullName 'pathname') -TotalCount 1

        # Keep the C# code and the 64-bit DLL. Skip the 32-bit and Windows Store ("Metro") DLLs.
        if ($target -like 'Assets/Standard Assets/*' -or $target -eq 'Assets/Plugins/x86_64/KinectUnityAddin.dll') {
            New-Item -ItemType Directory -Force (Split-Path $target) | Out-Null
            Copy-Item $asset $target
        }
    }

    # Left to itself, Unity imports this DLL switched off for the Editor, and Play mode then
    # fails with "DllNotFoundException: KinectUnityAddin". This .meta file switches it on
    # for the Editor and for 64-bit Windows builds.
    Set-Content "$kinectDll.meta" -Encoding Ascii -Value @'
fileFormatVersion: 2
guid: 82e143d16729051459ae6a54b37e9eab
PluginImporter:
  serializedVersion: 3
  platformData:
    Any:
      enabled: 0
      settings: {}
    Editor:
      enabled: 1
      settings:
        CPU: x86_64
        OS: Windows
        DefaultValueInitialized: true
    Win64:
      enabled: 1
      settings:
        CPU: x86_64
'@

    Remove-Item $temp -Recurse -Force
    Write-Host 'Kinect plugin: installed.'
}

Write-Host ''
Write-Host 'Setup done. You can now open this folder in Unity 6.3.'

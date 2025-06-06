# Powerwall Unity package

## Abstract
Small unity package with the aim to create a perspective projection on a Powerwall. Hardware is a ViveTracker and a Projector.
The ViveTracker position is provided by SteamVR. Unity can access the position either through SteamVR-Plugin (Asset store) or in the future with the OpenXR package.
Currently the OpenXR does only support controllers (left and right hand) but not the ViveTracker

## Requirements
- SteamVR installed as app (tested with 1.20)
- Download [SteamVR-Plugin](https://assetstore.unity.com/packages/tools/integration/steamvr-plugin-32647) and place it in the `Plugin` folder of the package

## Documentation
### Tracking object
- New Unity project
- Install SteamVR Plugin asset
- Create a `Steam VR_Tracked_Object` and add it as a component to the scene
    - Switch the `Index` field to match the index of the tracker (usually somewhere between 2 and 3)

### Tracking without HMD
- First, calibrate ground with headet plugged in
- Go to `<YOURSTEAMDIRECTORY>\Steam\steamapps\common\SteamVR\drivers\null\resources\settings\default.vrsettings`
    - Change `enabled` to `true`
- Go to `<YOURSTEAMDIRECTORY>\Steam\steamapps\common\SteamVR\resources\settings\default.vrsettings`
    - Change `requireHMD` to `false`
    - Change `forcedDriver` to `null`
    - Add and change `activateMultipleDrivers` to `true`,
- Now, only the USB needs to be plugged in, not hdmi





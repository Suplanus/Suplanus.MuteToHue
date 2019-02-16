# Suplanus.MuteToHue
Switch on a Hue lamp on mute.

**Installation**
- C#
  - Change the file path to the python script (todo: have to fix this)
- Hue
  - Setup up your lamp
  - Set brightness
  - Set color
- Python
  - Install `phue` via `pip`
  - Change ip `HUEBRIDGEIP` of Hue bridge
  - Change name of hue lamp `LIGHTNAME`
  - Execute the script once with replaced `$STATE$` to test if registered
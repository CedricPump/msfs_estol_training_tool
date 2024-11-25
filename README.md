<center><img src="./doc/icon.png" alt="drawing" width="200"/></center>

# MSFS eSTOL Training Tool

This Tool is intended for training purposes only.
The numbers give a quick feedback and rough estimate of your performance. They do not guarantee any accuracy.
Do not challenge any competition score based on this tools' estimation alone.\n Make sure to record your flight for any necessary score validation.\n\nPress Enter to accept

This tool is not officially associated with National STOL Series.

## Concept

This tool give quick performance data about a STOL competition run by recording Takeoff and Landing.
Distances are calculated based on an initial start point and heading marking the start line.
from here distances are measures along the heading axis.

<center><img src="./doc/STOL_Training_Overview.jpg" alt="drawing" width="200"/></center>

StartPoint: initial start point on Start Line
TakeoffPoint: Location where takeoff is detected. First point where plane is not on ground.
TouchdownPoint: Location where the Plane touched down first.
StopPoint: Location where plane came to a full stop.

Takeoff distance: Distance from StartPoint to TakeoffPoint
Touchdown distance: Distance from StartPoint to TouchdownPoint
Landing distance: Distance from StartPoint to StopPoint
Stopping distance: Distance from TouchdownPoint to StopPoint

<center><img src="./doc/STOL-Training-Tool.png" alt="drawing" width="200"/></center>

This tool has two modes: OpenWorld and Presets

### Open World 

The user can set the startpoint everywhere using parking breake or smoke.

###  Presets

Star Point are provided as preset for known eSTOL Fields

## Restrictions

The tool polls aircraft telemety data in an interval off 250ms. This limits the prescision the tool can detect any state changes.
For tochdown and landing it refers to Simconnect variable "SIM ON GROUND". Detection qualaty depends on Sim dettecting this parameter.
The tool does not access Takeoff or Touchdown Events (yet).
The start point and touchdown point are set by plane position. this position most likely refers to planes center of gravity and not to wheels touchdown point.
Since the offset between those is the same for lineup, takeoff and landing if may be ignored. For differen Plane Types, especially those of different size, this may be a Problem when working with presets.

## Usage

start `msts_estol_training_tool.exe`

```
...
```

- setup user name used for InfluxDB upload. Leave empty to ignore.
- select mode OpenWorld (default, timeout 10sec) or select a preset.
- lineup with start line and takeoff -> "takeoff detected"
- fly pattern and land -> "landing detected"
- result is shown after full stop
  - result summary is printed to console
  - result is saved to .csv file
  - result is pushed to InfluxDB
 
Exsample:

```
...
```

v1.4.3

- incresed BankAngle Threshold to 75
- renamed violations to deviations
- added deviation severities
- fixed deviation touchdownVS 
- fixed result MaxG MaxVS
- fixed result formatting

v1.4.2

- added Prop strike for Rans S6S
- added max G and VS to result output
- fixed button anchor
- added violations / warnings for: HightBankAngle HighClimbRate OverspeedVNE OverspeedFlaps
- added assits violation

v1.4.1

- added Auto Select Button (selects nearest reference line)
- added live violations box
- added plane type label
- added type specific G-Force and VSpeed Limits
- fixed Prop strike after result bug
- added alignment state down field

v1.4.0

- fixed Wilga Wheel detection
- added Beaver PlaneConfig

v1.3.9

- fixed Aklahoma field direction
- fixed UI GPX checkbock alignment
- fixed Wing Strike Contactpoints
- added Prop Strike detection
- added Prop Strike threshold

v1.3.8

- added wingtip indicators
- added collision indicator
- added plane config
- added collision indexes
- prepare contact point fixes

v1.3.7

- fixed scout offset
- fixed recor time zulu
- fixed recording broken by plane type

v1.3.6

- adapted startline for new KAXQ Scenery

v1.3.5

- fixed too many popups
- updated gear offset for Scout
- added event unflip
- removed event ExcessiveTouchdownSpin

v1.3.4

- added unflip
- added confirmation option in config
- added debug log
- minor UI fixes

v1.3.3

- fixed blocking export file from 1.3.1

v1.3.2

- added darkmode
- added transparency
- added telport popup
- added gpx recording checkbox
- removed console window

v1.3.1

- added GPX Recorder Feature

v1.3.0

- added session key
- added anitstall detection wip
- fixed typo in wingstrike
- added sores xml server
- added takeoff score to xml server

v1.2.9

- added aligned state display to UI
- added selected preset to telemetry
- added excessive spin buffer threshold of 1°

v1.2.8

- added spin violaton angle to flags calculation

v1.2.7

- changed simvars access to RequestDataOnSimObject
- added get date by frames setting
- increased polling interval

v1.2.6

- increased polling intervall for more pescise results 250 ms -> 100 ms
- seperated excessive spin events stop, touchdown and max
- added gear offset for C152 Trike
- added plane change detection
- changed disclaimer text
- added sim not connected error handling

v1.2.5

- added Maule Tundro to GearOffset
- added gear offset for maule m7
- added exception handling to events
- added send data check to events
- added Violations
- added Events: EXCESSIVE_VSPEED, SCRATCH, EXCESSIVE_SPIN, EXCESSIVE_G, WINGSTRIKE, TOUCH_N_GO

v1.2.4

- added piper comanche gear offset
- fixed Rans Gear offset for TD
- added eSTOL Events

v1.2.3

- added debug plane position
- added wind indicator
- added wind indicator before init
- added BN2 Islander
- added auto update
- added Plane alignment helper output
- improved field canvas autoscale
- improved gneral scaling (for vr users)
- improved showing TD / TO / Stop positions correctly
- improved result decimals
- fixed husky gear offset
- fixed c170bt gear offset
- fixed always on top on start

v1.2.2

- introduced settings saving
- added cycle state climbout (2nd rebound point on takeoff feature)
- added privacy disclaimer popup
- added always on top selection persistence
- added unit selection persistence
- added send telemetry persistence
- added send results persistence
- added UI loading early
- updated verison

v1.2.1

- corrected default gear offset by +1.5 for default wheel deflation
- unsing contact point vars for ongorund reading
- splitted preset.json
- added reload clickspot
- added link to privacy policy
- refactoring
- updated version
- splitted preset.json
- added reload clickspot
- added link to privacy policy
- refactoring

v1.2.0

- updated default gear offset
- updated scenery coordinates
fixes for field coordinates
pony coordinate fix
- fixed gear offset in telemetry
- updated old and Europe STOL fields

note:
This Update overhauls the underlying position reference system.
Please report any misalignments for plane types at teleport.

Wheel middle should align with the edge of the reference line.

v1.1.6

- fixed some spelling issues
- added custom timer offset
- changed stol field allignment
- added violations
- added 42VA Virginia Beach Heritage (old system)
- added debug button
- added debug output
- added Wheel RPM check for taildragging detection
- added max spin, pitch and bank checks
- added version to telemetry

v1.1.5

- added requested always on top feature

v1.1.4

- added draggin tail before line feature ... hopefully ... for BrownDog942
- improved disclaimer
- improved update link

Know Issues: requres .net Framework. see: known_issues.md

v1.1.3

- fixed fuel percent
- added unlimited fuel
- added GForce
- added takeoff -> hold to state machine
- added start (no offset button)
- fixed timer running on taxi
- fixed timer refresh rate

v1.1.2

- added Timer

v.1.1.1

- added config.json for setting custom intervals
- fixed PilotWeight
- added version update notification
reduced Telemetry Intervall (performance and traffic improvement)

v1.1.0

- changed database bucket
- added telemetry
- added send data checkboxes
- grafana magic

v1.0.4

- added unit selection

v1.0.3

- added unit selection

v1.0.2

- added visualizer
- added teleport feature
- added Gear to CG offset
- added basic GUI
- fixed create preset button

v1.0.1

- added teleport feature
- added Gear to CG offset
- added basic GUI
- fixed create preset button

v.1.0.0

- added teleport feature
- added Gear to CG offset
- added basic GUI

v0.2.0 Pre-release

- added preset creation mode
- added Music City field
- added Lonestar field

v.0.1.0 Pre-release

initiated Project
- added flight tracking
- added result calculation
- added result display
- added .csv export
- added influx send
- added presets
- added custom start
- added inithash
- added username
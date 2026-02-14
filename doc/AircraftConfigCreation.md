# Custom Aircraft Config Creation Guide

the properties of Sim planes are not as homogeneous as we would wish. While detail config worked fine for most taidraggers there are some settings that need to be adapted. 

this guide explains why aircraft cofigs are needed, what the config parameters mean, how to append a new config, and how to gain the necessary information.

## concept

STOL Training Tool calculates exact numbers for planes takeoff and landing. The distance is measured between the planes main gear and a reference line. Two facts make this task quite difficult:  
  
- tracked plane position is center of gravity, not wheel position. Wheel offset to CoG is different for every planen.   
  
- only main gear touchdown counts. in-sim touchdown event register any part of the plane touching the ground.   
  
these problems introduced the config variables gear offset to adjust the measurements to be take from main gear and gear collision Indices, allowing to check for dedicated wheel collision.  
  
additional parameters are added for types detection, prop collision handling and more.  

## config structure 

the aircraft cofigs is saved as a JSON file "planesConfig.json" containing a list of JSON objects.  

```
    [{
        "Class": "",
        "CollisionNoseIndex": 10,
        "CollisionPropIndex": 8,
        "CollisionWheelLeftIndex": 1,
        "CollisionWheelNoseTailIndex": 0,
        "CollisionWheelRightIndex": 2,
        "CollisionWheelWingtipLIndex": 3,
        "CollisionWheelWingtipRIndex": 4,
        "PropStrikeThreshold": 30,
        "DisplayName": "DEFAULT",
        "GearOffset": -0.45,
        "IsTaildragger": true,      
        "Key": "DEFAULT",
        "Regex": "^DEFAULT(?:\\|.*)?$",
        "MaxGForce": 2.0,
        "MaxVSpeed": -500
    }, ...]
```

## Parameters

- **Class**: STOL class (correctly not used)
- **CollisionNoseIndex**: nose contact point index if available
- **CollisionPropIndex**: prop contact point index if available (prop strike detection)
- **CollisionWheel_Index**: wheel Contact point of indicated wheel 
- **CollisionWingtip_Index**: wingtip Contact point for wing strike detection
- **PropStrikeThreshold**: pitch angle for prop strike detection in deg
- **DisplayName**: short display name 
- **GearOffset**: offset from CG plane position to main gear position 
- **IsTaildragger**: true if taidragger (for tail touch detection)
- **Key**: unique key (old type detection)
- **Regex**: Regex matches plane "type|model|title" (new type detection from v1.4.19?)
- **MaxGForce** G-Force limit for deviation 
- **MaxVSpeed** vertical speed limit for deviation



## appending new aircraft entry  

add a new template JSON object to the end of the list (watch the comma):  

```
    ,{
        "Class": "",
        "CollisionNoseIndex": 10,
        "CollisionPropIndex": 8,
        "CollisionWheelLeftIndex": 1,
        "CollisionWheelNoseTailIndex": 0,
        "CollisionWheelRightIndex": 2,
        "CollisionWheelWingtipLIndex": 3,
        "CollisionWheelWingtipRIndex": 4,
        "PropStrikeThreshold": 30,
        "DisplayName": "short aircraft nane",
        "GearOffset": -0.45,
        "IsTaildragger": true,      
        "Key": "unique key",
        "Regex": "Regex filtering model and type",
        "MaxGForce": 2.0,
        "MaxVSpeed": -500
    }
```

fill entries according to following section 

## obtaining config parameters 

### Regex (model, type, title)

The Regex parameters is the new form of plane to config matching. plane parameters model, type and title are concatenated to "type|model|title" the Regex is written to match the plane with all its variants. 
  
this approach is choosen to handle bar standardization for these parameters for developers, variations in usage, plane variants with same types and 2020 2024 value deviations.

get values:

- **SimvarWatcher** read values directly
- **From training tool**
Steps:
1. Load in with airplane
2. Start STOL Training Tool
3. Check Debugging
4. Click auto select
5. Click apply 

Aircraft identity parameters should be shown.  

### Key

unique key to access config.  
Old way of config matching using "type|model".  

### DisplayName

short display name of the plane. commonly used but presise and unique. 

e.g. C170B, Wilga 80X, Kitfox Comp, ...

### Gear Offset 

set adequate offset and check if plane teleports into the line. The aft edge of the wheel depilation should line up but not touch the line - barely not Bering a scratch. that's 0 ft score for landing and that's the reference.

if not aligning adapt Offset, restart tool and check again. Got no better method for now

### CollisionWheelXxxIndex

The wheel collision checks Simvars for contact points 0-20. most planes use the established standard of nose or tail wheel index 0, right and left main gear index 1 and 2.

some plane deviate from that. this can be seen if tilting the plane on ground and checking the green wheels indicator on the tool. 

test cases:
taidragger nose over, tail up - only main wheel indicators green. 
tip over to the side while pulling back touch a wing and raise a wheel. wheel indicators in that side is dark.  
nose wheel tail strike and hold - only main wheel indicators green.
tip over to the side while pushing forward touch a wing and raise a wheel. wheel indicators in that side is dark.  



if deviation choose an approach:

1. try and error: set an index, restart tool and check for success

2. monitor all Contact points: find out correct index using SimvarWatcher

### PropStrikeThrehold

the prop strike threshold is the pitch angle used to check if an propstrike is likely. many planes do not have a proper crash mechanic or evalen a prop or nose contact point configured. 

to get a proper value, pause the Sim and tilt the plane using Simvarwatcher by pitch variable until the prop disc allings with the ground. 

often a rough estimate is enough.

## Max GForce and VSpeed

aircraft stress limits. Standard by class and purpose (stock, backcountry, utitly, special STOL,...) or real life values If available.

... to be continued ... 




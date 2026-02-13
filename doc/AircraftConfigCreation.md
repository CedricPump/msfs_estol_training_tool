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

TBD

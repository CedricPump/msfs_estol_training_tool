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

the aircraft cofigs is saved as a JSON file "aircraft config.json" containing a list of JSON objects.  

```
to be added
```

## Parameters

- 

## appending new aircraft entry  

add a new template JSON object to the end of the list (watch the comma):  

```
to be added
```

fill entries according to following section 

## obtaining config parameters 

### Regex (model, type, title)

TBD

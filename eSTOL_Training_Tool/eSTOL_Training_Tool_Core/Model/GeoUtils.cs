﻿using System;
using System.Device.Location;

namespace eSTOL_Training_Tool
{
    public static class GeoUtils
    {
        public static double GetHeading(GeoCoordinate from, GeoCoordinate to)
        {
            // Convert degrees to radians
            double lat1Rad = from.Latitude * (Math.PI / 180);
            double lon1Rad = from.Longitude * (Math.PI / 180);
            double lat2Rad = to.Latitude * (Math.PI / 180);
            double lon2Rad = to.Longitude * (Math.PI / 180);

            // Difference in longitudes
            double dLon = lon2Rad - lon1Rad;

            // Calculate bearing using the formula
            double x = Math.Cos(lat2Rad) * Math.Sin(dLon);
            double y = Math.Cos(lat1Rad) * Math.Sin(lat2Rad) -
                       Math.Sin(lat1Rad) * Math.Cos(lat2Rad) * Math.Cos(dLon);

            double initialBearing = Math.Atan2(x, y);

            // Convert from radians to degrees
            initialBearing = initialBearing * (180 / Math.PI);

            // Normalize the bearing to be between 0° and 360°
            if (initialBearing < 0)
            {
                initialBearing += 360;
            }

            return initialBearing;
        }

        public static double GetShortestDistance(GeoCoordinate point, GeoCoordinate origin, double heading)
        {
            double earthRadius = 6371000; // Earth's radius in meters

            // Convert origin and point to radians
            double originLatRad = origin.Latitude * (Math.PI / 180);
            double originLonRad = origin.Longitude * (Math.PI / 180);
            double pointLatRad = point.Latitude * (Math.PI / 180);
            double pointLonRad = point.Longitude * (Math.PI / 180);

            // Convert heading to radians
            double headingRad = heading * (Math.PI / 180);

            // Convert GeoCoordinates to Cartesian (X, Y) coordinates
            double xOrigin = earthRadius * Math.Cos(originLatRad) * Math.Cos(originLonRad);
            double yOrigin = earthRadius * Math.Cos(originLatRad) * Math.Sin(originLonRad);

            Console.WriteLine($"Origing cartesian: ({xOrigin},{yOrigin})");

            double xPoint = earthRadius * Math.Cos(pointLatRad) * Math.Cos(pointLonRad);
            double yPoint = earthRadius * Math.Cos(pointLatRad) * Math.Sin(pointLonRad);

            Console.WriteLine($"Point cartesian: ({xPoint},{yPoint})");

            // Direction vector of the line (unit vector)
            double dx = Math.Cos(headingRad);
            double dy = Math.Sin(headingRad);

            Console.WriteLine($"Direction Vector: ({dx},{dy})");

            // Normal vector (90° offset)
            double normalX = -dy;
            double normalY = dx;

            Console.WriteLine($"Normal Vector: ({normalX},{normalY})");

            // Calculate the perpendicular (shortest) distance
            double distance = Math.Abs((xPoint - xOrigin) * normalY + (yPoint - yOrigin) * normalX) /
                              Math.Sqrt(normalX * normalX + normalY * normalY);

            return distance;
        }

        public static (double, double) GetOffsetXYByHeading(GeoCoordinate threshold, GeoCoordinate touchdown, double headingDegrees) 
        {
            // Schritt 1: Heading in Radian umrechnen
            double headingRadians = headingDegrees * Math.PI / 180;

            // Schritt 2: Delta der Koordinaten berechnen (in Metern)
            double deltaX = (touchdown.Longitude - threshold.Longitude) * 111320; // Näherung für 1° Längengrad in Meter
            double deltaY = (touchdown.Latitude - threshold.Latitude) * 110540; // Näherung für 1° Breitengrad in Meter

            // Schritt 3: Koordinaten in das Runway-Koordinatensystem rotieren
            double xRunway = deltaX * Math.Cos(headingRadians) + deltaY * Math.Sin(headingRadians);
            double yRunway = -deltaX * Math.Sin(headingRadians) + deltaY * Math.Cos(headingRadians);

            return (xRunway, yRunway);
        }

        public static (double distanceAtAxis, double offsetFromAxis) GetDistanceAlongAxis(GeoCoordinate origin, GeoCoordinate point, double headingDegrees)
        {
            double distance = origin.GetDistanceTo(point);
            double angleToPoint = GetHeading(origin, point);
            double deltaAngle = GetMinDeltaAngle(angleToPoint, headingDegrees) * Math.PI / 180; // Delta-Winkel in Bogenmaß

            // Projektionen berechnen
            double distanceAtAxis = distance * Math.Cos(deltaAngle);

            double offsetFromAxis = distance * Math.Sin(deltaAngle);

            return (distanceAtAxis, offsetFromAxis);
        }

        private static double GetMinDeltaAngle(double angle1, double angle2)
        {
            // Normalize both angles to the range [0, 360)
            angle1 = angle1 % 360;
            if (angle1 < 0) angle1 += 360;

            angle2 = angle2 % 360;
            if (angle2 < 0) angle2 += 360;

            // Calculate the difference
            double delta = Math.Abs(angle1 - angle2);

            // Ensure the delta is the smallest angle (<= 180 degrees)
            if (delta > 180) delta = 360 - delta;

            return delta;
        }

        public static string ConvertToDMS(GeoCoordinate coordinate)
        {
            double lat = coordinate.Latitude;
            double lon = coordinate.Longitude;

            string latDirection = lat >= 0 ? "N" : "S";
            string lonDirection = lon >= 0 ? "E" : "W";

            lat = Math.Abs(lat);
            lon = Math.Abs(lon);

            // Latitude
            int latDegrees = (int)lat;
            int latMinutes = (int)((lat - latDegrees) * 60);
            double latSeconds = ((lat - latDegrees) * 60 - latMinutes) * 60;

            // Longitude
            int lonDegrees = (int)lon;
            int lonMinutes = (int)((lon - lonDegrees) * 60);
            double lonSeconds = ((lon - lonDegrees) * 60 - lonMinutes) * 60;

            // Combine results
            string latitudeDMS = $"{latDirection}{latDegrees}°{latMinutes}'{latSeconds:0.##}\"";
            string longitudeDMS = $"{lonDirection}{lonDegrees}°{lonMinutes}'{lonSeconds:0.##}\"";

            return $"{latitudeDMS},{longitudeDMS}";
        }
    }
}
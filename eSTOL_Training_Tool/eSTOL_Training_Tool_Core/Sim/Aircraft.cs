using System;
using System.Device.Location;
using eSTOL_Training_Tool.Model;
using eSTOL_Training_Tool_Core.Core;
using eSTOL_Training_Tool_Core.Model;

namespace eSTOL_Training_Tool
{
    public abstract class Aircraft
    {
        public bool isInit = false;

        // Ident
        public string Model { get; protected set; }
        public string Type { get; protected set; }
        public string Title { get; protected set; }
        // Position
        public double Altitude { get; protected set; }
        public double Height_AGL { get; protected set; }
        public double Latitude { get; protected set; }
        public double Longitude { get; protected set; }
        public double Heading { get; protected set; }
        // Movement
        public double GroundSpeed { get; protected set; }
        public double VerticalSpeed { get; protected set; }
        public double vX { get; protected set; }
        public double vY { get; protected set; }
        public double vZ { get; protected set; }
        public double gforce { get; protected set; }
        public double Airspeed { get; protected set; }

        public double RWheelRPM { get; protected set; }
        public double LWheelRPM { get; protected set; }
        public double CWheelRPM { get; protected set; }
        public double AuxWheelRPM { get; protected set; }

        // Orientation
        public double pitch { get; protected set; }
        public double bank { get; protected set; }
        // State
        public bool IsOnRundway { get; protected set; }
        public bool IsOnGround { get; protected set; }
        public bool IsEngineOn { get; protected set; }
        public bool IsParkingBreak { get; protected set; }
        public double Fuel { get; protected set; }
        public double FuelPercent { get; protected set; }
        public bool FuelUnlimited { get; protected set; }
        public string Airport { get; protected set; }
        public bool[] ContactPoints { get; protected set; } = new bool[21];

        // Env
        public double AltitudeAGL { get; protected set; }

        // Sim
        public bool IsSimConnected { get; protected set; } = false;
        public bool SimDisabled { get; protected set; } = false;
        // AntiCheat
        public bool CrashEnabled { get; protected set; } = false;
        public bool IsSlew { get; protected set; } = false;
        public int TimeAcceleration { get; private set; } = 1;
        // Weight
        public double PilotWeight { get; protected set; }
        public double TotalWeight { get; protected set; }
        public double MaxTotalWeight { get; protected set; }

        // Ambient
        public double WindX { get; protected set; } = 0.0;
        public double WindY { get; protected set; } = 0.0;
        public double Antistall { get; protected set; } = 0;
        public bool Autotrim { get; protected set; } = false;
        public bool AICtrl { get; protected set; } = false;

        // flight controls
        public double AileronPosition { get; protected set; } = 0.0;
        public double ElevatorPosition { get; protected set; } = 0.0;
        public double RudderPosition { get; protected set; } = 0.0;
        public double FlapsPercent { get; protected set; } = 0.0;
        public uint FlapsIndex { get; protected set; } = 0;
        public double ThrottlePosition { get; protected set; } = 0.0;

        public bool IsFlapsSet { get { return FlapsPercent > 0; } }

        public bool IsVNEOverspeed { get; protected set; } = false;
        public bool IsFlapsOverspeed { get; protected set; } = false;

        protected Config conf;

        public bool IsTaildragger { get
            {
                return GetPlaneConfig().IsTaildragger;
            }
        }

        public PlaneConfig GetPlaneConfig() 
        {
            return PlaneConfigsService.GetPlaneConfig(this.Type + "|" + this.Model);
        }

        public bool HasPlaneConfig()
        {
            return PlaneConfigsService.HasPlaneConfig(this.Type + "|" + this.Model);
        }

        public string GetDisplayName() 
        {
            var dispalyName = GetPlaneConfig().DisplayName;
            if (GetPlaneConfig().Key == "DEFAULT")
            {
                dispalyName = $"{this.Type} {this.Model}: \"{this.Title}\" ({GetPlaneConfig().DisplayName} config)";
            }
            return string.IsNullOrEmpty(dispalyName) ? this.Title : dispalyName;
        }

        public string toString()
        {
            return Model + " [" + Latitude + "," + Longitude + "," + Altitude + "] " + GroundSpeed + "knts " + Heading + "° ";
        }

        public enum EVENTS
        {
            SimStart,
            SimStop,
            Crashed,
            AircraftLoaded,
            FlightLoaded,
            LANDING_LIGHTS_TOGGLE,
            PARKING_BRAKES,
            PARKING_BRAKE_SET,
            SMOKE_OFF,
            SMOKE_ON,
            SMOKE_SET,
            SMOKE_TOGGLE,
            TOW_PLANE_RELEASE,
            HORN_TRIGGER,
            PAUSE_TOGGLE,
            PAUSE_ON,
            PAUSE_OFF,
            PAUSE_SET,
            SIM_RATE,
            SIM_RATE_DECR,
            SIM_RATE_INCR,
            SIM_RATE_SET
        };

        public Telemetrie GetTelemetrie()
        {
            return new Telemetrie
            {
                Position = getPositionWithGearOffset(),
                PositionCG = getPosition(),
                Altitude = this.Altitude,
                AltitudeAGL = this.AltitudeAGL,
                Height = 0.0,
                GroundSpeed = this.GroundSpeed,
                Heading = this.Heading,
                vX = this.vX,
                vY = this.vY,
                vZ = this.vZ,
                pitch = this.pitch,
                bank = this.bank,
                verticalSpeed = this.VerticalSpeed,
                gForce = this.gforce,
                mainWheelRPM = Math.Max(this.RWheelRPM, this.LWheelRPM),
                centerWheelRPM = this.CWheelRPM,
                FlapsPercent = this.FlapsPercent,
                FlapsHandlePosition = this.FlapsIndex,
                AileronsPercent = this.AileronPosition,
                ElevatorsPercent = this.ElevatorPosition,
                RudderPercent   = this.RudderPosition,
                OnGround = this.IsOnGround,
                AirSpeed = this.Airspeed,
                ThrottlePosition = this.ThrottlePosition
            };
        }

        private GeoCoordinate getPosition()
        {
            return new GeoCoordinate(this.Latitude, this.Longitude, this.Altitude * 0.3048);
        }

        public GeoCoordinate getPositionWithGearOffset() 
        {
            double offset = PlaneConfigsService.GetGearOffset(this.Type + "|" + this.Model);
            GeoCoordinate simPos = new GeoCoordinate(this.Latitude, this.Longitude, this.Altitude * 0.3048);
            return GeoUtils.GetOffsetPosition(simPos, this.Heading, -offset);
        }

        public Ident GetIdent()
        {
            return new Ident
            {
                Model = this.Model,
                Title = this.Title,
                Type = this.Type,
            };
        }

        public bool IsFlipped()
        {
            return (this.IsStopped() || this.IsOnGround) && this.pitch >= this.GetPlaneConfig().PropStrikeThreshold;
        }

        public bool IsStopped()
        {
            return this.GroundSpeed < Config.GetInstance().GroundspeedThreshold && this.MainGearOnGround();
        }

        public bool MainGearOnGround()
        {
            return LeftGearOnGround() || RightGearOnGround();
        }

        public bool LeftGearOnGround()
        {
            return ContactPoints[this.GetPlaneConfig().CollisionWheelLeftIndex];
        }

        public bool RightGearOnGround()
        {
            return ContactPoints[this.GetPlaneConfig().CollisionWheelRightIndex];
        }

        public bool TailNoseGearOnGround()
        {
            return ContactPoints[this.GetPlaneConfig().CollisionWheelNoseTailIndex];
        }

        public bool WingtipOnGround()
        {
            return WingtipOnGroundR() || WingtipOnGroundL();
        }

        public bool WingtipOnGroundR()
        {
            return ContactPoints[this.GetPlaneConfig().CollisionWheelWingtipRIndex];
        }

        public bool WingtipOnGroundL()
        {
            return ContactPoints[this.GetPlaneConfig().CollisionWheelWingtipLIndex];
        }

        public bool IsPropstrike()
        {
            bool collisionProp = ContactPoints[this.GetPlaneConfig().CollisionPropIndex];
            bool propStrikeAngleReached = this.IsOnGround && this.pitch > this.GetPlaneConfig().PropStrikeThreshold;
            return collisionProp || propStrikeAngleReached;
        }

        public double getWindTotal()
        {
            double windTotal = Math.Sqrt(WindX * WindX + WindY * WindY);
            return windTotal;
        }

        public double getRelDir()
        {
            double angleRad = Math.Atan2(WindX, -WindY); // flip windX to get tailwind at 0°
            double angleDeg = angleRad * (180.0 / Math.PI);

            // Normalize to [0, 360)
            if (angleDeg < 0)
                angleDeg += 360;

            return angleDeg;
        }

        public AircraftState GetState()
        {
            return new AircraftState
            {
                EngineOn = this.IsEngineOn,
                Fuel = this.Fuel,
                FuelPercent = this.FuelPercent,
                ParkingBrake = this.IsParkingBreak,
                Weight = this.TotalWeight,
                MaxWeightPercent = this.TotalWeight / this.MaxTotalWeight * 100,
                PilotWeight = this.PilotWeight,
                FuelUnlimited = this.FuelUnlimited,
            };
        }



        public abstract bool Update();

        public abstract void OnEvent(EVENTS recEvent);

        public abstract void OnQuit();

        public abstract void setValue(string name, double value);

        public abstract void Pause();

        public abstract void Unpause();

        public abstract void sendEvent(EVENTS myEvent, uint dwData = 1);

        public void setPosition(GeoCoordinate position, double heading, bool setAttitude = false, double altitudeOffset = 0.0) 
        {
            double offset = PlaneConfigsService.GetGearOffset(this.Type + "|" + this.Model);

            GeoCoordinate offsetPos = GeoUtils.GetOffsetPosition(position, heading, offset);

            this.setValue("SIM DISABLED", 1);
            this.setValue("PLANE LATITUDE", offsetPos.Latitude);
            this.setValue("PLANE LONGITUDE", offsetPos.Longitude);
            this.setValue("PLANE ALTITUDE", offsetPos.Altitude + altitudeOffset);
            this.setValue("PLANE HEADING DEGREES TRUE", heading);
            this.resetSpeed();
            if (setAttitude)
            {
                this.setValue("PLANE PITCH DEGREES", 0);
                this.setValue("PLANE BANK DEGREES", 0);
            }
            System.Threading.Thread.Sleep(333);
            this.setValue("SIM DISABLED", 0);
        }


        public void resetSpeed()
        {
            this.setValue("VELOCITY WORLD X", 0);
            this.setValue("VELOCITY WORLD Y", 0);
            this.setValue("VELOCITY WORLD Z", 0);
            this.setValue("GROUND VELOCITY", 0);
            //this.setValue("AIRSPEED INDICATED", 0);
        }

        public abstract void SpawnObject(string objectName, double latitude, double longitude, double altitude);
    }
}

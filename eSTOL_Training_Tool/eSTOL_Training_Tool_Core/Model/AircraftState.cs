namespace eSTOL_Training_Tool.Model
{
    public class AircraftState
    {
        public bool EngineOn { get; set; }
        public bool ParkingBrake { get; set; }
        public string Airport { get; set; }
        public bool OnGround { get; set; }
        public double Fuel { get; set; }
    }
}

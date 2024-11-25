using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eSTOL_Training_Tool.Model
{
    public class InitFlightData
    {
        public Ident Ident { get; set; }
        public Telemetrie Telemetrie { get; set; }
        public AircraftState State { get; set; }
    }
}

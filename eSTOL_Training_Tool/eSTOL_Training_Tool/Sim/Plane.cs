using eSTOL_Training_Tool.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace eSTOL_Training_Tool
{
    internal class Plane : Aircraft
    {
        public delegate void PlaneEventCallBack(PlaneEvent planeEvent);
        private PlaneEventCallBack callBack = null;

        public Plane(PlaneEventCallBack callback)
        {
            this.callBack = callback;
        }

        public override void OnEvent(EVENTS recEvent)
        {
            switch (recEvent)
            {
                default:
                    {
                           
                        this.callBack(new PlaneEvent
                        {
                            Event = recEvent.ToString(),
                            Parameter = new InitFlightData
                            {
                                Ident = this.GetIdent(),
                                State = this.GetState(),
                                Telemetrie = this.GetTelemetrie()
                            }
                        });
                            
                        break;
                    }
                case EVENTS.SimStop:
                    {
                        this.callBack(new PlaneEvent
                        {
                            Event = EVENTS.SimStop.ToString(),
                            Parameter = new object[0]
                        });
                        break;
                    }
            }
        }

        public override void OnQuit()
        {
            this.callBack(new PlaneEvent
            {
                Event = "QUIT",
                Parameter = new object[0]
            });
            //System.Environment.Exit(0);
        }
    }
    
}

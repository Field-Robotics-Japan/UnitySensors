using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnitySensors
{
    /*
    [CreateAssetMenu]
    public class GPVTG : NMEAFormat
    {
        [SerializeField]
        private NMEAMode _mode;

        private const float _meterPerSec2knot = 1.9384f;
        private const float _meterPerSec2KiloMeterPerHour = 3.6f;

        public override string Serialize(GeoCoordinate coordinate, Vector3 velocity)
        {
            velocity.y = 0.0f;
            float vel = velocity.magnitude;
            float groundSpeed_knot = vel * _meterPerSec2knot;
            float groundSpeed_kiloMetersPerHour = vel * _meterPerSec2KiloMeterPerHour;
            float directionOfMovement = Mathf.Atan2(velocity.x, velocity.z) * Mathf.Rad2Deg;
            if (directionOfMovement < 0) directionOfMovement += 360.0f;

            string sentence = "$GPVTG,";

            sentence += directionOfMovement.ToString("000.0");
            sentence += ",T,";

            sentence += ",M,";

            sentence += groundSpeed_knot.ToString("000.0");
            sentence += ",N,";
            sentence += groundSpeed_kiloMetersPerHour.ToString("000.0");
            sentence += ",K,";

            switch (_mode)
            {
                case NMEAMode.NONE:
                    sentence += "N";
                    break;
                case NMEAMode.AUTONOMOUS:
                    sentence += "A";
                    break;
                case NMEAMode.DIFFERENTIAL:
                    sentence += "D";
                    break;
                case NMEAMode.ESTIMATED:
                    sentence += "E";
                    break;
            }

            base.AddChecksum(ref sentence);

            sentence += "\r\n";

            return sentence;
        }
    }
    */
}

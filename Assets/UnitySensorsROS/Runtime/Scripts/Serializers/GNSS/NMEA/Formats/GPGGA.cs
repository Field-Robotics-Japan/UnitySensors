using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnitySensors
{
    /*
    [CreateAssetMenu]
    public class GPGGA : NMEAFormat
    {
        private enum Quality
        {
            NONE = 0,
            SPS = 1,
            DIFFERENTIAL_GPS = 2
        }

        [SerializeField]
        private TimeMode _timeMode;
        [SerializeField]
        private Quality _quality;
        [SerializeField]
        private uint _satelliteNum = 8;
        [SerializeField]
        private float _HDOP = 0.0f;
        [SerializeField]
        private float _geoidHeight = 36.7071f;

        public override string Serialize(GeoCoordinate coordinate, Vector3 velocity)
        {
            string sentence = "$GPGGA,";

            switch (_timeMode)
            {
                case TimeMode.SIM:
                    base.AddSimTime(ref sentence);
                    break;
                case TimeMode.UTC:
                    base.AddUTCTime(ref sentence);
                    break;
            }

            base.AddLatitude(ref sentence, (float)coordinate.latitude);
            base.AddLongitude(ref sentence, (float)coordinate.longitude);

            sentence += ((int)_quality).ToString();
            sentence += ",";

            sentence += ((int)_satelliteNum).ToString("D02");
            sentence += ",";

            sentence += _HDOP.ToString();
            sentence += ",";

            sentence += coordinate.altitude.ToString();
            sentence += ",M,";

            sentence += Math.Round(_geoidHeight, 1).ToString();
            sentence += ",M,";

            sentence += ",";

            sentence += "0000";

            base.AddChecksum(ref sentence);

            sentence += "\r\n";

            return sentence;
        }
    }
    */
}

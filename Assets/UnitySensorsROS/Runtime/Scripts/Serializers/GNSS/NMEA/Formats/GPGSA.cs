using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnitySensors
{
    /*
    [CreateAssetMenu]
    public class GPGSA : NMEAFormat
    {
        private enum Mode
        {
            AUTO,
            MANUAL
        }

        private enum LocatingType
        {
            NONE = 1,
            TWO_D = 2,
            THREE_D = 3
        }

        [SerializeField]
        private Mode _mode;
        [SerializeField]
        private int[] _satelliteID;
        [SerializeField]
        private LocatingType _locatingType;
        [SerializeField]
        private float _PDOP = 0.0f;
        [SerializeField]
        private float _HDOP = 0.0f;
        [SerializeField]
        private float _VDOP = 0.0f;

        public override string Serialize(GeoCoordinate coordinate, Vector3 velocity)
        {
            string sentence = "$GPGSA,";

            switch (_mode)
            {
                case Mode.AUTO:
                    sentence += "A,";
                    break;
                case Mode.MANUAL:
                    sentence += "M,";
                    break;
            }

            sentence += ((int)_locatingType).ToString();

            for (int i = 0; i < _satelliteID.Length && i < 12; i++)
            {
                sentence += _satelliteID[i].ToString("D02");
                sentence += ",";
            }

            sentence += _PDOP.ToString("0.0");
            sentence += ",";

            sentence += _HDOP.ToString("0.0");
            sentence += ",";

            sentence += _VDOP.ToString("0.0");

            base.AddChecksum(ref sentence);

            sentence += "\r\n";

            return sentence;
        }
    }
    */
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnitySensors
{
    public class NMEAFormat : AttachableScriptableObject
    {
        protected enum TimeMode
        {
            SIM = 0,
            UTC = 1
        }

        protected enum NMEAMode
        {
            NONE,
            AUTONOMOUS,
            DIFFERENTIAL,
            ESTIMATED
        }

        public virtual void Init()
        {

        }

        public virtual string Serialize(GeoCoordinate coordinate, Vector3 velocity)
        {
            return "";
        }

        protected void AddUTCTime(ref string sentence)
        {
            sentence += DateTime.UtcNow.Hour.ToString("D02");
            sentence += DateTime.UtcNow.Minute.ToString("D02");
            sentence += DateTime.UtcNow.Second.ToString("D02");
            sentence += ".";
            sentence += DateTime.UtcNow.Millisecond.ToString("D3");
            sentence += ",";
        }
        protected void AddSimTime(ref string sentence)
        {
            uint raw_sec = (uint)Math.Truncate(Time.time);
            uint hour = raw_sec / 3600;
            uint min = (raw_sec - (3600 * hour)) / 60;
            uint sec = raw_sec - (3600 * hour) - (60 * min);
            uint millisec = (uint)((Time.time % 1) * 1e+3);
            sentence += hour.ToString("D02");
            sentence += min.ToString("D02");
            sentence += sec.ToString("D02");
            sentence += ".";
            sentence += millisec.ToString("D3");
            sentence += ",";
        }

        protected void AddLatitude(ref string sentence, float latitude)
        {
            sentence += ((latitude < 0 ? -latitude : latitude) * 1e2).ToString();
            sentence += ",";
            if (latitude >= 0)
                sentence += "N";
            else
                sentence += "S";
            sentence += ",";
        }

        protected void AddLongitude(ref string sentence, float longitude)
        {
            sentence += ((longitude < 0 ? -longitude : longitude) * 1e2).ToString();
            sentence += ",";
            if (longitude >= 0)
                sentence += "E";
            else
                sentence += "W";
            sentence += ",";
        }

        protected void AddChecksum(ref string sentence)
        {
            byte checksum = 0;
            for (int i = 1; i < sentence.Length; i++)
                checksum ^= (byte)sentence[i];
            sentence += "*";
            sentence += checksum.ToString("X2");
        }
    }
}

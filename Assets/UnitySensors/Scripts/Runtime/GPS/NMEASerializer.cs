using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;


namespace FRJ.Sensor
{
    [System.Serializable]
    public class NMEASerializer
    {
        public enum GPS_MODE
        {
            NONE,
            AUTONOMOUS,
            DIFFERENTIAL,
            ESTIMATED
        }
        public enum TIME_MODE
        {
            SIM,
            UTC
        }

        #region properties
        public float latitude { 
            set{
                this.GPRMC_DATA.latitude
                    = this.GPGGA_DATA.latitude
                    = value;
            } 
        }
        public float longitude
        {
            set
            {
                this.GPRMC_DATA.longitude
                    = this.GPGGA_DATA.longitude
                    = value;
            }
        }
        #endregion

        #region GPRMC

        [System.Serializable]
        public struct GPRMC_DATA_STRUCT
        {
            public bool status;                     // 0 : available, 1 : warning
            public float latitude;
            public float longitude;
            public float groundSpeed;               // 000.0 ~ 999.9 [knot]
            public float directionOfMovement;       // 000.0 ~ 359.9 [deg]
            public GPS_MODE mode;
            public TIME_MODE time_mode;
        }
        [SerializeField] public GPRMC_DATA_STRUCT GPRMC_DATA;

        public string GPRMC()
        {
            string ret = "$GPRMC,";
            
            // Update time 
            switch(GPRMC_DATA.time_mode)
            {
            case TIME_MODE.SIM:
            	 AddSimTime(ref ret);
            	 break;
            case TIME_MODE.UTC:
                AddUTCTime(ref ret);
                break;
            };
            
            // Update status
            ret += GPRMC_DATA.status ?"V":"A";
            ret += ",";

            // Update latitude
            AddLatitude(ref ret, GPRMC_DATA.latitude);

            // Update longitude
            AddLongitude(ref ret, GPRMC_DATA.longitude);

            // Update ground speed;
            ret += GPRMC_DATA.groundSpeed.ToString("000.0");
            ret += ",";

            // Update direction of movement
            ret += GPRMC_DATA.directionOfMovement.ToString("000.0");
            ret += ",";

            // Update UTC date
            AddUTCDate(ref ret);

            // Update angular difference between magnetice north and due north (in this case, empty)
            ret += ",,";

            // Update mode
            switch (GPRMC_DATA.mode)
            {
                case GPS_MODE.NONE:
                    ret += "N";
                    break;
                case GPS_MODE.AUTONOMOUS:
                    ret += "A";
                    break;
                case GPS_MODE.DIFFERENTIAL:
                    ret += "D";
                    break;
                case GPS_MODE.ESTIMATED:
                    ret += "E";
                    break;
            }

            // Update checksum
            AddChecksum(ref ret);

            // Insert CR LF
            ret += "\r\n";

            return ret;
        }
        #endregion

        #region GPGGA
        public enum GPGGA_QUALITY
        {
            NONE = 0,
            SPS = 1,
            DIFFERENTIAL_GPS = 2
        }

        [System.Serializable]
        public struct GPGGA_DATA_STRUCT
        {
            public float latitude;
            public float longitude;
            public GPGGA_QUALITY quality;
            public uint satelliteNum;
            public float hdop;
            public float altitude;
            public float geoidLevel;
            public TIME_MODE time_mode;
        }
        [SerializeField] public GPGGA_DATA_STRUCT GPGGA_DATA;

        public string GPGGA()
        {
            string ret = "$GPGGA,";
            
            // Update time 
            switch(GPGGA_DATA.time_mode)
            {
            case TIME_MODE.SIM:
            	 AddSimTime(ref ret);
            	 break;
            case TIME_MODE.UTC:
                AddUTCTime(ref ret);
                break;
            };

            // Update latitude
            AddLatitude(ref ret, GPGGA_DATA.latitude);

            // Update longitude
            AddLongitude(ref ret, GPGGA_DATA.longitude);

            // Update quality
            ret += ((int)GPGGA_DATA.quality).ToString();
            ret += ",";

            // Update number of satellites
            ret += GPGGA_DATA.satelliteNum.ToString("D02");
            ret += ",";

            // Update HDOP
            ret += GPGGA_DATA.hdop.ToString();
            ret += ",";

            // Update altitude
            ret += GPGGA_DATA.altitude.ToString();
            ret += ",M,";

            // Update geoid level
            ret += Math.Round(GPGGA_DATA.geoidLevel, 1).ToString();
            ret += ",M,";

            // Update DGPS data (in this case, empty)
            ret += ",";

            // Update differential reference point ID
            ret += "0000";

            // Update checksum
            AddChecksum(ref ret);

            // Insert CR LF
            ret += "\r\n";

            return ret;
        }
        #endregion

        #region GPGSA
        public enum GPGSA_MODE
        {
            MANUAL,
            AUTO
        }

        public enum GPGSA_LOCATING_TYPE
        {
            NONE = 1,
            TWO_D = 2,
            THREE_D = 3
        }

        [System.Serializable]
        public struct GPGSA_DATA_STRUCT
        {
            public GPGSA_MODE mode;
            public int[] satellightID;
            public GPGSA_LOCATING_TYPE locatingType;
            public float pdop;
            public float hdop;
            public float vdop;
        }
        [SerializeField] public GPGSA_DATA_STRUCT GPGSA_DATA;

        public string GPGSA()
        {
            string ret = "$GPGSA,";

            // Update mode
            switch (GPGSA_DATA.mode)
            {
                case GPGSA_MODE.MANUAL:
                    ret += "M,";
                    break;
                case GPGSA_MODE.AUTO:
                    ret += "A,";
                    break;
            }

            // Update locating type
            ret += ((int)GPGSA_DATA.locatingType).ToString();

            // Update satellight ID
            for (int i = 0; i < GPGSA_DATA.satellightID.Length && i < 12; i++)
            {
                ret += GPGSA_DATA.satellightID[i].ToString("D02");
                ret += ",";
            }

            // Update PDOP
            ret += GPGSA_DATA.pdop.ToString("0.0");
            ret += ",";

            // Update HDOP
            ret += GPGSA_DATA.hdop.ToString("0.0");
            ret += ",";

            // Update VDOP
            ret += GPGSA_DATA.vdop.ToString("0.0");

            // Update checksum
            AddChecksum(ref ret);

            // Insert CR LF
            ret += "\r\n";

            return ret;
        }
        #endregion

        #region GPVTG

        [System.Serializable]
        public struct GPVTG_DATA_STRUCT
        {
            public float directionOfMovement;       // 000.0 ~ 359.9 [deg]
            public float groundSpeed_knot;
            public float groundSpeed_kiloMetersPerHour;
            public GPS_MODE mode;
        }
        [SerializeField] public GPVTG_DATA_STRUCT GPVTG_DATA;

        public string GPVTG()
        {
            string ret = "$GPVTG,";

            // Update direction of movement
            ret += GPVTG_DATA.directionOfMovement.ToString("000.0");
            ret += ",T,";

            // Update magnetic direction of movement
            ret += ",M,";

            // Update ground speed
            ret += GPVTG_DATA.groundSpeed_knot.ToString("000.0");
            ret += ",N,";
            ret += GPVTG_DATA.groundSpeed_kiloMetersPerHour.ToString("000.0");
            ret += ",K,";

            // Update mode
            switch (GPVTG_DATA.mode)
            {
                case GPS_MODE.NONE:
                    ret += "N";
                    break;
                case GPS_MODE.AUTONOMOUS:
                    ret += "A";
                    break;
                case GPS_MODE.DIFFERENTIAL:
                    ret += "D";
                    break;
                case GPS_MODE.ESTIMATED:
                    ret += "E";
                    break;
            }

            // Update checksum
            AddChecksum(ref ret);

            // Insert CR LF
            ret += "\r\n";

            return ret;
        }
        #endregion

        #region private func

        private void AddUTCDate(ref string ret)
        {
            ret += DateTime.UtcNow.Day.ToString("D02");
            ret += DateTime.UtcNow.Month.ToString("D02");
            ret += ((int)(DateTime.UtcNow.Year - (int)(DateTime.UtcNow.Year*0.01)*100)).ToString("D02");
            ret += ",";
        }
        private void AddUTCTime(ref string ret)
        {
            ret += DateTime.UtcNow.Hour.ToString("D02");
            ret += DateTime.UtcNow.Minute.ToString("D02");
            ret += DateTime.UtcNow.Second.ToString("D02");
            ret += ".";
            ret += DateTime.UtcNow.Millisecond.ToString("D3");
            ret += ",";
        }
        private void AddSimTime(ref string ret)
        {
            uint raw_sec = (uint)Math.Truncate(Time.time);
            uint hour = raw_sec/3600;
            uint min = (raw_sec-(3600*hour))/60;
            uint sec = raw_sec-(3600*hour)-(60*min);
            uint millisec = (uint)( (Time.time % 1)*1e+3 );
            ret += hour.ToString("D02");
            ret += min.ToString("D02");
            ret += sec.ToString("D02");
            ret += ".";
            ret += millisec.ToString("D3");
            ret += ",";
        }
        private void AddLatitude(ref string ret, float latitude)
        {
            ret += ((latitude<0?-latitude:latitude) * 1e2).ToString();
            ret += ",";
            if (latitude >= 0)
                ret += "N";
            else
                ret += "S";
            ret += ",";
        }
        private void AddLongitude(ref string ret, float longitude)
        {
            ret += ((longitude<0?-longitude:longitude) * 1e2).ToString();
            ret += ",";
            if (longitude >= 0)
                ret += "E";
            else
                ret += "W";
            ret += ",";
        }

        private void AddChecksum(ref string ret)
        {
            byte checksum = 0;
            for (int i = 1; i < ret.Length; i++)
                checksum ^= (byte)ret[i];
            ret += "*";
            ret += checksum.ToString("X2");
        }
        #endregion
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FRJ.Sensor
{
    public class GPS : MonoBehaviour
    {
        #region Header("General")
        [Header("General")]
        [SerializeField] private float _updateRate = 10f;
        [SerializeField] private NMEASerializer.TIME_MODE _timeMode = NMEASerializer.TIME_MODE.SIM;
        [SerializeField] private double _baseLatitude = 35.71020206575301;      // 基地局の緯度
        [SerializeField] private double _baseLongitude = 139.81070039691542;    // 基地局の経度
        [SerializeField] private double _baseAltitude = 3.0;                    // 基地局の標高（海抜高さ）[m]
        [SerializeField] private NMEASerializer.GPS_MODE _gps_mode = NMEASerializer.GPS_MODE.AUTONOMOUS;
        [SerializeField] private float _HDOP = 1.0f;                            // 水平精度低下率
        [HideInInspector] public float updateRate { get => this._updateRate; }
        #endregion

        #region Header("Noise")
        // Random seed
        [SerializeField] private uint _randomSeed = 1;
        // Gaussian Noise sigma
        [SerializeField] private float _gaussianNoiseSigma_horizontal     = 0.0f;
        [SerializeField] private float _gaussianNoiseSigma_vertical     = 0.0f;
        #endregion

        #region Header("GPRMC")
        [Header("GPRMC")]
        [Tooltip("0 : Available, 1 : Warning")]
        [SerializeField] private bool _gprmc_status;
        #endregion

        #region Header("GPGGA")
        [Header("GPGGA")]
        [SerializeField] private NMEASerializer.GPGGA_QUALITY _gpgga_quality = NMEASerializer.GPGGA_QUALITY.SPS;
        [SerializeField] private uint _gpgga_satelliteNum = 8;                  // 使用衛星数
        [SerializeField] private float _gpgga_geoidHeight = 36.7071f;           // ジオイド高 [m]
        #endregion

        #region Header("GPGSA")
        [Header("GPGSA")]
        [SerializeField] private NMEASerializer.GPGSA_MODE _gpgsa_mode = NMEASerializer.GPGSA_MODE.AUTO;
        [SerializeField] private int[] _gpgsa_satelliteID;
        [SerializeField] private NMEASerializer.GPGSA_LOCATING_TYPE _gpgsa_locating_type = NMEASerializer.GPGSA_LOCATING_TYPE.THREE_D;
        [SerializeField] private float _gpgsa_PDOP = 1.0f;                      // 位置精度低下率
        [SerializeField] private float _gpgsa_VDOP = 1.0f;                      // 垂直精度低下率
        #endregion

        #region Header("Serializer")
        [Header("Serializer")]
        [SerializeField] private NMEASerializer _serializer;
        #endregion

        #region Header("RawData")
        [Header("RawData")]
        [SerializeField] private double _latitude;
        [SerializeField] private double _longitude;
        [SerializeField] private double _altitude;
        [SerializeField] private Vector3 _velocity;
        #endregion

        #region Header("SerializedData")
        [Header("SerializedData")]
        [SerializeField] private string _gprmc;
        [SerializeField] private string _gpgga;
        [SerializeField] private string _gpgsa;
        [SerializeField] private string _gpvtg;

        [HideInInspector] public string gprmc { get => this._gprmc; }
        [HideInInspector] public string gpgga { get => this._gpgga; }
        [HideInInspector] public string gpgsa { get => this._gpgsa; }
        [HideInInspector] public string gpvtg { get => this._gpvtg; }
        #endregion

        private GeoCoordinate _gc;
        private Vector3 _pos_old;
        private float _time_old;
        private const float meterPerSec2knot = 1.9384f;

        public void Init()
        {
            this._pos_old = this.transform.position;
            this._time_old = Time.time;

            this._gc = new GeoCoordinate(this._baseLatitude, this._baseLongitude);
            this._serializer = new NMEASerializer();

            // GPRMC
            this._serializer.GPRMC_DATA.status = this._gprmc_status;
            this._serializer.GPRMC_DATA.mode = this._gps_mode;
            this._serializer.GPRMC_DATA.time_mode = this._timeMode;

            // GPGGA
            this._serializer.GPGGA_DATA.quality = this._gpgga_quality;
            this._serializer.GPGGA_DATA.satelliteNum = this._gpgga_satelliteNum;
            this._serializer.GPGGA_DATA.hdop = this._HDOP;
            this._serializer.GPGGA_DATA.geoidLevel = this._gpgga_geoidHeight;
            this._serializer.GPGGA_DATA.time_mode = this._timeMode;

            // GPGSA
            this._serializer.GPGSA_DATA.mode = this._gpgsa_mode;
            this._serializer.GPGSA_DATA.satellightID = new int[this._gpgsa_satelliteID.Length];
            for (int i = 0; i < this._gpgsa_satelliteID.Length; i++)
                this._serializer.GPGSA_DATA.satellightID[i] = this._gpgsa_satelliteID[i];
            this._serializer.GPGSA_DATA.locatingType = this._gpgsa_locating_type;
            this._serializer.GPGSA_DATA.pdop = this._gpgsa_PDOP;
            this._serializer.GPGSA_DATA.hdop = this._HDOP;
            this._serializer.GPGSA_DATA.vdop = this._gpgsa_VDOP;
        }

        public void updateGPS()
        {
            float time = Time.time;
            this._velocity = (this.transform.position - _pos_old) / (time - _time_old);
            _pos_old = this.transform.position;
            _time_old = time;

            float groundSpeed_knot = Mathf.Sqrt(_velocity.x * _velocity.x + _velocity.z * _velocity.z) * meterPerSec2knot;
            float groundSpeed_kiloMetersPerHour = Mathf.Sqrt(_velocity.x * _velocity.x + _velocity.z * _velocity.z) * 3.6f;

            float directionOfMovement = Mathf.Atan2(_velocity.x, _velocity.z) * Mathf.Rad2Deg;
            if (directionOfMovement < 0) directionOfMovement += 360.0f;

            (this._latitude, this._longitude) = this._gc.XZ2LatLon( this.transform.position.x + GetGaussianNoise() * this._gaussianNoiseSigma_horizontal, 
                                                                    this.transform.position.z + GetGaussianNoise() * this._gaussianNoiseSigma_horizontal);
            this._altitude = this._baseAltitude + this.transform.position.y + GetGaussianNoise() * this._gaussianNoiseSigma_vertical;

            this._serializer.latitude = (float)this._latitude;
            this._serializer.longitude = (float)this._longitude;

            // GPRMC
            this._serializer.GPRMC_DATA.groundSpeed = groundSpeed_knot;
            this._serializer.GPRMC_DATA.directionOfMovement = directionOfMovement;

            // GPGGA
            this._serializer.GPGGA_DATA.altitude = (float)this._altitude;

            // GPGSA

            // GPVTG
            this._serializer.GPVTG_DATA.directionOfMovement = directionOfMovement;
            this._serializer.GPVTG_DATA.groundSpeed_knot = groundSpeed_knot;
            this._serializer.GPVTG_DATA.groundSpeed_kiloMetersPerHour = groundSpeed_kiloMetersPerHour;

            this._gprmc = this._serializer.GPRMC();
            this._gpgga = this._serializer.GPGGA();
            this._gpgsa = this._serializer.GPGSA();
            this._gpvtg = this._serializer.GPVTG();
        }

        private float GetGaussianNoise()
        {
            var rand2 = Random.value;
            var rand3 = Random.value;
            float normrand =
                (float)Mathf.Sqrt(-2.0f * Mathf.Log(rand2)) *
                (float)Mathf.Cos(2.0f * Mathf.PI * rand3);
            return normrand;
        }
    }
}

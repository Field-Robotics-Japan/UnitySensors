using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using RosMessageTypes.Sensor;

namespace UnitySensors.ROS
{
    [System.Serializable]
    public class NavSatFixSerializer : Serializer
    {
        enum Status
        {
            NO_FIX,
            FIX,
            SBAS_FIX,
            GBAS_FIX
        }

        enum Service
        {
            GPS,
            GLONASS,
            COMPASS,
            GALILEO
        }

        [SerializeField]
        private Status _status;
        [SerializeField]
        private Service _service;

        private NavSatFixMsg _msg;
        private AutoHeader _header;

        private uint _service_num;

        public NavSatFixMsg msg { get => _msg; }

        public void Init(string frame_id)
        {
            _msg = new NavSatFixMsg();
            _header = new AutoHeader();

            _msg.status = new NavSatStatusMsg();
            _msg.status.service = (ushort) Mathf.Pow(2, (int)(_service));
            _msg.position_covariance = new double[9];

            _header.Init(frame_id);
        }

        public void Serialize(float time, GeoCoordinate coordinate)
        {
            _header.Serialize(time);
            _msg.header = _header.header;
            _msg.status.status = (sbyte) ((int)(_status) - 1);
            _msg.latitude = coordinate.latitude;
            _msg.longitude = coordinate.longitude;
            _msg.altitude = coordinate.altitude;
            _msg.position_covariance_type = 0;

            Debug.Log(_msg.status.service);
        }
    }
}
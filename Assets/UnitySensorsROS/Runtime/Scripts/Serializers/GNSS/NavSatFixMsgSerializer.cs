using UnityEngine;
using RosMessageTypes.Sensor;
using UnitySensors.Sensor.GNSS;

namespace UnitySensors.ROS.Serializer.GNSS
{
    [System.Serializable]
    public class NavSatFixMsgSerializer : RosMsgSerializer<GNSSSensor, NavSatFixMsg>
    {
        private enum Status
        {
            NO_FIX,
            FIX,
            SBAS_FIX,
            GBAS_FIX
        }

        private enum Service
        {
            GPS,
            GLONASS,
            COMPASS,
            GALILEO
        }

        [SerializeField]
        private HeaderSerializer _header;

        [SerializeField]
        private Status _status = Status.FIX;
        [SerializeField]
        private Service _service = Service.GPS;

        public override void Init(GNSSSensor sensor)
        {
            base.Init(sensor);
            _header.Init(sensor);

            _msg.status = new NavSatStatusMsg();
            _msg.status.service = (ushort)Mathf.Pow(2, (int)(_service));
            _msg.position_covariance = new double[9];
        }
        public override NavSatFixMsg Serialize()
        {
            _msg.header = _header.Serialize();
            _msg.status.status = (sbyte)((int)(_status) - 1);
            _msg.latitude = sensor.coordinate.latitude;
            _msg.longitude = sensor.coordinate.longitude;
            _msg.altitude = sensor.coordinate.altitude;
            _msg.position_covariance_type = 0;
            return _msg;
        }
    }
}
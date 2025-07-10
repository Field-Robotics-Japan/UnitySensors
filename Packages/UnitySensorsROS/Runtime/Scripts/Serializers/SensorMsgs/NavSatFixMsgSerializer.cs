using UnityEngine;
using RosMessageTypes.Sensor;

using UnitySensors.Attribute;
using UnitySensors.Interface.Geometry;
using UnitySensors.ROS.Serializer.Std;

namespace UnitySensors.ROS.Serializer.Sensor
{
    [System.Serializable]
    public class NavSatFixMsgSerializer : RosMsgSerializer<NavSatFixMsg>
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

        [SerializeField, Interface(typeof(IGeoCoordinateInterface))]
        private Object _source;
        [SerializeField]
        private HeaderSerializer _header;

        [SerializeField]
        private Status _status = Status.FIX;
        [SerializeField]
        private Service _service = Service.GPS;

        private IGeoCoordinateInterface _sourceInterface;

        public override void Init()
        {
            base.Init();
            _header.Init();
            _sourceInterface = _source as IGeoCoordinateInterface;

            _msg.status = new NavSatStatusMsg();
            _msg.status.service = (ushort)Mathf.Pow(2, (int)(_service));
            _msg.position_covariance = new double[9];
        }
        public override NavSatFixMsg Serialize()
        {
            _msg.header = _header.Serialize();
            _msg.status.status = (sbyte)((int)(_status) - 1);
            _msg.latitude = _sourceInterface.coordinate.latitude;
            _msg.longitude = _sourceInterface.coordinate.longitude;
            _msg.altitude = _sourceInterface.coordinate.altitude;
            _msg.position_covariance_type = 0;
            return _msg;
        }
    }
}
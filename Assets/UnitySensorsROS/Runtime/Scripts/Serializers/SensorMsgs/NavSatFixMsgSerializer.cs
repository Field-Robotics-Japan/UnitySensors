using UnityEngine;
using RosMessageTypes.Sensor;

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

        [SerializeField]
        private HeaderSerializer _header;

        [SerializeField]
        private Status _status = Status.FIX;
        [SerializeField]
        private Service _service = Service.GPS;

        private IGeoCoordinateInterface _source;

        public override void Init(MonoBehaviour source)
        {
            base.Init(source);
            _header.Init(source);
            _source = (IGeoCoordinateInterface)source;

            _msg.status = new NavSatStatusMsg();
            _msg.status.service = (ushort)Mathf.Pow(2, (int)(_service));
            _msg.position_covariance = new double[9];
        }
        public override NavSatFixMsg Serialize()
        {
            _msg.header = _header.Serialize();
            _msg.status.status = (sbyte)((int)(_status) - 1);
            _msg.latitude = _source.coordinate.latitude;
            _msg.longitude = _source.coordinate.longitude;
            _msg.altitude = _source.coordinate.altitude;
            _msg.position_covariance_type = 0;
            return _msg;
        }

        public override bool IsCompatible(MonoBehaviour source)
        {
            return (_header.IsCompatible(source) && source is IGeoCoordinateInterface);
        }
    }
}
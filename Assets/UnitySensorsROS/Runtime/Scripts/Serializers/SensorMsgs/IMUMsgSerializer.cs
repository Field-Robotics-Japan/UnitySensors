using UnityEngine;
using Unity.Robotics.ROSTCPConnector.ROSGeometry;
using RosMessageTypes.Sensor;

using UnitySensors.Interface.Sensor;
using UnitySensors.ROS.Serializer.Std;

namespace UnitySensors.ROS.Serializer.Sensor
{
    [System.Serializable]
    public class IMUMsgSerializer : RosMsgSerializer<ImuMsg>
    {
        [SerializeField]
        private HeaderSerializer _header;

        private IImuDataInterface _source;

        public override void Init(MonoBehaviour source)
        {
            base.Init(source);
            _header.Init(source);
            _source = (IImuDataInterface)source;
        }

        public override ImuMsg Serialize()
        {
            _msg.header = _header.Serialize();
            _msg.linear_acceleration = _source.acceleration.To<FLU>();
            _msg.orientation = _source.rotation.To<FLU>();
            _msg.angular_velocity = _source.angularVelocity.To<FLU>();
            return _msg;
        }

        public override bool IsCompatible(MonoBehaviour source)
        {
            return (_header.IsCompatible(source) && source is IImuDataInterface);
        }
    }
}

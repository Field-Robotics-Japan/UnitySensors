using UnityEngine;
using Unity.Robotics.ROSTCPConnector.ROSGeometry;
using RosMessageTypes.Sensor;

using UnitySensors.Attribute;
using UnitySensors.Interface.Sensor;
using UnitySensors.ROS.Serializer.Std;

namespace UnitySensors.ROS.Serializer.Sensor
{
    [System.Serializable]
    public class IMUMsgSerializer : RosMsgSerializer<ImuMsg>
    {
        [SerializeField, Interface(typeof(IImuDataInterface))]
        private Object _source;
        [SerializeField]
        private HeaderSerializer _header;

        private IImuDataInterface _sourceInterface;

        public override void Init()
        {
            base.Init();
            _header.Init();
            _sourceInterface = _source as IImuDataInterface;
        }

        public override ImuMsg Serialize()
        {
            _msg.header = _header.Serialize();
            _msg.linear_acceleration = _sourceInterface.acceleration.To<FLU>();
            _msg.orientation = _sourceInterface.rotation.To<FLU>();
            _msg.angular_velocity = _sourceInterface.angularVelocity.To<FLU>();
            return _msg;
        }
    }
}

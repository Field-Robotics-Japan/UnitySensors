using UnityEngine;

using Unity.Robotics.ROSTCPConnector.ROSGeometry;
using RosMessageTypes.Geometry;

using UnitySensors.Data.Pose;
using UnitySensors.Sensor;

namespace UnitySensors.ROS.Serializer.PoseStamped
{
    [System.Serializable]
    public class PoseStampedMsgSerializer<T> : RosMsgSerializer<T, PoseStampedMsg> where T : UnitySensor, IPoseInterface
    {
        [SerializeField]
        private HeaderSerializer _header;

        public override void Init(T sensor)
        {
            base.Init(sensor);
            _header.Init(sensor);
        }

        public override PoseStampedMsg Serialize()
        {
            _msg.header = _header.Serialize();
            _msg.pose.position = sensor.transform.position.To<FLU>();
            _msg.pose.orientation = sensor.transform.rotation.To<FLU>();
            return _msg;
        }
    }
}

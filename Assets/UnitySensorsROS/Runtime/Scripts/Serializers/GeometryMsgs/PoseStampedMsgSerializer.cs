using UnityEngine;

using Unity.Robotics.ROSTCPConnector.ROSGeometry;
using RosMessageTypes.Geometry;

using UnitySensors.Attribute;
using UnitySensors.Interface.Geometry;
using UnitySensors.ROS.Serializer.Std;

namespace UnitySensors.ROS.Serializer.Geometry
{
    [System.Serializable]
    public class PoseStampedMsgSerializer : RosMsgSerializer<PoseStampedMsg>
    {
        [SerializeField, Interface(typeof(IPoseInterface))]
        private Object _source;
        [SerializeField]
        private HeaderSerializer _header;

        private IPoseInterface _sourceInterface;

        public override void Init()
        {
            base.Init();
            _header.Init();
            _sourceInterface = _source as IPoseInterface;
        }

        public override PoseStampedMsg Serialize()
        {
            _msg.header = _header.Serialize();
            _msg.pose.position = _sourceInterface.position.To<FLU>();
            _msg.pose.orientation = _sourceInterface.rotation.To<FLU>();
            return _msg;
        }
    }
}

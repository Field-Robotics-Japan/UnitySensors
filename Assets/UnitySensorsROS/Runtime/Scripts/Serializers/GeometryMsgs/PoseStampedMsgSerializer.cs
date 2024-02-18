using UnityEngine;

using Unity.Robotics.ROSTCPConnector.ROSGeometry;
using RosMessageTypes.Geometry;

using UnitySensors.Interface.Geometry;
using UnitySensors.ROS.Serializer.Std;

namespace UnitySensors.ROS.Serializer.Geometry
{
    [System.Serializable]
    public class PoseStampedMsgSerializer : RosMsgSerializer<PoseStampedMsg>
    {
        [SerializeField]
        private HeaderSerializer _header;

        private IPoseInterface _source;

        public override void Init(MonoBehaviour source)
        {
            base.Init(source);
            _header.Init(source);
            _source = (IPoseInterface)source;
        }

        public override PoseStampedMsg Serialize()
        {
            _msg.header = _header.Serialize();
            _msg.pose.position = _source.position.To<FLU>();
            _msg.pose.orientation = _source.rotation.To<FLU>();
            return _msg;
        }

        public override bool IsCompatible(MonoBehaviour source)
        {
            return (_header.IsCompatible(source) && source is IPoseInterface);
        }
    }
}

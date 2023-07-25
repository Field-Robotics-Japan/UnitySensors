using RosMessageTypes.Geometry;
using RosMessageTypes.Tf2;
using Unity.Robotics.ROSTCPConnector.ROSGeometry;
using UnityEngine;

namespace UnitySensors.ROS
{

    [System.Serializable]
    public class TFSerializer : Serializer
    {

        [SerializeField]
        private TFMessageMsg _msg;

        private AutoHeader _header;

        public TFMessageMsg msg { get => _msg; }

        public void Init(string parent_frame_id)
        {
            _msg = new TFMessageMsg();
            _msg.transforms = new TransformStampedMsg[1];
            _header = new AutoHeader();

            _header.Init(parent_frame_id);
        }

        public TFMessageMsg Serialize(float time, string thisFrameID, Vector3 position, Quaternion rotation)
        {
            _header.Serialize(time);
            TransformStampedMsg tfStamped = new TransformStampedMsg();
            tfStamped.header = _header.header;
            tfStamped.child_frame_id = thisFrameID;
            tfStamped.transform.translation = position.To<FLU>();
            tfStamped.transform.rotation = rotation.To<FLU>();
            _msg.transforms[0] = tfStamped;
            return _msg;
        }
    }
}
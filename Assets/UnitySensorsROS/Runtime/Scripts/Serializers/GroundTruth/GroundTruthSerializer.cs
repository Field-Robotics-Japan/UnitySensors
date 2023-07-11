using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.Robotics.ROSTCPConnector.ROSGeometry;

using RosMessageTypes.Geometry;

namespace UnitySensors.ROS
{
    [System.Serializable]
    public class GroundTruthSerializer : Serializer
    {
        [SerializeField]
        private PoseStampedMsg _msg;

        private AutoHeader _header;

        public PoseStampedMsg msg { get => _msg; }

        public void Init(string frame_id)
        {
            _msg = new PoseStampedMsg();
            _header = new AutoHeader();

            _header.Init(frame_id);
        }

        public PoseStampedMsg Serialize(float time, Vector3 position, Quaternion rotation)
        {
            _header.Serialize(time);
            _msg.header = _header.header;

            _msg.pose.position = position.To<FLU>();
            _msg.pose.orientation = rotation.To<FLU>();
            return _msg;
        }
    }
}

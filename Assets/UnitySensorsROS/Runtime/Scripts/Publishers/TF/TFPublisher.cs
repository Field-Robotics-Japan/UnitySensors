using RosMessageTypes.Tf2;
using UnityEngine;

namespace UnitySensors.ROS
{

    public class TFPublisher : Publisher<TFSensor, TFSerializer>
    {
        public string _topicName = "/tf";
        public string thisFrameID;
        public bool rootTF;
        public GameObject parentSensor;
        public string parentFrameID;


        private Vector3 position_rel;        // local position relative to parentSensor
        private Quaternion rotation_rel;     // local rotation relative to parentSensor

        protected override void Init()
        {
            _ros.RegisterPublisher<TFMessageMsg>(_topicName);
            if (rootTF)
            {
                _serializer.Init(parentFrameID);
            }
            else
            {
                _serializer.Init(parentFrameID);
                position_rel = parentSensor.transform.InverseTransformPoint(transform.position);
                rotation_rel = Quaternion.Inverse(parentSensor.transform.rotation) * transform.rotation;
            }
        }

        protected override void Publish(float time)
        {
            if (rootTF)
            {
                position_rel = transform.position;
                rotation_rel = transform.rotation;
            }
            _serializer.Serialize(time, thisFrameID, position_rel, rotation_rel);
            _ros.Publish(_topicName, _serializer.msg);
        }
    }
}
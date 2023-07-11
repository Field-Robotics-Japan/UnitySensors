using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using RosMessageTypes.Geometry;

namespace UnitySensors.ROS
{
    [RequireComponent(typeof(GroundTruthSensor))]
    public class GroundTruthPublisher : Publisher<GroundTruthSensor, GroundTruthSerializer>
    {
        [SerializeField]
        private string _topicName = "ground_truth_pose";
        [SerializeField]
        private string _frame_id = "base_link";

        protected override void Init()
        {
            _ros.RegisterPublisher<PoseStampedMsg>(_topicName);
            _serializer.Init(_frame_id);
        }

        protected override void Publish(float time)
        {
            _serializer.Serialize(time, _sensor.position, _sensor.rotation);
            _ros.Publish(_topicName, _serializer.msg);
        }
    }
}

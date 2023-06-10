using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using RosMessageTypes.Sensor;

namespace UnitySensors.ROS
{
    [RequireComponent(typeof(RGBCameraSensor))]
    public class RGBCameraPublisher : Publisher<RGBCameraSensor, RGBCameraSerializer>
    {
        [SerializeField]
        private string _topicName = "image";
        [SerializeField]
        private string _frame_id = "camera";

        protected override void Init()
        {
            _topicName += "/compressed";
            _ros.RegisterPublisher<CompressedImageMsg>(_topicName);
            _serializer.Init(_frame_id);
        }

        protected override void Publish(float time)
        {
            _serializer.Serialize(time, _sensor.texture, _sensor.quality);
            _ros.Publish(_topicName, _serializer.msg);
        }
    }
}

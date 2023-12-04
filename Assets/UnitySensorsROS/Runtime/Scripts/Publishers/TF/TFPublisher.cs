using RosMessageTypes.Tf2;
using UnityEngine;

namespace UnitySensors.ROS
{
    [RequireComponent(typeof(TFSensor))]
    public class TFPublisher : Publisher<TFSensor, TFSerializer>
    {
        public string _topicName = "/tf";

        protected override void Init()
        {
            _ros.RegisterPublisher<TFMessageMsg>(_topicName);
            _serializer.Init();
        }

        protected override void Publish(float time)
        {
            _serializer.Serialize(time, _sensor.tf);
            _ros.Publish(_topicName, _serializer.msg);
        }
    }
}
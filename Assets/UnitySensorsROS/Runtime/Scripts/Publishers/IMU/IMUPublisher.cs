using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using RosMessageTypes.Sensor;

namespace UnitySensors.ROS
{
    [RequireComponent(typeof(IMUSensor))]
    public class IMUPublisher : Publisher<IMUSensor, IMUSerializer>
    {
        [SerializeField]
        private string _topicName = "imu/raw_data";
        [SerializeField]
        private string _frame_id = "imu_link";

        protected override void Init()
        {
            _ros.RegisterPublisher<ImuMsg>(_topicName);
            _serializer.Init(_frame_id);
        }

        protected override void Publish(float time)
        {
            _serializer.Serialize(time, _sensor.acceleration, _sensor.rotation, _sensor.angularVelocity);
            _ros.Publish(_topicName, _serializer.msg);
        }
    }
}

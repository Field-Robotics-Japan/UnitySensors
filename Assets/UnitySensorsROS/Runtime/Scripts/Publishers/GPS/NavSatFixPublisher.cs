using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using RosMessageTypes.Sensor;

namespace UnitySensors.ROS
{
    [RequireComponent(typeof(GPSSensor))]
    public class NavSatFixPublisher : Publisher<GPSSensor, Serializer>
    {
        [SerializeField]
        private string _topicName = "gnss/raw_data";
        [SerializeField]
        private string _frame_id = "gnss_link";

        [SerializeField]
        private NavSatFixSerializer _serializer_navsat;

        protected override void Init()
        {
            _ros.RegisterPublisher<NavSatFixMsg>(_topicName);
            _serializer_navsat.Init(_frame_id);
        }

        protected override void Publish(float time)
        {
            _serializer_navsat.Serialize(time, _sensor.coordinate);
            _ros.Publish(_topicName, _serializer_navsat.msg);
        }
    }
}


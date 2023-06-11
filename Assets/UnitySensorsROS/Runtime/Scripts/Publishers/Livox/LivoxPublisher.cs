using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using RosMessageTypes.Sensor;

namespace UnitySensors.ROS
{
    [RequireComponent(typeof(LivoxSensor))]
    public class LivoxPublisher : Publisher<LivoxSensor, Serializer>
    {
        [SerializeField]
        private string _topicName = "/livox/lidar";

        [SerializeField]
        private string _frameId = "livox_frame";

        private PointCloud2Serializer _serializer_pc;
        private bool _init = false;

        protected override void Init()
        {
            if (!_sensor.initialized) return;
            _ros.RegisterPublisher<PointCloud2Msg>(_topicName);

            _serializer_pc = new PointCloud2Serializer();
            _serializer_pc.Init(_frameId, ref _sensor.points, _sensor.pointNum);
            _init = true;
        }
        private void OnApplicationQuit()
        {
            _serializer_pc.Dispose();
        }

        protected override void Publish(float time)
        {
            if (!_init)
            {
                if (_sensor.initialized) Init();
                return;
            }
            _sensor.CompleteJob();
            _serializer_pc.Serialize(time);
            //_ros.Publish(_topicName, _serializer_pc.msg);
        }
    }
}

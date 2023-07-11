using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using RosMessageTypes.Sensor;

namespace UnitySensors.ROS
{
    [RequireComponent(typeof(DepthCameraSensor))]
    public class DepthCameraPublisher : Publisher<DepthCameraSensor, TextureSerializer>
    {
        [SerializeField]
        private string _topicName_pc = "points";
        [SerializeField]
        private string _topicName_texture = "image";

        [SerializeField]
        private string _frameId = "camera_link";

        private PointCloud2Serializer _serializer_pc;

        private bool _init = false;

        protected override void Init()
        {
            if (!_sensor.initialized) return;
            _ros.RegisterPublisher<PointCloud2Msg>(_topicName_pc);
            _topicName_texture += "/compressed";
            _ros.RegisterPublisher<CompressedImageMsg>(_topicName_texture);

            _serializer_pc = new PointCloud2Serializer();
            _serializer_pc.Init(_frameId, ref _sensor.points, _sensor.pointsNum);

            _serializer.Init(_frameId);

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

            _serializer.Serialize(time, _sensor.texture, _sensor.quality);

            _ros.Publish(_topicName_pc, _serializer_pc.msg);
            _ros.Publish(_topicName_texture, _serializer.msg);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using RosMessageTypes.Velodyne;

namespace UnitySensors.ROS
{
    /*
    [RequireComponent(typeof(VelodyneSensor))]
    public class VelodynePacketsPublisher : Publisher<VelodyneSensor, Serializer>
    {
        [SerializeField]
        private string _topicName = "velodyne_packets";

        [SerializeField]
        private string _frameId = "velodyne_link";

        private VelodyneMsgSerializer _serializer_vmsg;
        private bool _init = false;

        protected override void Init()
        {
            if (!_sensor.initialized) return;
            _ros.RegisterPublisher<VelodyneScanMsg>(_topicName);

            _serializer_vmsg = new VelodyneMsgSerializer();
            _serializer_vmsg.Init(_frameId, ref _sensor.distances, ref _sensor.intensities, (int)_sensor.pointsNum, _sensor.azimuthResolution);
            _init = true;
        }
        private void OnApplicationQuit()
        {
            _serializer_vmsg.Dispose();
        }

        protected override void Publish(float time)
        {
            if (!_init)
            {
                if (_sensor.initialized) Init();
                return;
            }
            _sensor.CompleteJob();
            _serializer_vmsg.Serialize(time);
            _ros.Publish(_topicName, _serializer_vmsg.msg);
        }
    }
    */
}

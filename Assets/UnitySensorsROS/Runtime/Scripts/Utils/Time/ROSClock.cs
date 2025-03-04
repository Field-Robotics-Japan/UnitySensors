using System;
using UnityEngine;

using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Rosgraph;
using UnitySensors.Interface.Std;

namespace UnitySensors.ROS.Utils.Time
{
    public class ROSClock : MonoBehaviour, ITimeInterface
    {
        [SerializeField]
        private string _topicName = "/clock";

        [SerializeField]
        private float _frequency = 100.0f;
        private ROSConnection _ros;
        private ClockMsg _message;
        private float _time;
        private float _frequency_inv;
        private float _dt;

        public float time => _time;
        private void Awake()
        {
            _dt = 0.0f;
            _frequency_inv = 1.0f / _frequency;
        }

        private void Start()
        {
            this._ros = ROSConnection.GetOrCreateInstance();

            this._ros.RegisterPublisher<ClockMsg>(this._topicName);

            this._message = new ClockMsg();
            this._message.clock.sec = 0;
            this._message.clock.nanosec = 0;
        }

        private void Update()
        {
            _dt += UnityEngine.Time.deltaTime;
            if (_dt < _frequency_inv) return;
            _time = UnityEngine.Time.time;
            _dt -= _frequency_inv;
#if ROS2
            int sec = (int)Math.Truncate(time);
#else
            uint sec = (uint)Math.Truncate(time);
#endif
            uint nanosec = (uint)((time - sec) * 1e+9);
            _message.clock.sec = sec;
            _message.clock.nanosec = nanosec;

            _ros.Publish(this._topicName, this._message);
        }
    }
}
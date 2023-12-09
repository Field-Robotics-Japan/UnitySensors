using System;
using UnityEngine;
using UnitySensors.Sensor;
using RosMessageTypes.Std;

namespace UnitySensors.ROS.Serializer
{
    [System.Serializable]
    public class HeaderSerializer : RosMsgSerializer<UnitySensor, HeaderMsg>
    {
        [SerializeField]
        private string _frame_id;

        public override void Init(UnitySensor sensor)
        {
            base.Init(sensor);
            _msg.frame_id = _frame_id;
#if ROS2
#else
            _msg.seq = 0;
#endif
        }

        public override HeaderMsg Serialize()
        {
#if ROS2
            int sec = (int)Math.Truncate(sensor.time);
#else
            uint sec = (uint)Math.Truncate(sensor.time);
#endif
            _msg.stamp.sec = sec;
            _msg.stamp.nanosec = (uint)((sensor.time - sec) * 1e+9);
#if ROS2
#else
            _msg.seq++;
#endif
            return _msg;
        }
    }
}

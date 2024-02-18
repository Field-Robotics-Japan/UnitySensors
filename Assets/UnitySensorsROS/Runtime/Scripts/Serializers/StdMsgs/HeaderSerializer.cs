using System;
using UnityEngine;
using RosMessageTypes.Std;

using UnitySensors.Interface.Std;

namespace UnitySensors.ROS.Serializer.Std
{
    [System.Serializable]
    public class HeaderSerializer : RosMsgSerializer<HeaderMsg>
    {
        [SerializeField]
        private string _frame_id;

        private ITimeInterface _source;

        public override void Init(MonoBehaviour source)
        {
            base.Init(source);
            _source = (ITimeInterface)source;

            _msg = new HeaderMsg();

            _msg.frame_id = _frame_id;
#if ROS2
#else
            _msg.seq = 0;
#endif
        }

        public override HeaderMsg Serialize()
        {
#if ROS2
            int sec = (int)Math.Truncate(_source.time);
#else
            uint sec = (uint)Math.Truncate(_source.time);
#endif
            _msg.stamp.sec = sec;
            _msg.stamp.nanosec = (uint)((_source.time - sec) * 1e+9);
#if ROS2
#else
            _msg.seq++;
#endif
            return _msg;
        }

        public override bool IsCompatible(MonoBehaviour source)
        {
            return (source is ITimeInterface);
        }
    }
}

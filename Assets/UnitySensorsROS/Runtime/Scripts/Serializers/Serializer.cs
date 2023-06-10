using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using RosMessageTypes.Std;

namespace UnitySensors.ROS
{
    public class Serializer
    {
        protected class AutoHeader
        {
            private HeaderMsg _header;
            public HeaderMsg header { get => _header; }

            public void Init(string frame_id)
            {
                _header = new HeaderMsg();
                _header.frame_id = frame_id;
                _header.seq = 0;
            }

            public void Serialize(float time)
            {
                uint sec = (uint)Math.Truncate(time);
                _header.stamp.sec = sec;
                _header.stamp.nanosec = (uint)((time - sec) * 1e+9);
                _header.seq++;
            }
        }
    }
}

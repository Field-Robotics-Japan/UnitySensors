using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using RosMessageTypes.Std;

namespace UnitySensors.ROS
{
    /// <summary>
    /// センサが取得したデータをROSのMessage型にシリアライズする
    /// </summary>
    [System.Serializable]
    public class Serializer
    {
        /// <summary>
        /// std_msgs/Headerの生成を行う
        /// </summary>
        protected class AutoHeader
        {
            private HeaderMsg _header;
            public HeaderMsg header { get => _header; }

            /// <summary>
            /// 初期化関数
            /// </summary>
            public void Init(string frame_id)
            {
                _header = new HeaderMsg();
                _header.frame_id = frame_id;
#if ROS2
#else
                _header.seq = 0;
#endif
            }

            /// <summary>
            /// シリアライズ関数
            /// </summary>
            public void Serialize(float time)
            {
#if ROS2
                int sec = (int)Math.Truncate(time);
#else
                uint sec = (uint)Math.Truncate(time);
# endif
                _header.stamp.sec = sec;
                _header.stamp.nanosec = (uint)((time - sec) * 1e+9);
#if ROS2
#else
                _header.seq++;
#endif
            }
        }
    }
}

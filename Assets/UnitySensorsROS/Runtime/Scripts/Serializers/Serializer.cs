using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using RosMessageTypes.Std;

namespace UnitySensors.ROS
{
    /// <summary>
    /// �Z���T���擾�����f�[�^��ROS��Message�^�ɃV���A���C�Y����
    /// </summary>
    [System.Serializable]
    public class Serializer
    {
        /// <summary>
        /// std_msgs/Header�̐������s��
        /// </summary>
        protected class AutoHeader
        {
            private HeaderMsg _header;
            public HeaderMsg header { get => _header; }

            /// <summary>
            /// �������֐�
            /// </summary>
            public void Init(string frame_id)
            {
                _header = new HeaderMsg();
                _header.frame_id = frame_id;
                _header.seq = 0;
            }

            /// <summary>
            /// �V���A���C�Y�֐�
            /// </summary>
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

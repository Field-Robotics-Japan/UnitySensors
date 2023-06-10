using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.Robotics.ROSTCPConnector.ROSGeometry;
using RosMessageTypes.Sensor;

namespace UnitySensors.ROS
{
    [System.Serializable]
    public class IMUSerializer : Serializer
    {
        private ImuMsg _msg;

        private AutoHeader _header;

        public ImuMsg msg { get => _msg; }

        public void Init(string frame_id)
        {
            _msg = new ImuMsg();
            _header = new AutoHeader();

            _header.Init(frame_id);
        }

        public ImuMsg Serialize(float time, Vector3 acceleration, Quaternion rotation, Vector3 angularVelocity)
        {
            _header.Serialize(time);
            _msg.header = _header.header;

            Vector3<FLU> acceleration_ros = acceleration.To<FLU>();
            Quaternion<FLU> orientation_ros = rotation.To<FLU>();
            Vector3<FLU> angularVelocity_ros = angularVelocity.To<FLU>();

            _msg.linear_acceleration = acceleration_ros;
            _msg.orientation = orientation_ros;
            _msg.angular_velocity = angularVelocity_ros;
            return _msg;
        }
    }
}

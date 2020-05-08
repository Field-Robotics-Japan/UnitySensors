using System;
using UnityEngine;

namespace RosSharp.RosBridgeClient
{
    public class ImuSensor : UnityPublisher<MessageTypes.Sensor.Imu>
    {
        public Transform PublishedTransform;
        public string FrameId = "Unity";
        private MessageTypes.Sensor.Imu message;

        private float deltaTime;

        ///  values
        private Vector3 _AngVel = Vector3.zero;
        private Vector3 _LinAcc = Vector3.zero;

        /// previous value
        private Vector3 _prePosition = Vector3.zero;
        private Vector3 _preRotate = Vector3.zero;
        private Vector3 _preAngVel = Vector3.zero;
        private Vector3 _preLinVel = Vector3.zero;
        private Vector3 _preLinAcc = Vector3.zero;

        protected override void Start()
        {
            base.Start();
            InitializeMessage();
        }
        private void FixedUpdate()
        {
            UpdateMessage();
        }

        private void InitializeMessage()
        {
            message = new MessageTypes.Sensor.Imu();
            message.header.frame_id = FrameId;
        }

        private void UpdateMessage()
        {
            message.header.Update();
            Vector3 position = PublishedTransform.position;
            Quaternion quaternion = PublishedTransform.rotation;
            deltaTime = Time.deltaTime;
            message.orientation = GetGeometryQuaternion(quaternion);
            message.angular_velocity = GetAngularVelocity(quaternion.eulerAngles);
            message.linear_acceleration = GetLinearAcceleration(position, quaternion.eulerAngles);
            Publish(message);
        }

        private MessageTypes.Geometry.Quaternion GetGeometryQuaternion(Quaternion quaternion)
        {
            MessageTypes.Geometry.Quaternion geometryQuaternion = new MessageTypes.Geometry.Quaternion();
            geometryQuaternion.x = quaternion.Unity2Ros().x;
            geometryQuaternion.y = quaternion.Unity2Ros().y;
            geometryQuaternion.z = quaternion.Unity2Ros().z;
            geometryQuaternion.w = quaternion.Unity2Ros().w;
            return geometryQuaternion;
        }

        private MessageTypes.Geometry.Vector3 GetAngularVelocity(Vector3 rotation)
        {
            _AngVel = (rotation - _preRotate) / deltaTime;
            _preRotate = rotation;

            MessageTypes.Geometry.Vector3 result = new MessageTypes.Geometry.Vector3();
            result.x = _AngVel.Unity2Ros().x;
            result.y = _AngVel.Unity2Ros().y;
            result.z = _AngVel.Unity2Ros().z;
            return result;
        }

        private MessageTypes.Geometry.Vector3 GetLinearAcceleration(Vector3 pose, Vector3 rotation)
        {
            // move
            Vector3 linVel = (pose - _prePosition) / deltaTime;
            _LinAcc = (linVel - _preLinVel) / deltaTime;
            _preLinVel = linVel;
            _preLinAcc = _LinAcc;

            //gravity

            /* not yet */

            MessageTypes.Geometry.Vector3 result = new MessageTypes.Geometry.Vector3();
            result.x = _LinAcc.Unity2Ros().x;
            result.y = _LinAcc.Unity2Ros().y;
            result.z = _LinAcc.Unity2Ros().z;
            return result;
        }
    }
}

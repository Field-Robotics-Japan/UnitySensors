using System.IO;
using UnityEngine;
using SensorNoise;

namespace RosSharp.RosBridgeClient
{
    public class ImuSensor : UnityPublisher<MessageTypes.Sensor.Imu>
    {
        public bool EnableNoise;
        public bool EnableBoxMullerNoise;
        public bool EnableBiasNoise;
        public NoiseSetteing Setting = new NoiseSetteing();

        [System.Serializable]
        public class NoiseSetteing
        {
            public Vector4 QuatSigma;
            public Vector4 QuatBias;
            public Vector3 AngVelSigma;
            public Vector3 AngVelBias;
            public Vector3 LinAccSigma;
            public Vector3 LinAccBias;
        }

        public string FrameId = "imu";
        private Transform trans;
        private Rigidbody rb;
        private MessageTypes.Sensor.Imu message;
        private float deltaTime;

        /// previous value
        private Vector3 LastVelocity = Vector3.zero;

        protected override void Start()
        {
            base.Start();
            InitializeMessage();
            rb = GetComponent<Rigidbody>();
            trans = GetComponent<Transform>();
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
            Transform transform = trans;
            Rigidbody rigidbody = rb;
            deltaTime = Time.deltaTime;
            message.orientation = GetGeometryQuaternion(transform);
            message.angular_velocity = GetAngularVelocity(rigidbody);
            message.linear_acceleration = GetLinearAcceleration(transform, rigidbody);

            if (EnableNoise)
            {
                if (EnableBoxMullerNoise) applyBoxMuller(ref message);
                if (EnableBiasNoise) applyBias(ref message);
            }

            Debug.Log(message.angular_velocity.z);

            Publish(message);
        }

        private MessageTypes.Geometry.Quaternion GetGeometryQuaternion(Transform transform)
        {
            MessageTypes.Geometry.Quaternion geometryQuaternion = new MessageTypes.Geometry.Quaternion();
            geometryQuaternion.x = transform.rotation.Unity2Ros().x;
            geometryQuaternion.y = transform.rotation.Unity2Ros().y;
            geometryQuaternion.z = transform.rotation.Unity2Ros().z;
            geometryQuaternion.w = transform.rotation.Unity2Ros().w;
            return geometryQuaternion;
        }

        private MessageTypes.Geometry.Vector3 GetAngularVelocity(Rigidbody rigidbody)
        {
            MessageTypes.Geometry.Vector3 result = new MessageTypes.Geometry.Vector3();
            result.x = rigidbody.angularVelocity.Unity2Ros().x;
            result.y = rigidbody.angularVelocity.Unity2Ros().y;
            result.z = rigidbody.angularVelocity.Unity2Ros().z;
            return result;
        }

        private MessageTypes.Geometry.Vector3 GetLinearAcceleration(Transform transform, Rigidbody rigidbody)
        {
            // move element
            Vector3 localLinearVelocity = transform.InverseTransformDirection(rigidbody.velocity);
            Vector3 acceleration = ( localLinearVelocity - LastVelocity ) / deltaTime;
            LastVelocity = localLinearVelocity;
            //gravity element
            acceleration += transform.InverseTransformDirection(Physics.gravity);

            MessageTypes.Geometry.Vector3 result = new MessageTypes.Geometry.Vector3();
            result.x = acceleration.Unity2Ros().x;
            result.y = acceleration.Unity2Ros().y;
            result.z = acceleration.Unity2Ros().z;
            return result;
        }

        private void applyBoxMuller(ref MessageTypes.Sensor.Imu msg )
        {
            var BoxMullerNoise = new BoxMullerNoise();
            msg.orientation.x = BoxMullerNoise.apply(msg.orientation.x, Setting.QuatSigma[0]);
            msg.orientation.y = BoxMullerNoise.apply(msg.orientation.y, Setting.QuatSigma[1]);
            msg.orientation.z = BoxMullerNoise.apply(msg.orientation.z, Setting.QuatSigma[2]);
            msg.orientation.w = BoxMullerNoise.apply(msg.orientation.w, Setting.QuatSigma[3]);
            msg.angular_velocity.x = BoxMullerNoise.apply(msg.angular_velocity.x, Setting.AngVelSigma[0]);
            msg.angular_velocity.y = BoxMullerNoise.apply(msg.angular_velocity.y, Setting.AngVelSigma[1]);
            msg.angular_velocity.z = BoxMullerNoise.apply(msg.angular_velocity.z, Setting.AngVelSigma[2]);
            msg.linear_acceleration.x = BoxMullerNoise.apply(msg.linear_acceleration.x, Setting.LinAccSigma[0]);
            msg.linear_acceleration.y = BoxMullerNoise.apply(msg.linear_acceleration.y, Setting.LinAccSigma[1]);
            msg.linear_acceleration.z = BoxMullerNoise.apply(msg.linear_acceleration.z, Setting.LinAccSigma[2]);
        }

        private void applyBias(ref MessageTypes.Sensor.Imu msg )
        {
            var BiasNoise = new BiasNoise();
            msg.orientation.x = BiasNoise.apply( msg.orientation.x, Setting.QuatBias[0] );
            msg.orientation.y = BiasNoise.apply( msg.orientation.y, Setting.QuatBias[1] );
            msg.orientation.z = BiasNoise.apply( msg.orientation.z, Setting.QuatBias[2] );
            msg.orientation.w = BiasNoise.apply( msg.orientation.w, Setting.QuatBias[3] );
            msg.angular_velocity.x = BiasNoise.apply( msg.angular_velocity.x, Setting.AngVelBias[0] );
            msg.angular_velocity.y = BiasNoise.apply( msg.angular_velocity.y, Setting.AngVelBias[1] );
            msg.angular_velocity.z = BiasNoise.apply( msg.angular_velocity.z, Setting.AngVelBias[2] );
            msg.linear_acceleration.x = BiasNoise.apply( msg.linear_acceleration.x, Setting.LinAccBias[0] );
            msg.linear_acceleration.y = BiasNoise.apply( msg.linear_acceleration.y, Setting.LinAccBias[1] );
            msg.linear_acceleration.z = BiasNoise.apply( msg.linear_acceleration.z, Setting.LinAccBias[2] );

        }
    }
}

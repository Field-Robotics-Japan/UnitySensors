using System.Collections;
using UnityEngine;
using UnitySensors.Attribute;
using UnitySensors.Interface.Sensor;

namespace UnitySensors.Sensor.IMU
{
    public class IMUSensor : UnitySensor, IImuDataInterface
    {
        private Transform _transform;

        [SerializeField, ReadOnly]
        private Vector3 _position;
        [SerializeField, ReadOnly]
        private Vector3 _velocity;
        [SerializeField, ReadOnly]
        private Vector3 _acceleration;
        [SerializeField, ReadOnly]
        private Quaternion _rotation;
        [SerializeField, ReadOnly]
        private Vector3 _angularVelocity;

        private Vector3 _position_tmp;
        private Vector3 _velocity_tmp;
        private Vector3 _acceleration_tmp;
        private Quaternion _rotation_tmp;
        private Vector3 _angularVelocity_tmp;

        private Vector3 _position_last;
        private Vector3 _velocity_last;
        private Quaternion _rotation_last;

        public Vector3 position { get => _position; }
        public Vector3 velocity { get => _velocity; }
        public Vector3 acceleration { get => _acceleration; }
        public Quaternion rotation { get => _rotation; }
        public Vector3 angularVelocity { get => _angularVelocity; }

        public Vector3 localVelocity { get => _transform.InverseTransformDirection(_velocity); }
        public Vector3 localAcceleration { get => _transform.InverseTransformDirection(_acceleration.normalized) * _acceleration.magnitude; }

        private Vector3 _gravityDirection;
        private float _gravityMagnitude;
        private float _time_last;

        protected override void Init()
        {
            _transform = this.transform;
            _gravityDirection = Physics.gravity.normalized;
            _gravityMagnitude = Physics.gravity.magnitude;
        }

        /// <summary>
        /// Mean angular velocity [rad/s] of the rotation taking
        /// <paramref name="previous"/> to <paramref name="current"/> over
        /// <paramref name="dt"/> seconds, always measured along the short arc.
        /// </summary>
        /// <remarks>
        /// Quaternions double-cover rotations (q and -q are the same
        /// rotation), and consecutive transform.rotation samples can land on
        /// opposite hemispheres once the accumulated rotation passes a
        /// multiple of 2*pi. ToAngleAxis on such a delta reports the LONG way
        /// around -- ~(360 deg - delta) about the inverted axis -- so the
        /// angular velocity spikes to ~2*pi/dt for one sample
        /// (Field-Robotics-Japan/UnitySensors#155). Folding the delta onto
        /// the w >= 0 hemisphere makes ToAngleAxis measure the short arc.
        /// </remarks>
        public static Vector3 AngularVelocityBetween(Quaternion previous, Quaternion current, float dt)
        {
            Quaternion delta = Quaternion.Inverse(previous) * current;
            if (delta.w < 0.0f)
            {
                delta = new Quaternion(-delta.x, -delta.y, -delta.z, -delta.w);
            }
            delta.ToAngleAxis(out float angle, out Vector3 axis);
            return axis * (angle * Mathf.Deg2Rad / dt);
        }

        public override IEnumerator UpdateSensorOnce()
        {
            //FIXME: IMU sensor should be updated at a fixed frequency
            float dt = Time.deltaTime;

            _position_tmp = _transform.position;
            _velocity_tmp = (_position_tmp - _position_last) / dt;
            _acceleration_tmp = (_velocity_tmp - _velocity_last) / dt;
            _acceleration_tmp -= _transform.InverseTransformDirection(_gravityDirection) * _gravityMagnitude;

            _rotation_tmp = _transform.rotation;
            _angularVelocity_tmp = AngularVelocityBetween(_rotation_last, _rotation_tmp, dt);

            _position_last = _position_tmp;
            _velocity_last = _velocity_tmp;
            _rotation_last = _rotation_tmp;

            yield return base.UpdateSensorOnce();
        }

        protected override IEnumerator UpdateSensor()
        {
            //FIXME: The linear acceleration and angular velocity should be in imu local frame
            _position = _position_tmp;
            _velocity = _velocity_tmp;
            _acceleration = _acceleration_tmp;

            _rotation = _rotation_tmp;
            _angularVelocity = _angularVelocity_tmp;
            yield return null;
        }

        protected override void OnSensorDestroy()
        {
        }
    }
}

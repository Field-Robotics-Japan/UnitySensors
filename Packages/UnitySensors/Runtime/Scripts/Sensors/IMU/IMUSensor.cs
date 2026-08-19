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

        public override IEnumerator UpdateSensorOnce()
        {
            // dt must be the REAL time since the previous sensor update: the
            // position/rotation deltas below span the whole sensor period
            // (several frames at low frame rates, and one or two frames
            // depending on scheduling when the requested frequency is at or
            // above the frame rate), so dividing them by one frame's
            // Time.deltaTime overestimates every derivative by
            // (actual sample spacing / frame time) -- measured ~2x angular
            // velocity at 30 FPS with a 20 Hz IMU, and it is what makes the
            // velocity and angular velocity read too large and unstable at
            // high Frequency settings (Field-Robotics-Japan/UnitySensors#156).
            float now = Time.time;
            float dt = (_time_last > 0.0f) ? (now - _time_last) : Time.deltaTime;
            if (dt <= 0.0f) dt = Time.deltaTime;
            _time_last = now;

            _position_tmp = _transform.position;
            _velocity_tmp = (_position_tmp - _position_last) / dt;
            _acceleration_tmp = (_velocity_tmp - _velocity_last) / dt;
            _acceleration_tmp -= _transform.InverseTransformDirection(_gravityDirection) * _gravityMagnitude;

            _rotation_tmp = _transform.rotation;
            Quaternion rotation_delta = Quaternion.Inverse(_rotation_last) * _rotation_tmp;
            rotation_delta.ToAngleAxis(out float angle, out Vector3 axis);
            float angularSpeed = (angle * Mathf.Deg2Rad) / dt;
            _angularVelocity_tmp = axis * angularSpeed;

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

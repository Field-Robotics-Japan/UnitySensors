using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnitySensors
{
    public class IMUSensor : Sensor
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

        private float _dt { get => base._frequency_inv; }

        private Vector3 _gravity;
        private float _gravityMagnitude;

        protected override void Init()
        {
            _transform = this.transform;

            _gravity = Physics.gravity;
            _gravityMagnitude = _gravity.magnitude;
        }

        protected override void UpdateSensor()
        {
            _position = _transform.position;
            _rotation = _transform.rotation;

            _velocity = (_position - _position_last) / _dt;
            _acceleration = (_velocity - _velocity_last) / _dt;
            _acceleration += _transform.InverseTransformDirection(_gravity).normalized * _gravityMagnitude;

            Quaternion rotation_delta = Quaternion.Inverse(_rotation_last) * _rotation;
            rotation_delta.ToAngleAxis(out float angle, out Vector3 axis);
            float angularSpeed = (angle * Mathf.Deg2Rad) / _dt;
            _angularVelocity = axis * angularSpeed;

            _position_last = _position;
            _velocity_last = _velocity;
            _rotation_last = _rotation;
        }
    }
}

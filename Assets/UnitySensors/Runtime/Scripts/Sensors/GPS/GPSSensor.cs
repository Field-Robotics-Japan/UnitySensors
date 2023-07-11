using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnitySensors
{
    public class GPSSensor : Sensor
    {
        [SerializeField]
        private GeoCoordinate _baseCoordinate = new GeoCoordinate(35.71020206575301, 139.81070039691542, 3.0f);

        private GeoCoordinateConverter _gcc;
        private Transform _transform;

        [SerializeField, ReadOnly]
        private Vector3 _position;
        [SerializeField, ReadOnly]
        private Vector3 _velocity;
        [SerializeField]
        private GeoCoordinate _coordinate;

        private Vector3 _position_last;

        public Vector3 position { get => _position; }
        public Vector3 velocity { get => _velocity; }
        public GeoCoordinate coordinate { get => _coordinate; }

        private float _dt { get => base._frequency_inv; }

        protected override void Init()
        {
            _gcc = new GeoCoordinateConverter(_baseCoordinate.latitude, _baseCoordinate.longitude);

            _transform = this.transform;
            _position_last = _transform.position;
        }

        protected override void UpdateSensor()
        {
            _position = _transform.position;

            _velocity = (_position - _position_last) / _dt;
            (_coordinate.latitude, _coordinate.longitude) = _gcc.XZ2LatLon(_position.x, _position.z);
            _coordinate.altitude = _baseCoordinate.altitude + _position.y;

            _position_last = _position;
        }
    }
}

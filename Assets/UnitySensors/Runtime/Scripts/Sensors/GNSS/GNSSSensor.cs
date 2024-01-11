using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnitySensors.Attribute;
using UnitySensors.Utils.GeoCoordinate;

namespace UnitySensors.Sensor.GNSS
{
    public class GNSSSensor : UnitySensor
    {
        [SerializeField]
        private GeoCoordinate _baseCoordinate = new GeoCoordinate(35.71020206575301, 139.81070039691542, 3.0f);

        private Transform _transform;
        private GeoCoordinateConverter _gcc;

        [SerializeField]
        private GeoCoordinate _coordinate;
        public GeoCoordinate coordinate { get => _coordinate; }

        protected override void Init()
        {
            _transform = this.transform;
            _gcc = new GeoCoordinateConverter(_baseCoordinate.latitude, _baseCoordinate.longitude);
            _coordinate = new GeoCoordinate(_baseCoordinate.latitude, _baseCoordinate.longitude, _baseCoordinate.altitude);
        }

        protected override void UpdateSensor()
        {
            Vector3 position = _transform.position;
            (_coordinate.latitude, _coordinate.longitude) = _gcc.XZ2LatLon(position.x, position.z);
            _coordinate.altitude = _baseCoordinate.altitude + position.y;
            
            if (onSensorUpdated != null)
                onSensorUpdated.Invoke();
        }

        protected override void OnSensorDestroy()
        {
        }
    }
}

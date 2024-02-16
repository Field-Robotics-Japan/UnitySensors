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
        private GeoCoordinateSystem _coordinateSystem;

        private Transform _transform;
        private GeoCoordinateConverter _gcc;

        [SerializeField]
        private GeoCoordinate _coordinate;
        public GeoCoordinate coordinate { get => _coordinate; }

        protected override void Init()
        {
            _transform = this.transform;
        }

        protected override void UpdateSensor()
        {
            _coordinate = _coordinateSystem.GetCoordinate(_transform.position);
            if (onSensorUpdated != null)
                onSensorUpdated.Invoke();
        }

        protected override void OnSensorDestroy()
        {
        }
    }
}

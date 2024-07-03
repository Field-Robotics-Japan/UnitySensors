using UnityEngine;

using UnitySensors.DataType.Geometry;
using UnitySensors.Interface.Geometry;

namespace UnitySensors.Sensor.GNSS
{
    public class GNSSSensor : UnitySensor, IGeoCoordinateInterface
    {
        [SerializeField]
        private GeoCoordinateSystem _coordinateSystem;

        private Transform _transform;

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

using UnityEngine;

using UnitySensors.DataType.Geometry;
using UnitySensors.Utils.Geometry;

namespace UnitySensors.Sensor.GNSS
{
    public class GeoCoordinateSystem : MonoBehaviour
    {
        [SerializeField]
        private GeoCoordinate _coordinate = new GeoCoordinate(35.71020206575301, 139.81070039691542, 3.0f);

        private Transform _transform;
        private GeoCoordinateConverter _converter;

        private void Awake()
        {
            _transform = this.transform;
            _converter = new GeoCoordinateConverter(_coordinate);
        }

        public GeoCoordinate GetCoordinate(Vector3 worldPosition)
        {
            Vector3 localPosition = _transform.InverseTransformPoint(worldPosition);
            return _converter.Convert(new Vector3D(localPosition));
        }
    }
}
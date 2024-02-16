using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnitySensors.Utils.GeoCoordinate;

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
            _converter = new GeoCoordinateConverter(_coordinate.latitude, _coordinate.longitude);
        }

        public GeoCoordinate GetCoordinate(Vector3 worldPosition)
        {
            Vector3 localPosition = _transform.InverseTransformPoint(worldPosition);
            double latitude, longitude;
            (latitude, longitude) = _converter.XZ2LatLon(localPosition.x, localPosition.z);
            return new GeoCoordinate(latitude, longitude, localPosition.y + _coordinate.altitude);
        }
    }
}
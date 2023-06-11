using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnitySensors
{
    public class GroundTruthSensor : Sensor
    {
        [ReadOnly]
        private Vector3 _position;
        [ReadOnly]
        private Quaternion _rotation;

        private Transform _transform;
        protected override void Init()
        {
            _transform = transform;
        }

        protected override void UpdateSensor()
        {
            _position = _transform.position;
            _rotation = _transform.rotation;
        }
    }
}

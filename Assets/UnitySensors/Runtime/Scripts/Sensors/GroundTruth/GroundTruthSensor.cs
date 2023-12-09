using UnityEngine;
using UnitySensors.Data.Pose;

namespace UnitySensors.Sensor.GroundTruth
{
    public class GroundTruthSensor : UnitySensor, IPoseInterface
    {
        private Transform _transform;

        public Vector3 position { get => _transform.position; }
        public Quaternion rotation { get => _transform.rotation; }

        protected override void Init()
        {
            _transform = this.transform;
        }

        protected override void UpdateSensor()
        {
        }

        protected override void OnSensorDestroy()
        {
        }
    }
}

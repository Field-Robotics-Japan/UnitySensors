using UnityEngine;

namespace UnitySensors.Interface.Sensor
{
    public interface IImuDataInterface
    {
        public Vector3 acceleration { get; }
        public Quaternion rotation { get; }
        public Vector3 angularVelocity { get; }
    }
}

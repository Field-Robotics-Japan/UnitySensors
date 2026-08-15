using UnityEngine;

namespace UnitySensors.Interface.Sensor
{
    /// <summary>
    /// A force/torque pair expressed in the sensor's local frame.
    /// </summary>
    public interface IWrenchInterface
    {
        public Vector3 force { get; }
        public Vector3 torque { get; }
    }
}

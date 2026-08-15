using UnityEngine;

namespace UnitySensors.DataType.Sensor
{
    [System.Serializable]
    public struct ContactData
    {
        public string colliderName;
        public Vector3 position;
        public Vector3 normal;
        public Vector3 force;
    }
}

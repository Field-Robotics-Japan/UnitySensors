using Unity.Mathematics;

using UnitySensors.Interface.Sensor.PointCloud;

using UnityEngine;

namespace UnitySensors.DataType.Sensor.PointCloud
{
    [System.Serializable]
    public struct PointXYZRGB : IPointInterface
    {
        [SerializeField]
        private float3 _position;
        public byte r;
        public byte g;
        public byte b;
        public byte a;

        public float3 position { get => _position; set => _position = value; }
    }
}

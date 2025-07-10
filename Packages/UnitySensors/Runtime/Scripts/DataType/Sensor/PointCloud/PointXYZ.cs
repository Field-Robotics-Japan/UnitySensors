using Unity.Mathematics;

using UnitySensors.Interface.Sensor.PointCloud;

namespace UnitySensors.DataType.Sensor.PointCloud
{
    public struct PointXYZ : IPointInterface
    {
        private float3 _position;
        public float3 position { get => _position; set => _position = value; }
    }
}
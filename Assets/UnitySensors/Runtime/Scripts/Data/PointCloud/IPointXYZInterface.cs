using Unity.Mathematics;

namespace UnitySensors.Data.PointCloud
{
    public interface IPointXYZInterface : IPointInterface
    {
        public float3 position { get; }
    }
}
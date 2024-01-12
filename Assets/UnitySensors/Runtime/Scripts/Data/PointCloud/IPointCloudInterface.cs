using Unity.Collections;

namespace UnitySensors.Data.PointCloud
{
    public interface IPointCloudInterface<T> where T : struct, IPointInterface
    {
        public PointCloud<T> pointCloud { get; }
        public int pointsNum { get; }
    }
}

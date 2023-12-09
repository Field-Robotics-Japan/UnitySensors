using Unity.Collections;

namespace UnitySensors.Data.PointCloud
{
    public interface IPointCloudInterface
    {
        public NativeArray<Point> points { get; }
        public int pointsNum { get; }
    }
}

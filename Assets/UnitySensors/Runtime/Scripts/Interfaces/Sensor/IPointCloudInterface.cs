using UnitySensors.DataType.Sensor;
using UnitySensors.Interface.Sensor.PointCloud;

namespace UnitySensors.Interface.Sensor
{
    public interface IPointCloudInterface<T> where T : struct, IPointInterface
    {
        public PointCloud<T> pointCloud { get; }
        public int pointsNum { get; }
    }
}
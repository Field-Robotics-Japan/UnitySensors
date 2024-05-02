using System;
using Unity.Collections;
using UnitySensors.Interface.Sensor.PointCloud;

namespace UnitySensors.DataType.Sensor
{
    public class PointCloud<T> : IDisposable where T : struct, IPointInterface
    {
        public NativeArray<T> points;

        public void Dispose()
        {
            points.Dispose();
        }
    }
}
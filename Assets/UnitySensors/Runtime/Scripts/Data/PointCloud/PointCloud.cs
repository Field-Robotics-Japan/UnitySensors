using System;
using Unity.Collections;
using Unity.Mathematics;

namespace UnitySensors.Data.PointCloud
{
    public struct PointCloud<T> : IDisposable where T : struct, IPointInterface
    {
        public NativeArray<T> points;

        public void Dispose()
        {
            points.Dispose();
        }
    }
}
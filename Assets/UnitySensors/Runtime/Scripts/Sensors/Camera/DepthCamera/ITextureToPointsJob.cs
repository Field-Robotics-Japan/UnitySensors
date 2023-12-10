using UnityEngine;

using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

using UnitySensors.Data.PointCloud;

namespace UnitySensors.Sensor.Camera
{
    [BurstCompile]
    public struct ITextureToPointsJob : IJobParallelFor
    {
        public float far;

        [ReadOnly]
        public NativeArray<float3> directions;

        [ReadOnly]
        public NativeArray<Color> pixels;

        public NativeArray<PointXYZ> points;

        public void Execute(int index)
        {
            float distance = pixels.AsReadOnly()[index].r;
            PointXYZ point = new PointXYZ()
            {
                position = directions[index] * far * Mathf.Clamp01(1.0f - distance)
            };
            points[index] = point;
        }
    }
}

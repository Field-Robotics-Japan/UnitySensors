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
        public float near;
        public float far;

        [ReadOnly]
        public NativeArray<float3> directions;

        [ReadOnly]
        public NativeArray<Color> pixels;

        public NativeArray<PointXYZ> points;

        public void Execute(int index)
        {
            float distance = Mathf.Clamp01(1.0f - pixels.AsReadOnly()[index].r) * far;
            distance = (near < distance && distance < far) ? distance/* + noises[index]*/ : 0;
            PointXYZ point = new PointXYZ()
            {
                position = directions[index] * distance
            };
            points[index] = point;
        }
    }
}

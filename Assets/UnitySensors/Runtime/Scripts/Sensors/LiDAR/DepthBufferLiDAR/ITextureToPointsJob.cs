using UnityEngine;

using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

using UnitySensors.DataType.Sensor.PointCloud;

namespace UnitySensors.Sensor.LiDAR
{
    [BurstCompile]
    public struct ITextureToPointsJob : IJobParallelFor
    {
        public float near;
        public float sqrNear;
        public float far;
        public float maxIntensity;
        public int indexOffset;

        [ReadOnly]
        public NativeArray<float3> directions;
        [ReadOnly]
        public NativeArray<int> pixelIndices;
        [ReadOnly]
        public NativeArray<float> noises;

        [ReadOnly]
        public NativeArray<Color> pixels;

        [WriteOnly]
        public NativeArray<PointXYZI> points;

        public void Execute(int index)
        {
            int pixelIndex = pixelIndices[index + indexOffset];
            float distance = pixels[pixelIndex].r;
            float distance_noised = distance + noises[index];
            distance = (near < distance && distance < far && near < distance_noised && distance_noised < far) ? distance_noised : 0;
            PointXYZI point = new PointXYZI()
            {
                position = directions[index + indexOffset] * distance,
                intensity = (distance != 0) ? maxIntensity * sqrNear / (distance * distance) : 0
            };

            points[index] = point;
        }
    }
}


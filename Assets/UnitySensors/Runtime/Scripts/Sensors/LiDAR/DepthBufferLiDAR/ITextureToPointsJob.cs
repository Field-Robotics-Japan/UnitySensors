using UnityEngine;

using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

using UnitySensors.Data.PointCloud;

namespace UnitySensors.Sensor.LiDAR
{
    [BurstCompile]
    public struct ITextureToPointsJob : IJobParallelFor
    {
        public float near;
        public float far;
        public int indexOffset;

        [ReadOnly]
        public NativeArray<float3> directions;
        [ReadOnly]
        public NativeArray<int> pixelIndices;
        [ReadOnly]
        public NativeArray<float> noises;

        [ReadOnly, NativeDisableParallelForRestriction]
        public NativeArray<Color> pixels;

        [NativeDisableParallelForRestriction]
        public NativeArray<PointXYZI> points;

        public void Execute(int index)
        {
            int pixelIndex = pixelIndices[index + indexOffset];
            float distance = (1.0f - Mathf.Clamp01(pixels.AsReadOnly()[pixelIndex].r)) * far;
            float distance_noised = distance + noises[index];
            distance = (near < distance && distance < far && near < distance_noised && distance_noised < far) ? distance_noised : 0;
            PointXYZI point = new PointXYZI()
            {
                position = directions[index + indexOffset] * distance
            };
            
            points[index] = point;
        }
    }
}


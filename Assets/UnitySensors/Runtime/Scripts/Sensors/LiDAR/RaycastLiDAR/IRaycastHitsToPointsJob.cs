using UnityEngine;

using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

using UnitySensors.Data.PointCloud;

namespace UnitySensors.Sensor.LiDAR
{
    [BurstCompile]
    public struct IRaycastHitsToPointsJob : IJobParallelFor
    {
        [ReadOnly]
        public float minRange;
        [ReadOnly]
        public float minRange_sqr;
        [ReadOnly]
        public float maxRange;
        [ReadOnly]
        public float maxIntensity;
        [ReadOnly, NativeDisableParallelForRestriction]
        public NativeArray<float3> directions;
        [ReadOnly]
        public int indexOffset;
        [ReadOnly]
        public NativeArray<RaycastHit> raycastHits;
        [ReadOnly]
        public NativeArray<float> noises;

        public NativeArray<PointXYZI> points;

        public void Execute(int index)
        {
            float distance = raycastHits[index].distance;
            float distance_noised = distance + noises[index];
            distance = (minRange < distance && distance < maxRange && minRange < distance_noised && distance_noised < maxRange) ? distance_noised : 0;
            PointXYZI point = new PointXYZI()
            {
                position = directions[index + indexOffset] * distance,
                intensity = (distance != 0) ? maxIntensity * minRange_sqr / (distance * distance) : 0
            };
            points[index] = point;
        }
    }
}

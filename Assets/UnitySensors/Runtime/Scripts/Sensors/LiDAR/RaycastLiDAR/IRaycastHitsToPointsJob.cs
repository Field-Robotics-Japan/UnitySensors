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

        public NativeArray<Point> points;

        public void Execute(int index)
        {
            float distance = raycastHits[index].distance;
            distance = (minRange < distance && distance < maxRange) ? distance + noises[index] : 0;
            Point point = new Point()
            {
                position = directions[index + indexOffset] * distance,
                intensity = (distance != 0) ? maxIntensity * minRange_sqr / (distance * distance) : 0
            };
            points[index] = point;
        }
    }
}

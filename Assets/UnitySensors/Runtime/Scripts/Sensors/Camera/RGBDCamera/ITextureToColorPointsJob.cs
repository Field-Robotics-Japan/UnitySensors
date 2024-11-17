using UnityEngine;

using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

using UnitySensors.DataType.Sensor.PointCloud;

namespace UnitySensors.Sensor.Camera
{
    [BurstCompile]
    public struct ITextureToColorPointsJob : IJobParallelFor
    {
        public float near;
        public float far;

        [ReadOnly]
        public NativeArray<float3> directions;

        [ReadOnly]
        public NativeArray<Color> depthPixels;
        [ReadOnly]
        public NativeArray<Color32> colorPixels;
        [ReadOnly]
        public NativeArray<float> noises;

        public NativeArray<PointXYZRGB> points;

        public void Execute(int index)
        {
            float distance = depthPixels.AsReadOnly()[index].r * far;
            float distance_noised = distance + noises[index];
            distance = (near < distance && distance < far && near < distance_noised && distance_noised < far) ? distance_noised : 0;

            float radius = distance / directions[index].z;
            PointXYZRGB point = new PointXYZRGB()
            {
                position = directions[index] * radius,
                r = colorPixels[index].r,
                g = colorPixels[index].g,
                b = colorPixels[index].b,
                a = (byte)(distance > 0 ? 255 : 0)
            };
            points[index] = point;
        }
    }
}

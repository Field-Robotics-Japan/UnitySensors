using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;

namespace UnitySensors.ROS.Serializer.PointCloud
{
    // TODO: Use ComputeShader to accelerate
    [BurstCompile]
    public struct IInvertXJob : IJobParallelFor
    {
        public int pointStep;

        [WriteOnly, NativeDisableParallelForRestriction, NativeDisableContainerSafetyRestriction]
        public NativeArray<byte> data;

        public void Execute(int index)
        {
            data[index * pointStep + 3] += 128;
        }
    }
}
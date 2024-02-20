using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;

namespace UnitySensors.ROS.Serializer.PointCloud
{
    [BurstCompile]
    public struct IInvertXJob : IJobParallelFor
    {
        public int pointStep;

        [NativeDisableParallelForRestriction]
        public NativeArray<byte> data;

        public void Execute(int index)
        {
            data[index * pointStep + 3] += 128;
        }
    }
}
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;

using UnitySensors.Data.PointCloud;
using UnitySensors.ROS.Data.Livox;

namespace UnitySensors.ROS.Serializer.PointCloud
{
    [BurstCompile]
    public struct IPointsToLivoxCustomMsgJob<T> : IJobParallelFor
        where T : struct, IPointXYZInterface
    {

        [ReadOnly]
        public NativeArray<T> points;

        public NativeArray<CustomPoint> data;

        public void Execute(int index)
        {
            data[index] = new CustomPoint()
            {
                offset_time = 0,
                x = points[index].position.x,
                y = points[index].position.y,
                z = points[index].position.z,
                reflectivity = 255,
                tag = 0,
                line = 0
            };
        }
    }
}
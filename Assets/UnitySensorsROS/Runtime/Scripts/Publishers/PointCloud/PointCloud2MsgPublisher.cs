using UnityEngine;
using RosMessageTypes.Sensor;
using UnitySensors.Data.PointCloud;
using UnitySensors.Sensor;
using UnitySensors.ROS.Serializer.PointCloud;

namespace UnitySensors.ROS.Publisher.PointCloud
{
    public class PointCloud2MsgPublisher<T, TT> : RosMsgPublisher<T, PointCloud2MsgSerializer<T, TT>, PointCloud2Msg>
        where T : UnitySensor, IPointCloudInterface<TT>
        where TT : struct, IPointInterface
    {
        private void OnDestroy()
        {
            _serializer.Dispose();
        }
    }
}

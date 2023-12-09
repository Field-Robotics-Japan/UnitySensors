using UnityEngine;
using RosMessageTypes.Sensor;
using UnitySensors.Data.PointCloud;
using UnitySensors.Sensor;
using UnitySensors.ROS.Serializer.PointCloud;

namespace UnitySensors.ROS.Publisher.PointCloud
{
    public class PointCloud2MsgPublisher<T> : RosMsgPublisher<T, PointCloud2MsgSerializer<T>, PointCloud2Msg> where T : UnitySensor, IPointCloudInterface
    {
        private void OnDestroy()
        {
            _serializer.Dispose();
        }
    }
}

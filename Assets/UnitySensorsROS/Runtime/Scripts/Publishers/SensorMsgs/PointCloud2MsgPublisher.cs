using RosMessageTypes.Sensor;

using UnitySensors.Interface.Sensor.PointCloud;
using UnitySensors.ROS.Serializer.PointCloud;

namespace UnitySensors.ROS.Publisher.Sensor
{
    public class PointCloud2MsgPublisher<T> : RosMsgPublisher<PointCloud2MsgSerializer<T>, PointCloud2Msg> where T : struct, IPointInterface
    {
    }
}

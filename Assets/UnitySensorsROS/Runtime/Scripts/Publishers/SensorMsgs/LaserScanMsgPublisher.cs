using RosMessageTypes.Sensor;

using UnitySensors.Interface.Sensor.PointCloud;
using UnitySensors.ROS.Serializer.PointCloud;

namespace UnitySensors.ROS.Publisher.Sensor
{
    public class LaserScanMsgPublisher : RosMsgPublisher<LaserScanMsgSerializer, LaserScanMsg>
    {
    }
}

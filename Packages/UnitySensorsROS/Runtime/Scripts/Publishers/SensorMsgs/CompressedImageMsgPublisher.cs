using RosMessageTypes.Sensor;
using UnitySensors.ROS.Serializer.Sensor;

namespace UnitySensors.ROS.Publisher.Sensor
{
    public class CompressedImageMsgPublisher : RosMsgPublisher<CompressedImageMsgSerializer, CompressedImageMsg>
    {
    }
}

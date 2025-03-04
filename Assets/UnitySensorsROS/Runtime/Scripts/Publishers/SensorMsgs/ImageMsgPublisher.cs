using RosMessageTypes.Sensor;
using UnitySensors.ROS.Serializer.Sensor;

namespace UnitySensors.ROS.Publisher.Sensor
{
    public class ImageMsgPublisher : RosMsgPublisher<ImageMsgSerializer, ImageMsg>
    {
    }
}

using RosMessageTypes.Sensor;
using UnitySensors.ROS.Serializer.Sensor;

namespace UnitySensors.ROS.Publisher.Camera
{
    public class CameraInfoMsgPublisher : RosMsgPublisher<CameraInfoMsgSerializer, CameraInfoMsg>
    {
    }
}

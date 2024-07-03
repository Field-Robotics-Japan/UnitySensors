using RosMessageTypes.Geometry;

using UnitySensors.ROS.Serializer.Geometry;

namespace UnitySensors.ROS.Publisher.Geometry
{
    public class PoseStampedMsgPublisher : RosMsgPublisher<PoseStampedMsgSerializer, PoseStampedMsg>
    {
    }
}

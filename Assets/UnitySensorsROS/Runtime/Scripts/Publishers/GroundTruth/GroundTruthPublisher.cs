using UnityEngine;
using RosMessageTypes.Geometry;

using UnitySensors.Sensor.GroundTruth;
using UnitySensors.ROS.Serializer.PoseStamped;

namespace UnitySensors.ROS.Publisher
{
    [RequireComponent(typeof(GroundTruth))]
    public class GroundTruthPublisher : RosMsgPublisher<GroundTruth, PoseStampedMsgSerializer<GroundTruth>, PoseStampedMsg>
    {
    }
}

using UnityEngine;
using RosMessageTypes.Geometry;

using UnitySensors.Sensor.GroundTruth;
using UnitySensors.ROS.Serializer.PoseStamped;

namespace UnitySensors.ROS.Publisher
{
    [RequireComponent(typeof(GroundTruthSensor))]
    public class GroundTruthPublisher : RosMsgPublisher<GroundTruthSensor, PoseStampedMsgSerializer<GroundTruthSensor>, PoseStampedMsg>
    {
    }
}

using UnityEngine;

using RosMessageTypes.Tf2;

using UnitySensors.Sensor.TF;
using UnitySensors.ROS.Serializer.TF;

namespace UnitySensors.ROS.Publisher.TF
{
    using TFSensor = Sensor.TF.TF;

    [RequireComponent(typeof(TFSensor))]
    public class TFMsgPublisher : RosMsgPublisher<TFSensor, TFMsgSerializer, TFMessageMsg>
    {
    }
}
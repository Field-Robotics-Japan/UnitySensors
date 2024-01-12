using UnityEngine;
using RosMessageTypes.Sensor;
using UnitySensors.Sensor.Camera;
using UnitySensors.ROS.Serializer.Camera;

namespace UnitySensors.ROS.Publisher.Camera
{
    [RequireComponent(typeof(CameraSensor))]
    public class CameraInfoMsgPublisher : RosMsgPublisher<CameraSensor, CameraInfoMsgSerializer, CameraInfoMsg>
    {    
    }
}

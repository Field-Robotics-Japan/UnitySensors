using UnityEngine;
using UnitySensors.Sensor.Camera;

namespace UnitySensors.ROS.Publisher.Image
{
    [RequireComponent(typeof(CameraSensor))]
    public class CameraImageMsgPublisher : ImageMsgPublisher<CameraSensor>
    {
    }
}

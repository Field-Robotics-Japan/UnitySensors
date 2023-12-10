using UnityEngine;
using UnitySensors.Sensor.Camera;
using UnitySensors.ROS.Publisher.PointCloud;

namespace UnitySensors.ROS.Publisher.LiDAR
{
    [RequireComponent(typeof(CameraSensor))]
    public class CameraImageMsgPublisher : ImageMsgPublisher<CameraSensor>
    {
    }
}

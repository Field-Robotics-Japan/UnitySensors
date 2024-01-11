using UnityEngine;
using UnitySensors.Data.PointCloud;
using UnitySensors.Sensor.Camera;
using UnitySensors.ROS.Publisher.PointCloud;

namespace UnitySensors.ROS.Publisher.LiDAR
{
    [RequireComponent(typeof(DepthCameraSensor))]
    public class DepthCameraPointCloud2MsgPublisher : PointCloud2MsgPublisher<DepthCameraSensor, PointXYZ>
    {
    }
}

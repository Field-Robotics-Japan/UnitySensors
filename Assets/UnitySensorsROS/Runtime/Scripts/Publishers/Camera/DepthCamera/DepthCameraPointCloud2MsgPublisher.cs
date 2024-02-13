using UnityEngine;
using UnitySensors.Data.PointCloud;
using UnitySensors.Sensor.Camera;

namespace UnitySensors.ROS.Publisher.PointCloud
{
    [RequireComponent(typeof(DepthCameraSensor))]
    public class DepthCameraPointCloud2MsgPublisher : PointCloud2MsgPublisher<DepthCameraSensor, PointXYZ>
    {
    }
}

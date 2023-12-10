using UnityEngine;
using UnitySensors.Data.PointCloud;
using UnitySensors.Sensor.LiDAR;
using UnitySensors.ROS.Publisher.PointCloud;

namespace UnitySensors.ROS.Publisher.LiDAR
{
    [RequireComponent(typeof(RaycastLiDARSensor))]
    public class RaycastLiDARPointCloud2MsgPublisher : PointCloud2MsgPublisher<RaycastLiDARSensor, PointXYZI>
    {
    }
}

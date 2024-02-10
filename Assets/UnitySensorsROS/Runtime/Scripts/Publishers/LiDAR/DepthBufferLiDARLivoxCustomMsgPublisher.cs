using UnityEngine;
using UnitySensors.Data.PointCloud;
using UnitySensors.Sensor.LiDAR;
using UnitySensors.ROS.Publisher.PointCloud;

namespace UnitySensors.ROS.Publisher.LiDAR
{
    [RequireComponent(typeof(DepthBufferLiDARSensor))]
    public class DepthBufferLiDARLivoxCustomMsgPublisher : LivoxCustomMsgPublisher<DepthBufferLiDARSensor, PointXYZI>
    {
    }
}

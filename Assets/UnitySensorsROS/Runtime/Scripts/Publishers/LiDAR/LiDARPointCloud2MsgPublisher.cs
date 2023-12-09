using UnityEngine;
using UnitySensors.Sensor.LiDAR;
using UnitySensors.ROS.Publisher.PointCloud;

namespace UnitySensors.ROS.Publisher.LiDAR
{
    [RequireComponent(typeof(LiDARSensor))]
    public class LiDARPointCloud2MsgPublisher : PointCloud2MsgPublisher<LiDARSensor>
    {
    }
}

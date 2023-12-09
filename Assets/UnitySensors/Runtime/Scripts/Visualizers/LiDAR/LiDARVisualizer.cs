using UnityEngine;
using UnitySensors.Sensor.LiDAR;
using UnitySensors.Visualization.PointCloud;

using Unity.Mathematics;

namespace UnitySensors.Visualization.LiDAR
{
    [RequireComponent(typeof(LiDARSensor))]
    public class LiDARVisualizer : PointCloudVisualizer<LiDARSensor>
    {

    }
}

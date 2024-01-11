using UnityEngine;

using UnitySensors.Data.PointCloud;
using UnitySensors.Sensor.Camera;
using UnitySensors.Visualization.PointCloud;

namespace UnitySensors.Visualization.Camera
{
    [RequireComponent(typeof(DepthCameraSensor))]
    public class DepthCameraPointCloudVisualizer : PointCloudVisualizer<DepthCameraSensor, PointXYZ>
    {
    }
}

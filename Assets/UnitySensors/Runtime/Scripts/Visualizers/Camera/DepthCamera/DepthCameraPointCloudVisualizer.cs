using UnityEngine;
using UnitySensors.Sensor.Camera;
using UnitySensors.Visualization.PointCloud;

using Unity.Mathematics;

namespace UnitySensors.Visualization.Camera
{
    [RequireComponent(typeof(DepthCameraSensor))]
    public class DepthCameraPointCloudVisualizer : PointCloudVisualizer<DepthCameraSensor>
    {
    }
}

using UnityEngine;
using UnityEngine.UI;
using UnitySensors.Sensor.Camera;
using UnitySensors.Visualization.Texture;

namespace UnitySensors.Visualization.Camera
{
    [RequireComponent(typeof(CameraSensor))]
    public class CameraTextureVisualizer : TextureVisualizer<CameraSensor>
    {
    }
}

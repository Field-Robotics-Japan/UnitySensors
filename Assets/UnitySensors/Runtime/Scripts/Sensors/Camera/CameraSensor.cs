using UnityEngine;
using UnityEngine.Rendering;

using UnitySensors.Interface.Sensor;

namespace UnitySensors.Sensor.Camera
{
    [RequireComponent(typeof(UnityEngine.Camera))]
    public abstract class CameraSensor : UnitySensor, ICameraInterface
    {
        [SerializeField]
        protected Vector2Int _resolution = new Vector2Int(640, 480);
        [SerializeField]
        protected float _fov = 30.0f;

        public abstract UnityEngine.Camera m_camera { get; }
    }
}

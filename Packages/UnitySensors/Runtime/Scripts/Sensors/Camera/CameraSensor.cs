using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnitySensors.Interface.Sensor;

namespace UnitySensors.Sensor.Camera
{
    [RequireComponent(typeof(UnityEngine.Camera))]
    public abstract class CameraSensor : UnitySensor, ICameraInterface, ITextureInterface
    {
        [SerializeField]
        protected internal Vector2Int _resolution = new Vector2Int(640, 480);
        [SerializeField]
        protected internal float _fov = 30.0f;

        protected RenderTexture _rt = null;
        protected UnityEngine.Camera _camera;
        protected Texture2D _texture;

        public UnityEngine.Camera m_camera { get => _camera; }

        public virtual Texture2D texture0 { get => _texture; }

        public virtual Texture2D texture1 { get => _texture; }

        public float texture0FarClipPlane { get => _camera.farClipPlane; }

        protected override void Init()
        {
            _camera = GetComponent<UnityEngine.Camera>();
            _camera.fieldOfView = _fov;
            _camera.enabled = false;
        }
    }
}

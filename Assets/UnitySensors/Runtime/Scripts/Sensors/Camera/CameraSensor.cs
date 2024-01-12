using UnityEngine;
using UnityEngine.Rendering;

using UnitySensors.Data.Texture;

namespace UnitySensors.Sensor.Camera
{
    [RequireComponent(typeof(UnityEngine.Camera))]
    public abstract class CameraSensor : UnitySensor, ITextureInterface
    {
        [SerializeField]
        private Vector2Int _resolution = new Vector2Int(640, 480);
        [SerializeField]
        private float _fov = 30.0f;
        [SerializeField]
        private float _minRange = 0.05f;
        [SerializeField]
        private float _maxRange = 100.0f;

        private UnityEngine.Camera _m_camera;
        private RenderTexture _rt = null;
        private Texture2D _texture;

        public Vector2Int resolution { get => _resolution; }
        protected float maxRange { get => _maxRange; }
        public UnityEngine.Camera m_camera { get => _m_camera; }
        public Texture2D texture { get => _texture; }

        protected override void Init()
        {
            _m_camera = GetComponent<UnityEngine.Camera>();
            _m_camera.fieldOfView = _fov;
            _m_camera.nearClipPlane = _minRange;
            _m_camera.farClipPlane = _maxRange;

            _rt = new RenderTexture(_resolution.x, _resolution.y, 32, RenderTextureFormat.ARGBFloat);
            _m_camera.targetTexture = _rt;

            _texture = new Texture2D(_resolution.x, _resolution.y, TextureFormat.RGBAFloat, false);
        }

        protected override void UpdateSensor()
        {
            if (!LoadTexture()) return;
            
            if (onSensorUpdated != null)
                onSensorUpdated.Invoke();
        }

        protected bool LoadTexture()
        {
            bool result = false;
            AsyncGPUReadback.Request(_rt, 0, request => {
                if (request.hasError)
                {
                }
                else
                {
                    var data = request.GetData<Color>();
                    _texture.LoadRawTextureData(data);
                    _texture.Apply();
                    result = true;
                }
            });
            AsyncGPUReadback.WaitAllRequests();
            return result;
        }

        protected override void OnSensorDestroy()
        {
            _rt.Release();
        }
    }
}

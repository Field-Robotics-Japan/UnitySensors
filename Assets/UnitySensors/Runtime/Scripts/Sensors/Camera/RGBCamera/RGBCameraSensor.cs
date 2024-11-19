using UnityEngine;
using UnityEngine.Rendering;

using UnitySensors.Interface.Sensor;

namespace UnitySensors.Sensor.Camera
{
    public class RGBCameraSensor : CameraSensor, ITextureInterface
    {
        private UnityEngine.Camera _camera;
        private RenderTexture _rt = null;
        private Texture2D _texture;

        public override UnityEngine.Camera m_camera { get => _camera; }
        public Texture2D texture0 { get => _texture; }
        public Texture2D texture1 { get => _texture; }

        public float texture0FarClipPlane { get => _camera.farClipPlane; }

        protected override void Init()
        {
            _camera = GetComponent<UnityEngine.Camera>();
            _camera.fieldOfView = _fov;

            _rt = new RenderTexture(_resolution.x, _resolution.y, 32, RenderTextureFormat.ARGB32);
            _camera.targetTexture = _rt;

            _texture = new Texture2D(_resolution.x, _resolution.y, TextureFormat.RGBA32, false);
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
            AsyncGPUReadback.Request(_rt, 0, request =>
            {
                if (request.hasError)
                {
                    Debug.LogError("GPU readback error detected.");
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

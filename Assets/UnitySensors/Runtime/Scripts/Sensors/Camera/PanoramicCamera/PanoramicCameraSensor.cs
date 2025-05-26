using UnityEngine;
using UnityEngine.Rendering;
using UnitySensors.Interface.Sensor;

namespace UnitySensors.Sensor.Camera
{
    public class PanoramicCameraSensor : CameraSensor, ITextureInterface
    {
        [SerializeField]
        private Material _panoramicMat;
        private UnityEngine.Camera _camera;
        private RenderTexture _cubemap;
        private RenderTexture _rt;
        private Texture2D _texture;

        public override UnityEngine.Camera m_camera { get => _camera; }

        public Texture2D texture0 { get => _texture; }

        public Texture2D texture1 { get => _texture; }

        public float texture0FarClipPlane { get => m_camera.farClipPlane; }

        protected override void Init()
        {
            _camera = GetComponent<UnityEngine.Camera>();
            _cubemap = new RenderTexture(1024, 1024, 16, RenderTextureFormat.ARGB32)
            {
                dimension = TextureDimension.Cube
            };
            _rt = new RenderTexture(_resolution.x, _resolution.y, 16, RenderTextureFormat.ARGB32);
            _texture = new Texture2D(_resolution.x, _resolution.y, TextureFormat.RGBA32, false);
        }

        protected override void UpdateSensor()
        {
            _panoramicMat.SetVector("_Rotation", transform.rotation.eulerAngles);
            m_camera.RenderToCubemap(_cubemap);
            Graphics.Blit(_cubemap, _rt, _panoramicMat);

            if (!LoadTexture()) return;
            onSensorUpdated?.Invoke();
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
            _cubemap.Release();
            _rt.Release();
        }
    }
}
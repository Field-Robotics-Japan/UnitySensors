using UnityEngine;
using UnityEngine.Rendering;

namespace UnitySensors.Sensor.Camera
{
    public class PanoramicCameraSensor : CameraSensor
    {
        [SerializeField]
        private Material _panoramicMat;
        [SerializeField]
        protected Vector2Int _cubemapResolution = new Vector2Int(1024, 1024);
        private RenderTexture _cubemap;
        protected override void Init()
        {
            base.Init();
            _cubemap = new RenderTexture(_cubemapResolution.x, _cubemapResolution.y, 0, RenderTextureFormat.ARGB32)
            {
                dimension = TextureDimension.Cube
            };
            _rt = new RenderTexture(_resolution.x, _resolution.y, 0, RenderTextureFormat.ARGB32);
            _texture = new Texture2D(_resolution.x, _resolution.y, TextureFormat.RGBA32, false);
        }

        protected override void UpdateSensor()
        {
            _panoramicMat.SetVector("_Rotation", transform.rotation.eulerAngles);
            m_camera.RenderToCubemap(_cubemap);
            Graphics.Blit(_cubemap, _rt, _panoramicMat);

            if (!LoadTexture(_rt, ref _texture)) return;
        }
        protected override void OnSensorDestroy()
        {
            _cubemap.Release();
            _rt.Release();
        }
    }
}
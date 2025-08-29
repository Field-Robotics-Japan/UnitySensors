using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnitySensors.Utils.Texture;

namespace UnitySensors.Sensor.Camera
{
    public class PanoramicCameraSensor : CameraSensor
    {
        [SerializeField]
        private Material _panoramicMat;
        [SerializeField]
        protected Vector2Int _cubemapResolution = new Vector2Int(1024, 1024);
        private RenderTexture _cubemap;
        private TextureLoader _textureLoader;
        protected override void Init()
        {
            base.Init();
            _cubemap = new RenderTexture(_cubemapResolution.x, _cubemapResolution.y, 0, RenderTextureFormat.ARGB32)
            {
                dimension = TextureDimension.Cube
            };
            _rt = new RenderTexture(_resolution.x, _resolution.y, 0, RenderTextureFormat.ARGB32);
            _texture = new Texture2D(_resolution.x, _resolution.y, TextureFormat.RGBA32, false);
            _textureLoader = new TextureLoader
            {
                source = _rt,
                destination = _texture
            };
        }

        protected override IEnumerator UpdateSensor()
        {
            _panoramicMat.SetVector("_Rotation", transform.rotation.eulerAngles);
            m_camera.RenderToCubemap(_cubemap);
            Graphics.Blit(_cubemap, _rt, _panoramicMat);

            yield return _textureLoader.LoadTextureAsync();
        }
        protected override void OnSensorDestroy()
        {
            _cubemap.Release();
            _rt.Release();
        }
    }
}
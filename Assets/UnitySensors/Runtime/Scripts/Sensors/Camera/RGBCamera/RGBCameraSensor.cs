using UnityEngine;

namespace UnitySensors.Sensor.Camera
{
    public class RGBCameraSensor : CameraSensor
    {
        protected override void Init()
        {
            base.Init();
            _rt = new RenderTexture(_resolution.x, _resolution.y, 16, RenderTextureFormat.ARGB32);
            _camera.targetTexture = _rt;

            _texture = new Texture2D(_resolution.x, _resolution.y, TextureFormat.RGBA32, false);
        }

        protected override void UpdateSensor()
        {
            if (!LoadTexture(_rt, ref _texture)) return;

            if (onSensorUpdated != null)
                onSensorUpdated.Invoke();
        }

        protected override void OnSensorDestroy()
        {
            _rt.Release();
        }
    }
}

using UnityEngine;

namespace UnitySensors.Sensor.Camera
{
    public class PanoramicCameraSensor : CameraSensor
    {
        private Material      _mat;
        private RenderTexture _cubemap;

        protected override void Init()
        {
            _cubemap = new RenderTexture(1024, 1024, 16, RenderTextureFormat.ARGB32)
            {
                dimension = UnityEngine.Rendering.TextureDimension.Cube
            };
            _rt  = new RenderTexture(width, height, 16, RenderTextureFormat.ARGB32);
            _mat = new Material(Shader.Find("UnitySensors/Panoramic"));
            base.Init();
        }

        protected override void UpdateSensor()
        {
            _mat.SetVector("_Rotation", transform.eulerAngles);
            sensorCamera.RenderToCubemap(_cubemap);
            Graphics.Blit(_cubemap, _rt, _mat);

            if (!LoadTexture()) return;
            onSensorUpdated?.Invoke();
        }
    }
}
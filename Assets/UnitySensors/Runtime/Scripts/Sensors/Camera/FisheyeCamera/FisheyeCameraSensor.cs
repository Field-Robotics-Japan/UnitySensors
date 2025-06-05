using UnityEngine;
using UnityEngine.Rendering;

namespace UnitySensors.Sensor.Camera
{
    public class FisheyeCameraSensor : CameraSensor
    {
        //TODO: Change to MEI camera model
        [SerializeField]
        private Material _fisheyeMat;
        [SerializeField]
        private Vector2Int _cubemapResolution = new Vector2Int(1024, 1024);
        [Range(120, 360)]
        public float viewAngle = 210;
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
            m_camera.RenderToCubemap(_cubemap);

            _fisheyeMat.SetTexture("_Cubemap", _cubemap);
            _fisheyeMat.SetFloat("_Angle", viewAngle);
            var eulerAngles = transform.rotation.eulerAngles;
            var rot = Quaternion.Euler(eulerAngles.x + 90, eulerAngles.y, eulerAngles.z);
            var mat = Matrix4x4.TRS(Vector3.zero, rot, Vector3.one);
            _fisheyeMat.SetMatrix("_CubeToDome_WorldTransform", mat);
            Graphics.Blit(null, _rt, _fisheyeMat);

            if (!LoadTexture(_rt, ref _texture)) return;
            onSensorUpdated?.Invoke();
        }
        protected override void OnSensorDestroy()
        {
            _cubemap.Release();
            _rt.Release();
        }
    }
}
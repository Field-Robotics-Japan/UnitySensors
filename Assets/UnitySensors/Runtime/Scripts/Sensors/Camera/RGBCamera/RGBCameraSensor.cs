using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace UnitySensors
{
    [RequireComponent(typeof(Camera))]
    public class RGBCameraSensor : Sensor
    {
        [SerializeField]
        private Vector2Int _resolution = new Vector2Int(640, 480);

        [SerializeField, Range(0, 100)]
        private int _quality = 100;

        private Camera _cam;

        private RenderTexture _rt = null;
        private Texture2D _texture;

        public Texture2D texture { get => _texture; }
        public int quality { get => _quality; }

        protected override void Init()
        {
            _cam = GetComponent<Camera>();
            _rt = new RenderTexture(_resolution.x, _resolution.y, 0, RenderTextureFormat.ARGB32);
            _texture = new Texture2D(_resolution.x, _resolution.y, TextureFormat.RGBA32, false);
            _cam.targetTexture = _rt;
        }

        protected override void UpdateSensor()
        {
            AsyncGPUReadback.Request(_rt, 0, request => {
                if (request.hasError)
                {
                }
                else
                {
                    if (!Application.isPlaying) return;
                    var data = request.GetData<Color32>();
                    _texture.LoadRawTextureData(data);
                    _texture.Apply();
                }
            });
        }

        private void OnDestroy()
        {
            _cam.targetTexture = null;
            _rt.Release();
        }

        private void OnApplicationQuit()
        {
            _cam.targetTexture = null;
            _rt.Release();
        }
    }
}

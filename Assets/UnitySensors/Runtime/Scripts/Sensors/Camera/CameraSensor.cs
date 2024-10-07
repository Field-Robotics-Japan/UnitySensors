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
        private byte[] _data;

        public Vector2Int resolution { get => _resolution; }
        protected float maxRange { get => _maxRange; }
        public UnityEngine.Camera m_camera { get => _m_camera; }
        public Texture2D texture { get => _texture; }
        public byte[] data { get => _data; }
        public int width { get => _resolution.x; }
        public int height { get => _resolution.y; }
        public string encoding { get => "rgb8"; }


        protected override void Init()
        {
            _m_camera = GetComponent<UnityEngine.Camera>();
            _m_camera.fieldOfView = _fov;
            _m_camera.nearClipPlane = _minRange;
            _m_camera.farClipPlane = _maxRange;

            _rt = new RenderTexture(_resolution.x, _resolution.y, 16, RenderTextureFormat.ARGB32);
            _m_camera.targetTexture = _rt;

            _texture = new Texture2D(width, height, TextureFormat.RGB24, false);

            _data = new byte[width * height * 3];
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

            // Blit to a temporary texture and request readback on it.
            AsyncGPUReadback.Request(_rt, 0, TextureFormat.ARGB32, request => {
                if (request.hasError) {
                    Debug.LogWarning("GPU readback error was detected.");
                } else {
                    var dataBuffer = request.GetData<byte>();

                    // Flip image
                    int i=0;
                    int j=width*(height-1)*4;
                    for(int y=0; y<height; y++){
                        for(int x=0; x<width; x++) {
                            _data[i+0] = dataBuffer[j+1];
                            _data[i+1] = dataBuffer[j+2];
                            _data[i+2] = dataBuffer[j+3];
                            i+=3;
                            j+=4;
                        }
                        j -= width << 3; // width * 2 * 3;
                    }

                    _texture.LoadRawTextureData(_data);
                    _texture.Apply();

                    result = true;
                }
            });
            AsyncGPUReadback.WaitAllRequests();
            return result;
        }

        protected override void OnSensorDestroy()
        {
            if (_rt != null)
            {
                // Dispose the frame texture.
                m_camera.targetTexture = null;
                Destroy(_rt);
                _rt = null;
            }
        }
    }
}

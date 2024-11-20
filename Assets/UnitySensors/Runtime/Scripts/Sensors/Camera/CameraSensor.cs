using UnityEngine;
using UnityEngine.Rendering;

using UnitySensors.Data.Texture;

namespace UnitySensors.Sensor.Camera
{
    [RequireComponent(typeof(UnityEngine.Camera))]
    public abstract class CameraSensor : UnitySensor, ITextureInterface
    {
        [field: SerializeField] public Vector2Int resolution { get; protected set; } = new(640, 480);
        [field: SerializeField] public float      fov        { get; protected set; } = 30.0f;
        [field: SerializeField] public float      minRange   { get; protected set; } = 0.05f;
        [field: SerializeField] public float      maxRange   { get; protected set; } = 100.0f;

        public UnityEngine.Camera sensorCamera { get; protected set; }
        public byte[]             data         { get; protected set; }
        public string             encoding     { get; protected set; }
        public int                width        { get => resolution.x; }
        public int                height       { get => resolution.y; }
        public Texture2D          texture      { get; protected set; }

        protected RenderTexture _rt;

        protected override void Init()
        {
            sensorCamera = GetComponent<UnityEngine.Camera>();
            sensorCamera.fieldOfView   = fov;
            sensorCamera.nearClipPlane = minRange;
            sensorCamera.farClipPlane  = maxRange;

            if( _rt == null )
            {
                _rt = new RenderTexture(width, height, 16, RenderTextureFormat.ARGB32);
                sensorCamera.targetTexture = _rt;
            }

            texture  = new Texture2D(width, height, TextureFormat.RGB24, false);
            data     = new byte[width * height * 3];
            encoding = "rgb8";
        }

        protected override void UpdateSensor()
        {
            if (!LoadTexture()) return;

            onSensorUpdated?.Invoke();
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
                            data[i+0] = dataBuffer[j+1];
                            data[i+1] = dataBuffer[j+2];
                            data[i+2] = dataBuffer[j+3];
                            i+=3;
                            j+=4;
                        }
                        j -= width << 3; // width * 2 * 3;
                    }

                    texture.LoadRawTextureData(data);
                    texture.Apply();

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
                sensorCamera.targetTexture = null;
                Destroy(_rt);
                _rt = null;
            }
        }
    }
}

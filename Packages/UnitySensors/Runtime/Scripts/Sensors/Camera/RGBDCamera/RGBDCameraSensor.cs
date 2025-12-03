using System;
using UnityEngine;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

using UnitySensors.DataType.Sensor;
using UnitySensors.DataType.Sensor.PointCloud;
using UnitySensors.Interface.Sensor;
using UnitySensors.Utils.Noise;
using UnitySensors.Utils.Texture;
using Random = Unity.Mathematics.Random;
using System.Collections;

#if UNITY_6000_0_OR_NEWER
using UnityEngine.Rendering;
#endif

namespace UnitySensors.Sensor.Camera
{
    [RequireComponent(typeof(UnityEngine.Camera))]
    public class RGBDCameraSensor : CameraSensor, IPointCloudInterface<PointXYZRGB>
    {
        [SerializeField]
        protected float _minRange = 0.05f;
        [SerializeField]
        protected float _maxRange = 100.0f;
        [SerializeField]
        private float _gaussianNoiseSigma = 0.0f;
        [SerializeField]
        private Material _depthCameraMat;
        [SerializeField]
        private bool _convertToPointCloud = false;
        
        [Header("Performance Settings")]
        [SerializeField, Range(0.1f, 1.0f)]
        private float _raycastResolutionScale = 0.5f; // Reduce raycast resolution for better performance
        [SerializeField]
        private bool _useAdaptiveQuality = true; // Enable adaptive quality based on frame rate

        private RenderTexture _depthRt = null;
        private Texture2D _depthTexture;
        private Texture2D _raycastDepthTexture; // Reuse texture for raycast to avoid allocations
        private int _lastRaycastWidth, _lastRaycastHeight;
        private float _lastFrameTime;

        private UnityEngine.Camera _colorCamera;
        private RenderTexture _colorRt = null;
        private Texture2D _colorTexture;

        private JobHandle _jobHandle;

        private IUpdateGaussianNoisesJob _updateGaussianNoisesJob;
        private ITextureToColorPointsJob _textureToPointsJob;

        private NativeArray<float> _noises;
        private NativeArray<float3> _directions;

        private PointCloud<PointXYZRGB> _pointCloud;
        private int _pointsNum;

        //TODO: Maybe can output color in RGB channel and depth in alpha channel of the same texture.
        protected UnityEngine.Camera _depthCamera { get => _camera; }
        public override Texture2D texture0 { get => _depthTexture; }
        public override Texture2D texture1 { get => _colorTexture; }
        public PointCloud<PointXYZRGB> pointCloud { get => _pointCloud; }
        public int pointsNum { get => _pointsNum; }
        public bool convertToPointCloud { get => _convertToPointCloud; set => _convertToPointCloud = value; }

        private TextureLoader _depthTextureLoader, _colorTextureLoader;

        protected override void Init()
        {
            base.Init();
#if UNITY_6000_0_OR_NEWER
            // Unity 6000+ requires depth buffer for render textures used with cameras
            _depthRt = new RenderTexture(_resolution.x, _resolution.y, 24, RenderTextureFormat.ARGBFloat);
#else
            _depthRt = new RenderTexture(_resolution.x, _resolution.y, 0, RenderTextureFormat.ARGBFloat);
#endif
            _depthCamera.targetTexture = _depthRt;

            GameObject colorCameraObject = new GameObject();
            colorCameraObject.name = "ColorCamera";
            Transform colorCameraTransform = colorCameraObject.transform;
            colorCameraTransform.parent = transform;
            colorCameraTransform.localPosition = Vector3.zero;
            colorCameraTransform.localRotation = Quaternion.identity;

            _colorCamera = colorCameraObject.AddComponent<UnityEngine.Camera>();
#if UNITY_6000_0_OR_NEWER
            // Unity 6000+ requires depth buffer for render textures used with cameras
            _colorRt = new RenderTexture(_resolution.x, _resolution.y, 24, RenderTextureFormat.ARGB32);
#else
            _colorRt = new RenderTexture(_resolution.x, _resolution.y, 0, RenderTextureFormat.ARGB32);
#endif
            _colorCamera.targetTexture = _colorRt;

            _depthCamera.fieldOfView = _colorCamera.fieldOfView = _fov;
            _depthCamera.nearClipPlane = _minRange;
            _depthCamera.farClipPlane = _maxRange;

            _depthTexture = new Texture2D(_resolution.x, _resolution.y, TextureFormat.RGBAFloat, false);
            _colorTexture = new Texture2D(_resolution.x, _resolution.y, TextureFormat.RGBA32, false);

            _depthCamera.enabled = false;
            _colorCamera.enabled = false;

            _depthTextureLoader = new TextureLoader
            {
                source = _depthRt,
                destination = _depthTexture
            };
            _colorTextureLoader = new TextureLoader
            {
                source = _colorRt,
                destination = _colorTexture
            };

            if (_convertToPointCloud)
            {
                SetupDirections();
                SetupJob();
            }
        }

        private void SetupDirections()
        {
            _pointsNum = _resolution.x * _resolution.y;

            _directions = new NativeArray<float3>(_pointsNum, Allocator.Persistent);

            float z = _resolution.y * 0.5f / Mathf.Tan(m_camera.fieldOfView * 0.5f * Mathf.Deg2Rad);
            for (int y = 0; y < _resolution.y; y++)
            {
                for (int x = 0; x < _resolution.x; x++)
                {
                    Vector3 vec = new Vector3(-_resolution.x / 2 + x, -_resolution.y / 2 + y, z);
                    _directions[y * _resolution.x + x] = vec.normalized;
                }
            }
        }

        private void SetupJob()
        {
            _pointCloud = new PointCloud<PointXYZRGB>()
            {
                points = new NativeArray<PointXYZRGB>(_pointsNum, Allocator.Persistent)
            };

            _noises = new NativeArray<float>(pointsNum, Allocator.Persistent);

            _updateGaussianNoisesJob = new IUpdateGaussianNoisesJob()
            {
                sigma = _gaussianNoiseSigma,
                random = new Random((uint)Environment.TickCount),
                noises = _noises
            };

            _textureToPointsJob = new ITextureToColorPointsJob()
            {
                near = m_camera.nearClipPlane,
                far = m_camera.farClipPlane,
                directions = _directions,
                depthPixels = _depthTexture.GetPixelData<Color>(0),
                colorPixels = _colorTexture.GetPixelData<Color32>(0),
                noises = _noises,
                points = _pointCloud.points
            };
        }

        protected override IEnumerator UpdateSensor()
        {
#if UNITY_6000_0_OR_NEWER
            bool isURP = GraphicsSettings.currentRenderPipeline != null;

            if (isURP)
            {
                // For Unity 6000+ URP, use raycast for depth but normal rendering for color
                GenerateDepthImageUsingRaycast();
                _colorCamera.Render();
            }
            else
            {
                _depthCamera.Render();
                _colorCamera.Render();
            }
#else
            _depthCamera.Render();
            _colorCamera.Render();
#endif

            var depthLoad = _depthTextureLoader.LoadTextureAsync();
            var colorLoad = _colorTextureLoader.LoadTextureAsync();
            yield return depthLoad;
            yield return colorLoad;

            if (_depthTextureLoader.success && _convertToPointCloud)
            {
                JobHandle updateGaussianNoisesJobHandle = _updateGaussianNoisesJob.Schedule(_pointsNum, 1024);
                _jobHandle = _textureToPointsJob.Schedule(_pointsNum, 1024, updateGaussianNoisesJobHandle);
                // yield return new WaitUntil(() => _jobHandle.IsCompleted);
                _jobHandle.Complete();
            }
        }

        private void GenerateDepthImageUsingRaycast()
        {
            // Adaptive quality adjustment based on frame rate
            if (_useAdaptiveQuality)
            {
                float currentFrameTime = Time.unscaledDeltaTime;
                if (_lastFrameTime > 0)
                {
                    float currentFPS = 1.0f / currentFrameTime;
                    float targetFPS = frequency; // Use sensor frequency as target
                    if (currentFPS < targetFPS * 0.8f) // If FPS drops below 80% of target
                    {
                        _raycastResolutionScale = Mathf.Max(0.1f, _raycastResolutionScale - 0.05f);
                    }
                    else if (currentFPS > targetFPS * 1.1f) // If FPS is above 110% of target
                    {
                        _raycastResolutionScale = Mathf.Min(1.0f, _raycastResolutionScale + 0.02f);
                    }
                }
                _lastFrameTime = currentFrameTime;
            }

            // Calculate actual raycast resolution
            int raycastWidth = Mathf.Max(1, Mathf.RoundToInt(_depthRt.width * _raycastResolutionScale));
            int raycastHeight = Mathf.Max(1, Mathf.RoundToInt(_depthRt.height * _raycastResolutionScale));

            // Reuse texture if possible to avoid allocations
            if (_raycastDepthTexture == null || _lastRaycastWidth != raycastWidth || _lastRaycastHeight != raycastHeight)
            {
                if (_raycastDepthTexture != null)
                    DestroyImmediate(_raycastDepthTexture);
                    
                _raycastDepthTexture = new Texture2D(raycastWidth, raycastHeight, TextureFormat.RGBAFloat, false);
                _lastRaycastWidth = raycastWidth;
                _lastRaycastHeight = raycastHeight;
            }

            RenderTexture.active = _depthRt;
            GL.Clear(true, true, Color.white);
            RenderTexture.active = null;

            // Pre-calculate camera parameters
            float fovRad = _depthCamera.fieldOfView * Mathf.Deg2Rad;
            float aspect = (float)_depthRt.width / _depthRt.height;
            float tanHalfFov = Mathf.Tan(fovRad * 0.5f);

            Vector3 cameraPos = _depthCamera.transform.position;
            Vector3 forward = _depthCamera.transform.forward;
            Vector3 right = _depthCamera.transform.right;
            Vector3 up = _depthCamera.transform.up;

            // Use Color32 array for better performance
            Color32[] pixels = new Color32[raycastWidth * raycastHeight];
            
            // Batch raycast operations
            for (int y = 0; y < raycastHeight; y++)
            {
                for (int x = 0; x < raycastWidth; x++)
                {
                    // Map raycast coordinates to full resolution
                    float normalizedX = (float)x / (raycastWidth - 1);
                    float normalizedY = (float)y / (raycastHeight - 1);
                    
                    float ndcX = (2.0f * normalizedX) - 1.0f;
                    float ndcY = (2.0f * normalizedY) - 1.0f;

                    float viewX = ndcX * tanHalfFov * aspect;
                    float viewY = ndcY * tanHalfFov;

                    Vector3 rayDirection = (forward + right * viewX + up * viewY).normalized;
                    Ray ray = new Ray(cameraPos, rayDirection);

                    float depth = 1.0f;

                    if (Physics.Raycast(ray, out RaycastHit hit, _depthCamera.farClipPlane))
                    {
                        float distance = hit.distance;
                        depth = Mathf.Clamp01(distance / _depthCamera.farClipPlane);
                    }

                    byte depthByte = (byte)(depth * 255);
                    pixels[y * raycastWidth + x] = new Color32(depthByte, depthByte, depthByte, 255);
                }
            }

            // Apply pixels and scale to target resolution
            _raycastDepthTexture.SetPixels32(pixels);
            _raycastDepthTexture.Apply();

            // Scale to target resolution if needed
            if (raycastWidth != _depthRt.width || raycastHeight != _depthRt.height)
            {
                RenderTexture tempRT = RenderTexture.GetTemporary(_depthRt.width, _depthRt.height, 0, RenderTextureFormat.ARGBFloat);
                Graphics.Blit(_raycastDepthTexture, tempRT);
                Graphics.CopyTexture(tempRT, _depthRt);
                RenderTexture.ReleaseTemporary(tempRT);
            }
            else
            {
                RenderTexture.active = _depthRt;
                Graphics.CopyTexture(_raycastDepthTexture, _depthRt);
                RenderTexture.active = null;
            }
        }

        protected override void OnSensorDestroy()
        {
            if (_convertToPointCloud)
            {
                _jobHandle.Complete();
                _pointCloud.Dispose();
                _noises.Dispose();
                _directions.Dispose();
            }
            
            // Clean up raycast depth texture
            if (_raycastDepthTexture != null)
            {
                DestroyImmediate(_raycastDepthTexture);
                _raycastDepthTexture = null;
            }
            
            _depthRt.Release();
            _colorRt.Release();
        }

#if !UNITY_6000_0_OR_NEWER
        private void OnRenderImage(RenderTexture source, RenderTexture dest)
        {
            Graphics.Blit(null, dest, _depthCameraMat);
        }
#endif
    }
}

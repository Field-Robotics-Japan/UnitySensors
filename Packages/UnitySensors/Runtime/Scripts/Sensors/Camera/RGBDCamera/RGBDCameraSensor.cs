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

        private RenderTexture _depthRt = null;
        private Texture2D _depthTexture;

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
            RenderTexture.active = _depthRt;
            GL.Clear(true, true, Color.white);
            RenderTexture.active = null;

            Texture2D depthTexture = new Texture2D(_depthRt.width, _depthRt.height, TextureFormat.RGBAFloat, false);

            float fovRad = _depthCamera.fieldOfView * Mathf.Deg2Rad;
            float aspect = (float)_depthRt.width / _depthRt.height;
            float tanHalfFov = Mathf.Tan(fovRad * 0.5f);

            Vector3 cameraPos = _depthCamera.transform.position;
            Vector3 forward = _depthCamera.transform.forward;
            Vector3 right = _depthCamera.transform.right;
            Vector3 up = _depthCamera.transform.up;

            for (int y = 0; y < _depthRt.height; y++)
            {
                for (int x = 0; x < _depthRt.width; x++)
                {
                    float ndcX = (2.0f * x / (_depthRt.width - 1)) - 1.0f;
                    float ndcY = (2.0f * y / (_depthRt.height - 1)) - 1.0f;

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

                    Color depthColor = new Color(depth, depth, depth, 1.0f);
                    depthTexture.SetPixel(x, y, depthColor);
                }
            }

            depthTexture.Apply();

            RenderTexture.active = _depthRt;
            Graphics.CopyTexture(depthTexture, _depthRt);
            RenderTexture.active = null;

            DestroyImmediate(depthTexture);
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

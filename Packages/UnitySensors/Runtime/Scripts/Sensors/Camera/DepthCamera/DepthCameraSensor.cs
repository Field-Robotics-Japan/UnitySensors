using System;
using UnityEngine;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

using UnitySensors.DataType.Sensor;
using UnitySensors.DataType.Sensor.PointCloud;
using UnitySensors.Interface.Sensor;
using UnitySensors.Utils.Noise;

using Random = Unity.Mathematics.Random;
using System.Collections;
using UnitySensors.Utils.Texture;

#if UNITY_6000_0_OR_NEWER
using UnityEngine.Rendering;
#endif

namespace UnitySensors.Sensor.Camera
{
    [RequireComponent(typeof(UnityEngine.Camera))]
    public class DepthCameraSensor : CameraSensor, IPointCloudInterface<PointXYZ>
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
        private TextureLoader _textureLoader;

        private JobHandle _jobHandle;

        private IUpdateGaussianNoisesJob _updateGaussianNoisesJob;
        private ITextureToPointsJob _textureToPointsJob;

        private NativeArray<float> _noises;
        private NativeArray<float3> _directions;

        private PointCloud<PointXYZ> _pointCloud;
        private int _pointsNum;

        public PointCloud<PointXYZ> pointCloud { get => _pointCloud; }
        public int pointsNum { get => _pointsNum; }

        public bool convertToPointCloud { get => _convertToPointCloud; set => _convertToPointCloud = value; }

        protected override void Init()
        {
            base.Init();
            _camera.nearClipPlane = _minRange;
            _camera.farClipPlane = _maxRange;

#if UNITY_6000_0_OR_NEWER
            _rt = new RenderTexture(_resolution.x, _resolution.y, 24, RenderTextureFormat.ARGBFloat);
            _rt.Create();

            bool isURP = GraphicsSettings.currentRenderPipeline != null;
            if (isURP)
            {
                Debug.Log("DepthCameraSensor: Unity 6000+ URP mode initialized");
            }
#else
            _rt = new RenderTexture(_resolution.x, _resolution.y, 0, RenderTextureFormat.ARGBFloat);
#endif
            _camera.targetTexture = _rt;

            _texture = new Texture2D(_resolution.x, _resolution.y, TextureFormat.RGBAFloat, false);

            _textureLoader = new TextureLoader
            {
                source = _rt,
                destination = _texture
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
            _pointCloud = new PointCloud<PointXYZ>()
            {
                points = new NativeArray<PointXYZ>(_pointsNum, Allocator.Persistent)
            };

            _noises = new NativeArray<float>(pointsNum, Allocator.Persistent);

            _updateGaussianNoisesJob = new IUpdateGaussianNoisesJob()
            {
                sigma = _gaussianNoiseSigma,
                random = new Random((uint)Environment.TickCount),
                noises = _noises
            };

            _textureToPointsJob = new ITextureToPointsJob()
            {
                near = m_camera.nearClipPlane,
                far = m_camera.farClipPlane,
                directions = _directions,
                depthPixels = _texture.GetPixelData<Color>(0),
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
                GenerateDepthImageUsingRaycast();
            }
            else
            {
                _camera.Render();
            }
#else
            _camera.Render();
#endif

            yield return _textureLoader.LoadTextureAsync();

            if (_textureLoader.success && _convertToPointCloud)
            {
                JobHandle updateGaussianNoisesJobHandle = _updateGaussianNoisesJob.Schedule(_pointsNum, 1024);
                _jobHandle = _textureToPointsJob.Schedule(_pointsNum, 1024, updateGaussianNoisesJobHandle);
                // yield return new WaitUntil(() => _jobHandle.IsCompleted);
                _jobHandle.Complete();
            }
        }

        private void GenerateDepthImageUsingRaycast()
        {
            RenderTexture.active = _rt;
            GL.Clear(true, true, Color.white);
            RenderTexture.active = null;

            Texture2D depthTexture = new Texture2D(_rt.width, _rt.height, TextureFormat.RGBAFloat, false);

            float fovRad = _camera.fieldOfView * Mathf.Deg2Rad;
            float aspect = (float)_rt.width / _rt.height;
            float tanHalfFov = Mathf.Tan(fovRad * 0.5f);

            Vector3 cameraPos = _camera.transform.position;
            Vector3 forward = _camera.transform.forward;
            Vector3 right = _camera.transform.right;
            Vector3 up = _camera.transform.up;

            for (int y = 0; y < _rt.height; y++)
            {
                for (int x = 0; x < _rt.width; x++)
                {
                    float ndcX = (2.0f * x / (_rt.width - 1)) - 1.0f;
                    float ndcY = (2.0f * y / (_rt.height - 1)) - 1.0f;

                    float viewX = ndcX * tanHalfFov * aspect;
                    float viewY = ndcY * tanHalfFov;

                    Vector3 rayDirection = (forward + right * viewX + up * viewY).normalized;
                    Ray ray = new Ray(cameraPos, rayDirection);

                    float depth = 1.0f;

                    if (Physics.Raycast(ray, out RaycastHit hit, _camera.farClipPlane))
                    {
                        float distance = hit.distance;
                        depth = Mathf.Clamp01(distance / _camera.farClipPlane);
                    }

                    Color depthColor = new Color(depth, depth, depth, 1.0f);
                    depthTexture.SetPixel(x, y, depthColor);
                }
            }

            depthTexture.Apply();

            RenderTexture.active = _rt;
            Graphics.CopyTexture(depthTexture, _rt);
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
            _rt.Release();
        }

#if !UNITY_6000_0_OR_NEWER
        private void OnRenderImage(RenderTexture source, RenderTexture dest)
        {
            Graphics.Blit(null, dest, _depthCameraMat);
        }
#endif
    }
}

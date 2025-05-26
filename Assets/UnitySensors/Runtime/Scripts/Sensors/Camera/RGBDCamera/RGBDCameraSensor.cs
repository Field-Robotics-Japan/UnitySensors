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

        protected UnityEngine.Camera _depthCamera { get => _camera; }
        public override Texture2D texture0 { get => _depthTexture; }
        public override Texture2D texture1 { get => _colorTexture; }
        public PointCloud<PointXYZRGB> pointCloud { get => _pointCloud; }
        public int pointsNum { get => _pointsNum; }
        public bool convertToPointCloud { get => _convertToPointCloud; set => _convertToPointCloud = value; }

        protected override void Init()
        {
            base.Init();
            _depthRt = new RenderTexture(_resolution.x, _resolution.y, 32, RenderTextureFormat.ARGBFloat);
            _depthCamera.targetTexture = _depthRt;

            GameObject colorCameraObject = new GameObject();
            Transform colorCameraTransform = colorCameraObject.transform;
            colorCameraTransform.parent = transform;
            colorCameraTransform.localPosition = Vector3.zero;
            colorCameraTransform.localRotation = Quaternion.identity;

            _colorCamera = colorCameraObject.AddComponent<UnityEngine.Camera>();
            _colorRt = new RenderTexture(_resolution.x, _resolution.y, 16, RenderTextureFormat.ARGB32);
            _colorCamera.targetTexture = _colorRt;

            _depthCamera.fieldOfView = _colorCamera.fieldOfView = _fov;
            _depthCamera.nearClipPlane = _minRange;
            _depthCamera.farClipPlane = _maxRange;

            _depthTexture = new Texture2D(_resolution.x, _resolution.y, TextureFormat.RGBAFloat, false);
            _colorTexture = new Texture2D(_resolution.x, _resolution.y, TextureFormat.RGBA32, false);


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

        protected override void UpdateSensor()
        {
            if (!LoadTexture(_depthRt, ref _depthTexture) || !LoadTexture(_colorRt, ref _colorTexture)) return;

            if (_convertToPointCloud)
            {
                JobHandle updateGaussianNoisesJobHandle = _updateGaussianNoisesJob.Schedule(_pointsNum, 1024);
                _jobHandle = _textureToPointsJob.Schedule(_pointsNum, 1024, updateGaussianNoisesJobHandle);
                JobHandle.ScheduleBatchedJobs();
                _jobHandle.Complete();
            }

            if (onSensorUpdated != null)
                onSensorUpdated.Invoke();
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

        private void OnRenderImage(RenderTexture source, RenderTexture dest)
        {
            Graphics.Blit(null, dest, _depthCameraMat);
        }
    }
}

using System;
using UnityEngine;
using UnityEngine.Rendering;
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
    public class RGBDCameraSensor : CameraSensor, ITextureInterface, IPointCloudInterface<PointXYZRGB>
    {
        [SerializeField]
        protected float _minRange = 0.05f;
        [SerializeField]
        protected float _maxRange = 100.0f;
        [SerializeField]
        private float _gaussianNoiseSigma = 0.0f;
        [SerializeField]
        private Material _depthCameraMat;

        private UnityEngine.Camera _depthCamera;
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

        public override UnityEngine.Camera m_camera { get => _depthCamera; }
        public Texture2D texture0 { get => _depthTexture; }
        public Texture2D texture1 { get => _colorTexture; }
        public PointCloud<PointXYZRGB> pointCloud { get => _pointCloud; }
        public int pointsNum { get => _pointsNum; }
        public float texture0FarClipPlane { get => _depthCamera.farClipPlane; }

        protected override void Init()
        {
            _depthCamera = GetComponent<UnityEngine.Camera>();
            _depthRt = new RenderTexture(_resolution.x, _resolution.y, 32, RenderTextureFormat.ARGBFloat);
            _depthCamera.targetTexture = _depthRt;

            GameObject colorCameraObject = new GameObject();
            Transform colorCameraTransform = colorCameraObject.transform;
            colorCameraTransform.parent = this.transform;
            colorCameraTransform.localPosition = Vector3.zero;
            colorCameraTransform.localRotation = Quaternion.identity;

            _colorCamera = colorCameraObject.AddComponent<UnityEngine.Camera>();
            _colorRt = new RenderTexture(_resolution.x, _resolution.y, 32, RenderTextureFormat.BGRA32);
            _colorCamera.targetTexture = _colorRt;

            _depthCamera.fieldOfView = _colorCamera.fieldOfView = _fov;
            _depthCamera.nearClipPlane = _minRange;
            _depthCamera.farClipPlane = _maxRange;

            _depthTexture = new Texture2D(_resolution.x, _resolution.y, TextureFormat.RGBAFloat, false);
            _colorTexture = new Texture2D(_resolution.x, _resolution.y, TextureFormat.BGRA32, false);


            SetupDirections();
            SetupJob();
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
            if (!LoadDepthTexture() || !LoadColorTexture()) return;

            JobHandle updateGaussianNoisesJobHandle = _updateGaussianNoisesJob.Schedule(_pointsNum, 1024);
            _jobHandle = _textureToPointsJob.Schedule(_pointsNum, 1024, updateGaussianNoisesJobHandle);
            JobHandle.ScheduleBatchedJobs();
            _jobHandle.Complete();

            if (onSensorUpdated != null)
                onSensorUpdated.Invoke();
        }

        private bool LoadDepthTexture()
        {
            bool result = false;
            AsyncGPUReadback.Request(_depthRt, 0, request =>
            {
                if (request.hasError)
                {
                    Debug.LogError("GPU readback error detected.");
                }
                else
                {
                    var data = request.GetData<Color>();
                    _depthTexture.LoadRawTextureData(data);
                    _depthTexture.Apply();
                    result = true;
                }
            });
            AsyncGPUReadback.WaitAllRequests();
            return result;
        }

        private bool LoadColorTexture()
        {
            bool result = false;
            AsyncGPUReadback.Request(_colorRt, 0, request =>
            {
                if (request.hasError)
                {
                    Debug.LogError("GPU readback error detected.");
                }
                else
                {
                    var data = request.GetData<Color32>();
                    _colorTexture.LoadRawTextureData(data);
                    _colorTexture.Apply();
                    result = true;
                }
            });
            AsyncGPUReadback.WaitAllRequests();
            return result;
        }

        protected override void OnSensorDestroy()
        {
            _jobHandle.Complete();
            _pointCloud.Dispose();
            _noises.Dispose();
            _directions.Dispose();
            _depthRt.Release();
            _colorRt.Release();
        }

        private void OnRenderImage(RenderTexture source, RenderTexture dest)
        {
            Graphics.Blit(null, dest, _depthCameraMat);
        }
    }
}

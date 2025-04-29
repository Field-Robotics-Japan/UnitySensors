using System;
using UnityEngine;
using UnityEngine.Rendering;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

using UnitySensors.Utils.Noise;
using UnitySensors.Utils.Camera;

using Random = Unity.Mathematics.Random;

namespace UnitySensors.Sensor.LiDAR
{
    public class DepthBufferLiDARSensor : LiDARSensor
    {
        [SerializeField, Min(1)]
        private int _texturePixelsNum = 1;
        [SerializeField, Attribute.ReadOnly]
        private Vector2Int _textureSizePerCamera;
        [SerializeField] Material _depthBufferLidarMat;

        private Transform _transform;

        private JobHandle _jobHandle;
        private IUpdateGaussianNoisesJob _updateGaussianNoisesJob;
        private ITextureToPointsJob _textureToPointsJob;

        private RenderTexture _rt;
        private Texture2D _texture;
        private NativeArray<float> _noises;
        private NativeArray<Color> _pixels;
        private NativeArray<float3> _directions;
        private NativeArray<int> _pixelIndices;

        private int _camerasNum = 0;
        private float _horizontalFOV;

        protected override void Init()
        {
            if (scanPattern == null)
            {
                Debug.LogWarning("Initialization postponed: scanPattern is null. Ensure that scanPattern is assigned before calling Init.");
                return;
            }
            Initialize();
        }

        public override void Initialize()
        {
            base.Initialize();
            _transform = this.transform;
            SetupCamera();
            LoadScanData();
            SetupJobs();
        }

        private void SetupCamera()
        {
            float azimuthAngleRange = scanPattern.maxAzimuthAngle - scanPattern.minAzimuthAngle;
            _camerasNum = azimuthAngleRange > 0.0f ? Mathf.CeilToInt(azimuthAngleRange / 60.0f) : 1;

            _horizontalFOV = azimuthAngleRange / _camerasNum;
            float verticalFOV = Mathf.Max(1.0f, 2.0f * Mathf.Max(Mathf.Abs(scanPattern.maxZenithAngle), Mathf.Abs(scanPattern.minZenithAngle)));

            float aspectRatio = Mathf.Tan(0.5f * _horizontalFOV * Mathf.Deg2Rad) / Mathf.Tan(0.5f * verticalFOV * Mathf.Deg2Rad);
            _textureSizePerCamera.y = Mathf.RoundToInt(Mathf.Sqrt((_texturePixelsNum / _camerasNum) / aspectRatio));
            _textureSizePerCamera.x = Mathf.RoundToInt(_textureSizePerCamera.y * aspectRatio);

            _rt = new RenderTexture(_textureSizePerCamera.x, _textureSizePerCamera.y * _camerasNum, 32, RenderTextureFormat.ARGBFloat);
            _texture = new Texture2D(_textureSizePerCamera.x, _textureSizePerCamera.y * _camerasNum, TextureFormat.RGBAFloat, false);
            _pixels = _texture.GetPixelData<Color>(0);

            for (int i = 0; i < _camerasNum; i++)
            {
                GameObject camera_obj = new GameObject();
                camera_obj.name = "Camera " + i.ToString();

                Transform camera_tf = camera_obj.transform;
                camera_tf.parent = _transform;
                camera_tf.localPosition = Vector3.zero;
                camera_tf.localEulerAngles = new Vector3(0, scanPattern.minAzimuthAngle + _horizontalFOV * (i + 0.5f), 0);

                UnityEngine.Camera camera = camera_obj.AddComponent<UnityEngine.Camera>();
                camera.fieldOfView = verticalFOV;
                camera.nearClipPlane = minRange;
                camera.farClipPlane = maxRange;
                camera.targetTexture = _rt;

                Rect rect = camera.rect;
                rect.size = new Vector2(1.0f, 1.0f / _camerasNum);
                rect.position = Vector2.up * ((float)i / _camerasNum);
                camera.rect = rect;

                Color2Depth converter = camera_obj.AddComponent<Color2Depth>();
                converter.mat_source = _depthBufferLidarMat;
                converter.y_min = (float)i / _camerasNum;
                converter.y_max = (float)(i + 1.0f) / _camerasNum;
                converter.y_coef = _camerasNum;
            }
        }

        private void LoadScanData()
        {
            _directions = new NativeArray<float3>(scanPattern.size * 2, Allocator.Persistent);
            _pixelIndices = new NativeArray<int>(scanPattern.size * 2, Allocator.Persistent);

            float camerasNum_2 = _camerasNum * 0.5f - 0.5f;
            int textureSizePerCamera = _textureSizePerCamera.x * _textureSizePerCamera.y;
            float radius = _textureSizePerCamera.x * 0.5f / Mathf.Tan(_horizontalFOV * 0.5f * Mathf.Deg2Rad);

            for (int i = 0; i < scanPattern.size; i++)
            {
                float3 scan = scanPattern.scans[i];

                _directions[i] = _directions[i + scanPattern.size] = scan;

                float azimuthAngle = Mathf.Atan2(scan.x, scan.z) * Mathf.Rad2Deg;
                int cameraIndex = Mathf.FloorToInt(_camerasNum * Mathf.InverseLerp(scanPattern.minAzimuthAngle, scanPattern.maxAzimuthAngle, azimuthAngle));
                Vector3 dir = scan;
                dir = Quaternion.Euler(0, -(cameraIndex - camerasNum_2) * _horizontalFOV, 0) * dir;
                dir *= (radius / dir.z);

                int index_x = (int)Mathf.Clamp(_textureSizePerCamera.x * 0.5f + dir.x, 0, _textureSizePerCamera.x - 1);
                int index_y = (int)Mathf.Clamp(_textureSizePerCamera.y * 0.5f + dir.y, 0, _textureSizePerCamera.y - 1);
                _pixelIndices[i] = _pixelIndices[i + scanPattern.size] = cameraIndex * textureSizePerCamera + index_y * _textureSizePerCamera.x + index_x;
            }
        }

        private void SetupJobs()
        {
            _noises = new NativeArray<float>(pointsNum, Allocator.Persistent);

            _updateGaussianNoisesJob = new IUpdateGaussianNoisesJob()
            {
                sigma = gaussianNoiseSigma,
                random = new Random((uint)Environment.TickCount),
                noises = _noises
            };

            _textureToPointsJob = new ITextureToPointsJob()
            {
                near = minRange,
                sqrNear = minRange * minRange,
                far = maxRange,
                maxIntensity = maxIntensity,
                indexOffset = 0,
                directions = _directions,
                pixelIndices = _pixelIndices,
                noises = _noises,
                pixels = _pixels,
                points = pointCloud.points
            };
        }

        protected override void UpdateSensor()
        {
            if (!LoadTexture()) return;

            JobHandle updateGaussianNoisesJobHandle = _updateGaussianNoisesJob.Schedule(pointsNum, 1024);
            _jobHandle = _textureToPointsJob.Schedule(pointsNum, 1024, updateGaussianNoisesJobHandle);

            JobHandle.ScheduleBatchedJobs();
            _jobHandle.Complete();

            _textureToPointsJob.indexOffset = (_textureToPointsJob.indexOffset + pointsNum) % scanPattern.size;

            if (onSensorUpdated != null)
                onSensorUpdated.Invoke();
        }

        private bool LoadTexture()
        {
            bool result = false;
            AsyncGPUReadback.Request(_rt, 0, request =>
            {
                if (request.hasError)
                {
                    Debug.LogError("GPU readback error detected.");
                }
                else
                {
                    var data = request.GetData<Color>();
                    _texture.LoadRawTextureData(data);
                    _texture.Apply();
                    result = true;
                }
            });
            AsyncGPUReadback.WaitAllRequests();
            return result;
        }

        protected override void OnSensorDestroy()
        {
            _jobHandle.Complete();
            _rt.Release();
            _directions.Dispose();
            _pixelIndices.Dispose();
            _noises.Dispose();
            base.OnSensorDestroy();
        }
    }
}

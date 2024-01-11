using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

using Unity.Burst;
using Unity.Collections;
using UnityEngine.Jobs;
using Unity.Jobs;

using Random = Unity.Mathematics.Random;

namespace UnitySensors
{
    /*
    public class LivoxSensor : Sensor
    {
        [SerializeField]
        private CSVLiDARScanPattern _scanPattern;

        [SerializeField]
        private int _scanSeparation = 40;

        [SerializeField]
        private float _minDistance = 0.1f;
        [SerializeField]
        private float _maxDistance = 100.0f;
        [SerializeField]
        private float _maxIntensity = 255.0f;
        [SerializeField]
        private float _gaussianNoiseSigma = 0.0f;

        [SerializeField]
        private int _resolution = 100;

        private Transform _transform;

        private Camera _cam;

        private Vector2Int _textureSize;

        private RenderTexture _rt = null;
        private Texture2D _texture;

        private JobHandle _handle;
        private TextureToPointsJob _textureToPointsJob;
        private UpdateGaussianNoisesJob _updateGaussianNoisesJob;
        private Random _random;
        public NativeArray<Vector3> points;
        public NativeArray<float> intensities;
        private NativeArray<Vector3> _directions;
        private NativeArray<int> _pixelIndices;
        private NativeArray<float> _noises;

        private uint _randomSeed;
        private int _pointsNum;
        public uint pointsNum { get=>(uint)(_pointsNum);}
        public float minDistance { get => _minDistance; }
        public float maxDistance { get => _maxDistance; }
        public float maxIntensity { get => _maxIntensity; }

        protected override void Init()
        {
            _pointsNum = _scanPattern.size / _scanSeparation;
            CreateSensor();
            SetupCamera();
            SetupIndicesAndDirections();
            SetupJob();
            base.Init();
        }

        private void CreateSensor()
        {
            _transform = this.transform;

            GameObject cam_obj = new GameObject();
            Transform cam_transform = cam_obj.transform;
            _cam = cam_obj.AddComponent<Camera>();
            cam_transform.parent = _transform;
            cam_transform.name = "Camera";
            cam_transform.localPosition = Vector3.zero;
            cam_transform.localRotation = Quaternion.identity;
        }

        private void SetupCamera()
        {
            float fov = Mathf.Max(_scanPattern.maxAzimuth, _scanPattern.maxZenith * 2 / Mathf.Cos(_scanPattern.maxAzimuth * Mathf.Deg2Rad));

            float resolution_y = _resolution / (_scanPattern.maxZenith * 2) * fov;
            float resolution_x = Mathf.CeilToInt(resolution_y / Mathf.Tan(fov * 0.5f * Mathf.Deg2Rad) * Mathf.Tan(60.0f * Mathf.Deg2Rad));
            _textureSize.x = Mathf.CeilToInt(resolution_x);
            _textureSize.y = Mathf.CeilToInt(resolution_y);

            _rt = new RenderTexture(_textureSize.x, _textureSize.y, 32, RenderTextureFormat.ARGBFloat);

            _cam.targetTexture = _rt;
            _cam.fieldOfView = fov;
            _cam.nearClipPlane = _minDistance;
            _cam.farClipPlane = _maxDistance;
            _cam.gameObject.AddComponent<DepthCamera>();
            _cam.clearFlags = CameraClearFlags.SolidColor;

            _texture = new Texture2D(_textureSize.x, _textureSize.y, TextureFormat.RGBAFloat, false);
        }
        private void SetupIndicesAndDirections()
        {
            int scanPattern_size = _scanPattern.size;
            _directions = new NativeArray<Vector3>(scanPattern_size, Allocator.Persistent);
            _pixelIndices = new NativeArray<int>(scanPattern_size, Allocator.Persistent);

            float radius = _textureSize.y * 0.5f / Mathf.Tan(_cam.fieldOfView * 0.5f * Mathf.Deg2Rad);
            for (int i = 0; i < scanPattern_size; i++)
            {
                _directions[i] = _scanPattern.scans[i];

                Vector3 dir = _directions[i];
                dir *= (radius / dir.z);
                int index_x = (int)Mathf.Clamp(_textureSize.x * 0.5f + dir.x, 0, _textureSize.x - 1);
                int index_y = (int)Mathf.Clamp(_textureSize.y * 0.5f + dir.y, 0, _textureSize.y - 1);
                _pixelIndices[i] = index_y * _textureSize.x + index_x;
            }
        }

        private void SetupJob()
        {
            points = new NativeArray<Vector3>(_pointsNum, Allocator.Persistent);
            intensities = new NativeArray<float>(_pointsNum, Allocator.Persistent);
            _randomSeed = (uint)Environment.TickCount;
            _random = new Random(_randomSeed);

            _noises = new NativeArray<float>(_pointsNum, Allocator.Persistent);

            _updateGaussianNoisesJob = new UpdateGaussianNoisesJob()
            {
                sigma = _gaussianNoiseSigma,
                random = _random,
                noises = _noises
            };

            _textureToPointsJob = new TextureToPointsJob()
            {
                scanSeparation = _scanSeparation,
                separationCounter = 0,
                minDistance = _minDistance,
                minDistance_sqr = _minDistance * _minDistance,
                maxDistance = _maxDistance,
                maxIntensity = _maxIntensity,
                pixelIndices = _pixelIndices,
                directions = _directions,
                pixels = _texture.GetPixelData<Color>(0),
                noises = _noises,
                points = points,
                intensities = intensities
            };
        }

        protected override void UpdateSensor()
        {
            _handle.Complete();
            if (_randomSeed++ == 0) _randomSeed = 1;
            _updateGaussianNoisesJob.random.InitState(_randomSeed);

            _textureToPointsJob.separationCounter++;
            if (_textureToPointsJob.separationCounter >= _scanSeparation) _textureToPointsJob.separationCounter = 0;

            AsyncGPUReadback.Request(_rt, 0, request => {
                if (request.hasError)
                {
                }
                else
                {
                    if (!Application.isPlaying) return;
                    var data = request.GetData<Color>();
                    _texture.LoadRawTextureData(data);
                    _texture.Apply();
                }
            });

            JobHandle updateGaussianNoisesJobHandle = _updateGaussianNoisesJob.Schedule(_pointsNum, 1);
            _handle = _textureToPointsJob.Schedule(_pointsNum, 1, updateGaussianNoisesJobHandle);

            JobHandle.ScheduleBatchedJobs();
        }

        public void CompleteJob()
        {
            _handle.Complete();
        }

        private void OnDestroy()
        {
            _handle.Complete();
            _noises.Dispose();
            _pixelIndices.Dispose();
            _directions.Dispose();
            points.Dispose();
            intensities.Dispose();

            _rt.Release();
        }

        [BurstCompile]
        private struct UpdateGaussianNoisesJob : IJobParallelFor
        {
            public float sigma;
            public Random random;
            public NativeArray<float> noises;

            public void Execute(int index)
            {
                var rand2 = random.NextFloat();
                var rand3 = random.NextFloat();
                float normrand =
                    (float)Math.Sqrt(-2.0f * Math.Log(rand2)) *
                    (float)Math.Cos(2.0f * Math.PI * rand3);
                noises[index] = sigma * normrand;
            }
        }

        [BurstCompile]
        private struct TextureToPointsJob : IJobParallelFor
        {
            public int scanSeparation;
            public int separationCounter;

            [ReadOnly]
            public float minDistance;
            [ReadOnly]
            public float minDistance_sqr;
            [ReadOnly]
            public float maxDistance;
            [ReadOnly]
            public float maxIntensity;

            [ReadOnly]
            public NativeArray<int> pixelIndices;
            [ReadOnly]
            public NativeArray<Vector3> directions;

            [ReadOnly]
            public NativeArray<Color> pixels;
            [ReadOnly]
            public NativeArray<float> noises;

            public NativeArray<Vector3> points;
            public NativeArray<float> intensities;

            public void Execute(int index)
            {
                int offset = points.Length * separationCounter / scanSeparation;
                int pixelIndex = pixelIndices.AsReadOnly()[index + offset];
                float distance = maxDistance * Mathf.Clamp01(1.0f - pixels.AsReadOnly()[pixelIndex].r) + noises[index];
                bool isValid = (minDistance <= distance && distance <= maxDistance);
                if (!isValid) distance = 0;
                points[index] = directions.AsReadOnly()[index + offset] * distance;
                float distance_sqr = distance * distance;
                intensities[index] = isValid ? maxIntensity * minDistance_sqr / distance_sqr : 0;
            }
        }
    }
    */
}

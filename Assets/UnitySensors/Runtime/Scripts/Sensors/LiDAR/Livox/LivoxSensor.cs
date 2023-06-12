using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

using Unity.Burst;
using Unity.Collections;
using UnityEngine.Jobs;
using Unity.Jobs;

namespace UnitySensors
{
    public class LivoxSensor : Sensor
    {
        [SerializeField]
        private CSVLiDARScanPattern _scanPattern;

        [SerializeField]
        private int _scanSeparation = 40;

        [SerializeField]
        private float _minRange = 0.1f;
        [SerializeField]
        private float _maxRange = 100.0f;

        [SerializeField]
        private int _resolution = 100;

        private Transform _transform;

        private Camera _cam;

        private Vector2Int _textureSize;

        private RenderTexture _rt = null;
        private Texture2D _texture;

        private JobHandle _handle;
        private TextureToPointsJob _job;
        public NativeArray<Vector3> points;
        private NativeArray<Vector3> _directions;
        private NativeArray<int> _pixelIndices;

        public uint pointNum { get=>(uint)(_scanPattern.size / _scanSeparation);}

        protected override void Init()
        {
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
            _cam.nearClipPlane = _minRange;
            _cam.farClipPlane = _maxRange;
            _cam.gameObject.AddComponent<DepthCamera>();
            _cam.clearFlags = CameraClearFlags.SolidColor;

            _texture = new Texture2D(_textureSize.x, _textureSize.y, TextureFormat.RGBAFloat, false);
        }
        private void SetupIndicesAndDirections()
        {
            int pointsNum = _scanPattern.size;

            _directions = new NativeArray<Vector3>(pointsNum, Allocator.Persistent);
            _pixelIndices = new NativeArray<int>(pointsNum, Allocator.Persistent);

            float radius = _textureSize.y * 0.5f / Mathf.Tan(_cam.fieldOfView * 0.5f * Mathf.Deg2Rad);
            for (int i = 0; i < pointsNum; i++)
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
            points = new NativeArray<Vector3>(_scanPattern.size / _scanSeparation, Allocator.Persistent);
            _job = new TextureToPointsJob()
            {
                far = _maxRange,
                scanSeparation = _scanSeparation,
                separationCounter = 0,
                pixelIndices = _pixelIndices,
                directions = _directions,
                pixels = _texture.GetPixelData<Color>(0),
                points = points
            };
        }

        protected override void UpdateSensor()
        {
            _handle.Complete();
            _job.separationCounter++;
            if (_job.separationCounter >= _scanSeparation) _job.separationCounter = 0;

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

            _handle = _job.Schedule(_scanPattern.size / _scanSeparation, 1);

            JobHandle.ScheduleBatchedJobs();
        }

        public void CompleteJob()
        {
            _handle.Complete();
        }

        private void OnDestroy()
        {
            _rt.Release();
        }

        private void OnApplicationQuit()
        {
            _handle.Complete();
            _pixelIndices.Dispose();
            _directions.Dispose();
            points.Dispose();

            _rt.Release();
        }

        [BurstCompile]
        private struct TextureToPointsJob : IJobParallelFor
        {
            public float far;
            public int scanSeparation;
            public int separationCounter;

            [ReadOnly]
            public NativeArray<int> pixelIndices;
            [ReadOnly]
            public NativeArray<Vector3> directions;

            [ReadOnly]
            public NativeArray<Color> pixels;

            public NativeArray<Vector3> points;

            public void Execute(int index)
            {
                int offset = points.Length * separationCounter / scanSeparation;
                int pixelIndex = pixelIndices.AsReadOnly()[index + offset];
                float distance = pixels.AsReadOnly()[pixelIndex].r;
                points[index] = directions.AsReadOnly()[index + offset] * far * Mathf.Clamp01(1.0f - distance);
            }
        }
    }
}

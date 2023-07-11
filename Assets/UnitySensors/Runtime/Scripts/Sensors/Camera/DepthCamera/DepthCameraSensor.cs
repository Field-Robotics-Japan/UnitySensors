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
    [RequireComponent(typeof(Camera))]
    public class DepthCameraSensor : Sensor
    {
        public enum DepthCamerMode
        {
            TEXTURE_ONLY,
            POINTCLOUD_ONLY,
            BOTH
        }

        [SerializeField]
        private DepthCamerMode _mode;

        [SerializeField]
        private Vector2Int _resolution = new Vector2Int(640, 480);

        [SerializeField]
        private float _fov = 30.0f;
        [SerializeField]
        private float _minRange = 0.05f;
        [SerializeField]
        private float _maxRange = 10.0f;

        [SerializeField, Range(0, 100)]
        private int _quality = 100;

        private Camera _cam;

        private RenderTexture _rt = null;
        private Texture2D _texture;
        private Texture2D _texture_depth;

        private JobHandle _handle;
        private TextureToPointsJob _job;
        private NativeArray<Vector3> _directions;
        public NativeArray<Vector3> points;

        private int _pointsNum;

        private bool _textureInit;

        public DepthCamerMode mode { get => _mode; }
        public int quality { get => _quality; }
        public Texture2D texture { get => _texture; }
        public uint pointsNum { get => (uint)_pointsNum; }

        protected override void Init()
        {
            _textureInit = false;

            _cam = GetComponent<Camera>();
            _rt = new RenderTexture(_resolution.x, _resolution.y, 32, RenderTextureFormat.ARGBFloat);

            _cam.clearFlags = CameraClearFlags.SolidColor;
            _cam.fieldOfView = _fov;
            _cam.targetTexture = _rt;
            _cam.nearClipPlane = _minRange;
            _cam.farClipPlane = _maxRange;

            if (!GetComponent<DepthCamera>())
            {
                gameObject.AddComponent<DepthCamera>();
            }

            if (_mode != DepthCamerMode.POINTCLOUD_ONLY)
            {
                _texture = new Texture2D(_resolution.x, _resolution.y, TextureFormat.RGBAFloat, false);
            }

            if (_mode != DepthCamerMode.TEXTURE_ONLY)
            {
                _texture_depth = new Texture2D(_resolution.x, _resolution.y, TextureFormat.RGBAFloat, false);
                SetupDirections();
                SetupJob();
            }

            UpdateTexture();
            base.Init();
        }

        private void SetupDirections()
        {
            _pointsNum = _resolution.x * _resolution.y;

            _directions = new NativeArray<Vector3>(_pointsNum, Allocator.Persistent);

            float z = _resolution.y * 0.5f / Mathf.Tan(_cam.fieldOfView * 0.5f * Mathf.Deg2Rad);
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
            points = new NativeArray<Vector3>(_pointsNum, Allocator.Persistent);

            _job = new TextureToPointsJob()
            {
                far = _maxRange,
                directions = _directions,
                pixels = _texture_depth.GetPixelData<Color>(0),
                points = points
            };
        }

        protected override void UpdateSensor()
        {
            if (!_textureInit) return;
            if (_mode != DepthCamerMode.TEXTURE_ONLY)
            {
                _handle.Complete();
            }

            UpdateTexture();

            if (_mode != DepthCamerMode.TEXTURE_ONLY)
            {
                _handle = _job.Schedule(_pointsNum, 1);
                JobHandle.ScheduleBatchedJobs();
            }
        }

        private void UpdateTexture()
        {
            AsyncGPUReadback.Request(_rt, 0, request => {
                if (request.hasError)
                {
                }
                else
                {
                    if (!Application.isPlaying) return;
                    var data = request.GetData<Color>();
                    if (_mode != DepthCamerMode.POINTCLOUD_ONLY)
                    {
                        _texture.LoadRawTextureData(data);
                        _texture.Apply();
                    }
                    if (_mode != DepthCamerMode.TEXTURE_ONLY)
                    {
                        _texture_depth.LoadRawTextureData(data);
                        _texture_depth.Apply();
                    }
                    _textureInit = true;
                }
            });
        }

        public void CompleteJob()
        {
            if (_mode == DepthCamerMode.TEXTURE_ONLY) return;
            _handle.Complete();
        }

        private void OnDestroy()
        {
            _rt.Release();
        }

        private void OnApplicationQuit()
        {
            if (_mode != DepthCamerMode.TEXTURE_ONLY)
            {
                _handle.Complete();
                _directions.Dispose();
                points.Dispose();
            }
            _rt.Release();
        }

        [BurstCompile]
        private struct TextureToPointsJob : IJobParallelFor
        {
            public float far;

            [ReadOnly]
            public NativeArray<Vector3> directions;

            [ReadOnly]
            public NativeArray<Color> pixels;

            public NativeArray<Vector3> points;

            public void Execute(int index)
            {
                float distance = pixels.AsReadOnly()[index].r;
                points[index] = directions.AsReadOnly()[index] * far * Mathf.Clamp01(1.0f - distance);
            }
        }
    }
}

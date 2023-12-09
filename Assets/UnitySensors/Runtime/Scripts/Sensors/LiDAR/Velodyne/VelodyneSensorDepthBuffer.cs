using System;
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
    /*
    public class VelodyneSensorDepthBuffer : Sensor
    {
        [SerializeField]
        private RotatingLiDARScanPattern _scanPattern;
        [SerializeField]
        private float _textureResolutionCoef = 1.0f;

        [SerializeField, Range(3, 6)]
        private int _numOfCameras = 3;
        [SerializeField]
        private float _minRange = 0.01f;
        [SerializeField]
        private float _maxRange = 100.0f;

        private Transform _transform;

        private Camera[] _cams;
        private RenderTexture[] _rts = null;
        private Texture2D[] _textures;

        private JobHandle _handle;
        private TextureToPointsJob[] _jobs;
        private NativeArray<int>[] _pixelIndices;
        private NativeArray<Vector3>[] _directions;
        private NativeArray<Color>[] _pixels;

        public NativeArray<Vector3> points;

        private Vector2Int _textureSize;
        private bool[] _textureUpdated;

        protected override void Init()
        {
            _transform = transform;
            _textureUpdated = new bool[_numOfCameras];
            CreateSensor();
            SetupCameras();
            SetupIndicesAndDirections();
            SetupJobs();
            base.Init();
        }

        private void CreateSensor()
        {
            _cams = new Camera[_numOfCameras];
            float d_theta = 360.0f / _numOfCameras;
            float theta = d_theta * 0.5f;
            for(int i = 0; i < _numOfCameras; i++)
            {
                GameObject cam_obj = new GameObject();
                Transform cam_transform = cam_obj.transform;
                _cams[i] = cam_obj.AddComponent<Camera>();
                cam_transform.parent = _transform;
                cam_transform.name = "Camera" + i.ToString();
                cam_transform.localPosition = Vector3.zero;
                cam_transform.localEulerAngles = new Vector3(0, theta, 0);
                theta += d_theta;
            }
        }

        private void SetupCameras()
        {
            float maxZenithAngle = _scanPattern.maxZenith;

            float fov = maxZenithAngle * 4.0f;
            float fov_2 = fov * 0.5f;

            float resolution_y = _scanPattern.numOfLayer * fov_2 / maxZenithAngle;
            float resolution_x = Mathf.CeilToInt(resolution_y / Mathf.Tan(fov_2 * Mathf.Deg2Rad) * Mathf.Tan(180.0f / _numOfCameras * Mathf.Deg2Rad));

            _textureSize.x = Mathf.CeilToInt(resolution_x * _textureResolutionCoef);
            _textureSize.y = Mathf.CeilToInt(resolution_y * _textureResolutionCoef);

            _rts = new RenderTexture[_numOfCameras];
            _textures = new Texture2D[_numOfCameras];

            _pixels = new NativeArray<Color>[_numOfCameras];

            for(int i = 0; i < _numOfCameras; i++)
            {
                _rts[i] = new RenderTexture(_textureSize.x, _textureSize.y, 32, RenderTextureFormat.ARGBFloat);
                _cams[i].targetTexture = _rts[i];
                _cams[i].fieldOfView = fov;
                _cams[i].nearClipPlane = _minRange;
                _cams[i].farClipPlane = _maxRange;
                _cams[i].gameObject.AddComponent<DepthCamera>();
                _cams[i].clearFlags = CameraClearFlags.SolidColor;
                _textures[i] = new Texture2D(_textureSize.x, _textureSize.y, TextureFormat.RGBAFloat, false);
                _pixels[i] = _textures[i].GetPixelData<Color>(0);
            }
        }

        private void SetupIndicesAndDirections()
        {
            int pointsNum = _scanPattern.size;
            int pointsNumPerTexture = pointsNum / _numOfCameras;
            _pixelIndices = new NativeArray<int>[_numOfCameras];
            _directions = new NativeArray<Vector3>[_numOfCameras];
            for (int i = 0; i < _numOfCameras; i++)
            {
                _pixelIndices[i] = new NativeArray<int>(pointsNumPerTexture, Allocator.Persistent);
                _directions[i] = new NativeArray<Vector3>(pointsNumPerTexture, Allocator.Persistent);
            }

            float d_theta = 360.0f / _numOfCameras;
            float radius = _textureSize.x * 0.5f / Mathf.Tan(d_theta * 0.5f * Mathf.Deg2Rad);

            for (int i = 0; i < pointsNum; i++)
            {
                Vector3 dir = _scanPattern.scans[i];
                _directions[i / pointsNumPerTexture][i % pointsNumPerTexture] = dir;
                dir = Quaternion.Euler(0, -((i / pointsNumPerTexture) + 0.5f) * d_theta, 0) * dir;
                dir *= (radius / dir.z);
                int index_x = (int)Mathf.Clamp(_textureSize.x * 0.5f + dir.x, 0, _textureSize.x - 1);
                int index_y = (int)Mathf.Clamp(_textureSize.y * 0.5f + dir.y, 0, _textureSize.y - 1);
                _pixelIndices[i / pointsNumPerTexture][i % pointsNumPerTexture] = index_y * _textureSize.x + index_x;
            }
        }

        private void SetupJobs()
        {
            points = new NativeArray<Vector3>(_scanPattern.size, Allocator.Persistent);

            _jobs = new TextureToPointsJob[_numOfCameras];

            int offset = _scanPattern.size / _numOfCameras;
            for(int i = 0; i < _numOfCameras; i++)
            {
                _jobs[i] = new TextureToPointsJob()
                {
                    far = _maxRange,
                    offset = offset * i,
                    pixelIndices = _pixelIndices[i],
                    directions = _directions[i],
                    pixels = _pixels[i],
                    points = points
                };
            }
        }

        protected override void UpdateSensor()
        {
            UpdateTextures();
            for (int i = 0; i < _numOfCameras; i++)
                if (!_textureUpdated[i]) return;
            Array.Fill(_textureUpdated, false);
        }

        private void UpdateTextures()
        {
            _handle.Complete();

            for (int i = 0; i < _numOfCameras; i++)
            {
                if (_textureUpdated[i]) continue;
                UpdateTexure(i);
            }

            JobHandle[] handles = new JobHandle[_numOfCameras - 1];

            handles[0] = _jobs[0].Schedule(_scanPattern.size / _numOfCameras, 1);
            for(int i = 1; i < _numOfCameras - 1; i++)
            {
                handles[i] = _jobs[i].Schedule(_scanPattern.size / _numOfCameras, 1, handles[i - 1]);
            }
            _handle = _jobs[_numOfCameras - 1].Schedule(_scanPattern.size / _numOfCameras, 1, handles[_numOfCameras - 2]);

            JobHandle.ScheduleBatchedJobs();
        }

        private void UpdateTexure(int i)
        {
            AsyncGPUReadback.Request(_rts[i], 0, request => {
                if (request.hasError)
                {
                }
                else
                {
                    var data = request.GetData<Color>();
                    _textures[i].LoadRawTextureData(data);
                    _textures[i].Apply();
                    _textureUpdated[i] = true;
                }
            });
        }

        public void CompleteJob()
        {
            _handle.Complete();
        }

        private void OnDestroy()
        {
            _handle.Complete();
            for (int i = 0; i < _numOfCameras; i++)
            {
                _pixels[i].Dispose();
                _pixelIndices[i].Dispose();
                _directions[i].Dispose();
            }
            points.Dispose();
            foreach (RenderTexture rt in _rts)
            {
                rt.Release();
            }
        }
        
        [BurstCompile]
        private struct TextureToPointsJob : IJobParallelFor
        {
            public float far;
            public int offset;

            [ReadOnly]
            public NativeArray<int> pixelIndices;
            [ReadOnly]
            public NativeArray<Vector3> directions;

            [ReadOnly]
            public NativeArray<Color> pixels;

            [NativeDisableParallelForRestriction]
            public NativeArray<Vector3> points;

            public void Execute(int index)
            {
                int pixelIndex = pixelIndices.AsReadOnly()[index];
                float distance = pixels.AsReadOnly()[pixelIndex].r;
                points[index + offset] = directions[index] * far * Mathf.Clamp01(1.0f - distance);
            }
        }
    }
    */
}

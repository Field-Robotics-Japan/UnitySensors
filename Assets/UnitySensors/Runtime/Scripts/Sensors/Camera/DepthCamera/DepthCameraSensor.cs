using UnityEngine;
using UnityEngine.Rendering;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

using UnitySensors.Data.PointCloud;

namespace UnitySensors.Sensor.Camera
{
    public class DepthCameraSensor : CameraSensor, IPointCloudInterface<PointXYZ>
    {
        private Material _mat;

        private JobHandle _jobHandle;

        private ITextureToPointsJob _textureToPointsJob;
        private NativeArray<float3> _directions;

        private PointCloud<PointXYZ> _pointCloud;
        private int _pointsNum;
        public PointCloud<PointXYZ> pointCloud { get => _pointCloud; }
        public int pointsNum { get => _pointsNum; }

        protected override void Init()
        {
            base.Init();

            _mat = new Material(Shader.Find("UnitySensors/Color2Depth"));
            float f = m_camera.farClipPlane;
            _mat.SetFloat("_F", f);

            SetupDirections();
            SetupJob();
        }

        private void SetupDirections()
        {
            _pointsNum = resolution.x * resolution.y;

            _directions = new NativeArray<float3>(_pointsNum, Allocator.Persistent);

            float z = resolution.y * 0.5f / Mathf.Tan(m_camera.fieldOfView * 0.5f * Mathf.Deg2Rad);
            for (int y = 0; y < resolution.y; y++)
            {
                for (int x = 0; x < resolution.x; x++)
                {
                    Vector3 vec = new Vector3(-resolution.x / 2 + x, -resolution.y / 2 + y, z);
                    _directions[y * resolution.x + x] = vec.normalized;
                }
            }
        }

        private void SetupJob()
        {
            _pointCloud = new PointCloud<PointXYZ>()
            {
                points = new NativeArray<PointXYZ>(_pointsNum, Allocator.Persistent)
            };

            _textureToPointsJob = new ITextureToPointsJob()
            {
                far = m_camera.farClipPlane,
                directions = _directions,
                pixels = texture.GetPixelData<Color>(0),
                points = _pointCloud.points
            };
        }

        protected override void UpdateSensor()
        {
            if (!LoadTexture()) return;

            _jobHandle = _textureToPointsJob.Schedule(_pointsNum, 1);
            JobHandle.ScheduleBatchedJobs();
            _jobHandle.Complete();

            if (onSensorUpdated != null)
                onSensorUpdated.Invoke();
        }

        protected override void OnSensorDestroy()
        {
            _jobHandle.Complete();
            _pointCloud.Dispose();
            _directions.Dispose();
            base.OnSensorDestroy();
        }

        private void OnRenderImage(RenderTexture source, RenderTexture dest)
        {
            Graphics.Blit(source, dest, _mat);
        }
    }
}

using System;
using UnityEngine;

using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

using UnitySensors.Utils.Noise;

using Random = Unity.Mathematics.Random;

namespace UnitySensors.Sensor.LiDAR
{
    public class RaycastLiDARSensor : LiDARSensor
    {
        [SerializeField] private LayerMask _raycastLayerMask = 1;
        private Transform _transform;

        private JobHandle _jobHandle;

        private IUpdateRaycastCommandsJob _updateRaycastCommandsJob;
        private IUpdateGaussianNoisesJob _updateGaussianNoisesJob;
        private IRaycastHitsToPointsJob _raycastHitsToPointsJob;

        private NativeArray<float3> _directions;
        private NativeArray<RaycastCommand> _raycastCommands;
        private NativeArray<RaycastHit> _raycastHits;

        private NativeArray<float> _noises;

        protected override void Init()
        {
            base.Init();

            _transform = this.transform;

            LoadScanData();
            SetupJobs();
        }

        private void LoadScanData()
        {
            _directions = new NativeArray<float3>(scanPattern.size * 2, Allocator.Persistent);
            for (int i = 0; i < scanPattern.size; i++)
            {
                _directions[i] = _directions[i + scanPattern.size] = scanPattern.scans[i];
            }
        }

        private void SetupJobs()
        {
            _raycastCommands = new NativeArray<RaycastCommand>(pointsNum, Allocator.Persistent);
            _raycastHits = new NativeArray<RaycastHit>(pointsNum, Allocator.Persistent);
            _noises = new NativeArray<float>(pointsNum, Allocator.Persistent);

            _updateRaycastCommandsJob = new IUpdateRaycastCommandsJob()
            {
                origin = _transform.position,
                localToWorldMatrix = _transform.localToWorldMatrix,
                maxRange = maxRange,
                queryParameters = new() { layerMask = _raycastLayerMask },
                directions = _directions,
                indexOffset = 0,
                raycastCommands = _raycastCommands,

            };

            _updateGaussianNoisesJob = new IUpdateGaussianNoisesJob()
            {
                sigma = gaussianNoiseSigma,
                random = new Random((uint)Environment.TickCount),
                noises = _noises
            };

            _raycastHitsToPointsJob = new IRaycastHitsToPointsJob()
            {
                minRange = minRange,
                sqrMinRange = minRange * minRange,
                maxRange = maxRange,
                maxIntensity = maxIntensity,
                directions = _directions,
                indexOffset = 0,
                raycastHits = _raycastHits,
                noises = _noises,
                points = pointCloud.points,
            };
        }

        protected override void UpdateSensor()
        {
            _updateRaycastCommandsJob.origin = _transform.position;
            _updateRaycastCommandsJob.localToWorldMatrix = _transform.localToWorldMatrix;

            JobHandle updateRaycastCommandsJobHandle = _updateRaycastCommandsJob.Schedule(pointsNum, 1024);
            JobHandle updateGaussianNoisesJobHandle = _updateGaussianNoisesJob.Schedule(pointsNum, 1, updateRaycastCommandsJobHandle);
            JobHandle raycastJobHandle = RaycastCommand.ScheduleBatch(_raycastCommands, _raycastHits, 1024, updateGaussianNoisesJobHandle);
            _jobHandle = _raycastHitsToPointsJob.Schedule(pointsNum, 1024, raycastJobHandle);

            JobHandle.ScheduleBatchedJobs();
            _jobHandle.Complete();

            _updateRaycastCommandsJob.indexOffset = (_updateRaycastCommandsJob.indexOffset + pointsNum) % scanPattern.size;
            _raycastHitsToPointsJob.indexOffset = (_raycastHitsToPointsJob.indexOffset + pointsNum) % scanPattern.size;

            if (onSensorUpdated != null)
                onSensorUpdated.Invoke();
        }

        protected override void OnSensorDestroy()
        {
            _jobHandle.Complete();
            _noises.Dispose();
            _directions.Dispose();
            _raycastCommands.Dispose();
            _raycastHits.Dispose();
            base.OnSensorDestroy();
        }
    }
}

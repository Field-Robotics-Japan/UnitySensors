using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.Burst;
using Unity.Collections;
using UnityEngine.Jobs;
using Unity.Jobs;

namespace UnitySensors
{
    public class VelodyneSensor : Sensor
    {
        [SerializeField]
        private RotatingLiDARScanPattern _scanPattern;

        [SerializeField]
        private bool _safeDisposal = false;

        private JobHandle _handle;
        private UpdateRaycastCommandsJob _updateRaycastCommandsJob;
        private RaycastHitsToPointsJob _raycastHitsToPointsJob;

        private TransformAccessArray _transformAccessArray;

        private NativeArray<Vector3> _directions;
        private NativeArray<RaycastCommand> _raycastCommands;
        private NativeArray<RaycastHit> _raycastHits;
        public NativeArray<Vector3> points;

        private int _pointsNum;
        public uint pointsNum { get => (uint)_pointsNum; }

        protected override void Init()
        {
            _pointsNum = _scanPattern.size;
            SetupTransformAccessArray();
            SetupDirections();
            SetupJobs();
            base.Init();
        }

        private void SetupTransformAccessArray()
        {
            Transform _transform = transform;
            Transform[] transformArray = new Transform[_pointsNum];
            for(int i = 0; i < _pointsNum; i++)
            {
                transformArray[i] = _transform;
            }
            _transformAccessArray = new TransformAccessArray(transformArray, 1);
        }

        private void SetupDirections()
        {
            _directions = new NativeArray<Vector3>(_pointsNum, Allocator.Persistent);
            for(int i = 0; i < _pointsNum; i++)
            {
                _directions[i] = _scanPattern.scans[i];
            }
        }

        private void SetupJobs()
        {
            points = new NativeArray<Vector3>(_pointsNum, Allocator.Persistent);
            _raycastCommands = new NativeArray<RaycastCommand>(_pointsNum, Allocator.Persistent);
            _raycastHits = new NativeArray<RaycastHit>(_pointsNum, Allocator.Persistent);

            _updateRaycastCommandsJob = new UpdateRaycastCommandsJob()
            {
                directions = _directions,
                raycastCommands = _raycastCommands
            };

            _raycastHitsToPointsJob = new RaycastHitsToPointsJob()
            {
                directions = _directions,
                raycastHits = _raycastHits,
                points = points
            };
        }

        protected override void UpdateSensor()
        {
            _handle.Complete();

            JobHandle updateRaycastCommandsJobHandle = _updateRaycastCommandsJob.Schedule(_transformAccessArray);
            JobHandle raycastJobHandle = RaycastCommand.ScheduleBatch(_raycastCommands, _raycastHits, _pointsNum, updateRaycastCommandsJobHandle);
            _handle = _raycastHitsToPointsJob.Schedule(_pointsNum, 1, raycastJobHandle);

            JobHandle.ScheduleBatchedJobs();
        }

        public void CompleteJob()
        {
            _handle.Complete();
        }

        private void OnDestroy()
        {
            _handle.Complete();
            if(_safeDisposal)
                _transformAccessArray.Dispose();
            _directions.Dispose();
            _raycastCommands.Dispose();
            _raycastHits.Dispose();
            points.Dispose();
        }

        [BurstCompile]
        private struct UpdateRaycastCommandsJob : IJobParallelForTransform
        {
            [ReadOnly]
            public NativeArray<Vector3> directions;
            public NativeArray<RaycastCommand> raycastCommands;

            public void Execute(int index, TransformAccess transform)
            {
                Vector3 direction = transform.localToWorldMatrix * directions[index];
                raycastCommands[index] = new RaycastCommand(transform.position, direction);
            }
        }

        [BurstCompile]
        private struct RaycastHitsToPointsJob : IJobParallelFor
        {
            [ReadOnly]
            public NativeArray<Vector3> directions;
            [ReadOnly]
            public NativeArray<RaycastHit> raycastHits;

            public NativeArray<Vector3> points;

            public void Execute(int index)
            {
                points[index] = directions[index] * raycastHits[index].distance;
            }
        }
    }
}

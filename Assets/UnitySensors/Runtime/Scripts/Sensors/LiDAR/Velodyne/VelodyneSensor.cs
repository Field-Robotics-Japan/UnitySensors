using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.Burst;
using Unity.Collections;
using UnityEngine.Jobs;
using Unity.Jobs;

using Random = Unity.Mathematics.Random;

namespace UnitySensors
{
    public class VelodyneSensor : Sensor
    {
        [SerializeField]
        private RotatingLiDARScanPattern _scanPattern;
        [SerializeField]
        private float _gaussianNoiseSigma = 0.0f;

        private Transform _transform;
        private JobHandle _handle;
        private UpdateRaycastCommandsJob _updateRaycastCommandsJob;
        private UpdateGaussianNoisesJob _updateGaussianNoisesJob;
        private RaycastHitsToPointsJob _raycastHitsToPointsJob;

        private NativeArray<Vector3> _directions;
        private NativeArray<RaycastCommand> _raycastCommands;
        private NativeArray<RaycastHit> _raycastHits;
        private Random _random;
        private NativeArray<float> _noises;
        public NativeArray<Vector3> points;

        private uint _randomSeed;
        private int _pointsNum;
        public uint pointsNum { get => (uint)_pointsNum; }

        protected override void Init()
        {
            _transform = this.transform;
            _pointsNum = _scanPattern.size;
            SetupDirections();
            SetupJobs();
            base.Init();
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

            _randomSeed = (uint)Environment.TickCount;
            _random = new Random(_randomSeed);

            _noises = new NativeArray<float>(_pointsNum, Allocator.Persistent);

            _updateRaycastCommandsJob = new UpdateRaycastCommandsJob()
            {
                origin = _transform.position,
                localToWorldMatrix = _transform.localToWorldMatrix,
                directions = _directions,
                raycastCommands = _raycastCommands
            };

            _updateGaussianNoisesJob = new UpdateGaussianNoisesJob()
            {
                sigma = _gaussianNoiseSigma,
                random = _random,
                noises = _noises
            };

            _raycastHitsToPointsJob = new RaycastHitsToPointsJob()
            {
                directions = _directions,
                raycastHits = _raycastHits,
                noises = _noises,
                points = points
            };
        }

        protected override void UpdateSensor()
        {
            _handle.Complete();

            if(_randomSeed++ == 0) _randomSeed = 1;
            _updateGaussianNoisesJob.random.InitState(_randomSeed);

            _updateRaycastCommandsJob.origin = _transform.position;
            _updateRaycastCommandsJob.localToWorldMatrix = _transform.localToWorldMatrix;

            JobHandle updateRaycastCommandsJobHandle = _updateRaycastCommandsJob.Schedule(_pointsNum, 1);
            JobHandle updateGaussianNoisesJobHandle = _updateGaussianNoisesJob.Schedule(_pointsNum, 1, updateRaycastCommandsJobHandle);
            JobHandle raycastJobHandle = RaycastCommand.ScheduleBatch(_raycastCommands, _raycastHits, _pointsNum, updateGaussianNoisesJobHandle);
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
            _noises.Dispose();
            _directions.Dispose();
            _raycastCommands.Dispose();
            _raycastHits.Dispose();
            points.Dispose();
        }

        [BurstCompile]
        private struct UpdateRaycastCommandsJob : IJobParallelFor
        {
            [ReadOnly]
            public Vector3 origin;
            [ReadOnly]
            public Matrix4x4 localToWorldMatrix;
            [ReadOnly]
            public NativeArray<Vector3> directions;
            public NativeArray<RaycastCommand> raycastCommands;

            public void Execute(int index)
            {
                Vector3 direction = localToWorldMatrix * directions[index];
                raycastCommands[index] = new RaycastCommand(origin, direction);
            }
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
        private struct RaycastHitsToPointsJob : IJobParallelFor
        {
            [ReadOnly]
            public NativeArray<Vector3> directions;
            [ReadOnly]
            public NativeArray<RaycastHit> raycastHits;
            [ReadOnly]
            public NativeArray<float> noises;

            public NativeArray<Vector3> points;

            public void Execute(int index)
            {
                points[index] = directions[index] * (raycastHits[index].distance + noises[index]);
            }
        }
    }
}

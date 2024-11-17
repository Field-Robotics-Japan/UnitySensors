using UnityEngine;

using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace UnitySensors.Sensor.LiDAR
{
    [BurstCompile]
    public struct IUpdateRaycastCommandsJob : IJobParallelFor
    {
        [ReadOnly]
        public Vector3 origin;
        [ReadOnly]
        public Matrix4x4 localToWorldMatrix;
        [ReadOnly]
        public float maxRange;
        [ReadOnly, NativeDisableParallelForRestriction]
        public NativeArray<float3> directions;
        [ReadOnly]
        public int indexOffset;
        public NativeArray<RaycastCommand> raycastCommands;

        public void Execute(int index)
        {
            // FIXME: Update the api
            raycastCommands[index] = new RaycastCommand(origin, localToWorldMatrix * (Vector3)directions[index + indexOffset], maxRange);
        }
    }
}
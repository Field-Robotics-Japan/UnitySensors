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
        [ReadOnly]
        public QueryParameters queryParameters;
        [ReadOnly]
        public NativeArray<float3> directions;
        [ReadOnly]
        public int indexOffset;
        [WriteOnly]
        public NativeArray<RaycastCommand> raycastCommands;

        public void Execute(int index)
        {
            raycastCommands[index] = new(origin, localToWorldMatrix * (Vector3)directions[index + indexOffset], queryParameters, maxRange);
        }
    }
}
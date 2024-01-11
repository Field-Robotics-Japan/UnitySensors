using System;
using Unity.Collections;
using Unity.Mathematics;

namespace UnitySensors.Data.PointCloud
{
    public struct PointXYZ : IPointXYZInterface
    {
        private float3 _position;

        public float3 position { get => _position; set => _position = value; }

        public void CopyTo(NativeArray<byte> dst)
        {
            dst.GetSubArray(0, 4).CopyFrom(BitConverter.GetBytes(_position.x));
            dst.GetSubArray(4, 4).CopyFrom(BitConverter.GetBytes(_position.y));
            dst.GetSubArray(8, 4).CopyFrom(BitConverter.GetBytes(_position.z));
        }
    }
}
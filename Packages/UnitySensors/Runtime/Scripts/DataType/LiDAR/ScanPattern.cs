using UnityEngine;
using Unity.Mathematics;

using UnitySensors.Attribute;

namespace UnitySensors.DataType.LiDAR
{
    public class ScanPattern : ScriptableObject
    {
        [SerializeField, HideInInspector]
        public float3[] scans;
        [SerializeField, ReadOnly]
        public int size;
        [SerializeField, ReadOnly]
        public float minZenithAngle;
        [SerializeField, ReadOnly]
        public float maxZenithAngle;
        [SerializeField, ReadOnly]
        public float minAzimuthAngle;
        [SerializeField, ReadOnly]
        public float maxAzimuthAngle;
    }
}

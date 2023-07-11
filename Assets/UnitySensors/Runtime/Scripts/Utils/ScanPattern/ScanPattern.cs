using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnitySensors
{
    [CreateAssetMenu]
    public class ScanPattern : ScriptableObject
    {
        [SerializeField, ReadOnly]
        protected bool _generated;
        [SerializeField, ReadOnly]
        protected int _size;
        [SerializeField, ReadOnly]
        protected float _maxAzimuth;
        [SerializeField, ReadOnly]
        protected float _maxZenith;

        [SerializeField, HideInInspector]
        protected Vector3[] _scans;

        public bool generated { get => _generated; }
        public int size { get => _size; }
        public float maxAzimuth { get => _maxAzimuth; }
        public float maxZenith { get => _maxZenith; }
        public Vector3[] scans { get => _scans; }

        public virtual void GenerateScanPattern()
        {
        
        }
    }
}

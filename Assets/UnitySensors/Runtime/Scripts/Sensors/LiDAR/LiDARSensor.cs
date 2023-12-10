using System;

using UnityEngine;

using Unity.Mathematics;
using Unity.Collections;

using UnitySensors.Data.PointCloud;

namespace UnitySensors.Sensor.LiDAR
{
    public abstract class LiDARSensor<T> : UnitySensor, IPointCloudInterface<T>
        where T : struct, IPointXYZInterface
    {
        [SerializeField]
        private ScanPattern _scanPattern;
        [SerializeField]
        private int _pointsNumPerScan = 1;
        [SerializeField]
        private float _minRange = 0.5f;
        [SerializeField]
        private float _maxRange = 100.0f;
        [SerializeField]
        private float _gaussianNoiseSigma = 0.0f;
        [SerializeField]
        private float _maxIntensity = 255.0f;

        private PointCloud<T> _pointCloud;

        protected ScanPattern scanPattern { get => _scanPattern; }
        protected float minRange { get => _minRange; }
        protected float maxRange { get => _maxRange; }
        protected float gaussianNoiseSigma { get => _gaussianNoiseSigma; }
        protected float maxIntensity { get => _maxIntensity; }
        public PointCloud<T> pointCloud { get => _pointCloud; }
        public int pointsNum { get => _pointsNumPerScan; }

        protected override void Init()
        {
            _pointsNumPerScan = Mathf.Clamp(_pointsNumPerScan, 1, scanPattern.size);
            _pointCloud = new PointCloud<T>()
            {
                points = new NativeArray<T>(_pointsNumPerScan, Allocator.Persistent)
            };
        }

        protected override void OnSensorDestroy()
        {
            _pointCloud.Dispose();
        }
    }
}

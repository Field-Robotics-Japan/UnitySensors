using UnityEngine;
using UnitySensors.Interface.Std;
using System.Runtime.CompilerServices;
using System;

[assembly: InternalsVisibleTo("UnitySensorsEditor")]
[assembly: InternalsVisibleTo("UnitySensorsROSEditor")]
namespace UnitySensors.Sensor
{
    public abstract class UnitySensor : MonoBehaviour, ITimeInterface
    {
        [SerializeField, Min(0)]
        internal float _frequency = 10.0f;
        private static int _sensor_count = 0;
        private float _time;
        private float _dt;
        private float _frequency_inv;
        private int _sensor_id;

        public Action onSensorUpdateComplete;
        public float dt { get => _frequency_inv; }
        public float time { get => _time; }
        public float frequency
        {
            get => _frequency;
            set
            {
                _frequency = Mathf.Max(value, 0);
                _frequency_inv = 1.0f / _frequency;
                InitializeSensorOffset();
            }
        }

        private void Awake()
        {
            _frequency_inv = 1.0f / _frequency;

            _sensor_id = _sensor_count;
            _sensor_count++;

            InitializeSensorOffset();

            Init();
        }

        private void InitializeSensorOffset()
        {
            string sensorType = GetType().Name;
            int typeHash = sensorType.GetHashCode();

            // Combine sensor ID and type to create a more dispersed value
            // Use coprime numbers and operations to increase dispersion
            float seed = (_sensor_id * 16777619 + typeHash) * 0.618033988749895f;

            // Ensure the offset is in [0, 1)
            float normalizedOffset = seed % 1.0f;
            if (normalizedOffset < 0) normalizedOffset += 1.0f; // Ensure non-negative

            _dt = normalizedOffset * _frequency_inv;

            // Debug.Log($"Sensor {GetType().Name} ID:{_sensor_id} initialized with offset {normalizedOffset:F3} ({_dt:F3}s)");
        }
        protected virtual void Update()
        {
            _dt += Time.deltaTime;
            if (_dt < _frequency_inv) return;

            _time = Time.time;
            UpdateSensor();
            onSensorUpdateComplete?.Invoke();

            _dt -= _frequency_inv;
        }

        private void OnDestroy()
        {
            OnSensorDestroy();
        }

        public void UpdateSensorManually()
        {
            UpdateSensor();
            onSensorUpdateComplete?.Invoke();
        }

        protected abstract void Init();
        protected abstract void UpdateSensor();
        protected abstract void OnSensorDestroy();
    }
}

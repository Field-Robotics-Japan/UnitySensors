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

        private float _time;
        private float _dt;
        private float _frequency_inv;

        public Action onSensorUpdated;
        public float dt { get => _frequency_inv; }
        public float time { get => _time; }
        public float frequency
        {
            get => _frequency;
            set
            {
                _frequency = Mathf.Max(value, 0);
                _frequency_inv = 1.0f / _frequency;
            }
        }

        private void Awake()
        {
            _dt = 0.0f;
            _frequency_inv = 1.0f / _frequency;

            Init();
        }

        protected virtual void Update()
        {
            _dt += Time.deltaTime;
            if (_dt < _frequency_inv) return;

            _time = Time.time;
            UpdateSensor();

            _dt -= _frequency_inv;
        }

        private void OnDestroy()
        {
            onSensorUpdated = null;
            OnSensorDestroy();
        }

        public void UpdateSensorManually()
        {
            UpdateSensor();
        }

        protected abstract void Init();
        protected abstract void UpdateSensor();
        protected abstract void OnSensorDestroy();
    }
}

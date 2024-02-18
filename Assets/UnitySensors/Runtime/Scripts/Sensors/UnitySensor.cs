using UnityEngine;
using UnitySensors.Interface.Std;

namespace UnitySensors.Sensor
{
    public abstract class UnitySensor : MonoBehaviour, ITimeInterface
    {
        [SerializeField]
        private float _frequency = 10.0f;

        private float _time;
        private float _dt;

        public delegate void OnSensorUpdated();
        public OnSensorUpdated onSensorUpdated;


        private float _frequency_inv;

        public float dt { get => _frequency_inv; }
        public float time { get => _time; }

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

        protected abstract void Init();
        protected abstract void UpdateSensor();
        protected abstract void OnSensorDestroy();
    }
}

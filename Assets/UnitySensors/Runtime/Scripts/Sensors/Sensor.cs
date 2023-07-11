using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnitySensors
{
    public class Sensor : MonoBehaviour
    {
        [SerializeField]
        private float _frequency = 10.0f;

        protected float _time_now = 0.0f;
        private float _time_last = 0.0f;

        protected float _frequency_inv;

        protected bool _initialized = false;
        public bool initialized { get => _initialized; }

        private void Start()
        {
            _time_last = Time.time;
            _frequency_inv = 1.0f / _frequency;
            _initialized = false;
            Init();
        }

        protected virtual void Init()
        {
            _initialized = true;
        }

        private void Update()
        {
            _time_now = Time.time;
            if (_time_now - _time_last < _frequency_inv) return;
            UpdateSensor();
            _time_last = _time_now;
        }

        protected virtual void UpdateSensor()
        {

        }
    }
}

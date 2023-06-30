using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.Robotics.ROSTCPConnector;
using UnitySensors;

namespace UnitySensors.ROS
{
    /// <summary>
    /// T( : Sensor)が取得したデータをTT( : Serializer)でシリアライズし、ROSトピックとしてPubする
    /// </summary>
    public class Publisher<T, TT> : MonoBehaviour where T : Sensor where TT : Serializer, new()
    {
        [SerializeField]
        private float _frequency = 10.0f;

        protected ROSConnection _ros;
        protected T _sensor;
        [SerializeField]
        protected TT _serializer;

        protected float _time_now = 0.0f;
        private float _time_old = 0.0f;

        private float _frequency_inv;

        private void Awake()
        {
            _ros = ROSConnection.GetOrCreateInstance();
            _sensor = GetComponent<T>();
            _serializer = new TT();
        }

        protected virtual void Start()
        {
            if (!Application.isPlaying) return;
            _time_old = Time.time;
            _frequency_inv = 1.0f / _frequency;
            Init();
        }

        /// <summary>
        /// 初期化の仮想関数
        /// </summary>
        protected virtual void Init()
        {

        }

        protected virtual void Update()
        {
            if (!Application.isPlaying) return;
            _time_now = Time.time;
            if (_time_now - _time_old > _frequency_inv)
            {
                Publish(_time_now);
                _time_old = _time_now;
            }
        }

        /// <summary>
        /// ROSトピック送信の仮想関数
        /// </summary>
        protected virtual void Publish(float time)
        {

        }
    }
}

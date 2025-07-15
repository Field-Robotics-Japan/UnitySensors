using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using Unity.Robotics.ROSTCPConnector.MessageGeneration;

using UnitySensors.ROS.Serializer;

namespace UnitySensors.ROS.Publisher
{
    public class RosMsgPublisher<T, TT> : MonoBehaviour where T : RosMsgSerializer<TT> where TT : Message, new()
    {
        [SerializeField]
        private float _frequency = 10.0f;

        [SerializeField]
        protected string _topicName;

        [SerializeField]
        protected T _serializer;

        private ROSConnection _ros;
        private float _dt;
        private float _frequency_inv;

        public string topicName
        {
            get => _topicName;
            set
            {
                if (_topicName != value)
                {
                    _topicName = value;
                    if (_ros != null)
                    {
                        // Re-register the publisher with the new topic name
                        _ros.RegisterPublisher<TT>(value);
                    }
                }
            }
        }
        public float frequency
        {
            get => _frequency;
            set
            {
                if (_frequency != value)
                {
                    _frequency = value;
                    _frequency_inv = 1.0f / _frequency;
                }
            }
        }

        protected virtual void Start()
        {
            _dt = 0.0f;
            _frequency_inv = 1.0f / _frequency;

            _ros = ROSConnection.GetOrCreateInstance();
            _ros.RegisterPublisher<TT>(_topicName);

            _serializer.Init();
        }

        protected virtual void Update()
        {
            _dt += Time.deltaTime;
            if (_dt < _frequency_inv) return;

            _ros.Publish(_topicName, _serializer.Serialize());

            _dt -= _frequency_inv;
        }

        private void OnDestroy()
        {
            _serializer.OnDestroy();
        }
    }
}

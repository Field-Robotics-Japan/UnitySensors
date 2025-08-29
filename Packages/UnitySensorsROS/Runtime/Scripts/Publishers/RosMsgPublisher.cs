using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using Unity.Robotics.ROSTCPConnector.MessageGeneration;

using UnitySensors.ROS.Serializer;

namespace UnitySensors.ROS.Publisher
{
    public class RosMsgPublisher<T, TT> : MonoBehaviour where T : RosMsgSerializer<TT> where TT : Message, new()
    {
        [SerializeField, Min(0)]
        private float _frequency = 10.0f;

        [SerializeField]
        protected string _topicName;

        [SerializeField]
        protected T _serializer;

        private ROSConnection _ros;
        private float _dt;
        private float _frequency_inv;
        private RosTopicState _topicState;

        public string topicName { get => _topicName; set => _topicName = value; }
        public float frequency
        {
            get => _frequency;
            set
            {
                _frequency = Mathf.Max(value, 0);
                _frequency_inv = 1.0f / _frequency;
            }
        }

        protected virtual void Start()
        {
            _dt = 0.0f;
            _frequency_inv = 1.0f / _frequency;

            _ros = ROSConnection.GetOrCreateInstance();

            _serializer.Init();
        }

        // TODO: Use Coroutine for async publishing
        protected virtual void Update()
        {
            _dt += Time.deltaTime;
            if (_dt < _frequency_inv) return;

            // Register the publisher if it hasn't been registered yet
            _topicState = _ros.GetTopic(_topicName);
            if (_topicState == null || !_topicState.IsPublisher)
                _ros.RegisterPublisher<TT>(_topicName);

            _ros.Publish(_topicName, _serializer.Serialize());

            _dt -= _frequency_inv;
        }

        private void OnDestroy()
        {
            _serializer.OnDestroy();
        }
    }
}

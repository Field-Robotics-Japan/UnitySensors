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
        private static int _publisher_count = 0;

        private ROSConnection _ros;
        private float _dt;
        private float _frequency_inv;
        private RosTopicState _topicState;
        private int _publisher_id;

        public string topicName { get => _topicName; set => _topicName = value; }
        public float frequency
        {
            get => _frequency;
            set
            {
                _frequency = Mathf.Max(value, 0);
                _frequency_inv = 1.0f / _frequency;
                InitializePublisherOffset();
            }
        }
        private void Awake()
        {
            _frequency_inv = 1.0f / _frequency;
            _publisher_id = _publisher_count;
            _publisher_count++;

            InitializePublisherOffset();
        }

        private void InitializePublisherOffset()
        {
            string publisherType = GetType().Name;
            int typeHash = publisherType.GetHashCode();

            // Combine publisher ID and type to create a more dispersed value
            // Use coprime numbers and operations to increase dispersion
            float seed = (_publisher_id * 16777619 + typeHash) * 0.618033988749895f;

            // Ensure the offset is in [0, 1)
            float normalizedOffset = seed % 1.0f;
            if (normalizedOffset < 0) normalizedOffset += 1.0f; // Ensure non-negative

            _dt = normalizedOffset * _frequency_inv;

            // Debug.Log($"Publisher {GetType().Name} ID:{_publisher_id} initialized with offset {normalizedOffset:F3} ({_dt:F3}s)");
        }

        protected virtual void Start()
        {
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

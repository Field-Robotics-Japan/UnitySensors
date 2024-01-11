using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using Unity.Robotics.ROSTCPConnector.MessageGeneration;

using UnitySensors.Sensor;
using UnitySensors.ROS.Serializer;

namespace UnitySensors.ROS.Publisher
{
    public class RosMsgPublisher<T, TT, TTT> : MonoBehaviour where T : UnitySensor where TT : RosMsgSerializer<T, TTT> where TTT : Message, new()
    {
        [SerializeField]
        private float _frequency = 10.0f;

        [SerializeField]
        private string _topicName;

        [SerializeField]
        protected TT _serializer;

        private ROSConnection _ros;

        private float _time;
        private float _dt;

        private float _frequency_inv;

        protected virtual void Start()
        {
            _dt = 0.0f;
            _frequency_inv = 1.0f / _frequency;

            _ros = ROSConnection.GetOrCreateInstance();
            _ros.RegisterPublisher<TTT>(_topicName);

            _serializer.Init(GetComponent<T>());
        }

        protected virtual void Update()
        {
            _dt += Time.deltaTime;
            if (_dt < _frequency_inv) return;

            _ros.Publish(_topicName, _serializer.Serialize());

            _dt -= _frequency_inv;
        }
    }
}

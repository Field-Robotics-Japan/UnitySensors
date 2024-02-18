using UnityEngine;
using Unity.Robotics.ROSTCPConnector.MessageGeneration;

namespace UnitySensors.ROS.Serializer
{
    [System.Serializable]
    public abstract class RosMsgSerializer<T> where T : Message, new ()
    {
        protected T _msg;
        public T msg { get => _msg; }

        public virtual void Init(MonoBehaviour source) { _msg = new T(); }
        public abstract T Serialize();
        public virtual void OnDestroy() { }
        public abstract bool IsCompatible(MonoBehaviour source);
    }
}

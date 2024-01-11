using Unity.Robotics.ROSTCPConnector.MessageGeneration;
using RosMessageTypes.Std;


using UnitySensors.Sensor;

namespace UnitySensors.ROS.Serializer
{
    [System.Serializable]
    public abstract class RosMsgSerializer<T, TT> where T : UnitySensor where TT : Message, new()
    {
        private T _sensor;
        protected T sensor { get => _sensor; }

        protected TT _msg;

        public virtual void Init(T sensor)
        {
            _sensor = sensor;
            _msg = new TT();
        }

        public abstract TT Serialize();
    }
}

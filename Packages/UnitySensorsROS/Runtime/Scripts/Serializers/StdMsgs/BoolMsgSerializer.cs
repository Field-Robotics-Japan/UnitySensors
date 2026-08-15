using UnityEngine;
using RosMessageTypes.Std;

using UnitySensors.Attribute;
using UnitySensors.Interface.Std;

namespace UnitySensors.ROS.Serializer.Std
{
    [System.Serializable]
    public class BoolMsgSerializer : RosMsgSerializer<BoolMsg>
    {
        [SerializeField, Interface(typeof(IBoolStateInterface))]
        private Object _source;

        private IBoolStateInterface _sourceInterface;

        public override void Init()
        {
            base.Init();
            _sourceInterface = _source as IBoolStateInterface;
        }

        public override BoolMsg Serialize()
        {
            _msg.data = _sourceInterface.state;
            return _msg;
        }
    }
}

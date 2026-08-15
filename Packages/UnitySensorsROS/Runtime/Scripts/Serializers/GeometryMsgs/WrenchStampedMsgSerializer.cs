using UnityEngine;

using Unity.Robotics.ROSTCPConnector.ROSGeometry;
using RosMessageTypes.Geometry;

using UnitySensors.Attribute;
using UnitySensors.Interface.Sensor;
using UnitySensors.ROS.Serializer.Std;

namespace UnitySensors.ROS.Serializer.Geometry
{
    [System.Serializable]
    public class WrenchStampedMsgSerializer : RosMsgSerializer<WrenchStampedMsg>
    {
        [SerializeField, Interface(typeof(IWrenchInterface))]
        private Object _source;
        [SerializeField]
        private HeaderSerializer _header;

        private IWrenchInterface _sourceInterface;

        public override void Init()
        {
            base.Init();
            _header.Init();
            _sourceInterface = _source as IWrenchInterface;
        }

        public override WrenchStampedMsg Serialize()
        {
            _msg.header = _header.Serialize();
            // IWrenchInterface vectors are already in the header frame
            // (the sensor's local frame).
            _msg.wrench.force = _sourceInterface.force.To<FLU>();
            _msg.wrench.torque = _sourceInterface.torque.To<FLU>();
            return _msg;
        }
    }
}

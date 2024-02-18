using UnityEngine;
using Unity.Robotics.ROSTCPConnector.MessageGeneration;
using RosMessageTypes.Sensor;

using UnitySensors.Interface.Sensor;
using UnitySensors.ROS.Serializer.Std;

namespace UnitySensors.ROS.Serializer.Sensor
{
    [System.Serializable]
    public class CameraInfoMsgSerializer : RosMsgSerializer<CameraInfoMsg>
    {
        [SerializeField]
        private HeaderSerializer _header;

        private ICameraInterface _source;

        public override void Init(MonoBehaviour source)
        {
            base.Init(source);
            _header.Init(source);
            _source = (ICameraInterface)source;
        }

        public override CameraInfoMsg Serialize()
        {
            _msg = CameraInfoGenerator.ConstructCameraInfoMessage(_source.m_camera, _header.Serialize());
            return _msg;
        }

        public override bool IsCompatible(MonoBehaviour source)
        {
            return (_header.IsCompatible(source) && source is ICameraInterface);
        }
    }
}

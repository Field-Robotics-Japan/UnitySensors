using UnityEngine;
using Unity.Robotics.ROSTCPConnector.MessageGeneration;
using RosMessageTypes.Sensor;
using UnitySensors.Sensor.Camera;

namespace UnitySensors.ROS.Serializer.Camera
{
    [System.Serializable]
    public class CameraInfoMsgSerializer : RosMsgSerializer<CameraSensor, CameraInfoMsg>
    {
        [SerializeField]
        private HeaderSerializer _header;

        public override void Init(CameraSensor sensor)
        {
            base.Init(sensor);
            _header.Init(sensor);
        }

        public override CameraInfoMsg Serialize()
        {
            _msg = CameraInfoGenerator.ConstructCameraInfoMessage(sensor.m_camera, _header.Serialize());
            return _msg;
        }
    }
}

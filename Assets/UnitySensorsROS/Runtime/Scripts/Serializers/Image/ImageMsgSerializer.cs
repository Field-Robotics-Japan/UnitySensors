using UnityEngine;

using RosMessageTypes.Sensor;
using UnitySensors.Data.Texture;
using UnitySensors.Sensor;

namespace UnitySensors.ROS.Serializer.Image
{
    [System.Serializable]
    public class ImageMsgSerializer<T> : RosMsgSerializer<T, ImageMsg> where T : UnitySensor, ITextureInterface
    {
        [SerializeField]
        private HeaderSerializer _header;

        public override void Init(T sensor)
        {
            base.Init(sensor);
            _header.Init(sensor);

            _msg.height       = (uint)sensor.height;
            _msg.width        = (uint)sensor.width;
            _msg.encoding     = sensor.encoding;
            _msg.is_bigendian = 0;
            _msg.step         = 1 * 3 * _msg.width;
        }

        public override ImageMsg Serialize()
        {
            _msg.header = _header.Serialize();
            _msg.data = sensor.data;
            return _msg;
        }
    }
}

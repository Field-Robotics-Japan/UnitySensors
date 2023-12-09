using UnityEngine;

using RosMessageTypes.Sensor;
using UnitySensors.Data.Texture;
using UnitySensors.Sensor;

namespace UnitySensors.ROS.Serializer.Image
{
    public class ImageMsgSerializer<T> : RosMsgSerializer<T, CompressedImageMsg> where T : UnitySensor, ITextureInterface
    {
        [SerializeField]
        private HeaderSerializer _header;
        [SerializeField, Range(1, 100)]
        private int quality = 75;

        public override void Init(T sensor)
        {
            base.Init(sensor);
            _header.Init(sensor);

            _msg.format = "jpeg";
        }

        public override CompressedImageMsg Serialize()
        {
            _msg.header = _header.Serialize();
            _msg.data = sensor.texture.EncodeToJPG(quality);
            return _msg;
        }
    }
}

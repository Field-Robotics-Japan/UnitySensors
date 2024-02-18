using UnityEngine;

using RosMessageTypes.Sensor;

using UnitySensors.Interface.Sensor;
using UnitySensors.ROS.Serializer.Std;

namespace UnitySensors.ROS.Serializer.Sensor
{
    [System.Serializable]
    public class CompressedImageMsgSerializer : RosMsgSerializer<CompressedImageMsg>
    {
        private enum SourceTexture
        {
            Texture0,
            Texture1
        }

        [SerializeField]
        private SourceTexture _sourceTexture;

        [SerializeField]
        private HeaderSerializer _header;
        [SerializeField, Range(1, 100)]
        private int quality = 75;

        private ITextureInterface _source;

        public override void Init(MonoBehaviour source)
        {
            base.Init(source);
            _header.Init(source);
            _source = (ITextureInterface)source;
            _msg.format = "jpeg";
        }

        public override CompressedImageMsg Serialize()
        {
            _msg.header = _header.Serialize();
            _msg.data = (_sourceTexture == SourceTexture.Texture0 ? _source.texture0 : _source.texture1).EncodeToJPG(quality);
            return _msg;
        }

        public override bool IsCompatible(MonoBehaviour source)
        {
            return (_header.IsCompatible(source) && source is ITextureInterface);
        }
    }
}

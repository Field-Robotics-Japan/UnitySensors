using UnityEngine;

using RosMessageTypes.Sensor;

using UnitySensors.Attribute;
using UnitySensors.Interface.Sensor;
using UnitySensors.ROS.Serializer.Std;

namespace UnitySensors.ROS.Serializer.Sensor
{
    [System.Serializable]
    public class ImageMsgSerializer : RosMsgSerializer<ImageMsg>
    {
        private enum SourceTexture
        {
            Texture0,
            Texture1
        }

        [SerializeField, Interface(typeof(ITextureInterface))]
        private Object _source;
        [SerializeField]
        private SourceTexture _sourceTexture;

        [SerializeField]
        private HeaderSerializer _header;
        [SerializeField, Range(1, 100)]
        private int quality = 75;

        private ITextureInterface _sourceInterface;

        public override void Init()
        {
            base.Init();
            _header.Init();
            _sourceInterface = _source as ITextureInterface;
            // _msg.format = "jpeg";
            // FIXME: Depth image should not be compressed as jpeg.
            //        It is normally encoded as 32FC1 and 16UC1, whose units are meters and millimeters respectively.
            //        So we need a new serializer for depth images.
        }

        public override ImageMsg Serialize()
        {
            _msg.header = _header.Serialize();
            _msg.data = (_sourceTexture == SourceTexture.Texture0 ? _sourceInterface.texture0 : _sourceInterface.texture1).EncodeToJPG(quality);
            return _msg;
        }
    }
}

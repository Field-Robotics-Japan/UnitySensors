using Unity.Jobs;
using UnityEngine;

using RosMessageTypes.Sensor;

using UnitySensors.Attribute;
using UnitySensors.Interface.Sensor;
using UnitySensors.ROS.Serializer.Std;
using UnitySensors.ROS.Serializer.Image;

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
        private Encoding _encoding;
        [SerializeField, ReadOnly]
        private Texture2D _encodedTexture;

        [SerializeField]
        private HeaderSerializer _header;
        private int _width;
        private int _height;
        private ITextureInterface _sourceInterface;
        private ImageEncodeJob _imageEncodeJob;


        public override void Init()
        {
            base.Init();
            _header.Init();
            _sourceInterface = _source as ITextureInterface;

            var texture = _sourceTexture == SourceTexture.Texture0 ? _sourceInterface.texture0 : _sourceInterface.texture1;

            if (texture.format != TextureFormat.RGBAFloat && texture.format != TextureFormat.RGBA32)
            {
                Debug.LogError("ImageMsgSerializer: Source texture format must be RGBAFloat or RGBA32. Current format: " + texture.format);
                return;
            }

            _width = texture.width;
            _height = texture.height;

            uint bytesPerPixel;
            TextureFormat textureFormat;

            _imageEncodeJob = new()
            {
                width = _width,
                height = _height,
                encoding = _encoding,
                sourceTextureRawDataColorRGBAF = texture.GetRawTextureData<Color>(),
                sourceTextureRawDataColorRGBA32 = texture.GetRawTextureData<ColorRGBA32>()
            };

            switch (_encoding)
            {
                case Encoding._32FC1:
                    _msg.encoding = "32FC1";
                    bytesPerPixel = 1 * 4;
                    textureFormat = TextureFormat.RFloat;
                    _imageEncodeJob.distanceFactor = _sourceInterface.texture0FarClipPlane;

                    break;
                case Encoding._16UC1:
                    _msg.encoding = "16UC1";
                    bytesPerPixel = 1 * 2;
                    textureFormat = TextureFormat.R16;
                    _imageEncodeJob.distanceFactor = _sourceInterface.texture0FarClipPlane * 1000;
                    break;
                case Encoding._RGB8:
                default:
                    _msg.encoding = "rgb8";
                    bytesPerPixel = 3 * 1;
                    textureFormat = TextureFormat.RGB24;
                    _imageEncodeJob.distanceFactor = 1;
                    break;
            }

            _msg.is_bigendian = 0;
            _msg.width = (uint)_width;
            _msg.height = (uint)_height;
            _msg.step = (uint)(bytesPerPixel * _width);
            _msg.data = new byte[_msg.step * _height];

            _imageEncodeJob.bytesPerPixel = (int)bytesPerPixel;

            _encodedTexture = new Texture2D(_width, _height, textureFormat, false);
        }

        public override ImageMsg Serialize()
        {
            _msg.header = _header.Serialize();

            _imageEncodeJob.targetTextureRawData = _encodedTexture.GetRawTextureData<byte>();
            _imageEncodeJob.Schedule(_width * _height, 1024).Complete();

            _encodedTexture.Apply();

            // Manually copy the data to the message to avoid GC allocation
            _encodedTexture.GetRawTextureData<byte>().CopyTo(_msg.data);
            return _msg;
        }
    }
}

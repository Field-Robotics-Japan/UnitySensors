using UnityEngine;

using RosMessageTypes.Sensor;

using UnitySensors.Attribute;
using UnitySensors.Interface.Sensor;
using UnitySensors.ROS.Serializer.Std;
using UnitySensors.ROS.Utils.Image;
using Unity.Jobs;
using Unity.Collections;

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
        [SerializeField, Attribute.ReadOnly]
        private Texture2D _encodedTexture;

        [SerializeField]
        private HeaderSerializer _header;

        private ITextureInterface _sourceInterface;
        private ImageEncodeJob _imageEncodeJob;


        public override void Init()
        {
            base.Init();
            _header.Init();
            _sourceInterface = _source as ITextureInterface;

            var texture = _sourceTexture == SourceTexture.Texture0 ? _sourceInterface.texture0 : _sourceInterface.texture1;

            if (texture.format != TextureFormat.RGBAFloat)
            {
                Debug.LogError("ImageMsgSerializer: Source texture format must be RGBAFloat");
                return;
            }

            int width = texture.width;
            int height = texture.height;

            uint bytesPerPixel;
            TextureFormat textureFormat;
            switch (_encoding)
            {
                case Encoding._32FC1:
                    _msg.encoding = "32FC1";
                    bytesPerPixel = 1 * 4;
                    textureFormat = TextureFormat.RFloat;
                    float distanceFactor = _sourceInterface.texture0FarClipPlane;
                    _imageEncodeJob = new()
                    {
                        width = width,
                        height = height,
                        distanceFactor = distanceFactor,
                        encoding = _encoding,
                        targetTexture16UC1 = new NativeArray<Color16UC1>(0, Allocator.Persistent),
                        targetTextureRGB8 = new NativeArray<ColorRGB8>(0, Allocator.Persistent)
                    };
                    break;
                case Encoding._16UC1:
                    _msg.encoding = "16UC1";
                    bytesPerPixel = 1 * 2;
                    textureFormat = TextureFormat.R16;
                    distanceFactor = _sourceInterface.texture0FarClipPlane * 1000;
                    _imageEncodeJob = new()
                    {
                        width = width,
                        height = height,
                        distanceFactor = distanceFactor,
                        encoding = _encoding,
                        targetTexture32FC1 = new NativeArray<Color32FC1>(0, Allocator.Persistent),
                        targetTextureRGB8 = new NativeArray<ColorRGB8>(0, Allocator.Persistent)
                    };
                    break;
                case Encoding._RGB8:
                default:
                    _msg.encoding = "rgb8";
                    bytesPerPixel = 3 * 1;
                    textureFormat = TextureFormat.RGB24;
                    distanceFactor = 1;
                    _imageEncodeJob = new()
                    {
                        width = width,
                        height = height,
                        distanceFactor = distanceFactor,
                        encoding = _encoding,
                        targetTexture32FC1 = new NativeArray<Color32FC1>(0, Allocator.Persistent),
                        targetTexture16UC1 = new NativeArray<Color16UC1>(0, Allocator.Persistent),
                    };
                    break;
            }

            _msg.is_bigendian = 0;
            _msg.width = (uint)width;
            _msg.height = (uint)height;
            _msg.step = (uint)(bytesPerPixel * width);
            _msg.data = new byte[_msg.step * height];

            _encodedTexture = new Texture2D(width, height, textureFormat, false);

        }

        public override ImageMsg Serialize()
        {
            _msg.header = _header.Serialize();
            var texture = _sourceTexture == SourceTexture.Texture0 ? _sourceInterface.texture0 : _sourceInterface.texture1;

            _imageEncodeJob.sourceTextureRawData = texture.GetRawTextureData<Color>();

            switch (_encoding)
            {
                case Encoding._32FC1:
                    _imageEncodeJob.targetTexture32FC1 = _encodedTexture.GetRawTextureData<Color32FC1>();
                    break;
                case Encoding._16UC1:
                    _imageEncodeJob.targetTexture16UC1 = _encodedTexture.GetRawTextureData<Color16UC1>();
                    break;
                case Encoding._RGB8:
                default:
                    _imageEncodeJob.targetTextureRGB8 = _encodedTexture.GetRawTextureData<ColorRGB8>();
                    break;
            }

            _imageEncodeJob.Schedule(texture.width * texture.height, 1024).Complete();

            _encodedTexture.Apply();

            // Manually copy the data to the message to avoid GC allocation
            _encodedTexture.GetRawTextureData<byte>().CopyTo(_msg.data);
            return _msg;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            _imageEncodeJob.targetTexture32FC1.Dispose();
            _imageEncodeJob.targetTexture16UC1.Dispose();
            _imageEncodeJob.targetTextureRGB8.Dispose();
        }
    }
}

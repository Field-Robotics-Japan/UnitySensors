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
        private enum Encoding
        {
            _RGB8,
            _32FC1,
            _16UC1
        }
        private struct ColorRGB24
        {
            public byte r;
            public byte g;
            public byte b;
        }
        private struct Color32FC1
        {
            public float r;
        }
        private struct Color16UC1
        {
            public ushort r;
        }

        [SerializeField, Interface(typeof(ITextureInterface))]
        private Object _source;
        [SerializeField]
        private SourceTexture _sourceTexture;
        [SerializeField]
        private Encoding _encoding;

        [SerializeField]
        private HeaderSerializer _header;

        private ITextureInterface _sourceInterface;
        private Texture2D _flippedTexture;
        private float distanceFactor;

        private Color[] _sourcePixels;
        private ColorRGB24[] _targetPixelsRGB24;
        private Color32FC1[] _targetPixels32FC1;
        private Color16UC1[] _targetPixels16UC1;

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
                    distanceFactor = _sourceInterface.texture0FarClipPlane;
                    _targetPixels32FC1 = new Color32FC1[width * height];
                    break;
                case Encoding._16UC1:
                    _msg.encoding = "16UC1";
                    bytesPerPixel = 1 * 2;
                    textureFormat = TextureFormat.R16;
                    distanceFactor = _sourceInterface.texture0FarClipPlane * 1000;
                    _targetPixels16UC1 = new Color16UC1[width * height];
                    break;
                case Encoding._RGB8:
                default:
                    _msg.encoding = "rgb8";
                    bytesPerPixel = 3 * 1;
                    textureFormat = TextureFormat.RGB24;
                    distanceFactor = 1;
                    _targetPixelsRGB24 = new ColorRGB24[width * height];
                    break;
            }
            _sourcePixels = new Color[width * height];

            _msg.is_bigendian = 0;
            _msg.width = (uint)width;
            _msg.height = (uint)height;
            _msg.step = (uint)(bytesPerPixel * width);
            _msg.data = new byte[_msg.step * height];

            _flippedTexture = new Texture2D(width, height, textureFormat, false);

        }

        public override ImageMsg Serialize()
        {
            _msg.header = _header.Serialize();
            var texture = _sourceTexture == SourceTexture.Texture0 ? _sourceInterface.texture0 : _sourceInterface.texture1;

            FlipTextureVertically(texture, _flippedTexture);

            // Manually copy the data to the message to avoid GC allocation
            _flippedTexture.GetRawTextureData<byte>().CopyTo(_msg.data);
            return _msg;
        }
        private void FlipTextureVertically(Texture2D sourceTexture, Texture2D targetTexture)
        {
            // TODO: Use shader or jobs to flip the texture
            int width = sourceTexture.width;
            int height = sourceTexture.height;

            // Manually copy the data to the message to avoid GC allocation
            sourceTexture.GetPixelData<Color>(0).CopyTo(_sourcePixels);

            switch (_encoding)
            {
                case Encoding._32FC1:
                    for (int j = 0; j < height; j++)
                    {
                        for (int i = 0; i < width; i++)
                        {
                            _targetPixels32FC1[j * width + i].r = _sourcePixels[(height - j - 1) * width + i].r * distanceFactor;
                        }
                    }
                    targetTexture.SetPixelData(_targetPixels32FC1, 0);
                    break;
                case Encoding._16UC1:
                    for (int j = 0; j < height; j++)
                    {
                        for (int i = 0; i < width; i++)
                        {
                            _targetPixels16UC1[j * width + i].r = (ushort)(_sourcePixels[(height - j - 1) * width + i].r * distanceFactor);
                        }
                    }
                    targetTexture.SetPixelData(_targetPixels16UC1, 0);
                    break;
                case Encoding._RGB8:
                default:
                    for (int j = 0; j < height; j++)
                    {
                        for (int i = 0; i < width; i++)
                        {
                            _targetPixelsRGB24[j * width + i].r = (byte)(_sourcePixels[(height - j - 1) * width + i].r * 255);
                            _targetPixelsRGB24[j * width + i].g = (byte)(_sourcePixels[(height - j - 1) * width + i].g * 255);
                            _targetPixelsRGB24[j * width + i].b = (byte)(_sourcePixels[(height - j - 1) * width + i].b * 255);
                        }
                    }
                    targetTexture.SetPixelData(_targetPixelsRGB24, 0);
                    break;
            }
            targetTexture.Apply();
        }
    }
}

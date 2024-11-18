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
            // TODO: Support 16UC1 encoding
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
        private Color[] _pixels;
        private Color[] _pixelsRow;
        private float distanceFactor;

        public override void Init()
        {
            base.Init();
            _header.Init();
            _sourceInterface = _source as ITextureInterface;
            // FIXME: Depth image should not be compressed as jpeg.
            //        It is normally encoded as 32FC1 and 16UC1, whose units are meters and millimeters respectively.
            //        So we need a new serializer for depth images.

            var texture = _sourceTexture == SourceTexture.Texture0 ? _sourceInterface.texture0 : _sourceInterface.texture1;
            Debug.Log("FarClipPlane: " + _sourceInterface.texture0FarClipPlane);

            uint bytesPerPixel;
            TextureFormat textureFormat;
            switch (_encoding)
            {
                case Encoding._32FC1:
                    _msg.encoding = "32FC1";
                    bytesPerPixel = 1 * 4;
                    textureFormat = TextureFormat.RFloat;
                    distanceFactor = _sourceInterface.texture0FarClipPlane;
                    break;
                case Encoding._RGB8:
                default:
                    _msg.encoding = "rgb8";
                    bytesPerPixel = 3 * 1;
                    textureFormat = TextureFormat.RGB24;
                    distanceFactor = 1.0f;
                    break;
            }

            _msg.is_bigendian = 0;
            _msg.width = (uint)texture.width;
            _msg.height = (uint)texture.height;
            _msg.step = bytesPerPixel * _msg.width;
            _msg.data = new byte[_msg.step * _msg.height];

            _flippedTexture = new Texture2D(texture.width, texture.height, textureFormat, false);

            _pixels = new Color[texture.width * texture.height];
            _pixelsRow = new Color[texture.width];
        }

        public override ImageMsg Serialize()
        {
            _msg.header = _header.Serialize();
            // FIXME: The image is upside down.
            var texture = _sourceTexture == SourceTexture.Texture0 ? _sourceInterface.texture0 : _sourceInterface.texture1;
            FlipTextureVertically(texture, _flippedTexture);
            var byteNativeArray = _flippedTexture.GetRawTextureData<byte>();
            var colors = _flippedTexture.GetPixels();



            // Manually copy the data to the message to avoid GC allocation
            byteNativeArray.CopyTo(_msg.data);
            return _msg;
        }
        private void FlipTextureVertically(Texture2D sourceTexture, Texture2D targetTexture)
        {
            // TODO: Use a shader to flip the texture
            int width = sourceTexture.width;
            int height = sourceTexture.height;
            // var colorNativeArray = sourceTexture.GetPixelData<Color32>(0);
            // Manually copy the data to the message to avoid GC allocation
            // colorNativeArray.CopyTo(_pixels);

            // FIXME: Performance issue
            _pixels = sourceTexture.GetPixels(0);

            for (int j = 0; j < height / 2; j++)
            {
                System.Array.Copy(_pixels, j * width, _pixelsRow, 0, width);
                System.Array.Copy(_pixels, (height - j - 1) * width, _pixels, j * width, width);
                System.Array.Copy(_pixelsRow, 0, _pixels, (height - j - 1) * width, width);
            }
            for (int i = 0; i < _pixels.Length; i++)
            {
                _pixels[i] *= distanceFactor;
            }
            targetTexture.SetPixels(_pixels);
            targetTexture.Apply();
        }
    }
}

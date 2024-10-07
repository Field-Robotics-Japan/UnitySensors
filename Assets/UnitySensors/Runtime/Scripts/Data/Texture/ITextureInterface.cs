using UnityEngine;

namespace UnitySensors.Data.Texture
{
    public interface ITextureInterface
    {
        public Texture2D texture { get; }
        public byte[] data { get; }
        public int width { get; }
        public int height { get; }
        public string encoding { get; }
    }
}

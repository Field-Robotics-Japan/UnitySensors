using UnityEngine;

namespace UnitySensors.Interface.Sensor
{
    public interface ITextureInterface
    {
        public Texture2D texture0 { get; }
        public Texture2D texture1 { get; }
        public float texture0FarClipPlane { get; }

        // public byte[] data { get; }
        // public int width { get; }
        // public int height { get; }
        // public string encoding { get; }
    }
}

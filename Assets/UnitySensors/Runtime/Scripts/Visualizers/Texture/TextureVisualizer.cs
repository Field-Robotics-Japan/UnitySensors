using UnityEngine;
using UnityEngine.UI;
using UnitySensors.Sensor;
using UnitySensors.Data.Texture;

namespace UnitySensors.Visualization.Texture
{
    public class TextureVisualizer<T> : Visualizer<T> where T : UnitySensor, ITextureInterface
    {
        [SerializeField]
        private RawImage _image;

        protected override void Init()
        {
        }

        protected override void Visualize()
        {
            if (!_image) return;
            _image.texture = sensor.texture;
        }
    }
}
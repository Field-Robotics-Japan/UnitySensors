using UnityEngine;
using UnityEngine.UI;
using UnitySensors.Sensor;
using UnitySensors.Interface.Sensor;

namespace UnitySensors.Visualization.Sensor
{
    public class TextureVisualizer : Visualizer
    {
        private enum SourceTexture
        {
            Texture0,
            Texture1
        }

        [SerializeField]
        private RawImage _image;
        [SerializeField]
        private SourceTexture _sourceTexture;

        private ITextureInterface _source;

        protected override void Init(MonoBehaviour source)
        {
            Debug.Assert(source is ITextureInterface, "No compatibility between source and visualizer.", this);
            _source = (ITextureInterface)source;
        }

        protected override void Visualize()
        {
            if (!_image) return;
            _image.texture = _sourceTexture == SourceTexture.Texture0 ? _source.texture0 : _source.texture1;
        }
    }
}
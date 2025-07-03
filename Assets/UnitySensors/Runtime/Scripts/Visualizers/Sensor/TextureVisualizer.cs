using UnityEngine;
using UnityEngine.UI;
using UnitySensors.Attribute;
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

        [SerializeField, Interface(typeof(ITextureInterface))]
        private Object _source;
        [SerializeField]
        private SourceTexture _sourceTexture;
        [SerializeField]
        private RawImage _image;
        private ITextureInterface _sourceInterface;
        private void Start()
        {
            _sourceInterface = _source as ITextureInterface;
            var rectTransform = _image.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new(rectTransform.sizeDelta.x, rectTransform.sizeDelta.x * _sourceInterface.texture0.height / _sourceInterface.texture0.width);

            if (_source is UnitySensor)
            {
                (_source as UnitySensor).onSensorUpdated += Visualize;
            }

        }

        protected override void Visualize()
        {
            if (!_image) return;
            _image.texture = _sourceTexture == SourceTexture.Texture0 ? _sourceInterface.texture0 : _sourceInterface.texture1;
        }
    }
}
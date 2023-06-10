using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UnitySensors.Visualization
{
    [RequireComponent(typeof(RGBCameraSensor))]
    public class RGBCameraVisualizer : Visualizer<RGBCameraSensor>
    {
        [SerializeField]
        private RawImage _image;

        protected override void Update()
        {
            base.Update();
            VisualizeTexture();
        }

        protected override void Visualize()
        {
        }

        private void VisualizeTexture()
        {
            if (!_image || !_target.texture) return;
            _image.texture = _target.texture;
        }
    }
}

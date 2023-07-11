using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UnitySensors.Visualization
{
    [RequireComponent(typeof(DepthCameraSensor))]
    public class DepthCameraVisualizer : Visualizer<DepthCameraSensor>
    {
        [SerializeField]
        private RawImage _output;

        [SerializeField]
        private SphereSetting _point;
        [SerializeField]
        private int _maxPointNum = 1000;

        private Transform _transform;

        protected override void Update()
        {
            base.Update();
            if (_target.mode != DepthCameraSensor.DepthCamerMode.POINTCLOUD_ONLY)
                VisualizeTexture();
        }

        protected override void Visualize()
        {
            if (_target.mode == DepthCameraSensor.DepthCamerMode.TEXTURE_ONLY) return;
            if (!_transform) _transform = this.transform;

            _target.CompleteJob();
            Gizmos.color = _point.color;

            for (int i = 0; i < (_maxPointNum < _target.points.AsReadOnly().Length ? _maxPointNum : _target.points.AsReadOnly().Length); i++)
            {
                int index = (_maxPointNum < _target.points.AsReadOnly().Length ? UnityEngine.Random.Range(0, _target.points.AsReadOnly().Length) : i);
                Gizmos.DrawSphere(_transform.TransformPoint(_target.points.AsReadOnly()[index]), _point.radius);
            }
        }

        private void VisualizeTexture()
        {
            if (!_output || !_target.texture) return;
            _output.texture = _target.texture;
        }
    }
}

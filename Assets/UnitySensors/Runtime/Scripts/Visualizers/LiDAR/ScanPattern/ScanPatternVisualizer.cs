using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnitySensors.Visualization
{
    [RequireComponent(typeof(Sensor))]
    public class ScanPatternVisualizer : Visualizer<Sensor>
    {
        private enum Mode
        {
            LASER,
            POINT
        }

        [SerializeField]
        private ScanPattern _scanPattern;

        [SerializeField]
        private Mode _mode;

        [SerializeField]
        private float _range = 1.0f;
        
        [SerializeField]
        private int _drawNumPerVisualize = 1;

        [SerializeField]
        private float _duration = 0.1f;

        private Transform _transform;
        private int _counter = 0;

        protected override void Visualize()
        {
            if (!_scanPattern) return;
            if (!_scanPattern.loaded) return;

            if (!_transform) _transform = _target.transform;

            int counter_old = (_counter==0 ? _scanPattern.size - 1 : _counter - 1);
            for(int i = 0; i < _drawNumPerVisualize; i++)
            {
                Vector3 start = (_mode == Mode.LASER ? (_transform.position) : (_transform.position + _transform.TransformDirection(_scanPattern.scans[counter_old] * _range)));
                Debug.DrawLine(start, _transform.position + _transform.TransformDirection(_scanPattern.scans[_counter]) * _range, _defaultColor, _duration);
                counter_old = _counter;
                _counter++;
                if (_counter >= _scanPattern.size) _counter = 0;
            }
        }
    }
}

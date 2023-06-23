using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace UnitySensors
{
    [CreateAssetMenu]
    public class RotatingLiDARScanPattern : ScanPattern
    {
        private enum RotationDirection
        {
            CW,
            CCW
        }

        [SerializeField]
        private RotationDirection _rotationDirection;

        [SerializeField]
        private float[] _zenithAngles;

        [SerializeField]
        private int _azimuthResolution = 360;

        public int numOfLayer { get => _zenithAngles.Length; }

        public override void GenerateScanPattern()
        {
            _generated = false;

            _maxAzimuth = 180.0f;

            _size = _zenithAngles.Length * _azimuthResolution;
            _scans = new Vector3[_size];

            int index = 0;
            for(int azimuth = 0; azimuth < _azimuthResolution; azimuth++)
            {
                float azimuthAngle = 360.0f / _azimuthResolution * azimuth;
                if (_rotationDirection == RotationDirection.CCW) azimuthAngle *= -1;
                foreach(float zenithAngle in _zenithAngles)
                {
                    _scans[index] = Quaternion.Euler(-zenithAngle, azimuthAngle, 0) * Vector3.forward;
                    index++;
                }
            }

            _maxZenith = 0.0f;
            foreach (float zenithAngle in _zenithAngles)
            {
                if (Mathf.Abs(zenithAngle) > _maxZenith) _maxZenith = Mathf.Abs(zenithAngle);
            }

            _generated = true;

            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }
    }
}
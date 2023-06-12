using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace UnitySensors
{
    [CreateAssetMenu]
    public class LiDARScanPattern : ScriptableObject
    {
        [SerializeField]
        private TextAsset _file;
        [SerializeField, ReadOnly]
        private bool _loaded;
        [SerializeField, ReadOnly]
        private string _loadedFile;
        [SerializeField, ReadOnly]
        public int _row;
        [SerializeField, ReadOnly]
        public int _column;
        [SerializeField, ReadOnly]
        private float _maxAzimuth;
        [SerializeField, ReadOnly]
        private float _maxZenith;

        [SerializeField, HideInInspector]
        private Vector3[] _scans;

        public bool loaded { get => _loaded; }
        public int size { get => _row; }
        public float maxAzimuth { get => _maxAzimuth; }
        public float maxZenith { get => _maxZenith; }
        public Vector3[] scans { get => _scans; }

        public void LoadFile()
        {
            _loaded = false;
            _loadedFile = "";
            _row = 0;
            _column = 0;
            _maxAzimuth = 0.0f;
            _maxZenith = 0.0f;

            if (_file == null)
            {
                Debug.LogWarning(this.name + ": CSV file is not set.");
                return;
            }

            string fileText = _file.text;
            string[] lines = fileText.Split('\n');

            string[] headers = lines[0].Split(',');

            int azimuth_index = -1;
            int zenith_index = -1;

            for(int c = 0; c < headers.Length; c++)
            {
                string header = headers[c].ToLower();

                if (header.Contains("zenith")) zenith_index = c;
                else if (header.Contains("azimuth")) azimuth_index = c;
            }

            if(azimuth_index == -1 || zenith_index == -1)
            {
                Debug.LogWarning(this.name + ": Cannot find \"azimuth\" or \"zenith\" header.");
                return;
            }

            _scans = new Vector3[lines.Length - 2];

            for(int l = 1; l < lines.Length - 1; l++)
            {
                string[] line = lines[l].Split(',');
                
                if(line.Length != headers.Length)
                {
                    Debug.LogWarning(this.name + "Number of columns does not match.");
                    return;
                }
                
                float azimuth = float.Parse(line[azimuth_index]);
                if (Mathf.Abs(azimuth) > _maxAzimuth) _maxAzimuth = Mathf.Abs(azimuth);
                float zenith = float.Parse(line[zenith_index]) - 90;
                if (Mathf.Abs(zenith) > _maxZenith) _maxZenith = Mathf.Abs(zenith);
                _scans[l - 1] = Quaternion.Euler(zenith, azimuth, 0) * Vector3.forward;
            }

            _loaded = true;
            _loadedFile = _file.name;
            _row = lines.Length - 2;
            _column = headers.Length;

            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }
    }
}

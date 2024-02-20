using System;
using UnityEngine;
using UnityEditor;

using Unity.Mathematics;

namespace UnitySensors.DataType.LiDAR
{
#if UNITY_EDITOR
    class ScanPatternGenerator : EditorWindow
    {
        private enum Mode
        {
            FromCSV,
            FromSpecification
        }

        private Mode _mode = Mode.FromSpecification;

        private TextAsset _csvFile;

        private enum Direction
        {
            CW,
            CCW
        }

        [SerializeField]
        private Direction _direction;
        [SerializeField]
        private float[] _zenithAngles;
        [SerializeField]
        private float _minAzimuthAngle;
        [SerializeField]
        private float _maxAzimuthAngle;
        [SerializeField]
        private int _azimuthAngleResolution = 360;
        [SerializeField]
        private float _zenithAngleOffset = 0;

        private Vector2 _scrollPosition = Vector2.zero;
        private SerializedObject _so;

        [MenuItem("UnitySensors/LiDAR/Generate ScanPattern")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(ScanPatternGenerator));
        }

        private void OnEnable()
        {
            _so = new SerializedObject(this);
        }

        private void OnGUI()
        {
            GUILayout.Label("Setting", EditorStyles.boldLabel);

            EditorGUILayout.Space();

            _mode = (Mode)EditorGUILayout.EnumPopup("Source", _mode);

            switch (_mode)
            {
                case Mode.FromCSV:

                    _csvFile = EditorGUILayout.ObjectField("CSV File", _csvFile, typeof(TextAsset), true) as TextAsset;
                    _zenithAngleOffset = EditorGUILayout.FloatField("Zenith Angle Offset", _zenithAngleOffset);

                    break;

                case Mode.FromSpecification:

                    _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
                    _so.Update();
                    EditorGUILayout.PropertyField(_so.FindProperty("_zenithAngles"), true);
                    _so.ApplyModifiedProperties();
                    EditorGUILayout.EndScrollView();

                    _minAzimuthAngle = EditorGUILayout.FloatField("Min Azimuth Angle", _minAzimuthAngle);
                    _maxAzimuthAngle = EditorGUILayout.FloatField("Max Azimuth Angle", _maxAzimuthAngle);
                    _azimuthAngleResolution = EditorGUILayout.IntField("Azimuth Angle Resolution", _azimuthAngleResolution);

                    break;
            }

            EditorGUILayout.Space();

            if (GUILayout.Button("Generate ScanPattern"))
            {
                switch (_mode)
                {
                    case Mode.FromCSV:
                        GenerateFromCSV();
                        break;
                    case Mode.FromSpecification:
                        GenerateFromSpecification();
                        break;
                }
            }
        }

        private void GenerateFromCSV()
        {
            if (!_csvFile)
            {
                Debug.LogWarning(this.name + ": CSV file is not set.");
                return;
            }
            
            string fileText = _csvFile.text;
            string[] lines = fileText.Split('\n');
            string[] headers = lines[0].Split(',');

            int azimuth_index = -1;
            int zenith_index = -1;

            for (int c = 0; c < headers.Length; c++)
            {
                string header = headers[c].ToLower();
                if (header.Contains("zenith")) zenith_index = c;
                else if (header.Contains("azimuth")) azimuth_index = c;
            }

            if (azimuth_index == -1 || zenith_index == -1)
            {
                Debug.LogWarning(this.name + ": Cannot find \"azimuth\" or \"zenith\" header.");
                return;
            }

            ScanPattern scan = ScriptableObject.CreateInstance<ScanPattern>();
            scan.size = lines.Length - 2;
            scan.scans = new float3[scan.size];
            scan.minAzimuthAngle = float.MaxValue;
            scan.maxAzimuthAngle = float.MinValue;
            scan.minZenithAngle = float.MaxValue;
            scan.maxZenithAngle = float.MinValue;

            for (int l = 1; l < lines.Length - 1; l++)
            {
                string[] line = lines[l].Split(',');

                if (line.Length != headers.Length)
                {
                    Debug.LogWarning(this.name + "Number of columns does not match.");
                    return;
                }

                float azimuthAngle = float.Parse(line[azimuth_index]);
                float zenithAngle = float.Parse(line[zenith_index]) - _zenithAngleOffset;

                scan.minAzimuthAngle = Mathf.Min(scan.minAzimuthAngle, azimuthAngle);
                scan.maxAzimuthAngle = Mathf.Max(scan.maxAzimuthAngle, azimuthAngle);
                scan.minZenithAngle = Mathf.Min(scan.minZenithAngle, zenithAngle);
                scan.maxZenithAngle = Mathf.Max(scan.maxZenithAngle, zenithAngle);
                
                scan.scans[l - 1] = Quaternion.Euler(zenithAngle, azimuthAngle, 0) * Vector3.forward;
            }

            AssetDatabase.CreateAsset(scan, "Assets/NewScanPattern.asset");
            AssetDatabase.SaveAssets();
        }

        private void GenerateFromSpecification()
        {
            if (_zenithAngles == null || _zenithAngles.Length == 0 || _azimuthAngleResolution <= 0) return;

            ScanPattern scan = ScriptableObject.CreateInstance<ScanPattern>();

            scan.size = _zenithAngles.Length * _azimuthAngleResolution;
            scan.scans = new float3[scan.size];

            int index = 0;
            for (int azimuth = 0; azimuth < _azimuthAngleResolution; azimuth++)
            {
                float azimuthAngle = Mathf.Lerp(_minAzimuthAngle, _maxAzimuthAngle, (float)(_direction == Direction.CW ? azimuth : _azimuthAngleResolution - 1 - azimuth) / _azimuthAngleResolution);
                foreach (float zenithAngle in _zenithAngles)
                {
                    scan.scans[index] = Quaternion.Euler(-zenithAngle, azimuthAngle, 0) * Vector3.forward;
                    index++;
                }
            }

            scan.minAzimuthAngle = _minAzimuthAngle;
            scan.maxAzimuthAngle = _maxAzimuthAngle;

            scan.minZenithAngle = float.MaxValue;
            scan.maxZenithAngle = float.MinValue;
            foreach (float zenithAngle in _zenithAngles)
            {
                scan.minZenithAngle = Mathf.Min(scan.minZenithAngle, zenithAngle);
                scan.maxZenithAngle = Mathf.Max(scan.maxZenithAngle, zenithAngle);
            }

            AssetDatabase.CreateAsset(scan, "Assets/NewScanPattern.asset");
            AssetDatabase.SaveAssets();
        }
    }
#endif
}
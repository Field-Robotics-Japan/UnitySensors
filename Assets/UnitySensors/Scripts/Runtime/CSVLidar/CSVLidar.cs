using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Jobs;
using Unity.Jobs;
using Random = Unity.Mathematics.Random;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace FRJ.Sensor
{
    [System.Serializable]
    public class CSVLidar : MonoBehaviour
    {
        [System.Serializable]
        public class ScanPattern
        {
            public float Time;
            public float Azimuth;
            public float Zenith;
        }

        ScanPattern[] _scanPatterns;

        [Header("Parameters(CSV)")]
        [SerializeField] private TextAsset _dataFile;
        [SerializeField] private float _csvLoadingTimeout = 1.0f;

        [Header("Parameters(LiDAR)")]
        [SerializeField] public int numOfLasersPerScan = 100;
        // Minimum range (m)
        [SerializeField] private float _minRange = 0.1f;
        // Maximum range (m)
        [SerializeField] public float maxRange = 100f;
        // Maximum intensity ()
        [SerializeField] private byte _maxIntensity = 255;
        // Scanning rate (Hz)
        [SerializeField] public float scanRate = 20f;
        // Random seed
        [HideInInspector] public uint randomSeed = 1;
        // Gaussian noise sigma
        [SerializeField] private float _gaussianNoiseSigma = 0.01f;

        #region Informations
        [Header("Informations(No need to input)")]
        public bool isInitialized;
        [HideInInspector] public string csvFilePath;
        private bool _csvLoaded;
        private int _csvLength;
        private int _loadedLine;
        #endregion

#if UNITY_EDITOR
        [Header("Debug")]
        [SerializeField] private bool _debug_drawScanPattern = false;
        [SerializeField] private float _debug_drawScanPattern_distance = 1.0f;
        [SerializeField] private float _debug_drawScanPattern_duration = 0.1f;
        [SerializeField] private Color _debug_drawScanPattern_color = Color.white;
#endif

        #region NativeArray
        // Raycast command
        public NativeArray<RaycastCommand> commands;
        // Raycast direction vectors
        private Vector3[] _commandDirVecs;
        // Raycast results
        public NativeArray<RaycastHit> results;
        // Ray origin
        public NativeArray<Vector3> origin_pos;
        public NativeArray<Quaternion> origin_rot;
        // Ray hit point
        public NativeArray<Vector3> point;
        // Intensity data
        public NativeArray<byte> intensities;
        #endregion

        public Vector3[] commandDirVecs { get => this._commandDirVecs; }

        [Header("Job")]
        // Update distance and intensity data job
        public UpdateData job;

        void Awake()
        {
            isInitialized = false;
        }

        public void Init()
        {
            isInitialized = false;
            StartCoroutine(LoadCSV());

            // allocate commands
            this.commands = new NativeArray<RaycastCommand>(numOfLasersPerScan, Allocator.Persistent);
            this._commandDirVecs = new Vector3[numOfLasersPerScan];

            // setup raycast results
            this.results = new NativeArray<RaycastHit>(numOfLasersPerScan, Allocator.Persistent);

            // setup float native array
            this.origin_pos = new NativeArray<Vector3>(numOfLasersPerScan, Allocator.Persistent);
            this.origin_rot = new NativeArray<Quaternion>(numOfLasersPerScan, Allocator.Persistent);
            this.point = new NativeArray<Vector3>(numOfLasersPerScan, Allocator.Persistent);
            this.intensities = new NativeArray<byte>(numOfLasersPerScan, Allocator.Persistent);
            for (int i = 0; i < numOfLasersPerScan; i++)
            {
                this.intensities[i] = 255;
            }

            // Distance Parallel Job settings
            this.job = new UpdateData()
            {
                results = this.results,
                origin_pos = this.origin_pos,
                origin_rot = this.origin_rot,
                point = this.point,
                intensities = this.intensities
            };
            // Update parameter
            this.job.minRange = this._minRange;
            this.job.maxRange = this.maxRange;
            this.job.maxIntensity = this._maxIntensity;
            this.job.random = new Random(this.randomSeed);
            this.job.sigma = this._gaussianNoiseSigma;

            isInitialized = true;
        }

        IEnumerator LoadCSV()
        {
            float startTime = Time.time;
            _csvLoaded = false;
            _dataFile = null;
            Addressables.LoadAssetAsync<TextAsset>(csvFilePath).Completed += op =>
            {
                _dataFile = op.Result;
            };
            do
            {
                if (Time.time - startTime > _csvLoadingTimeout)
                {
                    Debug.Log("Failed to Load CSV");
                    yield break;
                }
                yield return null;
            } while (_dataFile == null);
            _scanPatterns = CSVSerializer.Deserialize<ScanPattern>(_dataFile.text);
            _csvLength = _scanPatterns.Length;
            _csvLoaded = true;
            yield break;
        }

#if UNITY_EDITOR
        private void Update()
        {
            if (_debug_drawScanPattern)
            {
                for(int i = 0; i < numOfLasersPerScan - 1; i++)
                {
                    Debug.DrawLine( this.transform.position + this.transform.rotation * this._commandDirVecs[i]     * _debug_drawScanPattern_distance,
                                    this.transform.position + this.transform.rotation * this._commandDirVecs[i+1]   * _debug_drawScanPattern_distance,
                                    _debug_drawScanPattern_color,
                                    _debug_drawScanPattern_duration
                                    );
                }
            }
        }
#endif

        public void UpdateCommandDirVecs()
        {
            for(int i = 0; i < numOfLasersPerScan; i++)
            {
                int j = (i + _loadedLine)%_csvLength;
                this._commandDirVecs[i] = Quaternion.Euler(90 - _scanPatterns[j].Zenith, _scanPatterns[j].Azimuth, 0) * Vector3.forward;
            }
            _loadedLine = (_loadedLine + numOfLasersPerScan) % _csvLength;
        }

        public void Dispose()
        {
            this.commands.Dispose();
            this.results.Dispose();
            this.intensities.Dispose();
            this.origin_pos.Dispose();
            this.origin_rot.Dispose();
            this.point.Dispose();
        }

        [BurstCompile]
        public struct UpdateData : IJobParallelFor
        {
            [ReadOnly] public NativeArray<RaycastHit> results;
            public NativeArray<byte> intensities;

            public NativeArray<Vector3> origin_pos;
            public NativeArray<Quaternion> origin_rot;
            public NativeArray<Vector3> point;

            [ReadOnly] public float minRange;
            [ReadOnly] public float maxRange;

            [ReadOnly] public float maxIntensity;

            public Random random;
            public float sigma;

            void IJobParallelFor.Execute(int index)
            {
                // Gaussian Noise part
                var rand2 = random.NextFloat();
                var rand3 = random.NextFloat();
                float normrand =
                    (float)Math.Sqrt(-2.0f * Math.Log(rand2)) *
                    (float)Math.Cos(2.0f * Math.PI * rand3);
                normrand *= sigma;

                point[index] = Quaternion.Inverse(origin_rot[index]) * new Vector3(+results[index].point.z - origin_pos[index].z,
                                                                                   +results[index].point.y - origin_pos[index].y,
                                                                                   -results[index].point.x + origin_pos[index].x);

                if (results[index].distance < minRange)
                {
                    intensities[index] = 0;
                }
                else if (results[index].distance > maxRange)
                {
                    intensities[index] = 0;
                }
                else
                {
                    intensities[index] = (byte)(maxIntensity * minRange * minRange / (results[index].distance * results[index].distance));
                }
            }
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Jobs;
using Unity.Jobs;
using Random = Unity.Mathematics.Random;

namespace FRJ.Sensor
{
    public class RotateLidar : MonoBehaviour
    {
        private enum RotateDir {
            CW,
            CCW
        }
        // Rotate Direction on Y unity axis
        [SerializeField] RotateDir _rotateDir              = RotateDir.CCW;
        // Number of Layers
        [SerializeField] private int   _numOfLayers        = 16;
        // Number of Increments for one rotation 
        [SerializeField] private int   _numOfIncrements    = 3600;
        // Minimum vertical angle (deg)
        [SerializeField] private float _minVerticalAngle   = -15f;
        // Maximum vertical angle (deg)
        [SerializeField] private float _maxVerticalAngle   = 15f;
        // Minimum azimuth angle (deg)
        [SerializeField] private float _minAzimuthAngle    = 0f;
        // Maximum azimuth angle (deg)
        [SerializeField] private float _maxAzimuthAngle    = 360f;
        // Minimum range (m)
        [SerializeField] private float _minRange           = 0.1f;
        // Maximum range (m)
        [SerializeField] private float _maxRange           = 100f;
        // Maximum intensity ()
        [SerializeField] private float _maxIntensity       = 255f;
        // Scanning rate (Hz)
        [SerializeField] private float _scanRate           = 20f;
        // Random seed
        [SerializeField] private uint _randomSeed          = 1;
        // Offset noise
        [SerializeField] private float _offsetNoise        = 0f;
        // Gaussian Noise sigma
        [SerializeField] private float _gaussianNoiseSigma = 0.01f;

        public int numOfLayers { get => this._numOfLayers; }
        public int numOfIncrements { get => this._numOfIncrements; }
        public float minAzimuthAngle { get => this._minAzimuthAngle; }
        public float maxAzimuthAngle { get => this._maxAzimuthAngle; }
        public float minRange { get => this._minRange; }
        public float maxRange { get => this._maxRange; }
        public float maxIntensity { get => this._maxIntensity; }
        public float scanRate { get => this._scanRate; }
        public uint randomSeed { get => this._randomSeed; set {this._randomSeed = value; }}

        // Raycast command
        public NativeArray<RaycastCommand> commands;
        // Raycast direction vectors
        private Vector3[] _commandDirVecs;
        private NativeArray<Vector3> commandDirVecsNative;
        // Raycast results
        public NativeArray<RaycastHit> results;
        // Distance data
        public NativeArray<float> distances;
        // Intensity data
        public NativeArray<float> intensities;

        public Vector3[] commandDirVecs        { get => this._commandDirVecs; }

        // Update distance and intensity data job
        public updateData job;

        public void Init()
        {
            // allocate commands
            this.commands = new NativeArray<RaycastCommand>(this._numOfLayers*this._numOfIncrements, Allocator.Persistent);
            this._commandDirVecs = new Vector3[this.commands.Length];
            this.commandDirVecsNative = new NativeArray<Vector3>(this._numOfLayers * this._numOfIncrements, Allocator.Persistent);
            float vinc;
            if(this._numOfLayers == 1)
                vinc = 0;
            else
                vinc = (float)(this._maxVerticalAngle - this._minVerticalAngle) / (float)this._numOfLayers;
            float ainc;
            if(this._numOfIncrements == 1)
                ainc = 0;
            else
                ainc = (float)(this._maxAzimuthAngle - this._minAzimuthAngle) / (float)this._numOfIncrements;
            int index = 0;
            float vangle, aangle;
            for (int incr = 0; incr < this._numOfIncrements; incr++)
            {
                for (int layer = 0; layer < this._numOfLayers; layer++)
                {
                    index = layer + incr * this._numOfLayers;
                    vangle = this._minVerticalAngle + (float)layer * vinc;
                    aangle = this._minAzimuthAngle + (float)incr * ainc;
                    switch(this._rotateDir)
                    {
                        case RotateDir.CCW:
                            this._commandDirVecs[index] = Quaternion.Euler(-vangle,-aangle,0)*Vector3.forward;
                            break;
                        case RotateDir.CW:
                            this._commandDirVecs[index] = Quaternion.Euler(-vangle,aangle,0)*Vector3.forward;
                            break;
                        default:
                            break;
                    }
                    this.commands[index] = new RaycastCommand(
                                                              this.transform.position,
                                                              this.transform.rotation * this._commandDirVecs[index],
                                                              this._maxRange);
                    this.commandDirVecsNative[index] = this.transform.rotation * this._commandDirVecs[index];
                }
            }
    
            // setup raycast results
            this.results = new NativeArray<RaycastHit>(this._numOfLayers*this._numOfIncrements, Allocator.Persistent);

            // setup float native array
            this.distances = new NativeArray<float>(this._numOfLayers*this._numOfIncrements, Allocator.Persistent);
            this.intensities = new NativeArray<float>(this._numOfLayers*this._numOfIncrements, Allocator.Persistent);
            for(int i=0; i< this._numOfLayers*this._numOfIncrements; i++)
            {
                this.distances[i] = this._maxRange;
                this.intensities[i] = 255;
            }

            // Distance Parallel Job settings
            this.job = new updateData()
                {
                    results     = this.results,
                    distances   = this.distances,
                    intensities = this.intensities
                };
            // Update parameter
            this.job.commandDirVecs = this.commandDirVecsNative;
            this.job.minRange = this._minRange;
            this.job.maxRange = this._maxRange;
            this.job.maxIntensity = this._maxIntensity;
            this.job.random   = new Random(this._randomSeed);
            this.job.sigma    = this._gaussianNoiseSigma;
            this.job.offset   = this._offsetNoise;   
        }

        public void Dispose()
        {
            this.commands.Dispose();
            this.commandDirVecsNative.Dispose();
            this.results.Dispose();
            this.distances.Dispose();
            this.intensities.Dispose();
        }
  
        [BurstCompile]
        public struct updateData : IJobParallelFor
        {
            public NativeArray<Vector3> commandDirVecs;

            [ReadOnly] public NativeArray<RaycastHit> results;
            public NativeArray<float> distances;
            public NativeArray<float> intensities;

            [ReadOnly] public float minRange;
            [ReadOnly] public float maxRange;

            [ReadOnly] public float maxIntensity;

            public Random random;
            public float sigma;
            public float offset;
    
            void IJobParallelFor.Execute(int index)
            {
                // Gaussian Noise part
                var rand2 = random.NextFloat();
                var rand3 = random.NextFloat();
                float normrand = 
                    (float)Math.Sqrt(-2.0f * Math.Log(rand2)) *
                    (float)Math.Cos(2.0f * Math.PI * rand3);
                normrand *= sigma;
      
                if(results[index].distance < minRange)
                {
                    distances[index] = 0;
                    intensities[index] = 0;
                }
                else if(results[index].distance > maxRange)
                {
                    distances[index] = 0;
                    intensities[index] = 0;
                }
                else
                {
                    distances[index] = results[index].distance + normrand + offset;
                    intensities[index] = maxIntensity * minRange * minRange / (distances[index] * distances[index]);

                    /*
                    OpticalMaterial om = results[index].collider.gameObject.GetComponent<OpticalMaterial>();
                    if (om)
                    {
                        float theta = Vector3.Angle(commandDirVecs[index], Vector3.Reflect(results[index].normal, results[index].normal)) * Mathf.Deg2Rad;
                        intensities[index] *= om.GetReflectance(theta);
                    }
                    */
                }
            }
        }
    }
}

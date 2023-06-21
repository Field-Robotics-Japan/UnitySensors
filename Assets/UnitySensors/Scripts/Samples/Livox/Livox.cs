using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using UnityEngine.Jobs;
using Unity.Jobs;
using UnityEngine;

[RequireComponent(typeof(FRJ.Sensor.CSVLidar))]
public class Livox : MonoBehaviour
{
    enum Model
    {
        Avia,
        Horizon,
        Mid40,
        Mid70,
        Tele
    };

    [Header("Parameters")]
    [SerializeField] private Model _model;

    [Header("Informations(No need to input)")]

    [SerializeField] private string _avia_filePath = "avia";
    [SerializeField] private string _horizon_filePath = "horizon";
    [SerializeField] private string _mid40_filePath = "mid40";
    [SerializeField] private string _mid70_filePath = "mid70";
    [SerializeField] private string _tele_filePath = "tele";

    private JobHandle _handle;
    public ulong timebase;
    private float _timeElapsed = 0f;
    private float _timeStamp = 0f;

    private FRJ.Sensor.CSVLidar _lidar;
    private FRJ.Sensor.LivoxSerializer _serializer;

    void Start()
    {
        // Get CSVLidar
        this._lidar = GetComponent<FRJ.Sensor.CSVLidar>();

        string filePath = "";
        switch (_model)
        {
            case Model.Avia:
                filePath = _avia_filePath;
                break;
            case Model.Horizon:
                filePath = _horizon_filePath;
                break;
            case Model.Mid40:
                filePath = _mid40_filePath;
                break;
            case Model.Mid70:
                filePath = _mid70_filePath;
                break;
            case Model.Tele:
                filePath = _tele_filePath;
                break;
        }
        this._lidar.csvFilePath = filePath;
        this._lidar.Init();

        this._serializer = new FRJ.Sensor.LivoxSerializer(this._lidar.numOfLasersPerScan);
        this._serializer.job.timebase = this.timebase = (UInt64)(Time.time * 1000);
        this._serializer.job.point = this._lidar.point;
        this._serializer.job.intensities = this._lidar.intensities;
    }

    void OnDisable()
    {
        this._handle.Complete();
        this._lidar.Dispose();
        this._serializer.Dispose();
    }

    void Update()
    {
        if (!this._lidar.isInitialized) return;
        this._handle.Complete();
        this._timeElapsed += Time.deltaTime;

        if (this._timeElapsed > (1f / this._lidar.scanRate))
        {
            // Update time
            this._timeElapsed = 0;
            this._timeStamp = Time.time;

            // Update ROS Message
            uint sec = (uint)Math.Truncate(this._timeStamp);
            uint nanosec = (uint)((this._timeStamp - sec) * 1e+9);
            this._serializer.job.time = (UInt64)(this._timeStamp * 1000 - this.timebase);

            this._lidar.UpdateCommandDirVecs();

            // Update Raycast Command and Origin Transform
            for (int i = 0; i < this._lidar.numOfLasersPerScan; i++)
            {
                this._lidar.origin_pos[i] = this.transform.position;
                this._lidar.origin_rot[i] = this.transform.rotation;
                this._lidar.commands[i] =
                    new RaycastCommand(this.transform.position,
                      this.transform.rotation * this._lidar.commandDirVecs[i],
                      this._lidar.maxRange);
            }

            // Update Parallel Jobs
            var raycastJobHandle = RaycastCommand.ScheduleBatch(this._lidar.commands, this._lidar.results, 360);
            // Update Distance data
            if (this._lidar.randomSeed++ == 0)
                this._lidar.randomSeed = 1;
            this._lidar.job.random.InitState(this._lidar.randomSeed);
            var pointJobHandle = this._lidar.job.Schedule(this._lidar.results.Length, 360, raycastJobHandle);
            this._handle = this._serializer.job.Schedule(pointJobHandle);
            JobHandle.ScheduleBatchedJobs();
        }
    }
}

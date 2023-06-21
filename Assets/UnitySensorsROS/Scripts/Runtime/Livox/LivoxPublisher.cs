using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using UnityEngine.Jobs;
using Unity.Jobs;
using UnityEngine;

using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.LivoxRosDriver;
using RosMessageTypes.Std;
using RosMessageTypes.Sensor;

[RequireComponent(typeof(FRJ.Sensor.CSVLidar))]
public class LivoxPublisher : MonoBehaviour
{
    enum Model
    {
        Avia,
        Horizon,
        Mid40,
        Mid70,
        Tele
    };

    enum PublishType
    {
        PointCloud2,
        LivoxCustomMsg
    }

    [Header("Parameters")]
    [SerializeField] private Model _model;
    [SerializeField] private PublishType _publishType;
    [SerializeField] private string _topicName = "/livox/lidar";
    [SerializeField] private string _frameId = "/livox";
    [SerializeField] private byte lidar_id;
    [SerializeField] private byte[] rsvd;

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

    private ROSConnection _ros;
    private CustomMsgMsg _message_cmm;
    private PointCloud2Msg _message_pc2;

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

        // setup ROS
        this._ros = ROSConnection.instance;
        switch (_publishType)
        {
            case PublishType.PointCloud2:
                this._ros.RegisterPublisher<PointCloud2Msg>(this._topicName);
                break;
            case PublishType.LivoxCustomMsg:
                this._ros.RegisterPublisher<CustomMsgMsg>(this._topicName);
                break;
        }

        // setup ROS Message

        switch (_publishType)
        {
            case PublishType.PointCloud2:
                this._message_pc2 = new PointCloud2Msg();
                this._message_pc2.header.frame_id = this._frameId;
                this._message_pc2.height = 1;
                this._message_pc2.width = (uint)(_lidar.numOfLasersPerScan);
                this._message_pc2.fields = new PointFieldMsg[3];
                for(int i = 0; i < 3; i++)
                {
                    this._message_pc2.fields[i] = new PointFieldMsg();
                }
                this._message_pc2.fields[0].name = "x";
                this._message_pc2.fields[0].offset = 0;
                this._message_pc2.fields[0].datatype = 7;
                this._message_pc2.fields[0].count = 1;
                this._message_pc2.fields[1].name = "y";
                this._message_pc2.fields[1].offset = 4;
                this._message_pc2.fields[1].datatype = 7;
                this._message_pc2.fields[1].count = 1;
                this._message_pc2.fields[2].name = "z";
                this._message_pc2.fields[2].offset = 8;
                this._message_pc2.fields[2].datatype = 7;
                this._message_pc2.fields[2].count = 1;
                this._message_pc2.is_bigendian = false;
                this._message_pc2.point_step = 12;
                this._message_pc2.row_step = (uint)(_lidar.numOfLasersPerScan * 12);
                this._message_pc2.data = new byte[_lidar.numOfLasersPerScan*12];
                this._message_pc2.is_dense = true;
                break;
            case PublishType.LivoxCustomMsg:
                this._message_cmm = new CustomMsgMsg();
                this._message_cmm.header.frame_id = this._frameId;
                this._message_cmm.points = new CustomPointMsg[this._lidar.numOfLasersPerScan];
                for (int i = 0; i < this._lidar.numOfLasersPerScan; i++)
                {
                    this._message_cmm.points[i] = new CustomPointMsg();
                }
                break;
        }
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

            switch (_publishType)
            {
                case PublishType.PointCloud2:
                    this._message_pc2.header.stamp.sec = sec;
                    this._message_pc2.header.stamp.nanosec = nanosec;
                    for (int i = 0; i < this._lidar.numOfLasersPerScan*12; i++)
                    {
                        this._message_pc2.data[i] = this._serializer.data[i];
                    }
                    Debug.Log(this._message_pc2.data[0]);
                    _ros.Send(this._topicName, this._message_pc2);
                    break;
                case PublishType.LivoxCustomMsg:
                    this._message_cmm.header.stamp.sec = sec;
                    this._message_cmm.header.stamp.nanosec = nanosec;
                    this._message_cmm.timebase = this.timebase;
                    this._message_cmm.point_num = (uint)this._lidar.numOfLasersPerScan;
                    this._message_cmm.lidar_id = this.lidar_id;
                    this._message_cmm.rsvd = this.rsvd;
                    for (int i = 0; i < this._lidar.numOfLasersPerScan; i++)
                    {
                        this._message_cmm.points[i].x = this._serializer.x[i];
                        this._message_cmm.points[i].y = this._serializer.y[i];
                        this._message_cmm.points[i].z = this._serializer.z[i];
                        this._message_cmm.points[i].reflectivity = this._serializer.reflectivity[i];
                        this._message_cmm.points[i].tag = this._serializer.tag[i];
                        this._message_cmm.points[i].line = this._serializer.line[i];
                    }
                    _ros.Send(this._topicName, this._message_cmm);
                    break;
            }

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

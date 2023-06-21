using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using UnityEngine.Jobs;
using Unity.Jobs;
using UnityEngine;

using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Sensor;

[RequireComponent(typeof(FRJ.Sensor.RotateLidar))]
public class LaserScanPublisher : MonoBehaviour
{

    [SerializeField] private string _topicName = "scan";
    [SerializeField] private string _frameId   = "scan_link";
    
    private JobHandle _handle;
    private float _timeElapsed = 0f;
    private float _timeStamp   = 0f;

    private ROSConnection _ros;
    private LaserScanMsg _message;    

    private FRJ.Sensor.RotateLidar _lidar;

    float Deg2Rad(float deg)
    {
        return deg * Mathf.PI / 180f;
    }
    
    void Start()
    {
        // Get Rotate Lidar
        this._lidar = GetComponent<FRJ.Sensor.RotateLidar>();
        this._lidar.Init();

        // setup ROS
        this._ros = ROSConnection.instance;
        this._ros.RegisterPublisher<LaserScanMsg>(this._topicName);

        // setup ROS Message
        this._message = new LaserScanMsg();
        this._message.header.frame_id = this._frameId;
        this._message.angle_min = Deg2Rad(this._lidar.minAzimuthAngle);
        this._message.angle_max = Deg2Rad(this._lidar.maxAzimuthAngle);
        var deg_increment = (this._lidar.maxAzimuthAngle - this._lidar.minAzimuthAngle) / (float)this._lidar.numOfIncrements;
        this._message.angle_increment = Deg2Rad(deg_increment);
        this._message.time_increment  = 1f/this._lidar.scanRate/(float)this._lidar.numOfIncrements;
        this._message.scan_time = 1f/this._lidar.scanRate;
        this._message.range_min = this._lidar.minRange;
        this._message.range_max = this._lidar.maxRange;
    }

    void OnDisable()
    {
        this._handle.Complete();
        this._lidar.Dispose();
    }

    void Update()
    {
        this._handle.Complete();
        this._timeElapsed += Time.deltaTime;

        if(this._timeElapsed > (1f/this._lidar.scanRate)) {
            // Update ROS Message
# if ROS2
            int sec = (int)Math.Truncate(this._timeStamp);
# else
            uint sec = (uint)Math.Truncate(this._timeStamp);
# endif
            uint nanosec = (uint)( (this._timeStamp - sec)*1e+9 );
            this._message.header.stamp.sec = sec;
            this._message.header.stamp.nanosec = nanosec;
            this._message.ranges = this._lidar.distances.ToArray();
            this._message.intensities = this._lidar.intensities.ToArray();
            _ros.Send(this._topicName, this._message);

            // Update time
            this._timeElapsed = 0;
            this._timeStamp = Time.time;

            // Update Raycast Command
            for (int incr = 0; incr < this._lidar.numOfIncrements; incr++) {
                for (int layer = 0; layer < this._lidar.numOfLayers; layer++) {
                    int index = layer + incr * this._lidar.numOfLayers;
                    this._lidar.commands[index] =
                        new RaycastCommand(this.transform.position,
                                           this.transform.rotation * this._lidar.commandDirVecs[index],
                                           this._lidar.maxRange);
                }
            }
      
            // Update Parallel Jobs
            var raycastJobHandle = RaycastCommand.ScheduleBatch(this._lidar.commands, this._lidar.results, 360);
            // Update Distance data
            if(this._lidar.randomSeed++ == 0)
                this._lidar.randomSeed = 1;
            this._lidar.job.random.InitState(this._lidar.randomSeed);
            this._handle = this._lidar.job.Schedule(this._lidar.results.Length, 360, raycastJobHandle);
            JobHandle.ScheduleBatchedJobs();
        }
    }
}

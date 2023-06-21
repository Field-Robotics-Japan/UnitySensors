using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using UnityEngine.Jobs;
using Unity.Jobs;
using UnityEngine;

using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Velodyne;
using RosMessageTypes.Std;

[RequireComponent(typeof(FRJ.Sensor.RotateLidar))]
public class VLP16Publisher : MonoBehaviour
{

  [SerializeField] private string _topicName = "velodyne_packets";
  [SerializeField] private string _frameId   = "velodyne";
    
  private JobHandle _handle;
  private float _timeElapsed = 0f;
  private float _timeStamp   = 0f;

  private ROSConnection _ros;
  private VelodyneScanMsg _message;    

  private FRJ.Sensor.RotateLidar _lidar;
  private FRJ.Sensor.VLP16Serializer _serializer;

    
  void Start()
  {
    // Get Rotate Lidar
    this._lidar = GetComponent<FRJ.Sensor.RotateLidar>();
    this._lidar.Init();

    // Setup serializer
    this._serializer =
        new FRJ.Sensor.VLP16Serializer(this._lidar.numOfLayers,
          this._lidar.numOfIncrements,
          this._lidar.minAzimuthAngle,
          this._lidar.maxAzimuthAngle);
    this._serializer.job.distances   = this._lidar.distances;
    this._serializer.job.intensities = this._lidar.intensities;

    // setup ROS
    this._ros = ROSConnection.instance;
    this._ros.RegisterPublisher<VelodyneScanMsg>(this._topicName);

    // setup ROS Message
    this._message = new VelodyneScanMsg();
    this._message.header.frame_id = this._frameId;
    this._message.packets = new VelodynePacketMsg[this._lidar.numOfIncrements/12];
    for(int i=0; i<this._message.packets.Length; i++) {
      this._message.packets[i] = new VelodynePacketMsg();
      this._message.packets[i].data = new byte[1206];
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
      this._serializer.job.timeStamp = this._timeStamp;
      this._message.header.stamp.sec = sec;
      this._message.header.stamp.nanosec = nanosec;
      for(int i=0; i<this._message.packets.Length; i++)
      {
        this._message.packets[i].data =
            this._serializer.packets.GetSubArray(i*1206,1206).ToArray();
        this._message.packets[i].stamp.sec = sec;
        this._message.packets[i].stamp.nanosec = nanosec;
      }
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
      var distanceJobHandle = this._lidar.job.Schedule(this._lidar.results.Length, 360, raycastJobHandle);
      // Update Packet data
      this._handle = this._serializer.job.Schedule(distanceJobHandle);
      JobHandle.ScheduleBatchedJobs();
    }
  }
}

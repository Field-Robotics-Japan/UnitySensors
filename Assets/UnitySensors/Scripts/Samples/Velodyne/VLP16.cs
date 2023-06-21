using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using UnityEngine.Jobs;
using Unity.Jobs;
using UnityEngine;

[RequireComponent(typeof(FRJ.Sensor.RotateLidar))]
public class VLP16 : MonoBehaviour
{
  private JobHandle _handle;
  private float _timeElapsed = 0f;
  private float _timeStamp   = 0f;

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
      // Update time stamp
      this._serializer.job.timeStamp = this._timeStamp;

      // You can access serialized packet data.
      // Here is also the way to pack that packet data.
      byte[,] sendPacketData = new byte[this._lidar.numOfIncrements/12,1206];
      for(int i=0; i<this._lidar.numOfIncrements/12; i++)
      {
        for(int j=0; j<1206; j++)
          sendPacketData[i,j] =  this._serializer.packets[i*1206+j];
      }

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

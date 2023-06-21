using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Sensor;

[RequireComponent(typeof(Camera))]
[RequireComponent(typeof(FRJ.Sensor.RGBCamera))]
public class RGBCameraPublisher : MonoBehaviour
{

  [SerializeField] private string _topicName = "image";
  [SerializeField] private string _frameId   = "camera";
    
  private float _timeElapsed = 0f;
  private float _timeStamp   = 0f;

  private ROSConnection _ros;
  private CompressedImageMsg _message;    

  private FRJ.Sensor.RGBCamera _camera;
    
  void Start()
  {
    // Get Rotate Lidar
    this._camera = GetComponent<FRJ.Sensor.RGBCamera>();
    this._camera.Init();

    // setup ROS
    this._ros = ROSConnection.instance;
    this._topicName += "/compressed";
    this._ros.RegisterPublisher<CompressedImageMsg>(this._topicName);

    // setup ROS Message
    this._message = new CompressedImageMsg();
    this._message.header.frame_id = this._frameId;
    this._message.format = "jpeg";
  }

    void Update()
    {
        this._timeElapsed += Time.deltaTime;

        if(this._timeElapsed > (1f/this._camera.scanRate))
        {
            // Update ROS Message
# if ROS2
            int sec = (int)Math.Truncate(this._timeStamp);
# else
            uint sec = (uint)Math.Truncate(this._timeStamp);
# endif
            uint nanosec = (uint)( (this._timeStamp - sec)*1e+9 );
            this._message.header.stamp.sec = sec;
            this._message.header.stamp.nanosec = nanosec;
            this._message.data = this._camera.data;
            this._ros.Send(this._topicName, this._message);

            // Update time
            this._timeElapsed = 0;
            this._timeStamp = Time.time;
        }
    }
}

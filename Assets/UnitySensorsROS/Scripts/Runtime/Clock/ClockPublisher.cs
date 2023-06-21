using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Rosgraph;

public class ClockPublisher : MonoBehaviour
{

  [SerializeField] private string _topicName = "clock";
    
  private float _timeStamp   = 0f;

  private ROSConnection _ros;
  private ClockMsg _message;    
    
  void Start()
  {
    // setup ROS
    this._ros = ROSConnection.instance;
    this._ros.RegisterPublisher<ClockMsg>(this._topicName);

    // setup ROS Message
    this._message = new ClockMsg();
    this._message.clock.sec = 0;
    this._message.clock.nanosec = 0;
  }

    void Update()
    {
        this._timeStamp = Time.time;
# if ROS2
        int sec = (int)Math.Truncate(this._timeStamp);
# else
        uint sec = (uint)Math.Truncate(this._timeStamp);
# endif
        uint nanosec = (uint)( (this._timeStamp - sec)*1e+9 );
        this._message.clock.sec = sec;
        this._message.clock.nanosec = nanosec;
        this._ros.Send(this._topicName, this._message);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Nmea;

[RequireComponent(typeof(FRJ.Sensor.GPS))]
public class GPSPublisher : MonoBehaviour
{
    private enum FORMAT_TYPE
    {
        GPRMC,
        GPGGA,
        GPGSA,
        GPVTG
    }

    [SerializeField] private string _topicName = "nmea/sentence";
    [SerializeField] private string _frameId   = "nmea_link";
    [SerializeField] private FORMAT_TYPE _formatType;
    
    private float _timeElapsed = 0f;
    private float _timeStamp   = 0f;

    private ROSConnection _ros;
    private SentenceMsg _message;
    
    private FRJ.Sensor.GPS _gps;
    
    void Start()
    {
        // Setup GPS
        this._gps = GetComponent<FRJ.Sensor.GPS>();
        this._gps.Init();

        // setup ROS
        this._ros = ROSConnection.instance;
        this._ros.RegisterPublisher<SentenceMsg>(this._topicName);
        
        // setup ROS Message
        this._message = new SentenceMsg();
        this._message.header.frame_id = this._frameId;
    }

    void Update()
    {
        this._timeElapsed += Time.deltaTime;

        if(this._timeElapsed > (1f/this._gps.updateRate))
        {
            // Update time
            this._timeElapsed = 0;
            this._timeStamp = Time.time;

            // Update GPS 
            this._gps.updateGPS();

            // Update ROS Message
# if ROS2
            int sec = (int)Math.Truncate(this._timeStamp);
# else
            uint sec = (uint)Math.Truncate(this._timeStamp);
# endif
            uint nanosec = (uint)( (this._timeStamp - sec)*1e+9 );
            this._message.header.stamp.sec = sec;
            this._message.header.stamp.nanosec = nanosec;

            switch (_formatType)
            {
                case FORMAT_TYPE.GPRMC:
                    this._message.sentence = this._gps.gprmc;
                    break;
                case FORMAT_TYPE.GPGGA:
                    this._message.sentence = this._gps.gpgga;
                    break;
                case FORMAT_TYPE.GPGSA:
                    this._message.sentence = this._gps.gpgsa;
                    break;
                case FORMAT_TYPE.GPVTG:
                    this._message.sentence = this._gps.gpvtg;
                    break;
                default:
                    this._message.sentence = "";
                    break;
            }

            this._ros.Send(this._topicName, this._message);
        }
    }
}

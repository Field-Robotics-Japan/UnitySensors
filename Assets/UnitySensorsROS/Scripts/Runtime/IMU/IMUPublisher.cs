using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.Robotics.ROSTCPConnector;
using Unity.Robotics.ROSTCPConnector.ROSGeometry;
using RosMessageTypes.Sensor;
using RosMessageTypes.Geometry;

[RequireComponent(typeof(FRJ.Sensor.IMU))]
public class IMUPublisher : MonoBehaviour
{

    [SerializeField] private string _topicName = "imu/raw_data";
    [SerializeField] private string _frameId = "imu_link";

    private float _timeElapsed = 0f;
    private float _timeStamp = 0f;

    private ROSConnection _ros;
    public ImuMsg _message;

    private FRJ.Sensor.IMU _imu;

    void Start()
    {
        // Get Rotate Lidar
        this._imu = GetComponent<FRJ.Sensor.IMU>();

        // setup ROS
        this._ros = ROSConnection.instance;
        this._ros.RegisterPublisher<ImuMsg>(this._topicName);

        // setup ROS Message
        this._message = new ImuMsg();
        this._message.header.frame_id = this._frameId;
    }

    void Update()
    {
        this._timeElapsed += Time.deltaTime;

        if (this._timeElapsed > (1f / this._imu.scanRate))
        {
            // Update time
            this._timeElapsed = 0;
            this._timeStamp = Time.time;

            // Update IMU data
            this._imu.UpdateIMU();

            // Update ROS Message
# if ROS2
            int sec = (int)Math.Truncate(this._timeStamp);
# else
            uint sec = (uint)Math.Truncate(this._timeStamp);
# endif
            uint nanosec = (uint)((this._timeStamp - sec) * 1e+9);
            this._message.header.stamp.sec = sec;
            this._message.header.stamp.nanosec = nanosec;
            Quaternion<FLU> orientation_ros = new Quaternion<FLU>(this._imu.GeometryQuaternion.x,
                                                                  this._imu.GeometryQuaternion.y,
                                                                  this._imu.GeometryQuaternion.z,
                                                                  this._imu.GeometryQuaternion.w).To<FLU>();
            QuaternionMsg orientation =
                new QuaternionMsg(orientation_ros.x,
                                  orientation_ros.y,
                                  orientation_ros.z,
                                  orientation_ros.w);
            this._message.orientation = orientation;
            Vector3<FLU> angular_velocity_ros = new Vector3<FLU>(this._imu.AngularVelocity).To<FLU>();
            Vector3Msg angular_velocity =
                new Vector3Msg(angular_velocity_ros.x,
                               angular_velocity_ros.y,
                               angular_velocity_ros.z);
            this._message.angular_velocity = angular_velocity;
            Vector3<FLU> linear_acceleration_ros = new Vector3<FLU>(this._imu.LinearAcceleration).To<FLU>();
            Vector3Msg linear_acceleration =
                new Vector3Msg(linear_acceleration_ros.x,
                               linear_acceleration_ros.y,
                               linear_acceleration_ros.z);
            this._message.linear_acceleration = linear_acceleration;
            this._ros.Send(this._topicName, this._message);
        }
    }
}

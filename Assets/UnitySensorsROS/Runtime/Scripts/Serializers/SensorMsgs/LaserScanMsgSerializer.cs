using UnityEngine;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using RosMessageTypes.Sensor;
using System;

using UnitySensors.Interface.Sensor;
using UnitySensors.Interface.Sensor.PointCloud;
using UnitySensors.DataType.Sensor;
using UnitySensors.DataType.Sensor.PointCloud;
using UnitySensors.DataType.LiDAR;
using UnitySensors.Sensor.LiDAR;
using UnitySensors.Utils.PointCloud;
using UnitySensors.ROS.Serializer.Std;
using UnitySensors.ROS.Utils.PointCloud;

namespace UnitySensors.ROS.Serializer.PointCloud
{
    [System.Serializable]
    public class LaserScanMsgSerializer : RosMsgSerializer<LaserScanMsg>
    {
        [SerializeField]
        private float _min_range = 0.5f;

        [SerializeField]
        private float _max_range = 100.0f;

        [SerializeField]
        private float _gaussian_noise_sigma = 0.0f;

        [SerializeField]
        private ScanPattern _scan_pattern;

        [SerializeField]
        private HeaderSerializer _header;

        protected IPointCloudInterface<PointXYZI> _sourceInterface;
        private int _pointsNum;

        public void SetSource(IPointCloudInterface<PointXYZI> sourceInterface)
        {
            _sourceInterface = sourceInterface;
        }

        public override void Init()
        {
            base.Init();
            _header.Init();

            _pointsNum = _sourceInterface.pointCloud.points.Length;

            _msg.angle_min = _scan_pattern.minAzimuthAngle;
            _msg.angle_max = _scan_pattern.maxAzimuthAngle;
            _msg.angle_increment = (_msg.angle_max - _msg.angle_min) / _pointsNum;
            _msg.time_increment = 0.0f;
            _msg.scan_time = 0.0f;
            _msg.range_min = _min_range;
            _msg.range_max = _max_range;
            _msg.ranges = new float[_pointsNum];
            _msg.intensities = new float[_pointsNum];
        }

        public override LaserScanMsg Serialize()
        {
            _msg.header = _header.Serialize();
            
            for (int i = 0; i < _pointsNum; i++)
            {
                var point = _sourceInterface.pointCloud.points[i];
                _msg.ranges[i] = Mathf.Sqrt(point.position.x * point.position.x + point.position.z * point.position.z);
                if (_msg.ranges[i] < _min_range || _msg.ranges[i] > _max_range)
                {
                    _msg.ranges[i] = float.NaN;
                }
                else
                {
                    double u1 = 1.0-UnityEngine.Random.Range(0f, 1f); //uniform(0,1] random doubles
                    double u2 = 1.0-UnityEngine.Random.Range(0f, 1f);
                    double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) *
                        Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)
                    _msg.ranges[i] += (float)randStdNormal * _gaussian_noise_sigma; //random normal(mean,stdDev^2)
                }
                _msg.intensities[i] = point.intensity;
            }
            
            return _msg;
        }

        public override void OnDestroy()
        {
        }
    }
}

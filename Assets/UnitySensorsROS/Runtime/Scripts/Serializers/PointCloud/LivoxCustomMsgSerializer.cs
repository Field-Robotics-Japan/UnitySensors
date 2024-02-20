using System;
using UnityEngine;
using Unity.Collections;
using Unity.Jobs;

using RosMessageTypes.Livox;

using UnitySensors.Data.PointCloud;
using UnitySensors.Sensor;

using UnitySensors.ROS.Data.Livox;

namespace UnitySensors.ROS.Serializer.PointCloud
{
    [System.Serializable]
    public class LivoxCustomMsgSerializer<T, TT> : RosMsgSerializer<T, CustomMsgMsg>
        where T : UnitySensor, IPointCloudInterface<TT>
        where TT : struct, IPointXYZInterface
    {
        [SerializeField]
        private HeaderSerializer _header;
        [SerializeField, Range(0, 255)]
        private byte _lidar_id;

        private JobHandle _jobHandle;
        private IPointsToLivoxCustomMsgJob<TT> _pointsToLivoxCustomMsgJob;
        private NativeArray<CustomPoint> _data;

        public override void Init(T sensor)
        {
            base.Init(sensor);
            _header.Init(sensor);

            _msg.timebase = 0;
            _msg.point_num = (uint) sensor.pointsNum;
            _msg.lidar_id = _lidar_id;
            _msg.rsvd = new byte[3];

            _msg.points = new CustomPointMsg[sensor.pointsNum];

            _data = new NativeArray<CustomPoint>(sensor.pointsNum, Allocator.Persistent);
            _pointsToLivoxCustomMsgJob = new IPointsToLivoxCustomMsgJob<TT>()
            {
                points = sensor.pointCloud.points,
                data = _data
            };
        }

        public override CustomMsgMsg Serialize()
        {
            _msg.header = _header.Serialize();
            _jobHandle = _pointsToLivoxCustomMsgJob.Schedule(sensor.pointsNum, 1);
            _jobHandle.Complete();
            for(int i = 0; i < sensor.pointsNum; i++)
            {
                _msg.points[i] = _data[i].ConvertToMsg();
            }
            return _msg;
        }

        public void Dispose()
        {
            _jobHandle.Complete();
            if (_data.IsCreated) _data.Dispose();
        }
    }
}

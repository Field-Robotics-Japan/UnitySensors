using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Unity.Jobs;
using RosMessageTypes.Sensor;

using UnitySensors.Data.PointCloud;
using UnitySensors.Sensor;

namespace UnitySensors.ROS.Serializer.PointCloud
{
    [System.Serializable]
    public class PointCloud2MsgSerializer<T, TT> : RosMsgSerializer<T, PointCloud2Msg>
        where T : UnitySensor, IPointCloudInterface<TT>
        where TT : struct, IPointXYZInterface
    {
        [SerializeField]
        private HeaderSerializer _header;

        private JobHandle _jobHandle;
        private IPointsToPointCloud2MsgJob<TT> _pointsToPointCloud2MsgJob;
        private NativeArray<byte> _data;

        public override void Init(T sensor)
        {
            base.Init(sensor);
            _header.Init(sensor);

            _msg.height = 1;
            _msg.width = (uint)sensor.pointsNum;
            _msg.fields = new PointFieldMsg[3];
            for (int i = 0; i < 3; i++)
            {
                _msg.fields[i] = new PointFieldMsg();
                _msg.fields[i].name = ((char)('x' + i)).ToString();
                _msg.fields[i].offset = (uint)(4 * i);
                _msg.fields[i].datatype = 7;
                _msg.fields[i].count = 1;
            }
            _msg.is_bigendian = false;
            _msg.point_step = 12;
            _msg.row_step = (uint)sensor.pointsNum * 12;
            _msg.data = new byte[(uint)sensor.pointsNum * 12];
            _msg.is_dense = true;

            _data = new NativeArray<byte>(sensor.pointsNum * 12, Allocator.Persistent);

            _pointsToPointCloud2MsgJob = new IPointsToPointCloud2MsgJob<TT>()
            {
                points = sensor.pointCloud.points,
                data = _data
            };
        }

        public override PointCloud2Msg Serialize()
        {
            _msg.header = _header.Serialize();
            _jobHandle = _pointsToPointCloud2MsgJob.Schedule(sensor.pointsNum, 1);
            _jobHandle.Complete();
            _pointsToPointCloud2MsgJob.data.CopyTo(_msg.data);
            return _msg;
        }

        public void Dispose()
        {
            _jobHandle.Complete();
            if(_data.IsCreated) _data.Dispose();
        }
    }
}
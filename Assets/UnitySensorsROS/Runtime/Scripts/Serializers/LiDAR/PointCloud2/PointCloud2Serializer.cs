using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.Burst;
using Unity.Collections;
using UnityEngine.Jobs;
using Unity.Jobs;

using RosMessageTypes.Sensor;

namespace UnitySensors.ROS
{
    [System.Serializable]
    public class PointCloud2Serializer : Serializer
    {
        [SerializeField]
        private PointCloud2Msg _msg;

        private AutoHeader _header;

        private JobHandle _handle;
        private PointsToPointCloud2MsgJob _job;

        private int _pointsNum;
        private NativeArray<byte> _data;

        public PointCloud2Msg msg { get => _msg; }

        public void Init(string frame_id, ref NativeArray<Vector3> points, uint pointsNum)
        {
            _pointsNum = (int)pointsNum;

            _header = new AutoHeader();
            _header.Init(frame_id);

            _msg = new PointCloud2Msg();
            _msg.height = 1;
            _msg.width = pointsNum;
            _msg.fields = new PointFieldMsg[3];
            for(int i = 0; i < 3; i++)
            {
                _msg.fields[i] = new PointFieldMsg();
                _msg.fields[i].name = ((char)('x' + i)).ToString();
                _msg.fields[i].offset = (uint)(4 * i);
                _msg.fields[i].datatype = 7;
                _msg.fields[i].count = 1;
            }
            _msg.is_bigendian = false;
            _msg.point_step = 12;
            _msg.row_step = pointsNum * 12;
            _msg.data = new byte[_pointsNum * 12];
            _msg.is_dense = true;

            _data = new NativeArray<byte>(_pointsNum * 12, Allocator.Persistent);

            _job = new PointsToPointCloud2MsgJob
            {
                pointNum = _pointsNum,
                points = points,
                data = _data
            };
        }

        public PointCloud2Msg Serialize(float time)
        {
            _handle = _job.Schedule(_pointsNum, 1);
            JobHandle.ScheduleBatchedJobs();
            _handle.Complete();

            _header.Serialize(time);
            _msg.data = _data.ToArray();
            _msg.header = _header.header;

            return _msg;
        }

        public void Dispose()
        {
            _handle.Complete();
            _data.Dispose();
        }

        [BurstCompile]
        public struct PointsToPointCloud2MsgJob : IJobParallelFor
        {
            public int pointNum;

            [ReadOnly]
            public NativeArray<Vector3> points;

            public NativeArray<byte> data;

            public void Execute(int index)
            {
                NativeArray<float> tmp = new NativeArray<float>(3, Allocator.Temp);
                tmp[0] = points[index].x;
                tmp[1] = points[index].z;
                tmp[2] = points[index].y;
                var slice = new NativeSlice<float>(tmp).SliceConvert<byte>();
                slice.CopyTo(data.GetSubArray(index * 12, 12));

            }
        }
    }
}
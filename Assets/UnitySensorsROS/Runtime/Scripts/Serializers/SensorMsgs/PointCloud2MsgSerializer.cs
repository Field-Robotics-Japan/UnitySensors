using UnityEngine;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using RosMessageTypes.Sensor;

using UnitySensors.Interface.Sensor;
using UnitySensors.Interface.Sensor.PointCloud;
using UnitySensors.Utils.PointCloud;
using UnitySensors.ROS.Serializer.Std;
using UnitySensors.ROS.Utils.PointCloud;

namespace UnitySensors.ROS.Serializer.PointCloud
{
    [System.Serializable]
    public class PointCloud2MsgSerializer<T> : RosMsgSerializer<PointCloud2Msg> where T : struct, IPointInterface
    {
        [SerializeField]
        private HeaderSerializer _header;

        protected IPointCloudInterface<T> _sourceInterface;
        private int _pointsNum;

        private JobHandle _jobHandle;
        private IInvertXJob _invertXJob;
        private NativeArray<byte> _data;

        public void SetSource(IPointCloudInterface<T> sourceInterface)
        {
            _sourceInterface = sourceInterface;
        }

        public override void Init()
        {
            base.Init();
            _header.Init();

            _pointsNum = _sourceInterface.pointCloud.points.Length;
            int sizeOfPoint = PointUtilities.pointDataSizes[typeof(T)];
            int dataSize = _pointsNum * sizeOfPoint;

            _msg.height = 1;
            _msg.width = (uint)_pointsNum;
            _msg.fields = PointUtilitiesROS.pointFields[typeof(T)];
            _msg.is_bigendian = true;
            _msg.point_step = (uint)sizeOfPoint;
            _msg.row_step = (uint)dataSize;
            _msg.data = new byte[dataSize];
            _msg.is_dense = true;

            _data = new NativeArray<byte>(dataSize, Allocator.Persistent);

            _invertXJob = new IInvertXJob()
            {
                pointStep = sizeOfPoint,
                data = _data
            };
        }

        public override PointCloud2Msg Serialize()
        {
            _msg.header = _header.Serialize();
            
            unsafe
            {
                UnsafeUtility.MemCpy(NativeArrayUnsafeUtility.GetUnsafePtr(_data), NativeArrayUnsafeUtility.GetUnsafePtr(_sourceInterface.pointCloud.points), _data.Length);
            }
            _jobHandle = _invertXJob.Schedule(_pointsNum, 1);
            _jobHandle.Complete();

            _data.CopyTo(_msg.data);
            
            return _msg;
        }

        public override void OnDestroy()
        {
            _jobHandle.Complete();
            if (_data.IsCreated) _data.Dispose();
        }
    }
}
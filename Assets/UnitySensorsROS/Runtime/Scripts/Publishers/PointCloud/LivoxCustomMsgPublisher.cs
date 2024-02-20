using UnityEngine;
using Unity.Robotics.ROSTCPConnector.MessageGeneration;
using RosMessageTypes.Livox;
using UnitySensors.Data.PointCloud;
using UnitySensors.Sensor;
using UnitySensors.ROS.Serializer.PointCloud;

namespace UnitySensors.ROS.Publisher.PointCloud
{
    public class LivoxCustomMsgPublisher<T, TT> : RosMsgPublisher<T,LivoxCustomMsgSerializer<T, TT>, CustomMsgMsg>
        where T : UnitySensor, IPointCloudInterface<TT>
        where TT : struct, IPointXYZInterface
    {
        private void OnDestroy()
        {
            _serializer.Dispose();
        }
    }
}

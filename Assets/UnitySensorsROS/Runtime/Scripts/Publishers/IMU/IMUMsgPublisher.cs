using UnityEngine;
using RosMessageTypes.Sensor;
using UnitySensors.Sensor.IMU;
using UnitySensors.ROS.Serializer.IMU;

namespace UnitySensors.ROS.Publisher.IMU
{
    [RequireComponent(typeof(IMUSensor))]
    public class IMUMsgPublisher : RosMsgPublisher<IMUSensor, IMUMsgSerializer, ImuMsg>
    {
    }
}

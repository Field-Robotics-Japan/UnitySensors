using UnityEngine;
using RosMessageTypes.Sensor;

using UnitySensors.Sensor.GNSS;
using UnitySensors.ROS.Serializer.GNSS;

namespace UnitySensors.ROS.Publisher.GNSS
{
    [RequireComponent(typeof(GNSSSensor))]
    public class NavSatFixMsgPublisher : RosMsgPublisher<GNSSSensor, NavSatFixMsgSerializer, NavSatFixMsg>
    {
    }
}


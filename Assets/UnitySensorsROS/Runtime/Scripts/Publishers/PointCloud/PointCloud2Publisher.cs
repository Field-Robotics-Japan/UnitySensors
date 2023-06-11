using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnitySensors.ROS
{
    public class PointCloud2Publisher<T> : Publisher<T, PointCloud2Serializer> where T : Sensor
    {
    }
}

using UnityEngine;
using UnitySensors.Attribute;
using UnitySensors.DataType.Sensor.PointCloud;
using UnitySensors.Interface.Sensor;

namespace UnitySensors.ROS.Publisher.Sensor
{
    public class LiDARPointCloud2MsgPublisher : PointCloud2MsgPublisher<PointXYZI>
    {
        [SerializeField, Interface(typeof(IPointCloudInterface<PointXYZI>))]
        private Object _source;

        private void Awake()
        {
            _serializer.SetSource(_source as IPointCloudInterface<PointXYZI>);
        }
    }
}
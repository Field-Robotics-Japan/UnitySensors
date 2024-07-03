using UnityEngine;
using UnitySensors.Attribute;
using UnitySensors.DataType.Sensor.PointCloud;
using UnitySensors.Interface.Sensor;

namespace UnitySensors.ROS.Publisher.Sensor
{
    public class RGBDCameraPointCloud2MsgPublisher : PointCloud2MsgPublisher<PointXYZRGB>
    {
        [SerializeField, Interface(typeof(IPointCloudInterface<PointXYZRGB>))]
        private Object _source;

        private void Awake()
        {
            _serializer.SetSource(_source as IPointCloudInterface<PointXYZRGB>);
        }
    }
}
using UnityEngine;
using UnitySensors.Attribute;
using UnitySensors.DataType.Sensor.PointCloud;
using UnitySensors.Interface.Sensor;
using UnitySensors.Sensor.Camera;

namespace UnitySensors.ROS.Publisher.Sensor
{
    public class RGBDCameraPointCloud2MsgPublisher : PointCloud2MsgPublisher<PointXYZRGB>
    {
        [SerializeField, Interface(typeof(IPointCloudInterface<PointXYZRGB>))]
        private Object _source;

        protected override void InitializePublisher()
        {
            base.InitializePublisher();

            _serializer.SetSource(_source as IPointCloudInterface<PointXYZRGB>);
            (_source as RGBDCameraSensor).convertToPointCloud = true;
        }
    }
}

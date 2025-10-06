using UnityEngine;
using UnitySensors.Attribute;
using UnitySensors.DataType.Sensor.PointCloud;
using UnitySensors.Interface.Sensor;
using UnitySensors.Sensor.Camera;

namespace UnitySensors.ROS.Publisher.Sensor
{
    public class DepthCameraPointCloud2MsgPublisher : PointCloud2MsgPublisher<PointXYZ>
    {
        [SerializeField, Interface(typeof(IPointCloudInterface<PointXYZ>))]
        private Object _source;

        protected override void InitializePublisher()
        {
            base.InitializePublisher();

            _serializer.SetSource(_source as IPointCloudInterface<PointXYZ>);
            (_source as DepthCameraSensor).convertToPointCloud = true;
        }
    }
}

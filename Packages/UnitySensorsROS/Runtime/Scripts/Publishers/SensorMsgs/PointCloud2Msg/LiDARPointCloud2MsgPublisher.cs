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

        protected override void InitializePublisher()
        {
            base.InitializePublisher();

            if (_source == null)
            {
                Debug.LogError("Source is not set in LiDARPointCloud2MsgPublisher. Please ensure that the '_source' field is assigned in the Unity Editor or via code. Expected type: IPointCloudInterface<PointXYZI>.");
                return;
            }
            _serializer.SetSource(_source as IPointCloudInterface<PointXYZI>);
        }
    }
}

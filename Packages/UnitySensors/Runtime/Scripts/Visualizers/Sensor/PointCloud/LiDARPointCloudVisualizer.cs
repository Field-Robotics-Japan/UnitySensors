using UnityEngine;
using UnitySensors.Attribute;
using UnitySensors.DataType.Sensor.PointCloud;
using UnitySensors.Interface.Sensor;
using UnitySensors.Sensor;

namespace UnitySensors.Visualization.Sensor
{
    public class LiDARPointCloudVisualizer : PointCloudVisualizer<PointXYZI>
    {
        [SerializeField, Interface(typeof(IPointCloudInterface<PointXYZI>))]
        private Object _source;

        protected override void Start()
        {
            if (_source is UnitySensor)
            {
                (_source as UnitySensor).onSensorUpdateComplete += Visualize;
            }
            base.SetSource(_source as IPointCloudInterface<PointXYZI>);
            base.Start();
        }
    }
}
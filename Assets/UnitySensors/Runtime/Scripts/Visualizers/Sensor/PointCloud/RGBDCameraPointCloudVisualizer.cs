using UnityEngine;
using UnitySensors.Attribute;
using UnitySensors.DataType.Sensor.PointCloud;
using UnitySensors.Interface.Sensor;
using UnitySensors.Sensor;

namespace UnitySensors.Visualization.Sensor
{
    public class RGBDCameraPointCloudVisualizer : PointCloudVisualizer<PointXYZRGB>
    {
        [SerializeField, Interface(typeof(IPointCloudInterface<PointXYZRGB>))]
        private Object _source;

        protected override void Start()
        {
            if (_source is UnitySensor)
            {
                (_source as UnitySensor).onSensorUpdated += Visualize;
            }
            base.SetSource(_source as IPointCloudInterface<PointXYZRGB>);
            base.Start();
        }
    }
}
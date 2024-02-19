using UnityEngine;
using UnitySensors.Attribute;
using UnitySensors.DataType.Sensor.PointCloud;
using UnitySensors.Interface.Sensor;

namespace UnitySensors.Visualization.Sensor
{
    public class DepthCameraPointCloudVisualizer : PointCloudVisualizer<PointXYZ>
    {
        [SerializeField, Interface(typeof(IPointCloudInterface<PointXYZ>))]
        private Object _source;

        protected override void Start()
        {
            base.SetSource(_source as IPointCloudInterface<PointXYZ>);
            base.Start();
        }
    }
}
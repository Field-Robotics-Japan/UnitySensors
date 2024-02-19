using UnityEngine;
using UnitySensors.Attribute;
using UnitySensors.DataType.Sensor.PointCloud;
using UnitySensors.Interface.Sensor;

namespace UnitySensors.Visualization.Sensor
{
    public class LiDARPointCloudVisualizer : PointCloudVisualizer<PointXYZI>
    {
        [SerializeField, Interface(typeof(IPointCloudInterface<PointXYZI>))]
        private Object _source;

        protected override void Start()
        {
            base.SetSource(_source as IPointCloudInterface<PointXYZI>);
            base.Start();
        }
    }
}
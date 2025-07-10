using UnityEngine;
using UnitySensors.Attribute;
using UnitySensors.DataType.Sensor.PointCloud;
using UnitySensors.Interface.Sensor;
using UnitySensors.Sensor;
using UnitySensors.Sensor.Camera;

namespace UnitySensors.Visualization.Sensor
{
    public class DepthCameraPointCloudVisualizer : PointCloudVisualizer<PointXYZ>
    {
        [SerializeField, Interface(typeof(IPointCloudInterface<PointXYZ>))]
        private Object _source;
        private void OnEnable()
        {
            if (_source is DepthCameraSensor)
                (_source as DepthCameraSensor).convertToPointCloud = true;
            else if (_source is RGBDCameraSensor)
                (_source as RGBDCameraSensor).convertToPointCloud = true;
        }

        protected override void Start()
        {
            if (_source is UnitySensor)
            {
                (_source as UnitySensor).onSensorUpdated += Visualize;
            }
            base.SetSource(_source as IPointCloudInterface<PointXYZ>);
            base.Start();
        }
    }
}
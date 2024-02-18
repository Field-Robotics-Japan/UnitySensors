using UnityEngine;
using UnitySensors.Sensor;

namespace UnitySensors.Visualization
{
    public abstract class Visualizer : MonoBehaviour
    {
        [SerializeField]
        private MonoBehaviour _source;

        private void Start()
        {
            if(_source is UnitySensor)
            {
                UnitySensor sensor = (UnitySensor)_source;
                sensor.onSensorUpdated += Visualize;
            }

            Init(_source);
        }

        protected abstract void Init(MonoBehaviour source);
        protected abstract void Visualize();
    }
}

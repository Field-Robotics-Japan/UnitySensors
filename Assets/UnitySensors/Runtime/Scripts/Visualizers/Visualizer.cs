using UnityEngine;
using UnitySensors.Attribute;
using UnitySensors.Sensor;

namespace UnitySensors.Visualization
{
    public abstract class Visualizer : MonoBehaviour
    {
        protected abstract void Visualize();
    }
}

using UnityEngine;

namespace UnitySensors.Interface.Geometry
{
    public interface IPoseInterface
    {
        public Vector3 position { get; }
        public Quaternion rotation { get; }
    }
}

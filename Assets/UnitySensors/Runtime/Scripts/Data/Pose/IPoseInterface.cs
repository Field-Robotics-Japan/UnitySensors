using UnityEngine;

namespace UnitySensors.Data.Pose
{
    public interface IPoseInterface
    {
        public Vector3 position { get; }
        public Quaternion rotation { get; }
    }
}

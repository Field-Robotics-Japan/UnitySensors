using System.Collections.Generic;
using UnityEngine;

using UnitySensors.DataType.Sensor;

namespace UnitySensors.Interface.Sensor
{
    public interface IContactDataInterface
    {
        public bool isContact { get; }
        public Vector3 totalForce { get; }
        public Vector3 totalTorque { get; }
        public Vector3 localTotalForce { get; }
        public Vector3 localTotalTorque { get; }
        public IReadOnlyList<ContactData> contacts { get; }
    }
}

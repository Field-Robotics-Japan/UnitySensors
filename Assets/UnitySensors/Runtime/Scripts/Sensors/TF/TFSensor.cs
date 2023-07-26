using UnityEngine;
using UnitySensors;

public class TFSensor : Sensor {

    [ReadOnly]
    private Vector3 _position;

    [ReadOnly]
    private Quaternion _rotation;

    public Vector3 position { get => _position; }
    public Quaternion rotation { get => _rotation; }
}
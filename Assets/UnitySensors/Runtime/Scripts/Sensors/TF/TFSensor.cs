using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnitySensors;

public class TFSensor : Sensor
{
    public struct TFData
    {
        public string frame_id_parent;
        public string frame_id_child;
        public Vector3 position;
        public Quaternion rotation;
    };

    [SerializeField]
    private string _frame_id;
    [SerializeField]
    private TFSensor[] _children;

    private Transform _transform;

    public TFData[] tf { get => GetTFData(); }

    protected override void Init()
    {
        _transform = transform;
        base.Init();
    }

    protected override void UpdateSensor()
    {
        base.UpdateSensor();
    }

    public TFData[] GetTFData()
    {
        List<TFData> tf = new List<TFData>();

        Matrix4x4 worldToLocalMatrix = _transform.worldToLocalMatrix;
        Quaternion worldToLocalQuaternion = Quaternion.Inverse(_transform.rotation);
        foreach (TFSensor child in _children)
        {
            tf.AddRange(child.GetTFData(_frame_id, worldToLocalMatrix, worldToLocalQuaternion));
        }
        return tf.ToArray();
    }

    public TFData[] GetTFData(string frame_id_parent, Matrix4x4 worldToLocalMatrix, Quaternion worldToLocalQuaternion)
    {
        List<TFData> tf = new List<TFData>();

        TFData tfData;
        tfData.frame_id_parent = frame_id_parent;
        tfData.frame_id_child = _frame_id;
        tfData.position = worldToLocalMatrix * _transform.position;
        tfData.rotation = worldToLocalQuaternion * _transform.rotation;
        tf.Add(tfData);

        worldToLocalMatrix = _transform.worldToLocalMatrix;
        worldToLocalQuaternion = Quaternion.Inverse(_transform.rotation);
        foreach (TFSensor child in _children)
        {
            tf.AddRange(child.GetTFData(_frame_id, worldToLocalMatrix, worldToLocalQuaternion));
        }
        return tf.ToArray();
    }
}
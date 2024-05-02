using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnitySensors.Attribute;

namespace UnitySensors.Sensor.TF
{
    [System.Serializable]
    public struct TFData
    {
        public string frame_id_parent;
        public string frame_id_child;
        public Vector3 position;
        public Quaternion rotation;
    };

    public class TFLink : UnitySensor
    {
        [SerializeField]
        private string _frame_id;
        [SerializeField]
        private TFLink[] _children;

        private Transform _transform;

        protected override void Init()
        {
            _transform = this.transform;
        }

        protected override void UpdateSensor()
        {
        }

        public TFData[] GetTFData()
        {
            List<TFData> tfData = new List<TFData>();

            Matrix4x4 worldToLocalMatrix = _transform.worldToLocalMatrix;
            Quaternion worldToLocalQuaternion = Quaternion.Inverse(_transform.rotation);

            foreach (TFLink child in _children)
            {
                tfData.AddRange(child.GetTFData(_frame_id, worldToLocalMatrix, worldToLocalQuaternion));
            }

            return tfData.ToArray();
        }

        public TFData[] GetTFData(string frame_id_parent, Matrix4x4 worldToLocalMatrix, Quaternion worldToLocalQuaternion)
        {
            List<TFData> tfData = new List<TFData>();

            TFData tfData_self;
            tfData_self.frame_id_parent = frame_id_parent;
            tfData_self.frame_id_child = _frame_id;
            tfData_self.position = (Vector3)(worldToLocalMatrix * new Vector4(_transform.position.x, _transform.position.y, _transform.position.z, 1.0f));
            Vector3 localScale = _transform.localScale;
            Vector3 lossyScale = _transform.lossyScale;
            Vector3 scaleVector = new Vector3()
            {
                x = localScale.x != 0 ? lossyScale.x / localScale.x : 0,
                y = localScale.y != 0 ? lossyScale.y / localScale.y : 0,
                z = localScale.z != 0 ? lossyScale.z / localScale.z : 0
            };
            tfData_self.position.Scale(scaleVector);
            tfData_self.rotation = worldToLocalQuaternion * _transform.rotation;
            tfData.Add(tfData_self);

            worldToLocalMatrix = _transform.worldToLocalMatrix;
            worldToLocalQuaternion = Quaternion.Inverse(_transform.rotation);

            foreach (TFLink child in _children)
            {
                tfData.AddRange(child.GetTFData(_frame_id, worldToLocalMatrix, worldToLocalQuaternion));
            }

            return tfData.ToArray();
        }

        protected override void OnSensorDestroy()
        {
        }
    }
}
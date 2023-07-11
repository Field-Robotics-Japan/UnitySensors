using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnitySensors;

namespace UnitySensors.Visualization
{ 
    /// <summary>
    /// T( : Sensor)���擾�����f�[�^��Gizmos�ŉ�������
    /// </summary>   
    public class Visualizer<T> : MonoBehaviour where T : Sensor
    {
        /// <summary>
        /// ���̉����ݒ�
        /// </summary>   
        [System.Serializable]
        protected class SphereSetting
        {
            public Color color = Color.white;
            public float radius = 0.5f;
        }

        /// <summary>
        /// ���̉����ݒ�
        /// </summary>   
        [System.Serializable]
        protected class LineSetting
        {
            public Color color = Color.white;
            public bool fixLineLength = true;
            public float lineLengthFactor = 1.0f;
        }

        /// <summary>
        /// �������[�h�ݒ�
        /// </summary>   
        protected enum VisualizeMode
        {
            NONE,       // �\���Ȃ�
            SELECTED,   // �I�u�W�F�N�g���I�����ꂽ���̂ݕ\��
            ALWAYS      // �펞�\��
        }

        [SerializeField]
        protected VisualizeMode _visualizeMode = VisualizeMode.ALWAYS;
        [SerializeField]
        protected Color _defaultColor = Color.white;

        protected T _target;

        protected virtual void Update()
        {
            if (_target != null) return;
            _target = GetComponent<T>();
        }

        private void OnDrawGizmosSelected()
        {
            if (!Application.isPlaying || _visualizeMode != VisualizeMode.SELECTED) return;
            Gizmos.color = _defaultColor;
            Visualize();
        }

        private void OnDrawGizmos()
        {
            if (!Application.isPlaying || _visualizeMode != VisualizeMode.ALWAYS) return;
            Gizmos.color = _defaultColor;
            Visualize();
        }

        /// <summary>
        /// �����̉��z�֐�
        /// </summary>   
        protected virtual void Visualize()
        {
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnitySensors;

namespace UnitySensors.Visualization
{ 
    /// <summary>
    /// T( : Sensor)が取得したデータをGizmosで可視化する
    /// </summary>   
    public class Visualizer<T> : MonoBehaviour where T : Sensor
    {
        /// <summary>
        /// 球の可視化設定
        /// </summary>   
        [System.Serializable]
        protected class SphereSetting
        {
            public Color color = Color.white;
            public float radius = 0.5f;
        }

        /// <summary>
        /// 線の可視化設定
        /// </summary>   
        [System.Serializable]
        protected class LineSetting
        {
            public Color color = Color.white;
            public bool fixLineLength = true;
            public float lineLengthFactor = 1.0f;
        }

        /// <summary>
        /// 可視化モード設定
        /// </summary>   
        protected enum VisualizeMode
        {
            NONE,       // 表示なし
            SELECTED,   // オブジェクトが選択された時のみ表示
            ALWAYS      // 常時表示
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
        /// 可視化関数の仮想関数
        /// </summary>   
        protected virtual void Visualize()
        {
        }
    }
}

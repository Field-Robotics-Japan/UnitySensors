using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace UnitySensors
{
    [CustomEditor(typeof(LiDARScanPattern))]
    public class LiDARScanPatternEditor : Editor
    {
        private LiDARScanPattern _target;
        private void OnEnable()
        {
            _target = target as LiDARScanPattern;
        }
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Load CSV"))
            {
                _target.LoadFile();
            }
        }
    }
}

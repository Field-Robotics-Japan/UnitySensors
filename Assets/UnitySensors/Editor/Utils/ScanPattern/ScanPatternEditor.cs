using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace UnitySensors
{
    [CustomEditor(typeof(ScanPattern))]
    public class ScanPatternEditor : Editor
    {
        private ScanPattern _target;
        private void OnEnable()
        {
            _target = target as ScanPattern;
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

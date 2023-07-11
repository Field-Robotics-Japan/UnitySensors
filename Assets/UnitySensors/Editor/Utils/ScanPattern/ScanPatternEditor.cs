using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace UnitySensors
{
    [CustomEditor(typeof(ScanPattern), true)]
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
            if (GUILayout.Button("Generate Scan Pattern"))
            {
                _target.GenerateScanPattern();
            }
        }
    }
}

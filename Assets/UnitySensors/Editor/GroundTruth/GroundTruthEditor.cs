using UnityEditor;
using UnitySensors.Sensor.GroundTruth;

namespace UnitySensors.Editor
{
    [CustomEditor(typeof(GroundTruth))]
    public class GroundTruthEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.HelpBox("GroundTruth does not use \"Frequency\" param.", MessageType.Info);
        }
    }
}
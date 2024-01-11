using UnityEditor;
using UnitySensors.Sensor.TF;

namespace UnitySensors.Editor
{
    [CustomEditor(typeof(TF))]
    public class TFEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.HelpBox("TF does not use \"Frequency\" param.", MessageType.Info);
        }
    }
}
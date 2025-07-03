using UnityEditor;
using UnitySensors.Sensor.TF;

namespace UnitySensors.ROS.Editor
{
    [CustomEditor(typeof(TFLink))]
    public class TFLinkEditor : UnityEditor.Editor
    {
        readonly string frequencyLabel = nameof(TFLink._frequency);
        readonly string scriptLabel = "m_Script";
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty(scriptLabel));
            EditorGUI.EndDisabledGroup();

            DrawPropertiesExcluding(serializedObject, frequencyLabel, scriptLabel);
            serializedObject.ApplyModifiedProperties();

        }
    }
}
using UnityEditor;
using UnitySensors.Sensor.TF;

namespace UnitySensors.ROS.Editor
{
    [CustomEditor(typeof(TFLink))]
    public class TFLinkEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.HelpBox("TFLink does not use \"Frequency\" param.", MessageType.Info);
        }
    }
}
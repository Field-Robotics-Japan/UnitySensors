using UnityEditor;
using UnitySensors.Sensor.Camera;

namespace UnitySensors.Editor
{
    [CustomEditor(typeof(FisheyeCameraSensor))]
    public class FisheyeCameraEditor : UnityEditor.Editor
    {
        SerializedProperty cameraModelProp;
        SerializedProperty alphaProp;
        SerializedProperty betaProp;
        SerializedProperty focalLengthProp;
        SerializedProperty principalPointProp;
        readonly string cameraModelLabel = nameof(FisheyeCameraSensor._cameraModel);
        readonly string alphaLabel = nameof(FisheyeCameraSensor._alpha);
        readonly string betaLabel = nameof(FisheyeCameraSensor._beta);
        readonly string focalLengthLabel = nameof(FisheyeCameraSensor._focalLength);
        readonly string principalPointLabel = nameof(FisheyeCameraSensor._principalPoint);
        readonly string fovLabel = nameof(FisheyeCameraSensor._fov);
        readonly string scriptLabel = "m_Script";

        void OnEnable()
        {
            cameraModelProp = serializedObject.FindProperty(cameraModelLabel);
            alphaProp = serializedObject.FindProperty(alphaLabel);
            betaProp = serializedObject.FindProperty(betaLabel);
            focalLengthProp = serializedObject.FindProperty(focalLengthLabel);
            principalPointProp = serializedObject.FindProperty(principalPointLabel);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty(scriptLabel));
            EditorGUI.EndDisabledGroup();

            DrawPropertiesExcluding(serializedObject,
                cameraModelLabel, alphaLabel, betaLabel, focalLengthLabel, principalPointLabel, fovLabel, scriptLabel);

            EditorGUILayout.PropertyField(cameraModelProp);
            if (cameraModelProp.enumValueIndex == (int)FisheyeCameraSensor.CameraModel.EUCM)
            {
                EditorGUILayout.PropertyField(alphaProp);
                EditorGUILayout.PropertyField(betaProp);
                EditorGUILayout.PropertyField(focalLengthProp);
                EditorGUILayout.PropertyField(principalPointProp);
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
}
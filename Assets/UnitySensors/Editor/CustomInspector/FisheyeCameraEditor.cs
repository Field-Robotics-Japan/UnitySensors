using UnityEditor;
using UnityEngine;
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
        SerializedProperty resolutionProp;
        readonly string cameraModelLabel = nameof(FisheyeCameraSensor._cameraModel);
        readonly string alphaLabel = nameof(FisheyeCameraSensor._alpha);
        readonly string betaLabel = nameof(FisheyeCameraSensor._beta);
        readonly string focalLengthLabel = nameof(FisheyeCameraSensor._focalLength);
        readonly string principalPointLabel = nameof(FisheyeCameraSensor._principalPoint);
        readonly string fovLabel = nameof(FisheyeCameraSensor._fov);
        readonly string resolutionLabel = nameof(FisheyeCameraSensor._resolution);
        readonly string scriptLabel = "m_Script";

        void OnEnable()
        {
            cameraModelProp = serializedObject.FindProperty(cameraModelLabel);
            alphaProp = serializedObject.FindProperty(alphaLabel);
            betaProp = serializedObject.FindProperty(betaLabel);
            focalLengthProp = serializedObject.FindProperty(focalLengthLabel);
            principalPointProp = serializedObject.FindProperty(principalPointLabel);
            resolutionProp = serializedObject.FindProperty(resolutionLabel);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty(scriptLabel));
            EditorGUI.EndDisabledGroup();

            DrawPropertiesExcluding(serializedObject,
                cameraModelLabel, alphaLabel, betaLabel, focalLengthLabel, principalPointLabel, fovLabel, resolutionLabel, scriptLabel);

            int resValue = Mathf.Max(0, EditorGUILayout.IntField("Resolution", resolutionProp.vector2IntValue.x));
            resolutionProp.vector2IntValue = new Vector2Int(resValue, resValue);

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
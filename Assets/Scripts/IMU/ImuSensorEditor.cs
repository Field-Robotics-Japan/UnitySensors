using UnityEditor;
using RosSharp.RosBridgeClient;

[CustomEditor(typeof(ImuSensor))]
public class ImuSensorEditor : Editor
{
    private ImuSensor _target;

    private void Awake()
    {
        _target = target as ImuSensor;
    }

    // inspectorのGUI設定
    public override void OnInspectorGUI()
    {
        _target.Topic = EditorGUILayout.TextField("Topic", _target.Topic);
        _target.FrameId = EditorGUILayout.TextField("FrameId", _target.FrameId);
        _target.EnableNoise = EditorGUILayout.ToggleLeft("EnableNoise", _target.EnableNoise);
        if (_target.EnableNoise)
        {
            _target.EnableBoxMullerNoise = EditorGUILayout.ToggleLeft("EnableBoxMullerNoise", _target.EnableBoxMullerNoise);
            if ( _target.EnableBoxMullerNoise )
            {
                _target.Setting.QuatSigma = EditorGUILayout.Vector4Field("Quaternion", _target.Setting.QuatSigma);
                _target.Setting.AngVelSigma = EditorGUILayout.Vector3Field("AngularVelocity", _target.Setting.AngVelSigma);
                _target.Setting.LinAccSigma = EditorGUILayout.Vector3Field("LinearAcceleration", _target.Setting.LinAccSigma);
            }
            _target.EnableBiasNoise = EditorGUILayout.ToggleLeft("EnableBiasNoise", _target.EnableBiasNoise);
            if ( _target.EnableBiasNoise )
            {
                EditorGUILayout.LabelField("BiasNoise");
                _target.Setting.QuatBias = EditorGUILayout.Vector4Field("Quaternion", _target.Setting.QuatBias);
                _target.Setting.AngVelBias = EditorGUILayout.Vector3Field("AngularVelocity", _target.Setting.AngVelBias);
                _target.Setting.LinAccBias = EditorGUILayout.Vector3Field("LinearAcceleration", _target.Setting.LinAccBias);
            }
        }
    }
}
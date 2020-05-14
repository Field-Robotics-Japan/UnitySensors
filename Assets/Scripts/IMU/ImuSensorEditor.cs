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
            EditorGUILayout.LabelField("Setting BoxMullerNoise");
            _target.Setting.Sigma = EditorGUILayout.DoubleField("標準偏差Σ", _target.Setting.Sigma);
        }
    }
}
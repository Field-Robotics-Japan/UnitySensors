using UnityEditor;
[CustomEditor(typeof(Noise))]
public class NoiseEditor : Editor
{
    private Noise _target;

    private void Awake()
    {
        _target = target as Noise;
    }

    public override void OnInspectorGUI()
    {
        _target.EnableColorShift = EditorGUILayout.ToggleLeft("EnableColorShift", _target.EnableColorShift);
        if (_target.EnableColorShift)
        {
            EditorGUILayout.LabelField("影の設定");
            _target.Setting.EffectColor = EditorGUILayout.ColorField("色", _target.Setting.EffectColor);
            _target.Setting.Distance = EditorGUILayout.Vector2Field("距離", _target.Setting.Distance);
            _target.Setting.UseAlpha = EditorGUILayout.Toggle("透過", _target.Setting.UseAlpha);
        }
    }

    public void hoge()
    {
        
    }
}
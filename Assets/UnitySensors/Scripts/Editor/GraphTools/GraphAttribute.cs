using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif


public class GraphAttribute : PropertyAttribute
{
    public int count;
    public GraphAttribute(int count)
    {
        this.count = count;
    }
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(GraphAttribute))]
public class GraphDrawer : PropertyDrawer
{
    private static readonly Color[] colors = { Color.red, Color.green, Color.blue, Color.cyan };
    private List<float[]> data;
    private int maxCount;
    private bool showGraph = true;


    protected void Setup(GraphAttribute attr)
    {
        data = new List<float[]>();
        maxCount = attr.count;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var type = property.propertyType;
        int dimentions = GetDimentions(type);
        if (dimentions <= 0)
        {
            throw new Exception("Unsupported property");
        }
        if (data == null)
        {
            Setup((GraphAttribute)attribute);
        }

        data.Add(GetRawValues(property));
        while (data.Count > maxCount)
        {
            data.RemoveAt(0);
        }

        // Draw Vector3
        EditorGUI.BeginProperty(position, label, property);
        {
            Rect rect = position;
            rect.height = 16;
            DrawPropertyField(rect, property, label);

            rect.y += 20f;
            showGraph = EditorGUI.Foldout(rect, showGraph, "Graph");
            if (showGraph)
            {
                rect.x += 50f;
                rect.y += 20f;
                rect.width -= 50f;
                rect.height = 200f;
                DrawGraph(rect, dimentions);
            }
        }
        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float height = base.GetPropertyHeight(property, label);
        height += showGraph ? 240 : 40;
        return height;
    }

    protected int GetDimentions(SerializedPropertyType type)
    {
        switch (type)
        {
            case SerializedPropertyType.Float:
                return 1;
            case SerializedPropertyType.Integer:
                return 1;
            case SerializedPropertyType.Vector2:
                return 2;
            case SerializedPropertyType.Vector3:
                return 3;
            case SerializedPropertyType.Vector4:
                return 4;
            case SerializedPropertyType.Quaternion:
                return 4;
            case SerializedPropertyType.Color:
                return 4;
            default:
                return -1;
        }
    }

    protected float[] GetRawValues(SerializedProperty property)
    {
        switch (property.propertyType)
        {
            case SerializedPropertyType.Float:
                return new float[] { property.floatValue };
            case SerializedPropertyType.Integer:
                return new float[] { (float)property.intValue };
            case SerializedPropertyType.Vector2:
                var v2 = property.vector2Value;
                return new float[] { v2[0], v2[1] };
            case SerializedPropertyType.Vector3:
                var v3 = property.vector3Value;
                return new float[] { v3[0], v3[1], v3[2] };
            case SerializedPropertyType.Vector4:
                var v4 = property.vector4Value;
                return new float[] { v4[0], v4[1], v4[2], v4[3] };
            case SerializedPropertyType.Quaternion:
                var q = property.quaternionValue;
                return new float[] { q[0], q[1], q[2], q[3] };
            case SerializedPropertyType.Color:
                var c = property.colorValue;
                return new float[] { c[0], c[1], c[2], c[3] };
            default:
                return null;
        }
    }

    protected void DrawPropertyField(Rect position, SerializedProperty property, GUIContent label)
    {
        switch (property.propertyType)
        {
            case SerializedPropertyType.Quaternion:
                var q = property.quaternionValue;
                Vector4 v4 = new Vector4(q[0], q[1], q[2], q[3]);
                EditorGUI.Vector4Field(position, label, v4);
                return;
        }
        EditorGUI.PropertyField(position, property);
    }

    protected void DrawGraph(Rect area, int dimentions)
    {
        // Rect area
        Handles.color = Color.white;
        Handles.DrawSolidRectangleWithOutline(area, new Color(0, 0, 0, 0.1f), Color.white);

        if (data.Count <= 0)
        {
            return;
        }

        // Draw Lines
        float max = data.Max(v => v.Max());
        float min = data.Min(v => v.Min());

        float dx = area.width / data.Count;
        float x0 = area.x;
        float y0 = area.y + area.height;

        // Draw min max
        EditorGUI.LabelField(new Rect(area.x - 50, area.y, 40, 16), string.Format("{0:f3}", max));
        EditorGUI.LabelField(new Rect(area.x - 50, area.y + area.height - 16, 40, 16), string.Format("{0:f3}", min));

        // Draw graph 
        for (int dim = 0; dim < dimentions; ++dim)
        {
            var values = new Vector3[data.Count];
            for (int i = 0; i < data.Count; ++i)
            {
                values[i] = new Vector3(
                    x0 + dx * i,
                    y0 - Mathf.InverseLerp(min, max, data[i][dim]) * area.height,
                    0
                );
            }
            Handles.color = colors[dim];
            Handles.DrawAAPolyLine(values.ToArray());
        }
    }
}
#endif
using UnityEngine;
using UnityEditor;

namespace UnitySensors.Attribute
{
    [CustomPropertyDrawer(typeof(InterfaceAttribute))]
    public class InterfaceTypeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            InterfaceAttribute ia = attribute as InterfaceAttribute;

            if (property.propertyType != SerializedPropertyType.ObjectReference) return;

            MonoBehaviour old = property.objectReferenceValue as MonoBehaviour;

            GameObject temp = null;
            string oldName = "";

            if (Event.current.type == EventType.Repaint)
            {
                if (old == null)
                {
                    temp = new GameObject("None [" + ia.type.Name + "]");
                    old = temp.AddComponent<Interface>();
                }
                else
                {
                    oldName = old.name;
                    old.name = oldName + " [" + ia.type.Name + "]";
                }
            }

            MonoBehaviour present = EditorGUI.ObjectField(position, label, old, typeof(MonoBehaviour), true) as MonoBehaviour;

            if (Event.current.type == EventType.Repaint)
            {
                if (temp != null)
                    GameObject.DestroyImmediate(temp);
                else
                    old.name = oldName;
            }

            if (old == present) return;

            if (present != null)
            {
                if (present.GetType() != ia.type)
                    present = present.gameObject.GetComponent(ia.type) as MonoBehaviour;

                if (present == null) return;
            }

            property.objectReferenceValue = present;
            property.serializedObject.ApplyModifiedProperties();
        }
    }

    public class Interface : MonoBehaviour
    {
    }
}
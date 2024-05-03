using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//This class contain custom drawer for ReadOnly attribute
[CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
public class ReadOnlyDrawer : PropertyDrawer
{
    //Unity method for drawing GUI in Editor
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var previousGUIState = GUI.enabled;
        GUI.enabled = false;
        EditorGUI.PropertyField(position, property, label);
        GUI.enabled = previousGUIState;
    }
}

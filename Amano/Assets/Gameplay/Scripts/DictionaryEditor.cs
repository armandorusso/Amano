/*
 Attempt at creating a Serializable Dictionary
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(FactsScriptableObject))]
[CanEditMultipleObjects]
public class DictionaryEditor : Editor
{
    private SerializedObject _serializedObject;
    private SerializedProperty dictionaryProperty;

    private void OnEnable()
    {
        _serializedObject = new SerializedObject(target);
        dictionaryProperty = _serializedObject.FindProperty("SOFacts");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        if (dictionaryProperty != null)
        {
            EditorGUILayout.LabelField("Dictionary:");
            EditorGUILayout.BeginVertical();
            {
                for (int i = 0; i < dictionaryProperty.arraySize; i++)
                {
                    SerializedProperty elementProperty = dictionaryProperty.GetArrayElementAtIndex(i);
                    SerializedProperty keyProperty = elementProperty.FindPropertyRelative("Key");
                    SerializedProperty valueProperty = elementProperty.FindPropertyRelative("Value");

                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.PropertyField(keyProperty, GUIContent.none);
                        EditorGUILayout.PropertyField(valueProperty, GUIContent.none);
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }
            EditorGUILayout.EndVertical();
        }
        serializedObject.ApplyModifiedProperties();
    }
}
*/

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SceneCompletion))]
public class SceneCompletionEditor : Editor
{
    SerializedProperty m_ScreenFaderProp;
    SerializedProperty m_PanelProp;
    SerializedProperty m_ZeroStarDirectorProp;
    SerializedProperty m_OneStarDirectorProp;
    SerializedProperty m_TwoStarDirectorProp;
    SerializedProperty m_ThreeStarDirectorProp;

    void OnEnable ()
    {
        m_ScreenFaderProp = serializedObject.FindProperty ("screenFader");
        m_PanelProp = serializedObject.FindProperty ("panel");
        m_ZeroStarDirectorProp = serializedObject.FindProperty ("zeroStarDirector");
        m_OneStarDirectorProp = serializedObject.FindProperty ("oneStarDirector");
        m_TwoStarDirectorProp = serializedObject.FindProperty ("twoStarDirector");
        m_ThreeStarDirectorProp = serializedObject.FindProperty ("threeStarDirector");
    }

    public override void OnInspectorGUI ()
    {
        serializedObject.Update ();

        EditorGUILayout.PropertyField (m_ScreenFaderProp);
        EditorGUILayout.PropertyField (m_PanelProp);
        EditorGUILayout.PropertyField (m_ZeroStarDirectorProp);
        EditorGUILayout.PropertyField (m_OneStarDirectorProp);
        EditorGUILayout.PropertyField (m_TwoStarDirectorProp);
        EditorGUILayout.PropertyField (m_ThreeStarDirectorProp);

        serializedObject.ApplyModifiedProperties ();
    }
}

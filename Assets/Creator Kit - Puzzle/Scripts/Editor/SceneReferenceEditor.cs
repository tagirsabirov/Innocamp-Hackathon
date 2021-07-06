using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SceneReference))]
public class SceneReferenceEditor : Editor
{
    SerializedProperty m_MenuSceneProp;
    SerializedProperty m_LevelSceneProp;
    SerializedProperty m_IgnoreProp;
    
    static readonly GUIContent k_MenuContent = new GUIContent("Menu", "A reference to the Scene asset where the Scene contains the menu this Level belongs to.");
    static readonly GUIContent k_LevelContent = new GUIContent ("Level", "The Level to be referenced by the above menu.");
    static readonly GUIContent k_IgnoreContent = new GUIContent("Ignore", "If this is checked then the scenes referenced will not be used.  Note that if a menu is referenced elsewhere it will still be used.");
    static readonly GUIContent k_ResetStarsButtonContent = new GUIContent("Reset All Level Stars", "Press this button to reset earned stars for every level in the project.");
    static readonly GUILayoutOption k_ButtonWidth = GUILayout.Width (130f);
    
    void OnEnable ()
    {
        m_MenuSceneProp = serializedObject.FindProperty ("menuScene");
        m_LevelSceneProp = serializedObject.FindProperty ("levelScene");
        m_IgnoreProp = serializedObject.FindProperty ("ignore");
    }

    public override void OnInspectorGUI ()
    {
        serializedObject.Update ();

        EditorGUILayout.PropertyField (m_MenuSceneProp, k_MenuContent);
        EditorGUILayout.PropertyField (m_LevelSceneProp, k_LevelContent);
        EditorGUILayout.PropertyField (m_IgnoreProp, k_IgnoreContent);
        
        EditorGUILayout.Space ();

        EditorGUILayout.BeginHorizontal ();
        GUILayout.FlexibleSpace ();
        if (GUILayout.Button (k_ResetStarsButtonContent, k_ButtonWidth))
        {
            ResetAllEarnedStars ();
        }
        EditorGUILayout.EndHorizontal ();
        
        serializedObject.ApplyModifiedProperties ();
    }

    static void ResetAllEarnedStars ()
    {
        List<SceneReference> sceneReferences = GetAllSceneReferences ();

        for (int i = 0; i < sceneReferences.Count; i++)
        {
            SerializedObject so = new SerializedObject (sceneReferences[i]);
            SerializedProperty starsProp = so.FindProperty ("earnedStars");
            starsProp.intValue = 0;
            so.ApplyModifiedProperties ();
        }
        
        Debug.Log ("All earned stars have been reset.");
    }
    
    public static List<SceneReference> GetAllSceneReferences ()
    {
        string[] guids = AssetDatabase.FindAssets ("t:SceneReference");
        List<SceneReference> sceneReferences = new List<SceneReference>(guids.Length);

        for (int i = 0; i < guids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath (guids[i]);
            sceneReferences[i] = AssetDatabase.LoadAssetAtPath<SceneReference> (path);
        }

        return sceneReferences;
    }
}

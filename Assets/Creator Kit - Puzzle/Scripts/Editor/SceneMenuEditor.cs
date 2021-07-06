using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.SceneManagement;

[CustomEditor(typeof(SceneMenu))]
public class SceneMenuEditor : Editor
{
    SceneMenu m_TargetSceneMenu;
    
    SerializedProperty m_AccessDeniedClipProp;
    SerializedProperty m_ScreenFaderProp;
    SerializedProperty m_SceneMenuIconPrefabProp;
    SerializedProperty m_ParentCanvasProp;
    SerializedProperty m_LevelsProp;

    ReorderableList m_ReorderableLevelsList;

    SceneAsset m_NewSceneReferenceLevel;

    static readonly GUIContent k_AccessDeniedClipProp = new GUIContent("Access Denied Clip");
    static readonly GUIContent k_ScreenFaderContent = new GUIContent("Screen Fader");
    static readonly GUIContent k_SceneMenuIconPrefabContent = new GUIContent("Scene Menu Icon Prefab");
    static readonly GUIContent k_ParentCanvasContent = new GUIContent("Parent Canvas");
    static readonly GUIContent k_LevelsContent = new GUIContent("Levels");
    static readonly GUIContent k_LevelContent = new GUIContent("Scene Reference", "The menu needs to know which levels to include.  For this project this is done using Scene References.");
    static readonly GUIContent k_DisplayNameContent = new GUIContent("Display Name", "The text that the menu will use to represent the level.");
    static readonly GUIContent k_TotalRequiredStarsContent = new GUIContent("Total Required Stars", "The total number of stars a player must have earned to have access to this level.");
    static readonly GUIContent k_OneStarTimeContent = new GUIContent("One Star Time", "The time the level must be completed in to earn one star.  This should be the longest time.");
    static readonly GUIContent k_TwoStarTimeContent = new GUIContent("Two Star Time", "The time the level must be completed in to earn one star.");
    static readonly GUIContent k_ThreeStarTimeContent = new GUIContent("Three Star Time", "The time the level must be completed in to earn one star.  This should be the shortest time.");
    static readonly GUIContent k_SortContent = new GUIContent("Sort", "Sort the levels so that they are in order from least to most stars required.");
    static readonly GUIContent k_CreateAndAddContent = new GUIContent("Create New Level:", "Click the button to the right to create a new Level, a Scene Reference for that level and add it to the levels list for this menu.");
    static readonly GUIContent k_CreateNewLevelButtonContent = new GUIContent("Create and Add", "Click to create a new Level, a Scene Reference for that level and add it to the levels list for this menu.");
    static readonly GUIContent k_SetupScenesForBuildContent = new GUIContent("Setup Scenes For Build:", "Click the button to the right to set all of the appropriate information before building your game.");
    static readonly GUIContent k_SetupScenesForBuildButtonContent = new GUIContent("Setup Scenes", "Click to set all of the appropriate information before building your game.");
    static readonly GUILayoutOption k_ButtonWidth = GUILayout.Width (100f);
    
    // IMPORTANT NOTE: if these paths don't exist, menu stuff won't work!
    const string k_NewSceneReferencePath = "Assets/Creator Kit - Puzzle/SceneReferences/UserCreated/newSceneReference.asset";
    const string k_NewLevelPath = "Assets/Creator Kit - Puzzle/Scenes/UserCreated/newLevel.unity";
    const string k_NewMenuPath = "Assets/Creator Kit - Puzzle/Scenes/UserCreated/newMenu.unity";
    const string k_DefaultLevelPath = "Assets/Creator Kit - Puzzle/Scenes/DefaultScenes/DefaultLevel.unity";
    const string k_DefaultMenuPath = "Assets/Creator Kit - Puzzle/Scenes/DefaultScenes/DefaultMenu.unity";

    void OnEnable ()
    {
        m_TargetSceneMenu = (SceneMenu)target;
        
        m_AccessDeniedClipProp = serializedObject.FindProperty ("accessDeniedClip");
        m_ScreenFaderProp = serializedObject.FindProperty ("screenFader");
        m_SceneMenuIconPrefabProp = serializedObject.FindProperty ("sceneMenuIconPrefab");
        m_ParentCanvasProp = serializedObject.FindProperty ("parentRectTransform");
        m_LevelsProp = serializedObject.FindProperty ("levels");
        
        m_ReorderableLevelsList = new ReorderableList (serializedObject, m_LevelsProp, true, true, false, true);

        m_ReorderableLevelsList.elementHeight = (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * 7f;

        m_ReorderableLevelsList.drawElementCallback = (rect, index, active, focused) =>
        {
            SerializedProperty levelInfoProp = m_ReorderableLevelsList.serializedProperty.GetArrayElementAtIndex (index);

            SerializedProperty levelProp = levelInfoProp.FindPropertyRelative ("level");
            SerializedProperty displayNameProp = levelInfoProp.FindPropertyRelative ("displayName");
            SerializedProperty totalStarsRequiredProp = levelInfoProp.FindPropertyRelative ("totalStarsRequired");
            SerializedProperty oneStarTimeProp = levelInfoProp.FindPropertyRelative ("oneStarTime");
            SerializedProperty twoStarTimeProp = levelInfoProp.FindPropertyRelative ("twoStarTime");
            SerializedProperty threeStarTimeProp = levelInfoProp.FindPropertyRelative ("threeStarTime");
            
            rect.height = EditorGUIUtility.singleLineHeight;
            string label = "Level";
            SceneReference sceneReference = levelProp.objectReferenceValue as SceneReference;
            if (sceneReference != null)
            {
                if (sceneReference.levelScene != null)
                {
                    label = sceneReference.levelScene.name;
                }
                else
                {
                    label = levelProp.objectReferenceValue.name;
                }
            }
            
            EditorGUI.LabelField (rect, label);
            rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            EditorGUI.indentLevel++;
            EditorGUI.PropertyField (rect, levelProp, k_LevelContent);
            rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            EditorGUI.PropertyField (rect, displayNameProp, k_DisplayNameContent);
            rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            EditorGUI.PropertyField (rect, totalStarsRequiredProp, k_TotalRequiredStarsContent);
            rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            EditorGUI.PropertyField (rect, oneStarTimeProp, k_OneStarTimeContent);
            rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            EditorGUI.PropertyField (rect, twoStarTimeProp, k_TwoStarTimeContent);
            rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            EditorGUI.PropertyField (rect, threeStarTimeProp, k_ThreeStarTimeContent);
            EditorGUI.indentLevel--;
        };

        m_ReorderableLevelsList.drawHeaderCallback = rect =>
        {
            EditorGUI.LabelField (rect, k_LevelsContent);
        };
    }

    public override void OnInspectorGUI ()
    {
        serializedObject.Update ();
        
        EditorGUILayout.PropertyField (m_AccessDeniedClipProp, k_AccessDeniedClipProp);
        EditorGUILayout.PropertyField (m_ScreenFaderProp, k_ScreenFaderContent);
        EditorGUILayout.PropertyField (m_SceneMenuIconPrefabProp, k_SceneMenuIconPrefabContent);
        EditorGUILayout.PropertyField (m_ParentCanvasProp, k_ParentCanvasContent);

        m_ReorderableLevelsList.DoLayoutList ();
        
        EditorGUILayout.Space ();

        EditorGUILayout.BeginHorizontal ();
        GUILayout.FlexibleSpace ();
        if (GUILayout.Button (k_SortContent, k_ButtonWidth))
        {
            SortLevels ();
        }
        EditorGUILayout.EndHorizontal ();
        
        EditorGUILayout.Space ();

        EditorGUILayout.BeginHorizontal ();
        EditorGUILayout.LabelField (k_CreateAndAddContent);
        if (GUILayout.Button (k_CreateNewLevelButtonContent, k_ButtonWidth))
        {
            CreateAndAddNewLevel ();
        }
        EditorGUILayout.EndHorizontal ();
        
        EditorGUILayout.BeginHorizontal ();
        EditorGUILayout.LabelField (k_SetupScenesForBuildContent);
        if (GUILayout.Button (k_SetupScenesForBuildButtonContent, k_ButtonWidth))
        {
            SceneReferencesBuildSetup.SceneSetup(m_TargetSceneMenu);
        }
        EditorGUILayout.EndHorizontal ();
        
        serializedObject.ApplyModifiedProperties ();
    }

    void SortLevels ()
    {
        if(m_LevelsProp.arraySize <= 1)
            return;

        bool didSwap;

        do
        {
            didSwap = false;
            
            for (int i = 1; i < m_LevelsProp.arraySize; i++)
            {
                SerializedProperty previousProp = m_LevelsProp.GetArrayElementAtIndex (i - 1);
                SerializedProperty currentProp = m_LevelsProp.GetArrayElementAtIndex (i);

                SerializedProperty previousStarsProp = previousProp.FindPropertyRelative ("totalStarsRequired");
                SerializedProperty currentStarsProp = currentProp.FindPropertyRelative ("totalStarsRequired");

                if (previousStarsProp.intValue > currentStarsProp.intValue)
                {
                    didSwap = true;
                
                    SerializedProperty previousLevelProp = previousProp.FindPropertyRelative ("level");
                    SerializedProperty previousIconProp = previousProp.FindPropertyRelative ("icon");
                    SerializedProperty previousOneStarProp = previousProp.FindPropertyRelative ("oneStarTime");
                    SerializedProperty previousTwoStarProp = previousProp.FindPropertyRelative ("twoStarTime");
                    SerializedProperty previousThreeStarProp = previousProp.FindPropertyRelative ("threeStarTime");

                    SerializedProperty currentLevelProp = currentProp.FindPropertyRelative ("level");
                    SerializedProperty currentIconProp = currentProp.FindPropertyRelative ("icon");
                    SerializedProperty currentOneStarProp = currentProp.FindPropertyRelative ("oneStarTime");
                    SerializedProperty currentTwoStarProp = currentProp.FindPropertyRelative ("twoStarTime");
                    SerializedProperty currentThreeStarProp = currentProp.FindPropertyRelative ("threeStarTime");
                
                    SceneReference tempSceneRef = previousLevelProp.objectReferenceValue as SceneReference;
                    Sprite tempIcon = previousIconProp.objectReferenceValue as Sprite;
                    int tempStars = previousStarsProp.intValue;
                    float tempOneStar = previousOneStarProp.floatValue;
                    float tempTwoStar = previousTwoStarProp.floatValue;
                    float tempThreeStar = previousThreeStarProp.floatValue;

                    previousLevelProp.objectReferenceValue = currentLevelProp.objectReferenceValue;
                    previousIconProp.objectReferenceValue = currentIconProp.objectReferenceValue;
                    previousStarsProp.intValue = currentStarsProp.intValue;
                    previousOneStarProp.floatValue = currentOneStarProp.floatValue;
                    previousTwoStarProp.floatValue = currentTwoStarProp.floatValue;
                    previousThreeStarProp.floatValue = currentThreeStarProp.floatValue;

                    currentLevelProp.objectReferenceValue = tempSceneRef;
                    currentIconProp.objectReferenceValue = tempIcon;
                    currentStarsProp.intValue = tempStars;
                    currentOneStarProp.floatValue = tempOneStar;
                    currentTwoStarProp.floatValue = tempTwoStar;
                    currentThreeStarProp.floatValue = tempThreeStar;
                }
            }
        }
        while (didSwap);
    }

    void CreateAndAddNewSceneReference (SceneAsset newSceneReferenceLevel)
    {
        if(newSceneReferenceLevel == null)
            return;

        SceneReference newSceneReference = CreateInstance<SceneReference> ();
        newSceneReference.levelScene = newSceneReferenceLevel;
        newSceneReference.menuScene = AssetDatabase.LoadAssetAtPath<SceneAsset> (((SceneMenu)target).gameObject.scene.path);

        string newSceneReferencePath = AssetDatabase.GenerateUniqueAssetPath (k_NewSceneReferencePath);
        AssetDatabase.CreateAsset (newSceneReference, newSceneReferencePath);
        
        string newLevelPath = AssetDatabase.GetAssetOrScenePath (newSceneReferenceLevel);
        Scene newLevelScene = EditorSceneManager.OpenScene (newLevelPath, OpenSceneMode.Additive);
        
        SceneCompletion sceneCompletion = FindObjectOfType<SceneCompletion> ();
        sceneCompletion.sceneReference = newSceneReference;
        
        EditorUtility.SetDirty (sceneCompletion);
        EditorSceneManager.SaveScene (newLevelScene);
        EditorSceneManager.CloseScene (newLevelScene, true);
        
        m_LevelsProp.arraySize++;
        m_LevelsProp.GetArrayElementAtIndex (m_LevelsProp.arraySize - 1).FindPropertyRelative ("level").objectReferenceValue = newSceneReference;
    }

    void CreateAndAddNewLevel ()
    {
        if(AssetDatabase.LoadAssetAtPath<SceneAsset> (k_DefaultLevelPath) == null)
        {
            EditorGUILayout.EndHorizontal ();
            throw new UnityException("The default level was not found at " + k_DefaultLevelPath + " and so a new level could not be copied from it.");
        }
        
        string newLevelPath = AssetDatabase.GenerateUniqueAssetPath (k_NewLevelPath);
        bool successful = AssetDatabase.CopyAsset (k_DefaultLevelPath, newLevelPath);

        if (successful)
        {
            SceneAsset newLevel = AssetDatabase.LoadAssetAtPath<SceneAsset> (newLevelPath);
            CreateAndAddNewSceneReference (newLevel);
            
            EditorGUIUtility.PingObject (newLevel);
        }
        else
        {
            EditorGUILayout.EndHorizontal ();
            throw new UnityException("The default level was not successfully copied so a new Scene Reference could not be created and added to the list of levels.");
        }
    }
    
    [MenuItem("Puzzle Kit Tools/Create Menu")]
    static void CreateNewMenu ()
    {
        if(AssetDatabase.LoadAssetAtPath<SceneAsset> (k_DefaultMenuPath) == null)
        {
            throw new UnityException("The default menu was not found at " + k_DefaultMenuPath + " and so a new menu could not be copied from it.");
        }
        
        string newMenuPath = AssetDatabase.GenerateUniqueAssetPath (k_NewMenuPath);
        bool successful = AssetDatabase.CopyAsset (k_DefaultMenuPath, newMenuPath);

        if (successful)
        {
            SceneAsset newLevel = AssetDatabase.LoadAssetAtPath<SceneAsset> (newMenuPath);
            EditorGUIUtility.PingObject (newLevel);
        }
        else
        {
            throw new UnityException("The default menu was not successfully copied.");
        }
    }
}

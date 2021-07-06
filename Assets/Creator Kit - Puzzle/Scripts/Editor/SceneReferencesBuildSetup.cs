using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
public static class SceneReferencesBuildSetup
{
    static SceneReferencesBuildSetup ()
    {
        // Always listen for play mode state changed.
        EditorApplication.playModeStateChanged += PlayModeStateChanged;
    }
    
    // Called when the play button is pressed.
    static void PlayModeStateChanged (PlayModeStateChange playModeStateChange)
    {
        // If the play mode is exiting edit mode then the scenes need to be setup.
        if (playModeStateChange == PlayModeStateChange.ExitingEditMode)
        {
            // Get the paths to the scene assets and the scene reference assets for the menu/levels combos based on what is currently loaded.
            List<string> uniqueScenePaths = GetSceneReferencesFromActiveScene(out List<SceneReference> sceneReferences);

            // Use the paths to the scene assets to set the scenes in the build settings.
            SetBuildSettingsScenes(uniqueScenePaths);

            // Set the data on the scene reference assets such as scene indexes.
            SetAllSceneReferenceInfo(sceneReferences);
        }
    }

    static List<string> GetSceneReferencesFromActiveScene(out List<SceneReference> sceneReferences)
    {
        // Look for a SceneMenu in the scene.
        SceneMenu sceneMenu = Object.FindObjectOfType<SceneMenu>();
        if (sceneMenu != null)
            // If there is one, just use the method which takes a reference to the SceneMenu.
            return GetSceneReferencesFromSceneMenu(out sceneReferences, sceneMenu);

        // Otherwise initialise the variables to be returned.
        List<string> uniqueScenePaths = new List<string>();
        sceneReferences = new List<SceneReference>();
        
        // Look for the SceneCompletion component.
        SceneCompletion sceneCompletion = Object.FindObjectOfType<SceneCompletion>();
        
        if(sceneCompletion == null)
            throw new UnityException("Neither a SceneMenu or a SceneCompletion component were found in this Scene. No Scene setup can be done.");
        
        // Find the menu for this level.
        SceneAsset menuScene = sceneCompletion.sceneReference.menuScene;
        
        if(menuScene == null)
            throw new UnityException("No reference to the menu scene for this level was found. No Scene setup can be done.");
        
        // Add a path to the menu first so it can be added to the build settings and will be the first opened for a build.
        uniqueScenePaths.Add(AssetDatabase.GetAssetOrScenePath(menuScene));

        // Get all SceneReference assets so they can be checked.
        List<SceneReference> allSceneReferences = SceneReferenceEditor.GetAllSceneReferences();

        for (int i = 0; i < allSceneReferences.Count; i++)
        {
            // Find all the SceneReference assets which use this menu scene.
            if (allSceneReferences[i].menuScene == menuScene)
            {
                // Add those SceneReferences to the list and get paths to them so they can be added to the build settings.
                sceneReferences.Add(allSceneReferences[i]);
                uniqueScenePaths.Add(AssetDatabase.GetAssetOrScenePath(allSceneReferences[i].levelScene));
            }
        }
        
        return uniqueScenePaths;
    }

    // Called by a GUI button on the SceneMenu component.
    public static void SceneSetup(SceneMenu sceneMenu)
    {
        // Get the paths to the scene assets and the scene reference assets for the menu/levels combos based on what is currently loaded.
        List<string> uniqueScenePaths = GetSceneReferencesFromSceneMenu(out List<SceneReference> sceneReferences, sceneMenu);
        
        // Use the paths to the scene assets to set the scenes in the build settings.
        SetBuildSettingsScenes(uniqueScenePaths);

        // Set the data on the scene reference assets such as scene indexes.
        SetAllSceneReferenceInfo(sceneReferences);
    }

    static List<string> GetSceneReferencesFromSceneMenu(out List<SceneReference> sceneReferences, SceneMenu sceneMenu)
    {
        // Initialise the returned variables.
        List<string> uniqueScenePaths = new List<string>();
        sceneReferences = new List<SceneReference>();

        // Find the path to the menu scene.
        string menuPath = sceneMenu.gameObject.scene.path;
        
        // Add the path to the menu scene first so it appears first when built.
        uniqueScenePaths.Add(menuPath);
        
        // Go through all the levels in the SceneMenu and add them to the return variables.
        for (int i = 0; i < sceneMenu.levels.Count; i++)
        {
            SceneReference sceneReference = sceneMenu.levels[i].level;
            
            uniqueScenePaths.Add (AssetDatabase.GetAssetOrScenePath (sceneReference.levelScene));
            sceneReferences.Add(sceneReference);
        }

        return uniqueScenePaths;
    }

    static void SetBuildSettingsScenes(List<string> uniqueScenePaths)
    {
        // Create an array of EditorBuildSettingsScenes for each of the scene paths.
        EditorBuildSettingsScene[] scenes = new EditorBuildSettingsScene[uniqueScenePaths.Count];

        for (int i = 0; i < scenes.Length; i++)
        {
            // Create each EditorBuildSettingsScene based on the path.
            scenes[i] = new EditorBuildSettingsScene(uniqueScenePaths[i], true);
        }

        // Set the Build Settings to use those scenes.
        EditorBuildSettings.scenes = scenes;
    }

    static void SetAllSceneReferenceInfo(List<SceneReference> sceneReferences)
    {
        // Go through all the selected SceneReferences.
        for (int i = 0; i < sceneReferences.Count; i++)
        {
            // Find the paths to the level and menu.
            string levelPath = AssetDatabase.GetAssetOrScenePath(sceneReferences[i].levelScene);
            string menuPath = AssetDatabase.GetAssetOrScenePath(sceneReferences[i].menuScene);
            
            // Go through all the scenes to be built.
            for (int j = 0; j < EditorBuildSettings.scenes.Length; j++)
            {
                // If the path to the scene matches the one from the SceneReference then cache the index.
                if (EditorBuildSettings.scenes[j].path == levelPath)
                    sceneReferences[i].levelBuildIndex = j;
                else if (EditorBuildSettings.scenes[j].path == menuPath)
                    sceneReferences[i].menuBuildIndex = j;
            }
        }
    }
}

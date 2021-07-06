using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[InitializeOnLoad]
public static class DefaultSetup
{
    const string k_NewSceneFolderParent = "Assets/Creator Kit - Puzzle/Scenes";
    const string k_NewSceneFolderName = "UserCreated";
    const string k_NewSceneFolder = k_NewSceneFolderParent + "/" + k_NewSceneFolderName;

    const string k_NewSceneReferenceFolderParent = "Assets/Creator Kit - Puzzle/SceneReferences";
    const string k_NewSceneReferenceFolderName = "UserCreated";
    const string k_NewSceneReferenceFolder = k_NewSceneReferenceFolderParent + "/" + k_NewSceneReferenceFolderName;
    
    static DefaultSetup ()
    {
        EditorApplication.update += FirstFrameUpdate;
    }

    static void FirstFrameUpdate ()
    {
        bool skyboxUpdated = UpdateSceneViewSkybox ();
        CheckAndCreateFolders ();

        if(skyboxUpdated)
            EditorApplication.update -= FirstFrameUpdate;
    }

    static bool UpdateSceneViewSkybox ()
    {
        if (StageUtility.GetCurrentStageHandle () != StageUtility.GetMainStageHandle ())
            return false;

        SceneView sceneView = SceneView.lastActiveSceneView;

        if (sceneView == null)
            return false;
        
        SceneView.SceneViewState state = sceneView.sceneViewState;
        state.showSkybox = false;
        sceneView.sceneViewState = state;

        return true;
    }

    static void CheckAndCreateFolders ()
    {
        if (!AssetDatabase.IsValidFolder (k_NewSceneFolder))
        {
            AssetDatabase.CreateFolder (k_NewSceneFolderParent, k_NewSceneFolderName);
        }
        
        if (!AssetDatabase.IsValidFolder (k_NewSceneReferenceFolder))
        {
            AssetDatabase.CreateFolder (k_NewSceneReferenceFolderParent, k_NewSceneReferenceFolderName);
        }
    }
}

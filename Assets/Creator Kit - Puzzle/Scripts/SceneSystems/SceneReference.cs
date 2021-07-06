using System;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class SceneReference : ScriptableObject
{
    public int levelBuildIndex;    // BUG: this is 0 in builds. needs some code to set it before building...
    public int menuBuildIndex;
    public int earnedStars;
    public float oneStarTime;
    public float twoStarTime;
    public float threeStarTime;
    
#if UNITY_EDITOR
    public SceneAsset levelScene;
    public SceneAsset menuScene;
#endif

    Action m_LoadLevelAction;
    Action m_LoadMenuAction;

    public void UpdateActions ()
    {
        m_LoadLevelAction = () => SceneManager.LoadSceneAsync (levelBuildIndex, LoadSceneMode.Single);
        m_LoadMenuAction = () => SceneManager.LoadSceneAsync (menuBuildIndex, LoadSceneMode.Single);
    }

    public void LoadMenu (ScreenFader screenFader)
    {
        if(m_LoadMenuAction == null)
            UpdateActions ();
        
        screenFader.FadeOut(m_LoadMenuAction);
    }

    public void ReloadLevel (ScreenFader screenFader)
    {
        if(m_LoadLevelAction == null)
            UpdateActions ();
        
        screenFader.FadeOut(m_LoadLevelAction);
    }
}
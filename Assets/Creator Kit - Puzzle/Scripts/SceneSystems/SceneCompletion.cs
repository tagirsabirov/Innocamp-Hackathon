using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneCompletion : MonoBehaviour
{
    public ScreenFader screenFader;
    public GameObject panel;
    public PlayableDirector zeroStarDirector;
    public PlayableDirector oneStarDirector;
    public PlayableDirector twoStarDirector;
    public PlayableDirector threeStarDirector;
    public SceneReference sceneReference;        // Note that this is assigned automatically when the level is created by the SceneMenuEditor class.
    
    // Called when target of level is achieved
    public void CompleteLevel (float time)
    {
        panel.SetActive (true);
        
        int earnedStars = 0;

        if (time <= sceneReference.threeStarTime)
        {
            earnedStars = 3;
            threeStarDirector.Play();
        }
        else if (time <= sceneReference.twoStarTime)
        {
            earnedStars = 2;
            twoStarDirector.Play();
        }
        else if (time <= sceneReference.oneStarTime)
        {
            earnedStars = 1;
            oneStarDirector.Play();
        }
        else
        {
            zeroStarDirector.Play();
        }
        
        if(sceneReference.earnedStars < earnedStars)
            sceneReference.earnedStars = earnedStars;
    }

    // UI Button
    public void ReloadLevel ()
    {
        if (sceneReference != null)
        {
            sceneReference.ReloadLevel (screenFader);
        }
        else
        {
            screenFader.FadeOut(() => SceneManager.LoadSceneAsync (gameObject.scene.buildIndex));
        }
    }

    // UI Button
    public void LoadMenu ()
    {
        if (sceneReference != null)
        {
            sceneReference.LoadMenu (screenFader);
        }
    }
}
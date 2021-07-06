using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
public class SceneMenu : MonoBehaviour
{
    [Serializable]
    public struct LevelInfo
    {
        public SceneReference level;
        public string displayName;
        public int totalStarsRequired;
        public float oneStarTime;
        public float twoStarTime;
        public float threeStarTime;

        public bool LoadLevel (ScreenFader screenFader, int totalEarnedStars)
        {
            if (totalEarnedStars >= totalStarsRequired)
            {
                level.ReloadLevel (screenFader);
                return true;
            }

            return false;
        }
    }


    public AudioClip accessDeniedClip;
    public ScreenFader screenFader;
    public SceneMenuIcon sceneMenuIconPrefab;
    public RectTransform parentRectTransform;
    public List<LevelInfo> levels = new List<LevelInfo> ();
    
    List<SceneMenuIcon> m_Icons = new List<SceneMenuIcon> ();

    const string k_AssetReferencerName = "AssetReferencer";

    void Start ()
    {
        CreateGrid ();

        int totalEarnedStars = GetTotalEarnedStars ();
        for (int i = 0; i < m_Icons.Count; i++)
        {
            m_Icons[i].UpdateMenuUI (totalEarnedStars, levels[i].totalStarsRequired, levels[i].level.earnedStars);
        }

        for (int i = 0; i < levels.Count; i++)
        {
            levels[i].level.UpdateActions ();
            levels[i].level.oneStarTime = levels[i].oneStarTime;
            levels[i].level.twoStarTime = levels[i].twoStarTime;
            levels[i].level.threeStarTime = levels[i].threeStarTime;
        }

        if(GameObject.Find (k_AssetReferencerName) != null)
            return;
        
        GameObject assetReferencerGo = new GameObject("AssetReferencer");
        AssetReferencer assetReferencer = assetReferencerGo.AddComponent<AssetReferencer> ();
        assetReferencer.assets = new ScriptableObject[levels.Count];

        for (int i = 0; i < assetReferencer.assets.Length; i++)
        {
            assetReferencer.assets[i] = levels[i].level;
        }
    }

    void CreateGrid ()
    {
        if(levels.Count <= 0)
            throw new UnityException("There are no Scene References assigned to construct a menu from.  Make sure you have created some Scene Reference assets and assigned a level Scene and this menu Scene to them.");
        
        Rect menuSpaceRect = parentRectTransform.rect;
        float menuSpaceWidth = menuSpaceRect.width;
        float menuSpaceHeight = menuSpaceRect.height;

        float sqrt = Mathf.Sqrt (levels.Count);
        int gridX = Mathf.CeilToInt (sqrt);
        int gridY = gridX * (gridX - 1) > levels.Count ? gridX - 1 : gridX;
        float iconAreaSizeX = 1f / gridX;
        float iconAreaSizeY = 1f / gridY;
        
        float iconAreaPixelSizeX = iconAreaSizeX * menuSpaceWidth;
        float iconAreaPixelSizeY = iconAreaSizeY * menuSpaceHeight;

        Vector2 freeSpacePerGridPerSide = Vector2.zero;
        
        if (iconAreaPixelSizeX < iconAreaPixelSizeY)
            freeSpacePerGridPerSide.y = (iconAreaPixelSizeY - iconAreaPixelSizeX) * 0.5f / menuSpaceHeight;
        else
            freeSpacePerGridPerSide.x = (iconAreaPixelSizeX - iconAreaPixelSizeY) * 0.5f / menuSpaceWidth;

        for (int i = 0; i < gridY; i++)
        {
            for (int j = 0; j < gridX; j++)
            {
                int sceneIndex = i * gridX + j;
                
                if(sceneIndex > levels.Count - 1)
                    return;
                
                SceneMenuIcon sceneMenuIcon = Instantiate (sceneMenuIconPrefab, parentRectTransform);

                sceneMenuIcon.sceneReference = levels[sceneIndex].level;
                sceneMenuIcon.sceneMenu = this;
                sceneMenuIcon.textMesh.text = levels[sceneIndex].displayName;
                
                Rect anchors = new Rect();
                anchors.min = new Vector2(j * iconAreaSizeX, 1f - (i + 1) * iconAreaSizeY);
                anchors.max = new Vector2((j + 1) * iconAreaSizeX, 1f - i * iconAreaSizeY);

                anchors.min += freeSpacePerGridPerSide;
                anchors.max -= freeSpacePerGridPerSide;

                sceneMenuIcon.parentRectTransform.anchorMin = anchors.min;
                sceneMenuIcon.parentRectTransform.anchorMax = anchors.max;
                sceneMenuIcon.parentRectTransform.anchoredPosition = Vector2.zero;
                sceneMenuIcon.parentRectTransform.sizeDelta = Vector2.zero;
                
                m_Icons.Add (sceneMenuIcon);
            }
        }
    }
    
    public int GetTotalEarnedStars ()
    {
        int totalEarnedStars = 0;
        for (int i = 0; i < levels.Count; i++)
        {
            totalEarnedStars += levels[i].level.earnedStars;
        }

        return totalEarnedStars;
    }

    public void LoadLevel (SceneReference sceneReference)
    {
        for (int i = 0; i < levels.Count; i++)
        {
            if (levels[i].level == sceneReference)
            {
                if(!levels[i].LoadLevel (screenFader, GetTotalEarnedStars ()))
                    AudioSource.PlayClipAtPoint (accessDeniedClip, Vector3.zero);
                return;
            }
        }
        
        throw new UnityException("The provided SceneReference was either null or could not be found.");
    }
}
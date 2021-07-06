using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SceneMenuIcon : MonoBehaviour
{
    public RectTransform parentRectTransform;
    public TextMeshProUGUI textMesh;
    public GameObject unavailableGameObject;
    public GameObject availableGameObject;
    public Image starOne;
    public Image starTwo;
    public Image starThree;
    [HideInInspector]
    public SceneReference sceneReference;
    [HideInInspector]
    public SceneMenu sceneMenu;

    public void UpdateMenuUI (int totalEarnedStars, int requiredStars, int earnedStars)
    {
        availableGameObject.SetActive (totalEarnedStars >= requiredStars);
        unavailableGameObject.SetActive (totalEarnedStars < requiredStars);

        starOne.enabled = earnedStars >= 1;
        starTwo.enabled = earnedStars >= 2;
        starThree.enabled = earnedStars >= 3;
    }

    public void LoadLevel ()
    {
        sceneMenu.LoadLevel (sceneReference);
    }
}

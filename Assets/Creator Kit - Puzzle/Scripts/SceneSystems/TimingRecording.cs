using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class TimingRecording : MonoBehaviour
{
    public KeyCode resetKeyCode = KeyCode.R;
    public Rigidbody startingMarble;
    public SceneCompletion sceneCompletion;
    public TextMeshProUGUI textMesh;
    public Action enableControlAction;
    [HideInInspector]
    public float timer;

    bool m_IsTiming;
    BaseInteractivePuzzlePiece[] m_PuzzlePieces;

    void Awake ()
    {
        m_PuzzlePieces = FindObjectsOfType<BaseInteractivePuzzlePiece> ();
        
        enableControlAction = EnableControl;
    }

    void EnableControl ()
    {
        startingMarble.isKinematic = false;
        m_IsTiming = true;
        for (int i = 0; i < m_PuzzlePieces.Length; i++)
        {
            m_PuzzlePieces[i].EnableControl ();
        }
    }

    void Update ()
    {
        if(m_IsTiming)
            timer += Time.deltaTime;

        textMesh.text = timer.ToString ("0.00");
        
        if(Input.GetKeyDown (resetKeyCode))
            sceneCompletion.ReloadLevel ();
    }

    public void GoalReached (float uiDelay)
    {
        m_IsTiming = false;
        StartCoroutine (CompleteLevelWithDelay (uiDelay));
    }

    IEnumerator CompleteLevelWithDelay (float delay)
    {
        yield return new WaitForSeconds (delay);
        sceneCompletion.CompleteLevel (timer);
    }
}

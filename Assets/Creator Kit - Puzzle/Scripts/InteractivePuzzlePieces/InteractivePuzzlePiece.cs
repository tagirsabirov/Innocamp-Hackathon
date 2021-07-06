using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractivePuzzlePiece<TComponent> : BaseInteractivePuzzlePiece
where TComponent : Component
{
    public TComponent physicsComponent;
}


public abstract class BaseInteractivePuzzlePiece : MonoBehaviour
{
    public KeyCode interactKey = KeyCode.Space;
    public Rigidbody rb;
    public AudioClip activateSound;
    public AudioClip deactivateSound;
    public AudioSource puzzleAudioSource;

    bool m_IsControlable;
    
    protected void FixedUpdate ()
    {
        if (Input.GetKey (interactKey) && m_IsControlable)
        {
            ApplyActiveState ();
        }
        else
        {
            ApplyInactiveState ();
        }
    }
    
    void Update()
    {
        if (deactivateSound != null && Input.GetKeyUp(interactKey))
        {
            puzzleAudioSource.pitch = Random.Range(0.8f, 1.2f);
            puzzleAudioSource.PlayOneShot(deactivateSound);
        }
        if (activateSound != null && Input.GetKeyDown(interactKey))
        {
            puzzleAudioSource.pitch = Random.Range(0.8f, 1.2f);
            puzzleAudioSource.PlayOneShot(activateSound);
        }
    }

    protected abstract void ApplyActiveState ();

    protected abstract void ApplyInactiveState ();

    public void EnableControl ()
    {
        m_IsControlable = true;
    }
}
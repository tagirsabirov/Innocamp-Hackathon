using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider)), RequireComponent(typeof(AudioSource))]
public class TargetTrigger : MonoBehaviour
{
    public float uiDelay = 3f;
    public Collider marble;
    public TimingRecording timingRecording;
    public TargetGroupWeightControl targetGroupWeightControl;
    public ParticleSystem completeParticleSystem;

    AudioSource m_AudioSource;

    void Awake ()
    {
        m_AudioSource = GetComponent<AudioSource> ();
    }

    void OnTriggerEnter (Collider other)
    {
        if (other == marble)
        {
            completeParticleSystem.Play();
            timingRecording.GoalReached (uiDelay);
            targetGroupWeightControl.ApplySpecificFocus (marble.attachedRigidbody);
            m_AudioSource.PlayOneShot (m_AudioSource.clip);
        }
    }
}

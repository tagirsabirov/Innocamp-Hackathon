using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;


public class ScreenFader : MonoBehaviour
{
    public float fadeDuration = 1f;
    public CanvasGroup canvasGroup;
    public TimingRecording timingRecording;
    public AudioMixer audioMixer;

    float m_FadeSpeed;
    bool m_IsFading;
    
    readonly AudioMixerSnapshot[] m_Snapshots = new AudioMixerSnapshot[2];
    readonly float[] m_Weights = new float[2];

    void Awake ()
    {
        canvasGroup.alpha = 1f;
        
        m_FadeSpeed = 1f / fadeDuration;
        
        m_Snapshots[0] = audioMixer.FindSnapshot ("Normal");
        m_Snapshots[1] = audioMixer.FindSnapshot ("Silent");

        m_Weights[0] = 1f;
    }

    void Start ()
    {
        if (timingRecording == null)
            FadeIn ();
        else
            FadeIn (timingRecording.enableControlAction);
    }

    public void FadeOut (Action afterFade)
    {
        if(!m_IsFading)
            StartCoroutine (Fade (1f, afterFade));
    }

    public void FadeOut ()
    {
        if(!m_IsFading)
            StartCoroutine (Fade (1f, null));
    }

    public void FadeIn (Action afterFade)
    {
        if(!m_IsFading)
            StartCoroutine (Fade (0f, afterFade));
    }

    public void FadeIn ()
    {
        if(!m_IsFading)
            StartCoroutine (Fade (0f, null));
    }

    IEnumerator Fade (float finalAlpha, Action afterFade)
    {
        m_IsFading = true;
        
        m_Weights[0] = 1f - finalAlpha;
        m_Weights[1] = finalAlpha;
        audioMixer.TransitionToSnapshots (m_Snapshots, m_Weights, fadeDuration);
        
        canvasGroup.blocksRaycasts = true;
        
        while (!Mathf.Approximately(canvasGroup.alpha, finalAlpha))
        {
            canvasGroup.alpha = Mathf.MoveTowards (canvasGroup.alpha, finalAlpha, m_FadeSpeed * Time.deltaTime);
            yield return null;
        }

        canvasGroup.alpha = finalAlpha;
        canvasGroup.blocksRaycasts = false;
        
        afterFade?.Invoke ();

        m_IsFading = false;
    }
}

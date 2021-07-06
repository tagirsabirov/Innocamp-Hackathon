using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;


public class TargetGroupWeightControl : MonoBehaviour
{
    [Header("Weight Control Settings")]
    public float weightDamping = 1f;
    public AnimationCurve speedToWeightCurve = new AnimationCurve(new Keyframe(0.1f, 0f), new Keyframe(1f, 1f, 3f, 3f));
    [Header("Post Processing Settings")]
    public PostProcessVolume postProcessVolume;
    public AnimationCurve averageToFocusDistanceCurve = AnimationCurve.Linear (0f, 0f, 20f, 20f);
    public AnimationCurve rangeToApertureCurve = AnimationCurve.Linear (0f, 0.65f, 6f, 1f);

    Rigidbody m_FocusTarget;
    Transform m_Camera;
    CinemachineTargetGroup m_TargetGroup;
    Rigidbody[] m_TargetRigidbodies;
    FloatParameter m_FocusDistanceParameter;
    FloatParameter m_ApertureParameter;

    void Awake ()
    {
        m_Camera = FindObjectOfType<Camera> ().transform;
        m_TargetGroup = GetComponent<CinemachineTargetGroup> ();

        for (int i = 0; i < m_TargetGroup.m_Targets.Length; i++)
        {
            m_TargetGroup.m_Targets[i].weight = i == 0 ? 1f : 0f;
        }
        
        m_TargetRigidbodies = new Rigidbody[m_TargetGroup.m_Targets.Length];
        for (int i = 0; i < m_TargetRigidbodies.Length; i++)
        {
            m_TargetRigidbodies[i] = m_TargetGroup.m_Targets[i].target.GetComponent<Rigidbody> ();
        }

        DepthOfField depthOfField = postProcessVolume.profile.GetSetting<DepthOfField> ();
        m_FocusDistanceParameter = depthOfField.focusDistance;
        m_FocusDistanceParameter.overrideState = true;
        m_ApertureParameter = depthOfField.aperture;
        m_ApertureParameter.overrideState = true;
    }

    void Update ()
    {
        for (int i = 0; i < m_TargetRigidbodies.Length; i++)
        {
            float weight;
            if (m_FocusTarget == null)
            {
                weight = speedToWeightCurve.Evaluate (m_TargetRigidbodies[i].velocity.magnitude);
            }
            else
            {
                weight = m_TargetRigidbodies[i] == m_FocusTarget ? 1f : 0f;
            }
            weight = Mathf.Clamp01 (weight);
            m_TargetGroup.m_Targets[i].weight = Mathf.MoveTowards (m_TargetGroup.m_Targets[i].weight, weight, weightDamping * Time.deltaTime);
        }
        
        m_FocusDistanceParameter.value = averageToFocusDistanceCurve.Evaluate (m_Camera.InverseTransformPoint(m_TargetGroup.Sphere.position).z);
        m_ApertureParameter.value = rangeToApertureCurve.Evaluate (m_TargetGroup.Sphere.radius * 2f);
    }

    public void ApplySpecificFocus (Rigidbody focusTarget)
    {
        m_FocusTarget = focusTarget;
    }
}

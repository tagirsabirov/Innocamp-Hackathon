using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationalVelocityAudio : MonoBehaviour
{
    public AudioSource rotationAudioSource;
    public new Rigidbody rigidbody;
    
    static readonly AudioAdjustmentSettings k_Volume = new AudioAdjustmentSettings(0.5f, 0f, 0.4f, 5f);
    static readonly AudioAdjustmentSettings k_Pitch = new AudioAdjustmentSettings(1f, 0.8f, 2f, 5f);

    void FixedUpdate()
    {
        float rotationSpeed = rigidbody.angularVelocity.magnitude;
        rotationAudioSource.volume = AudioAdjustmentSettings.ClampAndInterpolate (rotationAudioSource.volume, rotationSpeed, k_Volume);
        rotationAudioSource.pitch = AudioAdjustmentSettings.ClampAndInterpolate (rotationAudioSource.pitch, rotationSpeed, k_Pitch);
    }
}

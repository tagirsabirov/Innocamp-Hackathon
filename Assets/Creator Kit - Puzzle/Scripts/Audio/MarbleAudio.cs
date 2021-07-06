using UnityEngine;

public class MarbleAudio : MonoBehaviour
{
    public LayerMask plasticMask;
    public LayerMask concreteMask;
    public LayerMask metalMask;
    public float distanceToGround;
    public AudioSource movementAudioSource;
    public AudioSource rollingAudioSource;
    public AudioSource impactAudioSource;
    public AudioClip plasticImpact;
    public AudioClip metalImpact;
    public AudioClip concreteImpact;

    bool m_IsGrounded;
    float m_Speed;
    Rigidbody m_Rigidbody;

    static readonly AudioAdjustmentSettings k_GroundedRollingVolume = new AudioAdjustmentSettings (1f / 1.5f, 0f, 1f, 25f );
    static readonly AudioAdjustmentSettings k_GroundedRollingPitch = new AudioAdjustmentSettings (1f / 1.2f, 0.5f, 2f, 15f);
    static readonly AudioAdjustmentSettings k_MovementVolume = new AudioAdjustmentSettings(1f / 7f, 0f, 0.7f, 70f);
    static readonly AudioAdjustmentSettings k_MovementPitch = new AudioAdjustmentSettings(1f / 7f, 0f, 1f, 70f);
    static readonly AudioAdjustmentSettings k_ImpactPitch = new AudioAdjustmentSettings(1f / 1.2f, 0.8f, 2f, float.PositiveInfinity);

    const float k_AirborneRollingTargetVolume = 0f;
    const float k_AirborneRollingVolumeChangeRate = 15f;
    
    const float k_AirborneRollingTargetPitch = 2f;
    const float k_AirborneRollingPitchChangeRate = 15f;
    
    const float k_ImpactSpeedToVolume = 1f / 3f;

    void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
    }
    
    void FixedUpdate()
    {
        m_IsGrounded = Physics.Raycast(m_Rigidbody.position, Vector3.down, distanceToGround);
        m_Speed = m_Rigidbody.velocity.magnitude;

        if (m_IsGrounded)
        {
            rollingAudioSource.volume = AudioAdjustmentSettings.ClampAndInterpolate (rollingAudioSource.volume, m_Speed, k_GroundedRollingVolume);
            rollingAudioSource.pitch = AudioAdjustmentSettings.ClampAndInterpolate (rollingAudioSource.pitch, m_Speed, k_GroundedRollingPitch);
        }
        else
        {
            rollingAudioSource.volume = Mathf.Lerp(rollingAudioSource.volume, k_AirborneRollingTargetVolume, k_AirborneRollingVolumeChangeRate * Time.deltaTime);
            rollingAudioSource.pitch = Mathf.Lerp(rollingAudioSource.pitch, k_AirborneRollingTargetPitch, k_AirborneRollingPitchChangeRate * Time.deltaTime);
        }

        movementAudioSource.volume = AudioAdjustmentSettings.ClampAndInterpolate (movementAudioSource.volume, m_Speed, k_MovementVolume);
        movementAudioSource.pitch = AudioAdjustmentSettings.ClampAndInterpolate (movementAudioSource.pitch, m_Speed, k_MovementPitch);
    }

    void OnCollisionEnter(Collision collision)
    {
        int gameObjectLayerMask = 1 << collision.collider.gameObject.layer;
        AudioClip clip = null;
        
        if ((gameObjectLayerMask & plasticMask) == gameObjectLayerMask)
            clip = plasticImpact;
        else if ((gameObjectLayerMask & concreteMask) == gameObjectLayerMask)
            clip = concreteImpact;
        else if ((gameObjectLayerMask & metalMask) == gameObjectLayerMask)
            clip = metalImpact;

        if (clip != null)
        {
            impactAudioSource.pitch = Mathf.Clamp(m_Speed * k_ImpactPitch.speedTo, k_ImpactPitch.min, k_ImpactPitch.max);
            impactAudioSource.PlayOneShot(metalImpact, Mathf.Clamp01(m_Speed * k_ImpactSpeedToVolume));
        }
    }
}

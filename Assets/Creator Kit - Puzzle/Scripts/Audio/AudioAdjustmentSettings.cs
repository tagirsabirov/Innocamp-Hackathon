using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct AudioAdjustmentSettings
{
    public readonly float speedTo;
    public readonly float min;
    public readonly float max;
    public readonly float changeRate;

    public AudioAdjustmentSettings (float speedTo, float min, float max, float changeRate)
    {
        this.speedTo = speedTo;
        this.min = min;
        this.max = max;
        this.changeRate = changeRate;
    }
    
    public static float ClampAndInterpolate (float value, float speed, AudioAdjustmentSettings settings)
    {
        float speedBasedRollingVolume = Mathf.Clamp(speed * settings.speedTo, settings.min, settings.max);
        return Mathf.Lerp(value, speedBasedRollingVolume, settings.changeRate * Time.deltaTime);
    }
}
using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[Serializable, VolumeComponentMenu("NeonGame/CustomPostScreenTint3")]
public class CustomOverride3 : VolumeComponent
{
    [ColorUsage(true, true)]
    public ColorParameter newTintColor = new ColorParameter(Color.white);

    [Range(0f, 1f)]
    public FloatParameter newIntensity = new FloatParameter(1f);
    
}
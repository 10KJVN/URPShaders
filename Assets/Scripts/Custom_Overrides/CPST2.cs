using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Custom_Overrides
{
    [Serializable, VolumeComponentMenu("NeonGame/CustomPostScreenTint2")]
    public class CustomOverride2 : VolumeComponent, IPostProcessComponent
    {
        public FloatParameter tintIntensity = new FloatParameter(1);
        public ColorParameter tintColor = new ColorParameter(Color.white);
        public bool IsActive() => true;
        public bool IsTileCompatible() => true;
    }
}


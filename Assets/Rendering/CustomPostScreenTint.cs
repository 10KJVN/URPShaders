using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Rendering
{
    [Serializable, VolumeComponentMenuForRenderPipeline("NeonGame/CustomPostScreenTint", typeof(UniversalRenderPipeline))]

    public class CustomPostScreenTint : VolumeComponent, IPostProcessComponent
    {
        public FloatParameter tintIntensity = new FloatParameter(1);
        public ColorParameter tintColor = new ColorParameter(Color.white);
        public bool IsActive() => true;
        public bool IsTileCompatible() => true;
    }
}
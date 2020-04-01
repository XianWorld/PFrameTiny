using PFrame.Entities;
using Unity.Entities;
using Unity.Mathematics;

namespace PFrame.Tiny.UI
{
    [GenerateAuthoringComponent]
    public struct TweenScaleTransition : IComponentData
    {
        public float3 OverScale;
        public float3 PressedScale;
        public float Duration;
    }
}

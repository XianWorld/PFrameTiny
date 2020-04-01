using PFrame.Entities;
using Unity.Entities;
using Unity.Mathematics;

namespace PFrame.Tiny.UI
{
    //public enum EButtonTransitionType : byte
    //{
    //    None,
    //    MaterialSwap,
    //    Animation,
    //    Tween
    //}

    [GenerateAuthoringComponent]
    public struct UIButton : IComponentData
    {
        public bool IsDisabled;
    }

    //public struct MaterialSwapTransition : IComponentData
    //{

    //}

    //[EventComponent]
    public struct UIButtonClickEvent : IComponentData
    {
    }
}

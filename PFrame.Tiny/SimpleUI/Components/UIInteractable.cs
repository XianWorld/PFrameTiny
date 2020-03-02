using Unity.Entities;

namespace PFrame.Tiny.SimpleUI
{
    [GenerateAuthoringComponent]
    public struct UIInteractable : IComponentData
	{
        public bool IsPointerEnter;
        public bool IsPointerDown;
        public int DownButtonIndex;
    }

    public struct UIPointerEnterEvent : IComponentData
    {

    }
    public struct UIPointerExitEvent : IComponentData
    {

    }
    public struct UIPointerDownEvent : IComponentData
    {
        public int buttonIndex;
    }
    public struct UIPointerUpEvent : IComponentData
    {
        public int buttonIndex;
    }
    public struct UIPointerClickEvent : IComponentData
    {
        public int buttonIndex;
    }
}

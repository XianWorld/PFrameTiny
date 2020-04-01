using Unity.Entities;
using Unity.Mathematics;
using Unity.Tiny;

namespace PFrame.Tiny.UI
{
	public struct UIElement : IComponentData
	{
        public Rect Rect;
    }
}

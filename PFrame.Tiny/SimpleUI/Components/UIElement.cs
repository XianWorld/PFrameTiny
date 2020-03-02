using Unity.Entities;
using Unity.Mathematics;
using Unity.Tiny;

namespace PFrame.Tiny.SimpleUI
{
	public struct UIElement : IComponentData
	{
	    //public Rect Rect;

        public float3 Point0;
        public float3 Point1;
        public float3 Point2;
        public float3 Point3;
    }
}

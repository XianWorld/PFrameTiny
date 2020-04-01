using Unity.Entities;
using Unity.Mathematics;

namespace PFrame.Tiny.UI
{
    public enum EUILocationType : byte
    {
        World,
        HUD
    }

	[GenerateAuthoringComponent]
	public struct UIRoot : IComponentData
	{
	    public Entity CameraEntity;
        //0: world, 1: HUD
        public EUILocationType LocationType;
        //offset from camera, used when type is HUD
        public float3 Offset;
    }

}

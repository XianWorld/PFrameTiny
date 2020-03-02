using Unity.Entities;

namespace PFrame.Tiny.SimpleUI
{
	[GenerateAuthoringComponent]
	public struct UIWindow : IComponentData
	{
	    public Entity RootEntity;
	
	}
}

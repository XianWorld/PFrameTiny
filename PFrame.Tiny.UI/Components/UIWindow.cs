using PFrame.Entities;
using Unity.Entities;
using Unity.Mathematics;

namespace PFrame.Tiny.UI
{
	[GenerateAuthoringComponent]
	public struct UIWindow : IComponentData
	{
	    public Entity RootEntity;
	
	}
}

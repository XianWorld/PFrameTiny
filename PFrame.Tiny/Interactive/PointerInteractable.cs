using PFrame.Entities;
using System;
using Unity.Entities;
using Unity.Mathematics;

namespace PFrame.Tiny
{
    //[GenerateAuthoringComponent]
    public struct PointerInteractable : IComponentData, IComparable<PointerInteractable>
	{
        //public float3 Point0;
        //public float3 Point1;
        //public float3 Point2;
        //public float3 Point3;

        public bool IsPointerEnter;
        public bool IsPointerDown;

        public int DownButtonIndex;
        //public BlobAssetReference<ShortArrayAsset> ShortArrayAssetRef;

        public BlobAssetReference<Float3ArrayAsset> PolyPointsAssetRef;

        public short Layer;

        public int CompareTo(PointerInteractable other)
        {
            if (Layer > other.Layer)
                return -1;
            else if (Layer == other.Layer)
                return 0;
            else
                return 1;
        }
    }

    //[EventComponent]
    public struct PointerEnterEvent : IComponentData
    {

    }
    //[EventComponent]
    public struct PointerExitEvent : IComponentData
    {

    }
    //[EventComponent]
    public struct PointerDownEvent : IComponentData
    {
        public int buttonIndex;
    }
    //[EventComponent]
    public struct PointerUpEvent : IComponentData
    {
        public int buttonIndex;
    }
    //[EventComponent]
    public struct PointerClickEvent : IComponentData
    {
        public int buttonIndex;
    }
}

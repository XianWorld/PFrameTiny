using PFrame.Entities;
using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Tiny;

namespace PFrame.Tiny
{
    public struct TextMesh : IComponentData
    {
        public NativeString128 Text;
        public ushort FontId;
        public float OffsetZ;
        public float CharSize;
        public EHAlignType HAlignType;
        public EVAlignType VAlignType;

        public float HSpacing;
        public float VSpacing;

        public Color Color;
    }

    public struct TextMeshState : ISystemStateComponentData
    {
        public NativeString128 Text;
        public ushort FontId;
        public float OffsetZ;
        public float CharSize;

        public EHAlignType HAlignType;
        public EVAlignType VAlignType;

        public float HSpacing;
        public float VSpacing;

        public Color Color;
    }

    public struct TextLayoutData : IDisposable
    {
        public float MinX;
        public float MinY;
        public float Width;
        public float Height;

        public int LineNum;
        public int CharNum;

        public NativeList<TextLineLayoutData> LineLayoutDataList;

        public void Dispose()
        {
            if (LineLayoutDataList.IsCreated)
                LineLayoutDataList.Dispose();
        }
    }

    public struct TextLineLayoutData
    {
        public int LineIndex;
        public int CharIndex;
        public int CharNum;

        public float Width;
        public float Height;

        public float MinX;
        public float MinY;
    }
}

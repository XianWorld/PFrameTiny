using PFrame.Entities;
using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Tiny;
//#if !UNITY_DOTSPLAYER
//using UnityEngine;
//#endif

namespace PFrame.Tiny
{
//#if !UNITY_DOTSPLAYER
//    public struct FontMaterial : ISharedComponentData, IEquatable<FontMaterial>
//    {
//        public Material Material;

//        public bool Equals(FontMaterial other)
//        {
//            return Material == (other.Material);
//        }

//        public override int GetHashCode()
//        {
//            return Material.GetHashCode();
//        }
//    }
//#endif

    public struct FontData : IBufferElementData, IGameData
    {
        public ushort Id;
        public NativeString32 Name;
        public ushort DataId => Id;
        public byte DataType => (byte)ECommonGameDataType.Font;
        public string DataName => Name.ToString();

        public int Ascent;
        public bool Dynamic;
        public Entity MaterialEntity;
//#if UNITY_DOTSPLAYER
//        public Entity Material;
//#else
//        public Material Material;
//#endif
        public int LineHeight;
        public int FontSize;

        public BlobAssetReference<CharacterInfoArrayAsset> CharacterInfoArrayAssetRef;
    }

    public struct CharacterInfoArrayAsset
    {
        public ushort Count;
        public BlobArray<CharacterInfo> CharacterInfoArray;
    }

    //     Font Style applied to GUI Texts, Text Meshes or GUIStyles.
    public enum FontStyle
    {
        //     No special style is applied.
        Normal = 0,
        //     Bold style applied to your texts.
        Bold = 1,
        //     Italic style applied to your texts.
        Italic = 2,
        //     Bold and Italic styles applied to your texts.
        BoldAndItalic = 3
    }

    public struct CharacterInfo
    {
        //     Unicode value of the character.
        public int index;
        //     UV coordinates for the character in the texture.
        public Rect uv;
        //     Screen coordinates for the character in generated text meshes.
        public Rect vert;
        //     How far to advance between the beginning of this charcater and the next.
        public float width;
        //     The size of the character or 0 if it is the default font size.
        public int size;
        //     The style of the character.
        public FontStyle style;
        //     Is the character flipped?
        public bool flipped;

#pragma warning disable 0618
        public int advance
        {
            get { return (int)math.round(width); }
            set { width = value; }
        }

        public int glyphWidth
        {
            get { return (int)vert.width; }
            set { vert.width = value; }
        }

        public int glyphHeight
        {
            get { return (int)-vert.height; }
            set
            {
                var old = vert.height;
                vert.height = -value;
                vert.y += old - vert.height;
            }
        }

        public int bearing
        {
            get { return (int)vert.x; }
            set { vert.x = value; }
        }

        public int minY
        {
            get { return (int)(vert.y + vert.height); }
            set { vert.height = value - vert.y; }
        }

        public int maxY
        {
            get { return (int)vert.y; }
            set
            {
                var old = vert.y;
                vert.y = value;
                vert.height += old - vert.y;
            }
        }

        public int minX
        {
            get { return (int)vert.x; }
            set
            {
                var old = vert.x;
                vert.x = value;
                vert.width += old - vert.x;
            }
        }

        public int maxX
        {
            get { return (int)(vert.x + vert.width); }
            set { vert.width = value - vert.x; }
        }

        internal float2 uvBottomLeftUnFlipped
        {
            get { return new float2(uv.x, uv.y); }
            set
            {
                var old = uvTopRightUnFlipped;
                uv.x = value.x;
                uv.y = value.y;
                uv.width = old.x - uv.x;
                uv.height = old.y - uv.y;
            }
        }

        internal float2 uvBottomRightUnFlipped
        {
            get { return new float2(uv.x + uv.width, uv.y); }
            set
            {
                var old = uvTopRightUnFlipped;
                uv.width = value.x - uv.x;
                uv.y = value.y;
                uv.height = old.y - uv.y;
            }
        }

        internal float2 uvTopRightUnFlipped
        {
            get { return new float2(uv.x + uv.width, uv.y + uv.height); }
            set
            {
                uv.width = value.x - uv.x;
                uv.height = value.y - uv.y;
            }
        }

        internal float2 uvTopLeftUnFlipped
        {
            get { return new float2(uv.x, uv.y + uv.height); }
            set
            {
                var old = uvTopRightUnFlipped;
                uv.x = value.x;
                uv.height = value.y - uv.y;
                uv.width = old.x - uv.x;
            }
        }

        public float2 uvBottomLeft
        {
            get { return uvBottomLeftUnFlipped; }
            set { uvBottomLeftUnFlipped = value; }
        }

        public float2 uvBottomRight
        {
            get { return flipped ? uvTopLeftUnFlipped : uvBottomRightUnFlipped; }
            set
            {
                if (flipped)
                    uvTopLeftUnFlipped = value;
                else
                    uvBottomRightUnFlipped = value;
            }
        }

        public float2 uvTopRight
        {
            get { return uvTopRightUnFlipped; }
            set { uvTopRightUnFlipped = value; }
        }

        public float2 uvTopLeft
        {
            get { return flipped ? uvBottomRightUnFlipped : uvTopLeftUnFlipped; }
            set
            {
                if (flipped)
                    uvBottomRightUnFlipped = value;
                else
                    uvTopLeftUnFlipped = value;
            }
        }
#pragma warning restore 0618
    }

    public struct Font : IDisposable
    {
        public ushort DataId;
        public int Ascent;
        public int LineHeight;
        public int FontSize;
        //public FontData Data;
        public Entity MaterialEntity;
        public NativeHashMap<int, CharacterInfo> CharacterInfoMap;

        public void Initialize(FontData fontData)
        {
            DataId = fontData.Id;
            Ascent = fontData.Ascent;
            LineHeight = fontData.LineHeight;
            FontSize = fontData.FontSize;
            MaterialEntity = fontData.MaterialEntity;
            //Data = fontData;

            var ucharInfos = fontData.CharacterInfoArrayAssetRef;
            var num = ucharInfos.Value.Count;
            ref var array = ref ucharInfos.Value.CharacterInfoArray;
            var characterInfoMap = new NativeHashMap<int, CharacterInfo>(num, Allocator.Persistent);
            for (int i = 0; i < num; i++)
            {
                var charInfo = array[i];
                characterInfoMap.Add(charInfo.index, charInfo);
            }
            CharacterInfoMap = characterInfoMap;
        }

        public void Dispose()
        {
            CharacterInfoMap.Dispose();
        }
    }

}

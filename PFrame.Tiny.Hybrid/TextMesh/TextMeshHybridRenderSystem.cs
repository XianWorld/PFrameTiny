using PFrame.Entities;
using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;

namespace PFrame.Tiny.Hybrid
{
#if !UNITY_DOTSPLAYER
    public struct FontMaterial : ISharedComponentData, IEquatable<FontMaterial>
    {
        public Material Material;

        public bool Equals(FontMaterial other)
        {
            return Material == (other.Material);
        }

        public override int GetHashCode()
        {
            return Material.GetHashCode();
        }
    }

    [UpdateInGroup(typeof(PresentationSystemGroup))]
    public unsafe class TextMeshHybridRenderSystem : ComponentSystem
    {

        FontSystem fontSystem;
        EntityQuery noStateQuery;
        EntityQuery destroyedQuery;

        protected override void OnCreate()
        {
            //RequireSingletonForUpdate<GameDataManager>();

            noStateQuery = GetEntityQuery(new EntityQueryDesc
            {
                All = new[] { ComponentType.ReadWrite<TextMesh>() },
                None = new[] { ComponentType.ReadWrite<TextMeshState>() },
            });
            destroyedQuery = GetEntityQuery(new EntityQueryDesc
            {
                All = new[] { ComponentType.ReadWrite<TextMeshState>() },
                None = new[] { ComponentType.ReadWrite<TextMesh>() },
            });
        }

        protected override void OnStartRunning()
        {
            base.OnStartRunning();
            fontSystem = World.GetExistingSystem<FontSystem>();
        }

        protected override void OnUpdate()
        {
            if (!fontSystem.IsInited())
                return;

            EntityManager.AddComponent<TextMeshState>(noStateQuery);
            EntityManager.RemoveComponent<TextMeshState>(destroyedQuery);

            //Entities
            //.WithAll<TextMesh>()
            //.WithNone<TextMeshState>()
            //.ForEach((Entity entity, ref TextMesh textMesh) =>
            //{
            //    var fontId = textMesh.FontId;

            //    if (!fontSystem.TryGetFont(fontId, out var font))
            //        return;

            //    Entity meshEntity;
            //    Entity materialEntity;
            //    if (!EntityManager.HasComponent<RenderMesh>(entity))
            //    {
            //        EntityManager.AddComponent<RenderMesh>(entity);
            //        LogUtil.LogFormat("TextMeshHybridRenderSystem: Add MeshRenderer\r\n");
            //    }

            //    var meshRenderer = EntityManager.GetSharedComponentData<RenderMesh>(entity);
            //    var mesh = meshRenderer.mesh;
            //    if (mesh == null)
            //    {
            //        mesh = new Mesh();
            //        meshRenderer.mesh = mesh;
            //    }
            //    //meshRenderer.material = font.MaterialEntity
            //    //meshRenderer.castShadows = UnityEngine.Rendering.ShadowCastingMode.Off;
            //    //meshRenderer.receiveShadows = false;

            //    EntityManager.SetSharedComponentData(entity, meshRenderer);

            //    EntityManager.AddComponentData(entity, new PerInstanceCullingTag());
            //    EntityManager.AddComponentData(entity, new RenderBounds { Value = mesh.bounds.ToAABB() });

            //    EntityManager.AddComponent<TextMeshState>(entity);
            //});

            //Entities
            //.WithAll<TextMeshState>()
            //.WithNone<TextMesh>()
            //.ForEach((Entity entity, ref TextMeshState TextMeshState) =>
            //{
            //    var meshRenderer = EntityManager.GetSharedComponentData<RenderMesh>(entity);
            //    var mesh = meshRenderer.mesh;

            //    EntityManager.RemoveComponent<RenderMesh>(entity);

            //    EntityManager.RemoveComponent<TextMeshState>(entity);
            //});

            Entities
                .ForEach((Entity entity, RenderMesh renderMesh, ref TextMesh textMesh, ref TextMeshState textMeshState) =>
            {
                bool isDirty = false;

                var text = textMesh.Text;
                var fontId = textMesh.FontId;
                var charSize = textMesh.CharSize;
                var color = textMesh.Color;
                var offsetZ = textMesh.OffsetZ;
                var hAlignType = textMesh.HAlignType;
                var vAlignType = textMesh.VAlignType;
                var hSpacing = textMesh.HSpacing;
                var vSpacing = textMesh.VSpacing;

                if (!text.Equals(textMeshState.Text)
                    || fontId != textMeshState.FontId
                    || charSize != textMeshState.CharSize
                    || color != textMeshState.Color
                    || offsetZ != textMeshState.OffsetZ
                    || hAlignType != textMeshState.HAlignType
                    || vAlignType != textMeshState.VAlignType
                    || hSpacing != textMeshState.HSpacing
                    || vSpacing != textMeshState.VSpacing
                    )
                {
                    textMeshState.Text = text;
                    textMeshState.FontId = fontId;
                    textMeshState.CharSize = charSize;
                    textMeshState.Color = color;
                    textMeshState.OffsetZ = offsetZ;
                    textMeshState.HAlignType = hAlignType;
                    textMeshState.VAlignType = vAlignType;
                    textMeshState.HSpacing = hSpacing;
                    textMeshState.VSpacing = vSpacing;

                    isDirty = true;
                }

                if (isDirty)
                {
                    if (!fontSystem.TryGetFont(fontId, out var font))
                        return;

                    //rebuild mesh
                    //string textStr = text.ToString();
                    //var charNum = text.LengthInBytes;
                    //byte* chars = &text.buffer.byte0000;
                    //var textBuffer = new NativeArray<int>(charNum, Allocator.TempJob);
                    //for (int j = 0; j < charNum; j++)
                    //{
                    //    textBuffer[j] = chars[j];
                    //}

                    var materialEntity = font.MaterialEntity;
                    var material = EntityManager.GetSharedComponentData<FontMaterial>(materialEntity).Material;
                    if (renderMesh.material != material)
                        renderMesh.material = material;

                    //var builder = new TextMeshHybridBuilder(textBuffer, font, charSize, color, offsetZ);
                    //builder.Execute();

                    var mesh = renderMesh.mesh;
                    //Debug.LogFormat("TextMeshHybridRenderSystem: {0}, {1}", text.ToString(), mesh);
                    if (mesh == null)
                        mesh = new Mesh();
                    else
                        mesh.Clear();
                    //mesh.Clear();

                    //mesh.SetVertices(builder.Verts);
                    //mesh.SetUVs(0, builder.UVs);
                    //mesh.SetIndices(builder.Tris, 0, builder.Tris.Length, MeshTopology.Triangles, 0, true, 0);
                    //mesh.SetColors(builder.Colors);

                    TextMeshHybridUtil.CreateTextMesh(textMesh, font, mesh);

                    renderMesh.mesh = mesh;

                    EntityManager.SetComponentData(entity, new RenderBounds { Value = mesh.bounds.ToAABB() });

                    //builder.Dispose();
                    //textBuffer.Dispose();
                }
            });

        }
    }

    //public struct TextMeshHybridBuilder
    //{
    //    private NativeArray<int> textBuffer;
    //    PFrame.Tiny.Font font;

    //    public NativeArray<int> Tris;
    //    public NativeArray<float3> Verts;
    //    public NativeArray<float2> UVs;
    //    public NativeArray<Unity.Tiny.Color> Colors;

    //    int Idx;
    //    int TriIdx;
    //    float offsetX;
    //    float offsetZ;
    //    Unity.Tiny.Color color;
    //    int fontSize;
    //    float scale;

    //    public TextMeshHybridBuilder(NativeArray<int> textBuffer, PFrame.Tiny.Font font, float charSize, Unity.Tiny.Color color, float offsetZ)
    //    {
    //        this.textBuffer = textBuffer;
    //        this.font = font;
    //        this.color = color;
    //        this.offsetZ = offsetZ;

    //        var len = textBuffer.Length;
    //        int triCount = 2 * 3 * len;
    //        int vertCount = 4 * len;

    //        fontSize = font.FontSize;
    //        scale = charSize / fontSize;
    //        Idx = 0;
    //        TriIdx = 0;
    //        offsetX = 0;
    //        Tris = new NativeArray<int>(triCount, Allocator.TempJob);
    //        Verts = new NativeArray<float3>(vertCount, Allocator.TempJob);
    //        UVs = new NativeArray<float2>(vertCount, Allocator.TempJob);
    //        Colors = new NativeArray<Unity.Tiny.Color>(vertCount, Allocator.TempJob);
    //    }

    //    public void Execute()
    //    {
    //        var len = textBuffer.Length;
    //        var map = font.CharacterInfoMap;

    //        for (int i = 0; i < len; i++)
    //        {
    //            int charCode = textBuffer[i];
    //            if(!map.TryGetValue(charCode, out var charInfo))
    //            {
    //                //var charInfo = map[charCode];
    //                charInfo = map[' '];
    //            }
    //            PutChar(charInfo);
    //        }
    //    }

    //    private void PutChar(PFrame.Tiny.CharacterInfo charInfo)
    //    {
    //        float x = offsetX + charInfo.bearing * scale;
    //        float y = charInfo.minY * scale;
    //        var width = charInfo.glyphWidth * scale;
    //        var height = charInfo.glyphHeight * scale;

    //        Verts[Idx + 0] = new float3(x, y, offsetZ);
    //        Verts[Idx + 1] = new float3(x, y + height, offsetZ);
    //        Verts[Idx + 2] = new float3(x + width, y + height, offsetZ);
    //        Verts[Idx + 3] = new float3(x + width, y, offsetZ);

    //        UVs[Idx + 0] = charInfo.uvBottomLeft;
    //        UVs[Idx + 1] = charInfo.uvTopLeft;
    //        UVs[Idx + 2] = charInfo.uvTopRight;
    //        UVs[Idx + 3] = charInfo.uvBottomRight;

    //        Tris[TriIdx + 0] = Idx + 0;
    //        Tris[TriIdx + 1] = Idx + 1;
    //        Tris[TriIdx + 2] = Idx + 2;
    //        Tris[TriIdx + 3] = Idx + 0;
    //        Tris[TriIdx + 4] = Idx + 2;
    //        Tris[TriIdx + 5] = Idx + 3;

    //        Idx += 4;
    //        TriIdx += 6;
    //        offsetX += charInfo.advance * scale;
    //    }

    //    public void Dispose()
    //    {
    //        Verts.Dispose();
    //        Tris.Dispose();
    //        UVs.Dispose();
    //        Colors.Dispose();
    //    }
    //}

    public class TextMeshHybridUtil
    {
        public static void CreateTextMesh(
            TextMesh textMesh,
            PFrame.Tiny.Font font,
            Mesh mesh
            )
        {
            var Idx = 0;
            var TriIdx = 0;

            var textLayoutData = TextMeshUtil.GetTextLayoutData(textMesh, font);
            var charNum = textLayoutData.CharNum;
            //var text = textMesh.Text;
            //var charNum = text.LengthInBytes;
            int triCount = 2 * 3 * charNum;
            int vertCount = 4 * charNum;

            var Tris = new NativeArray<int>(triCount, Allocator.TempJob);
            var Verts = new NativeArray<float3>(vertCount, Allocator.TempJob);
            var UVs = new NativeArray<float2>(vertCount, Allocator.TempJob);
            var Colors = new NativeArray<Unity.Tiny.Color>(vertCount, Allocator.TempJob);

            TextMeshUtil.CreateTextMesh(textMesh, font, textLayoutData, (charInfo, pos, scale) =>
            {
                var posX = pos.x;
                var posY = pos.y;
                var posZ = pos.z;

                float minx = posX + charInfo.minX * scale;
                float maxx = posX + charInfo.maxX * scale;
                float miny = posY + charInfo.minY * scale;
                float maxy = posY + charInfo.maxY * scale;

                Verts[Idx + 0] = new float3(minx, miny, posZ);
                Verts[Idx + 1] = new float3(minx, maxy, posZ);
                Verts[Idx + 2] = new float3(maxx, maxy, posZ);
                Verts[Idx + 3] = new float3(maxx, miny, posZ);
                //float x = posX + charInfo.bearing * scale;
                //float y = posY + charInfo.minY * scale;
                //var width = charInfo.glyphWidth * scale;
                //var height = charInfo.glyphHeight * scale;

                //Verts[Idx + 0] = new float3(x, y, posZ);
                //Verts[Idx + 1] = new float3(x, y + height, posZ);
                //Verts[Idx + 2] = new float3(x + width, y + height, posZ);
                //Verts[Idx + 3] = new float3(x + width, y, posZ);

                UVs[Idx + 0] = charInfo.uvBottomLeft;
                UVs[Idx + 1] = charInfo.uvTopLeft;
                UVs[Idx + 2] = charInfo.uvTopRight;
                UVs[Idx + 3] = charInfo.uvBottomRight;

                Tris[TriIdx + 0] = Idx + 0;
                Tris[TriIdx + 1] = Idx + 1;
                Tris[TriIdx + 2] = Idx + 2;
                Tris[TriIdx + 3] = Idx + 0;
                Tris[TriIdx + 4] = Idx + 2;
                Tris[TriIdx + 5] = Idx + 3;

                Idx += 4;
                TriIdx += 6;
            });

            mesh.SetVertices(Verts);
            mesh.SetUVs(0, UVs);
            mesh.SetIndices(Tris, 0, Tris.Length, MeshTopology.Triangles, 0, true, 0);
            mesh.SetColors(Colors);

            Verts.Dispose();
            Tris.Dispose();
            UVs.Dispose();
            Colors.Dispose();

            textLayoutData.Dispose();

        }
    }
#endif
}

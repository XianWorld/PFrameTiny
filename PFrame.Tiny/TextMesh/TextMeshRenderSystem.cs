using PFrame.Entities;
using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Tiny;
using Unity.Tiny.Rendering;

namespace PFrame.Tiny
{
#if UNITY_DOTSPLAYER
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    public unsafe class TextMeshRenderSystem : ComponentSystem
    {
        FontSystem fontSystem;
        EntityQuery noStateQuery;
        EntityQuery destroyedQuery;

        protected override void OnCreate()
        {
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

            //EntityManager.AddComponent<TextMeshState>(noStateQuery);
            //EntityManager.RemoveComponent<TextMeshState>(destroyedQuery);

            Entities
                .WithAll<TextMesh>()
                .WithNone<TextMeshState>()
                .ForEach((Entity entity, ref TextMesh textMesh) =>
                {
                    var fontId = textMesh.FontId;

                    if (!fontSystem.TryGetFont(fontId, out var font))
                        return;

                    Entity meshEntity;
                    Entity materialEntity;
                    if (!EntityManager.HasComponent<MeshRenderer>(entity))
                    {
                        EntityManager.AddComponent<MeshRenderer>(entity);
                        LogUtil.LogFormat("TextMeshRenderSystem: Add MeshRenderer\r\n");
                    }

                    var meshRenderer = EntityManager.GetComponentData<MeshRenderer>(entity);
                    meshEntity = meshRenderer.mesh;
                    materialEntity = meshRenderer.material;

                    if (!EntityManager.HasComponent<SimpleMeshRenderer>(entity))
                    {
                        EntityManager.AddComponent<SimpleMeshRenderer>(entity);
                        LogUtil.LogFormat("TextMeshRenderSystem: Add SimpleMeshRenderer\r\n");
                    }

                    //setup material entity
                    //if (materialEntity == Entity.Null)
                    //{
                    //    materialEntity = font.MaterialEntity;
                    //    meshRenderer.material = materialEntity;
                    //}
                    //var material = EntityManager.GetComponentData<SimpleMaterial>(materialEntity);
                    //var fontMaterial = EntityManager.GetComponentData<SimpleMaterial>(font.MaterialEntity);
                    //if (!fontMaterial.Equals(material))
                    //    EntityManager.SetComponentData(materialEntity, fontMaterial);
                    materialEntity = font.MaterialEntity;
                    meshRenderer.material = materialEntity;

                    //setup mesh entity
                    if (meshEntity == Entity.Null)
                    {
                        meshEntity = EntityManager.CreateEntity();
                        meshRenderer.mesh = meshEntity;
                    }

                    if (EntityManager.HasComponent<SimpleMeshRenderData>(meshEntity))
                    {
                        EntityManager.RemoveComponent<SimpleMeshRenderData>(meshEntity);
                        LogUtil.LogFormat("TextMeshRenderSystem: Remove SimpleMeshRenderData\r\n");
                    }

                    //DynamicMeshData
                    if (!EntityManager.HasComponent<DynamicMeshData>(meshEntity))
                    {
                        EntityManager.AddComponent<DynamicMeshData>(meshEntity);
                        LogUtil.LogFormat("TextMeshRenderSystem: Add DynamicMeshData\r\n");
                    }
                    //DynamicIndex
                    if (!EntityManager.HasComponent<DynamicIndex>(meshEntity))
                        EntityManager.AddBuffer<DynamicIndex>(meshEntity);
                    //DynamicSimpleVertex
                    if (!EntityManager.HasComponent<DynamicSimpleVertex>(meshEntity))
                        EntityManager.AddBuffer<DynamicSimpleVertex>(meshEntity);

                    var iBuffer = EntityManager.GetBuffer<DynamicIndex>(meshEntity);
                    var vBuffer = EntityManager.GetBuffer<DynamicSimpleVertex>(meshEntity);
                    int maxCharNum = 256;
                    vBuffer.Capacity = maxCharNum * 4;
                    //vBuffer.ResizeUninitialized(maxCharNum * 4);
                    iBuffer.Capacity = maxCharNum * 6;
                    //iBuffer.ResizeUninitialized(maxCharNum * 6);

                    DynamicMeshData dmd = EntityManager.GetComponentData<DynamicMeshData>(meshEntity);
                    dmd.Dirty = false;
                    dmd.IndexCapacity = iBuffer.Capacity;
                    dmd.VertexCapacity = vBuffer.Capacity;
                    dmd.NumIndices = 0;
                    dmd.NumVertices = 0;
                    //dmd.UseDynamicGPUBuffer = true;
                    EntityManager.SetComponentData<DynamicMeshData>(meshEntity, dmd);

                    //MeshBounds
                    if (!EntityManager.HasComponent<MeshBounds>(meshEntity))
                        EntityManager.AddComponent<MeshBounds>(meshEntity);
                    //mb.Bounds = MeshHelper.ComputeBounds(vBuffer.AsNativeArray().Reinterpret<DynamicSimpleVertex, SimpleVertex>());
                    //EntityManager.SetComponentData(meshEntity, mb);

                    EntityManager.SetComponentData(entity, meshRenderer);

                    EntityManager.AddComponent<TextMeshState>(entity);
                });

            Entities
            .WithAll<TextMeshState>()
            .WithNone<TextMesh>()
            .ForEach((Entity entity, ref TextMeshState TextMeshState) =>
            {
                Entity meshEntity;
                Entity materialEntity;

                var meshRenderer = EntityManager.GetComponentData<MeshRenderer>(entity);
                meshEntity = meshRenderer.mesh;
                materialEntity = meshRenderer.material;

                PostUpdateCommands.DestroyEntity(meshEntity);
                PostUpdateCommands.DestroyEntity(materialEntity);

                EntityManager.RemoveComponent<MeshRenderer>(entity);

                EntityManager.RemoveComponent<TextMeshState>(entity);
            });

            Entities
                .ForEach((Entity entity, ref TextMesh textMesh, ref TextMeshState textMeshState, ref MeshRenderer renderMesh) =>
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

                    var materialEntity = renderMesh.material;
                    var meshEntity = renderMesh.mesh;

                    //update material
                    materialEntity = font.MaterialEntity;
                    renderMesh.material = materialEntity;

                    //var builder = new TextMeshBuilder(textBuffer, font, charSize, color);
                    //builder.Execute();

                    //var tris = builder.Tris;
                    //var verts = builder.Verts;

                    //var iBuffer = EntityManager.GetBuffer<DynamicIndex>(meshEntity);
                    //var vBuffer = EntityManager.GetBuffer<DynamicSimpleVertex>(meshEntity);

                    //iBuffer.Clear();
                    ////iBuffer.AddRange(builder.Tris.Reinterpret<ushort, DynamicIndex>());
                    //for(int i = 0;i< tris.Length; i++)
                    //{
                    //    iBuffer.Add(new DynamicIndex { Value = tris[i] });
                    //}

                    //vBuffer.Clear();
                    ////vBuffer.AddRange(builder.Verts.Reinterpret<SimpleVertex, DynamicSimpleVertex>());
                    //for (int i = 0; i < verts.Length; i++)
                    //{
                    //    vBuffer.Add(new DynamicSimpleVertex { Value = verts[i] });
                    //}

                    //DynamicMeshData dmd = EntityManager.GetComponentData<DynamicMeshData>(meshEntity);
                    //dmd.Dirty = true;
                    //dmd.NumIndices = iBuffer.Length;
                    //dmd.NumVertices = vBuffer.Length;
                    //EntityManager.SetComponentData<DynamicMeshData>(meshEntity, dmd);

                    //MeshBounds mb;
                    //mb.Bounds = MeshHelper.ComputeBounds(builder.Verts);
                    //EntityManager.SetComponentData(meshEntity, mb);

                    //builder.Dispose();

                    //update mesh
                    var layoutData = TextMeshUtil.GetTextLayoutData(textMesh, font);
                    var charNum = layoutData.CharNum;

                    var iBuffer = EntityManager.GetBuffer<DynamicIndex>(meshEntity);
                    var vBuffer = EntityManager.GetBuffer<DynamicSimpleVertex>(meshEntity);
                    var nv = charNum * 4;
                    var ni = charNum * 6;
                    vBuffer.ResizeUninitialized(nv);
                    iBuffer.ResizeUninitialized(ni);

                    //MeshHelper.FillPlaneMesh(
                    //    vBuffer.AsNativeArray().Reinterpret<DynamicSimpleVertex, SimpleVertex>(),
                    //    iBuffer.AsNativeArray().Reinterpret<DynamicIndex, ushort>(),
                    //    new float3(0f),
                    //    new float3(3f, 0f, 0f),
                    //    new float3(0f, -3f, 0f),
                    //    out var bb
                    //    );
                    //CreateTextMesh(textBuffer, font, charSize, color, offsetZ,
                    //    vBuffer.AsNativeArray().Reinterpret<DynamicSimpleVertex, SimpleVertex>(),
                    //    iBuffer.AsNativeArray().Reinterpret<DynamicIndex, ushort>(),
                    //    out var bb
                    //    );

                    var destVertices = vBuffer.AsNativeArray().Reinterpret<DynamicSimpleVertex, SimpleVertex>();
                    var destIndices = iBuffer.AsNativeArray().Reinterpret<DynamicIndex, ushort>();
                    var Idx = 0;
                    var TriIdx = 0;

                    TextMeshUtil.CreateTextMesh(textMesh, font, layoutData, (charInfo, pos, scale) =>
                    {
                        var posX = pos.x;
                        var posY = pos.y;
                        var posZ = pos.z;
                        float minx = posX + charInfo.minX * scale;
                        float maxx = posX + charInfo.maxX * scale;
                        float miny = posY + charInfo.minY * scale;
                        float maxy = posY + charInfo.maxY * scale;

                        var bl = charInfo.uvBottomLeft;
                        bl.y = 1f - bl.y;
                        var br = charInfo.uvBottomRight;
                        br.y = 1f - br.y;
                        var tr = charInfo.uvTopRight;
                        tr.y = 1f - tr.y;
                        var tl = charInfo.uvTopLeft;
                        tl.y = 1f - tl.y;

                        destVertices[Idx + 0] = new SimpleVertex { Position = new float3(minx, miny, posZ), TexCoord0 = bl, Color = color.AsFloat4() };
                        destVertices[Idx + 1] = new SimpleVertex { Position = new float3(maxx, miny, posZ), TexCoord0 = br, Color = color.AsFloat4() };
                        destVertices[Idx + 2] = new SimpleVertex { Position = new float3(maxx, maxy, posZ), TexCoord0 = tr, Color = color.AsFloat4() };
                        destVertices[Idx + 3] = new SimpleVertex { Position = new float3(minx, maxy, posZ), TexCoord0 = tl, Color = color.AsFloat4() };

                        destIndices[TriIdx + 0] = (ushort)(Idx + 0);
                        destIndices[TriIdx + 1] = (ushort)(Idx + 1);
                        destIndices[TriIdx + 2] = (ushort)(Idx + 2);
                        destIndices[TriIdx + 3] = (ushort)(Idx + 2);
                        destIndices[TriIdx + 4] = (ushort)(Idx + 3);
                        destIndices[TriIdx + 5] = (ushort)(Idx + 0);

                        Idx += 4;
                        TriIdx += 6;
                    });

                    DynamicMeshData dmd = EntityManager.GetComponentData<DynamicMeshData>(meshEntity);
                    dmd.Dirty = true;
                    dmd.NumIndices = iBuffer.Length;
                    dmd.NumVertices = vBuffer.Length;
                    EntityManager.SetComponentData<DynamicMeshData>(meshEntity, dmd);

                    MeshBounds mb;
                    mb.Bounds = MeshHelper.ComputeBounds(destVertices);
                    EntityManager.SetComponentData(meshEntity, mb);

                    renderMesh.indexCount = dmd.NumIndices;

                    LogUtil.LogFormat("TextMeshRenderSystem: {0}, {1}, {2}\r\n", iBuffer.Length, vBuffer.Length, mb.Bounds);

                    //textBuffer.Dispose();
                    layoutData.Dispose();
                }
            });

        }

        //public static void CreateTextMesh(
        //    NativeArray<int> textBuffer,
        //    PFrame.Tiny.Font font,
        //    float charSize,
        //    Unity.Tiny.Color color,
        //    float offsetZ,
        //    NativeArray<SimpleVertex> destVertices,
        //    NativeArray<ushort> destIndices,
        //    out AABB bb)
        //{
        //    var len = textBuffer.Length;
        //    int triCount = 2 * 3 * len;
        //    int vertCount = 4 * len;

        //    var fontSize = font.FontSize;
        //    var scale = charSize / fontSize;
        //    var Idx = 0;
        //    var TriIdx = 0;
        //    float offsetX = 0;

        //    var map = font.CharacterInfoMap;

        //    for (int i = 0; i < len; i++)
        //    {
        //        int charCode = textBuffer[i];
        //        //var charInfo = map[charCode];
        //        if (!map.TryGetValue(charCode, out var charInfo))
        //        {
        //            charInfo = map[' '];
        //        }


        //        //float x = offsetX + charInfo.bearing * scale;
        //        //float y = charInfo.minY * scale;
        //        //var width = charInfo.glyphWidth * scale;
        //        //var height = charInfo.glyphHeight * scale;

        //        float minx = offsetX + charInfo.minX * scale;
        //        float maxx = offsetX + charInfo.maxX * scale;
        //        float miny = charInfo.minY * scale;
        //        float maxy = charInfo.maxY * scale;

        //        //var vertex = new SimpleVertex();

        //        //destVertices[0] = new SimpleVertex { Position = org, Color = new float4(1, 1, 1, 1), TexCoord0 = new float2(0, 0) };
        //        //destVertices[1] = new SimpleVertex { Position = org + du, Color = new float4(1, 1, 1, 1), TexCoord0 = new float2(1, 0) };
        //        //destVertices[2] = new SimpleVertex { Position = org + du + dv, Color = new float4(1, 1, 1, 1), TexCoord0 = new float2(1, 1) };
        //        //destVertices[3] = new SimpleVertex { Position = org + dv, Color = new float4(1, 1, 1, 1), TexCoord0 = new float2(0, 1) };
        //        //destIndices[0] = 0; destIndices[1] = 1; destIndices[2] = 2;
        //        //destIndices[3] = 2; destIndices[4] = 3; destIndices[5] = 0;

        //        var bl = charInfo.uvBottomLeft;
        //        bl.y = 1f - bl.y;
        //        var br = charInfo.uvBottomRight;
        //        br.y = 1f - br.y;
        //        var tr = charInfo.uvTopRight;
        //        tr.y = 1f - tr.y;
        //        var tl = charInfo.uvTopLeft;
        //        tl.y = 1f - tl.y;

        //        destVertices[Idx + 0] = new SimpleVertex { Position = new float3(minx, miny, offsetZ), TexCoord0 = bl, Color = color.AsFloat4() };
        //        destVertices[Idx + 1] = new SimpleVertex { Position = new float3(maxx, miny, offsetZ), TexCoord0 = br, Color = color.AsFloat4() };
        //        destVertices[Idx + 2] = new SimpleVertex { Position = new float3(maxx, maxy, offsetZ), TexCoord0 = tr, Color = color.AsFloat4() };
        //        destVertices[Idx + 3] = new SimpleVertex { Position = new float3(minx, maxy, offsetZ), TexCoord0 = tl, Color = color.AsFloat4() };

        //        destIndices[TriIdx + 0] = (ushort)(Idx + 0);
        //        destIndices[TriIdx + 1] = (ushort)(Idx + 1);
        //        destIndices[TriIdx + 2] = (ushort)(Idx + 2);
        //        destIndices[TriIdx + 3] = (ushort)(Idx + 2);
        //        destIndices[TriIdx + 4] = (ushort)(Idx + 3);
        //        destIndices[TriIdx + 5] = (ushort)(Idx + 0);

        //        //destVertices[Idx + 0] = new SimpleVertex { Position = new float3(x, y, 0f), TexCoord0 = charInfo.uvBottomLeft, Color = new float4(1, 1, 1, 1) };
        //        //destVertices[Idx + 1] = new SimpleVertex { Position = new float3(x, y + height, 0f), TexCoord0 = charInfo.uvTopLeft, Color = new float4(1, 1, 1, 1) };
        //        //destVertices[Idx + 2] = new SimpleVertex { Position = new float3(x + width, y + height, 0f), TexCoord0 = charInfo.uvTopRight, Color = new float4(1, 1, 1, 1) };
        //        //destVertices[Idx + 3] = new SimpleVertex { Position = new float3(x + width, y, 0f), TexCoord0 = charInfo.uvBottomRight, Color = new float4(1, 1, 1, 1) };

        //        //destIndices[TriIdx + 0] = (ushort)(Idx + 0);
        //        //destIndices[TriIdx + 1] = (ushort)(Idx + 1);
        //        //destIndices[TriIdx + 2] = (ushort)(Idx + 2);
        //        //destIndices[TriIdx + 3] = (ushort)(Idx + 0);
        //        //destIndices[TriIdx + 4] = (ushort)(Idx + 2);
        //        //destIndices[TriIdx + 5] = (ushort)(Idx + 3);

        //        LogUtil.LogFormat("TextMeshRenderSystem.CreateTextMesh: {0}, {1}, {2}, {3}, {4}\r\n", charInfo.index, charInfo.uvBottomLeft, charInfo.uvTopLeft, charInfo.uvTopRight, charInfo.uvBottomRight);

        //        Idx += 4;
        //        TriIdx += 6;
        //        offsetX += charInfo.advance * scale;
        //    }
        //    bb = MeshHelper.ComputeBounds(destVertices);
        //}

    }

    //public struct TextMeshBuilder
    //{
    //    private NativeArray<int> textBuffer;
    //    PFrame.Tiny.Font font;

    //    public NativeArray<ushort> Tris;
    //    public NativeArray<SimpleVertex> Verts;
    //    //public NativeArray<float3> Verts;
    //    //public NativeArray<float2> UVs;
    //    //public NativeArray<Unity.Tiny.Color> Colors;

    //    int Idx;
    //    int TriIdx;
    //    float offsetX;
    //    Unity.Tiny.Color color;
    //    int fontSize;
    //    float scale;

    //    public TextMeshBuilder(NativeArray<int> textBuffer, PFrame.Tiny.Font font, float charSize, Unity.Tiny.Color color)
    //    {
    //        this.textBuffer = textBuffer;
    //        this.font = font;
    //        this.color = color;

    //        var len = textBuffer.Length;
    //        int triCount = 2 * 3 * len;
    //        int vertCount = 4 * len;

    //        fontSize = font.FontSize;
    //        scale = charSize / fontSize;
    //        Idx = 0;
    //        TriIdx = 0;
    //        offsetX = 0;
    //        Tris = new NativeArray<ushort>(triCount, Allocator.TempJob);
    //        Verts = new NativeArray<SimpleVertex>(vertCount, Allocator.TempJob);
    //        //Verts = new NativeArray<float3>(vertCount, Allocator.TempJob);
    //        //UVs = new NativeArray<float2>(vertCount, Allocator.TempJob);
    //        //Colors = new NativeArray<Unity.Tiny.Color>(vertCount, Allocator.TempJob);
    //    }

    //    public void Execute()
    //    {
    //        var len = textBuffer.Length;
    //        var map = font.CharacterInfoMap;

    //        for (int i = 0; i < len; i++)
    //        {
    //            int charCode = textBuffer[i];
    //            var charInfo = map[charCode];
    //            PutChar(charInfo);
    //        }
    //    }

    //    private void PutChar(PFrame.Tiny.CharacterInfo charInfo)
    //    {
    //        float x = offsetX + charInfo.bearing * scale;
    //        float y = charInfo.minY * scale;
    //        var width = charInfo.glyphWidth * scale;
    //        var height = charInfo.glyphHeight * scale;

    //        //var vertex = new SimpleVertex();

    //        Verts[Idx + 0] = new SimpleVertex { Position = new float3(x, y, 0f), TexCoord0 = charInfo.uvBottomLeft, Color = color.AsFloat4() };
    //        Verts[Idx + 1] = new SimpleVertex { Position = new float3(x, y + height, 0f), TexCoord0 = charInfo.uvTopLeft, Color = color.AsFloat4() };
    //        Verts[Idx + 2] = new SimpleVertex { Position = new float3(x + width, y + height, 0f), TexCoord0 = charInfo.uvTopRight, Color = color.AsFloat4() };
    //        Verts[Idx + 3] = new SimpleVertex { Position = new float3(x + width, y, 0f), TexCoord0 = charInfo.uvBottomRight, Color = color.AsFloat4() };
    //        //Verts[Idx + 1] = new float3(x, y + height, 0f);
    //        //Verts[Idx + 2] = new float3(x + width, y + height, 0f);
    //        //Verts[Idx + 3] = new float3(x + width, y, 0f);

    //        //UVs[Idx + 0] = charInfo.uvBottomLeft;
    //        //UVs[Idx + 1] = charInfo.uvTopLeft;
    //        //UVs[Idx + 2] = charInfo.uvTopRight;
    //        //UVs[Idx + 3] = charInfo.uvBottomRight;

    //        Tris[TriIdx + 0] = (ushort)(Idx + 0);
    //        Tris[TriIdx + 1] = (ushort)(Idx + 1);
    //        Tris[TriIdx + 2] = (ushort)(Idx + 2);
    //        Tris[TriIdx + 3] = (ushort)(Idx + 0);
    //        Tris[TriIdx + 4] = (ushort)(Idx + 2);
    //        Tris[TriIdx + 5] = (ushort)(Idx + 3);

    //        Idx += 4;
    //        TriIdx += 6;
    //        offsetX += charInfo.advance * scale;
    //    }

    //    public void Dispose()
    //    {
    //        Verts.Dispose();
    //        Tris.Dispose();
    //        //UVs.Dispose();
    //        //Colors.Dispose();
    //    }
    //}
#endif

    public unsafe class TextMeshUtil
    {
        public static TextLayoutData GetTextLayoutData(
            TextMesh textMesh,
            PFrame.Tiny.Font font
            )
        {
            var text = textMesh.Text;
            var charNum = text.LengthInBytes;
            byte* chars = &text.buffer.byte0000;
            var textBuffer = new NativeArray<int>(charNum, Allocator.TempJob);
            for (int j = 0; j < charNum; j++)
            {
                textBuffer[j] = chars[j];
            }

            float charSize = textMesh.CharSize;
            var fontSize = font.FontSize;
            var scale = charSize / fontSize;

            Unity.Tiny.Color color = textMesh.Color;
            float offsetZ = textMesh.OffsetZ;
            var hAlignType = textMesh.HAlignType;
            var vAlignType = textMesh.VAlignType;
            var hSpacing = textMesh.HSpacing * scale;
            var vSpacing = textMesh.VSpacing * scale;

            float lineHeight = font.LineHeight * scale;
            //float offsetX = 0;

            var map = font.CharacterInfoMap;
            CharacterInfo charInfo;

            //float charWidth;
            //float charHeight;
            var textMinX = 0f;
            var textMinY = 0f;
            var textWidth = 0f;// = len * charWidth;
            var textHeight = lineHeight;
            var lineWidth = 0f;
            var lineMinX = 0f;
            var lineMinY = 0f;

            var line = 0;
            int lineCharIndex = 0;
            TextLineLayoutData lineLayoutData;
            var lineLayoutDataList = new NativeList<TextLineLayoutData>(Allocator.Temp);
            for (int i = 0; i < charNum; i++)
            {
                var charCode = textBuffer[i];

                if (charCode == '\n')
                {
                    textHeight += lineHeight;
                    textHeight += vSpacing;

                    if (hAlignType == EHAlignType.Middle)
                    {
                        lineMinX = -(lineWidth) * 0.5f;
                    }
                    else if (hAlignType == EHAlignType.Right)
                    {
                        lineMinX = -(lineWidth);
                    }

                    lineLayoutData = new TextLineLayoutData();
                    lineLayoutData.LineIndex = line;
                    lineLayoutData.CharIndex = lineCharIndex;
                    lineLayoutData.CharNum = i - lineCharIndex;
                    lineLayoutData.Width = lineWidth;
                    lineLayoutData.MinX = lineMinX;
                    lineLayoutDataList.Add(lineLayoutData);

                    line++;
                    lineCharIndex = i + 1;

                    textMinX = math.min(textMinX, lineMinX);
                    textWidth = math.max(textWidth, lineWidth);
                    lineWidth = 0f;
                    continue;
                }

                if (!map.TryGetValue(charCode, out charInfo))
                {
                    charInfo = map[' '];
                }

                if (i > 0)
                    lineWidth += hSpacing;

                lineWidth += charInfo.advance * scale;
            }

            //last line
            if (hAlignType == EHAlignType.Middle)
            {
                lineMinX = -(lineWidth) * 0.5f;
            }
            else if (hAlignType == EHAlignType.Right)
            {
                lineMinX = -(lineWidth);
            }

            lineLayoutData = new TextLineLayoutData();
            lineLayoutData.LineIndex = line;
            lineLayoutData.CharIndex = lineCharIndex;
            lineLayoutData.CharNum = charNum - lineCharIndex;
            lineLayoutData.Width = lineWidth;
            lineLayoutData.MinX = lineMinX;
            lineLayoutDataList.Add(lineLayoutData);

            line++;

            textMinX = math.min(textMinX, lineMinX);
            textWidth = math.max(textWidth, lineWidth);

            if (vAlignType == EVAlignType.Middle)
            {
                textMinY = -(textHeight) * 0.5f;
            }
            else if (vAlignType == EVAlignType.Top)
            {
                textMinY = -(textHeight);
            }

            textBuffer.Dispose();

            TextLayoutData textLayoutData = new TextLayoutData();
            textLayoutData.LineNum = line;
            textLayoutData.CharNum = charNum - (line -1);

            textLayoutData.MinX = textMinX;
            textLayoutData.MinY = textMinY;
            textLayoutData.Width = textWidth;
            textLayoutData.Height = textHeight;

            textLayoutData.LineLayoutDataList = lineLayoutDataList;

            return textLayoutData;
        }

        public static void CreateTextMesh(
            TextMesh textMesh,
            PFrame.Tiny.Font font,
            TextLayoutData textLayoutData,
            Action<CharacterInfo, float3, float> action
            )
        {
            var text = textMesh.Text;
            var charNum = text.LengthInBytes;
            byte* chars = &text.buffer.byte0000;
            var textBuffer = new NativeArray<int>(charNum, Allocator.TempJob);
            for (int j = 0; j < charNum; j++)
            {
                textBuffer[j] = chars[j];
            }

            float charSize = textMesh.CharSize;
            var fontSize = font.FontSize;
            var scale = charSize / fontSize;

            Unity.Tiny.Color color = textMesh.Color;
            float offsetZ = textMesh.OffsetZ;
            //var hAlignType = textMesh.HAlignType;
            //var vAlignType = textMesh.VAlignType;
            var hSpacing = textMesh.HSpacing * scale;
            var vSpacing = textMesh.VSpacing * scale;


            float lineHeight = font.LineHeight * scale;
            //float offsetX = 0;

            var map = font.CharacterInfoMap;
            CharacterInfo charInfo;

            //var textLayoutData = GetTextLayoutData(textMesh, font);

            //int triCount = 2 * 3 * charNum;
            //int vertCount = 4 * charNum;

            //TextLineLayoutData lineLayoutData;
            //int lineCharIndex = 0;
            var lineNum = textLayoutData.LineNum;
            var lineLayoutDataList = textLayoutData.LineLayoutDataList;
            var textMinY = textLayoutData.MinY;
            for (int j = 0; j < lineNum; j++)
            {
                float lineOffsetX = 0f;
                float lineOffsetY = 0f;
                var lineLayoutData = lineLayoutDataList[j];
                var lineCharIndex = lineLayoutData.CharIndex;
                var lineMinX = lineLayoutData.MinX;
                var lineMinY = lineLayoutData.MinY;
                var lineCharNum = lineLayoutData.CharNum;
                lineOffsetY = textMinY + (lineNum - 1 - j) * (lineHeight + vSpacing);

                for (int i = 0; i < lineCharNum; i++)
                {
                    if (i > 0)
                        lineOffsetX += hSpacing;

                    var charCode = textBuffer[lineCharIndex + i];

                    if (!map.TryGetValue(charCode, out charInfo))
                    {
                        charInfo = map[' '];
                    }

                    var x = lineMinX + lineOffsetX;
                    var y = lineOffsetY;
                    action(charInfo, new float3(x, y, offsetZ), scale);

                    lineOffsetX += charInfo.advance * scale;
                }
            }

            //for (int i = 0; i < charNum; i++)
            //{
            //    int charCode = textBuffer[i];
            //    //var charInfo = map[charCode];
            //    if (!map.TryGetValue(charCode, out charInfo))
            //    {
            //        charInfo = map[' '];
            //    }

            //    action(charInfo, new float3(offsetX, 0, offsetZ), scale);
            //    //float minx = offsetX + charInfo.minX * scale;
            //    //float maxx = offsetX + charInfo.maxX * scale;
            //    //float miny = charInfo.minY * scale;
            //    //float maxy = charInfo.maxY * scale;

            //    //var bl = charInfo.uvBottomLeft;
            //    //bl.y = 1f - bl.y;
            //    //var br = charInfo.uvBottomRight;
            //    //br.y = 1f - br.y;
            //    //var tr = charInfo.uvTopRight;
            //    //tr.y = 1f - tr.y;
            //    //var tl = charInfo.uvTopLeft;
            //    //tl.y = 1f - tl.y;

            //    //destVertices[Idx + 0] = new SimpleVertex { Position = new float3(minx, miny, offsetZ), TexCoord0 = bl, Color = color.AsFloat4() };
            //    //destVertices[Idx + 1] = new SimpleVertex { Position = new float3(maxx, miny, offsetZ), TexCoord0 = br, Color = color.AsFloat4() };
            //    //destVertices[Idx + 2] = new SimpleVertex { Position = new float3(maxx, maxy, offsetZ), TexCoord0 = tr, Color = color.AsFloat4() };
            //    //destVertices[Idx + 3] = new SimpleVertex { Position = new float3(minx, maxy, offsetZ), TexCoord0 = tl, Color = color.AsFloat4() };

            //    //destIndices[TriIdx + 0] = (ushort)(Idx + 0);
            //    //destIndices[TriIdx + 1] = (ushort)(Idx + 1);
            //    //destIndices[TriIdx + 2] = (ushort)(Idx + 2);
            //    //destIndices[TriIdx + 3] = (ushort)(Idx + 2);
            //    //destIndices[TriIdx + 4] = (ushort)(Idx + 3);
            //    //destIndices[TriIdx + 5] = (ushort)(Idx + 0);

            //    //Idx += 4;
            //    //TriIdx += 6;

            //    //LogUtil.LogFormat("TextMeshRenderSystem.CreateTextMesh: {0}, {1}, {2}, {3}, {4}\r\n", charInfo.index, charInfo.uvBottomLeft, charInfo.uvTopLeft, charInfo.uvTopRight, charInfo.uvBottomRight);

            //    offsetX += charInfo.advance * scale;
            //}

            textBuffer.Dispose();
        }
    }
}

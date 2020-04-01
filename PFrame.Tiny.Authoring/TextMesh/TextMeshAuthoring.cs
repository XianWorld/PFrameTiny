using PFrame.Entities;
using PFrame.Tiny.Hybrid;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace PFrame.Tiny.Authoring
{
    //[ExecuteAlways]
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(MeshFilter))]
    public class TextMeshAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        [Multiline]
        public string Text;
        public FontDataSO FontData;
        public float CharacterSize = 1f;
        public float OffsetZ;

        public EHAlignType HAlignType;
        public EVAlignType VAlignType;

        public float HSpacing;
        public float VSpacing;

        public Color Color = Color.white;

        private void OnValidate()
        {
            if (FontData == null)
                return;

            var meshRenderer = GetComponent<MeshRenderer>();
            if(meshRenderer.sharedMaterial != FontData.Font.material)
                meshRenderer.sharedMaterial = FontData.Font.material;

            if (Text == null || FontData == null)
                return;

            var meshFilter = GetComponent<MeshFilter>();

            Mesh mesh = meshFilter.mesh;
            if (mesh != null)
                mesh.Clear();
            else
            {
                mesh = new Mesh();
                meshFilter.mesh = mesh;
            }
            //Mesh mesh = new Mesh();

            //var idx = 4;
            //var triIdx = 6;
            //var _color = Color.red;

            //var Verts = new Vector3[4];
            //Verts[0] = new Vector3(0f, 0f);
            //Verts[1] = new Vector3(0f, 1f);
            //Verts[2] = new Vector3(1f, 1f);
            //Verts[3] = new Vector3(1f, 0f);

            //var UVs = new Vector2[4];
            //UVs[0] = new Vector2(0f, 0f);
            //UVs[1] = new Vector2(0f, 1f);
            //UVs[2] = new Vector2(1f, 1f);
            //UVs[3] = new Vector2(1f, 0f);

            //var Tris = new int[6];
            //Tris[0] = 0;
            //Tris[1] = 1;
            //Tris[2] = 2;
            //Tris[3] = 0;
            //Tris[4] = 2;
            //Tris[5] = 3;

            //mesh.SetVertices(Verts, 0, idx);
            //mesh.SetUVs(0, UVs, 0, idx);
            //mesh.SetIndices(Tris, 0, triIdx, MeshTopology.Triangles, 0, true, 0);
            //var colors = new NativeArray<Color>(idx, Allocator.Temp);
            //for (int i = 0; i < idx; i++)
            //{
            //    colors[i] = _color;
            //}
            //mesh.SetColors(colors, 0, idx);

            var font = TinyAuthoringUtil.Convert(FontData.Font);

            //var textBuffer = new NativeArray<int>(Text.Length, Allocator.TempJob);
            //for (int j = 0; j < Text.Length; j++)
            //{
            //    textBuffer[j] = Text[j];
            //}
            //var color = TinyAuthoringUtil.Convert(Color);

            //var builder = new TextMeshHybridBuilder(textBuffer, font, CharacterSize, color, OffsetZ);
            //builder.Execute();

            //mesh.SetVertices(builder.Verts);
            //mesh.SetUVs(0, builder.UVs);
            //mesh.SetIndices(builder.Tris, 0, builder.Tris.Length, MeshTopology.Triangles, 0, true, 0);
            ////var colors = new NativeArray<Color>(idx, Allocator.Temp);
            ////for (int i = 0; i < idx; i++)
            ////{
            ////    colors[i] = _color;
            ////}
            //mesh.SetColors(builder.Colors);

            //builder.Dispose();

            var textMesh = GetTextMesh();
            TextMeshHybridUtil.CreateTextMesh(textMesh, font, mesh);

            //textBuffer.Dispose();
            font.Dispose();
        }

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            //var textMesh = new TextMesh();
            //textMesh.Text = Text;
            //if(FontData != null)
            //    textMesh.FontId = FontData.Id;
            //textMesh.CharSize = CharacterSize;
            //textMesh.Color = TinyAuthoringUtil.Convert(Color);
            //textMesh.OffsetZ = OffsetZ;

            var textMesh = GetTextMesh();

            dstManager.AddComponentData(entity, textMesh);
        }

        private TextMesh GetTextMesh()
        {
            var textMesh = new TextMesh();
            textMesh.Text = Text;
            if (FontData != null)
                textMesh.FontId = FontData.Id;
            textMesh.CharSize = CharacterSize;
            textMesh.Color = TinyAuthoringUtil.Convert(Color);
            textMesh.OffsetZ = OffsetZ;

            textMesh.HAlignType = HAlignType;
            textMesh.VAlignType = VAlignType;
            textMesh.HSpacing = HSpacing;
            textMesh.VSpacing = VSpacing;

            return textMesh;
        }
    }

}

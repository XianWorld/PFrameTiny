//using Unity.Entities;
//using Unity.Mathematics;
//using Unity.Tiny;
//using UnityEngine;

//namespace PFrame.Tiny.SimpleUI.Authoring
//{
//    public class UI3DIconTextAuthoring : MonoBehaviour, IConvertGameObjectToEntity
//    {
//        public UI3DIconFontAuthoring FontAuthoring;
//        public string Text = "";
//        public EHAlignType HAlignType;
//        public EVAlignType VAlignType;
//        public float HSpacing;
//        public float VSpacing;

//        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
//        {
//            var fontId = 0;
//            if (FontAuthoring != null)
//                fontId = FontAuthoring.Id;

//            //bool isNeedUpdate = Text != "";
//            var textComp = new UI3DIconText
//            {
//                FontId = fontId,
//                Text = Text,
//                HAlignType = (int)HAlignType,
//                VAlignType = (int)VAlignType,
//                HSpacing = HSpacing,
//                VSpacing = VSpacing,
//                //IsNeedUpdate = isNeedUpdate
//            };
//            dstManager.AddComponentData(entity, textComp);

//            var buffer = dstManager.AddBuffer<UI3DIconCharElement>(entity);

//            //UnityEngine.Debug.Log("UI3DIconTextAuthoring: " + entity);
//        }

//        private void OnDrawGizmos()
//        {
//            if (FontAuthoring == null)
//                return;

//            Gizmos.color = UnityEngine.Color.blue;

//            var prefabs = FontAuthoring.IconPrefabs;
//            var startCharCode = (int)FontAuthoring.StartChar;
//            var iconElementNum = prefabs.Length;
//            var charWidth = FontAuthoring.CharWidth;
//            var charHeight = FontAuthoring.CharHeight;

//            //add new char elements
//            var text = Text;
//            if (!FontAuthoring.IsSupportLowerCase)
//                text = text.ToUpper();
//            var chars = text.ToCharArray();

//            var len = chars.Length;
//            if(len > 0)
//            {
//                //calculate x, y 
//                var minX = 0f;
//                var minY = 0f;
//                var textWidth = len * charWidth;
//                var textHeight = charHeight;
//                if (HAlignType == EHAlignType.Middle)
//                {
//                    minX = -(textWidth + HSpacing * (len - 1)) * 0.5f;
//                }
//                else if (HAlignType == EHAlignType.Right)
//                {
//                    minX = -(textWidth + HSpacing * (len - 1));
//                }

//                if (VAlignType == EVAlignType.Middle)
//                {
//                    minY = -(textHeight + VSpacing * (len - 1)) * 0.5f;
//                }
//                else if (VAlignType == EVAlignType.Top)
//                {
//                    minY = -(textHeight + VSpacing * (len - 1));
//                }

//                for (int i = 0; i < len; i++)
//                {
//                    var charCode = (int)chars[i];
//                    var index = charCode - startCharCode;
//                    if (index < 0 || index >= iconElementNum)
//                        continue;

//                    var prefab = prefabs[index];
//                    if (prefab == null)
//                        continue;

//                    var prefabMatrix = prefab.transform.localToWorldMatrix;
//                    var a = transform.localToWorldMatrix * prefabMatrix;
//                    var rot = a.rotation;
//                    //var pos = a.MultiplyPoint(Vector3.zero);
//                    var scale = Vector3.Scale(transform.lossyScale, prefab.transform.localScale);
//                    float4x4 b = a;

//                    var pos = transform.TransformPoint(new Vector3(minX + i * charWidth + HSpacing * i, minY, 0f));
//                    var mesh = prefab.GetComponent<MeshFilter>().sharedMesh;
//                    Gizmos.DrawMesh(mesh, 0, pos, rot, scale);
//                }
//            }
//        }
//    }
//}

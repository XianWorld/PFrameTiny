//using System.Collections.Generic;
//using Unity.Entities;
//using Unity.Tiny;
//using UnityEngine;

//namespace PFrame.Tiny.SimpleUI.Authoring
//{
//    public class UI3DIconFontAuthoring : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
//    {
//        public int Id;
//        public char StartChar;
//        public float CharWidth = 0.01f;
//        public float CharHeight = 0.01f;
//        public GameObject[] IconPrefabs;
//        public bool IsSupportLowerCase;

//        //private EditorUI3DIconFont font;
//        //public EditorUI3DIconFont Font => font;

//        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
//        {
//            var manager = new UI3DIconFont
//            {
//                Id = Id,
//                StartChar = StartChar,
//                CharWidth = CharWidth,
//                CharHeight = CharHeight,
//                IsSupportLowerCase = IsSupportLowerCase
//            };
//            dstManager.AddComponentData<UI3DIconFont>(entity, manager);

//            var buffer = dstManager.AddBuffer<UI3DIconFontChar>(entity);

//            if (IconPrefabs != null)
//            {
//                for (int i = 0; i < IconPrefabs.Length; i++)
//                {
//                    var iconPrefab = IconPrefabs[i];
//                    var iconEntity = conversionSystem.GetPrimaryEntity(iconPrefab);
//                    buffer.Add(new UI3DIconFontChar { CharPrefab = iconEntity });
//                }
//            }

//            //UnityEngine.Debug.Log("UI3DIconFontAuthoring: " + manager);

//            //if (font != null)
//            //    font.Dispose();

//            //font = new EditorUI3DIconFont(dstManager, entity, conversionSystem, this);
//        }

//        public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
//        {
//            if (IconPrefabs != null)
//            {
//                for (int i = 0; i < IconPrefabs.Length; i++)
//                {
//                    referencedPrefabs.Add(IconPrefabs[i]);
//                }
//            }
//        }

//        //private void OnDestroy()
//        //{
//        //    if (font != null)
//        //        font.Dispose();
//        //}
//    }
//}

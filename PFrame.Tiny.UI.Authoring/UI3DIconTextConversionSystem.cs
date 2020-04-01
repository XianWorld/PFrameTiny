//using PFrame.Entities;
//using Unity.Entities;
//using Unity.Rendering;
//using Unity.Tiny;
//using Unity.Transforms;
//using UnityEngine;

//namespace PFrame.Tiny.SimpleUI.Authoring
//{
//    public class EditorUI3DIconFont : UI3DIconFont
//    {
//        private UI3DIconFontAuthoring fontAuthoring;
//        private GameObjectConversionSystem conversionSystem;

//        public EditorUI3DIconFont(EntityManager entityManager, Entity entity, GameObjectConversionSystem conversionSystem, UI3DIconFontAuthoring fontAuthoring) : base(entityManager, entity)
//        {
//            this.fontAuthoring = fontAuthoring;
//            this.conversionSystem = conversionSystem;
//        }

//        protected override Entity CreateCharEntity(EntityManager entityManager, Entity prefab)
//        {
//            var entity = conversionSystem.CreateAdditionalEntity(fontAuthoring);

//            EntityUtil.CopyComponentData<RenderBounds>(entityManager, prefab, entity);
//            EntityUtil.CopyComponentData<WorldRenderBounds>(entityManager, prefab, entity);
//            EntityUtil.CopyComponentData<ChunkWorldRenderBounds>(entityManager, prefab, entity);

//            EntityUtil.CopySharedComponentData<RenderMesh>(entityManager, prefab, entity);

//            EntityUtil.CopyComponentData<LocalToWorld>(entityManager, prefab, entity);
//            EntityUtil.CopyComponentData<Rotation>(entityManager, prefab, entity);
//            EntityUtil.CopyComponentData<Translation>(entityManager, prefab, entity);

//            return entity;
//        }
//    }

//    [UpdateInGroup(typeof(GameObjectAfterConversionGroup))]
//    internal class UI3DIconTextConversionSystem : GameObjectConversionSystem
//    {
//        //private EditorUI3DIconFont font;

//        //protected override void OnStartRunning()
//        //{
//        //    base.OnStartRunning();

//        //    Entities.ForEach((UI3DIconFontAuthoring fontAuthoring) =>
//        //    {
//        //        var primaryEntity = GetPrimaryEntity(fontAuthoring);
//        //        //var buffer = DstEntityManager.GetBuffer<UI3DIconFontChar>(primaryEntity);

//        //        font = new EditorUI3DIconFont(EntityManager, primaryEntity, this, fontAuthoring);
//        //    });

//        //}

//        //protected override void OnStopRunning()
//        //{
//        //    if(font != null)
//        //        font.Dispose();
//        //}

//        protected override void OnUpdate()
//        {
//            //EditorUI3DIconFont font = null;

//            //Entities.ForEach((UI3DIconFontAuthoring iconFont) =>
//            //{
//            //    var primaryEntity = GetPrimaryEntity(iconFont);
//            //    //var buffer = DstEntityManager.GetBuffer<UI3DIconFontChar>(primaryEntity);

//            //    //DstEntityManager.Instantiate(element.IconPrefab);
//            //    font = iconFont.Font;
//            //});

//            //Entities.ForEach((UI3DIconTextAuthoring textAuthoring) =>
//            //{
//            //    var primaryEntity = GetPrimaryEntity(textAuthoring);
//            //    var buffer = DstEntityManager.GetBuffer<UI3DIconCharElement>(primaryEntity);

//            //    font.UpdateText(DstEntityManager, primaryEntity);

//            //    UnityEngine.Debug.Log("UI3DIconTextConversionSystem: " + textAuthoring.Text);
//            //});
//        }
//    }
//}

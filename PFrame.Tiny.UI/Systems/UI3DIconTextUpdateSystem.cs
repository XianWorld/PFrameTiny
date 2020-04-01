//using PFrame.Entities;
//using Unity.Entities;
//using Unity.Transforms;

//namespace PFrame.Tiny.SimpleUI
//{
//    [UpdateAfter(typeof(UI3DIconFontSystem))]
//    [UpdateBefore(typeof(TransformSystemGroup))]
//    public class UI3DIconTextUpdateSystem : ComponentSystem
//    {
//        private UI3DIconFontSystem fontSystem;
//        protected override void OnCreate()
//        {
//            base.OnCreate();
//            fontSystem = World.GetExistingSystem<UI3DIconFontSystem>();
//        }

//        protected override void OnUpdate()
//        {
//            //UnityEngine.Debug.Log("UI3DIconTextUpdateSystem.OnUpdate!");

//            Entities.ForEach((Entity entity, ref UI3DIconText textComp, ref UISetTextCmd setTextCmdComp) =>
//            {
//                var text = textComp.Text;
//                var newText = setTextCmdComp.Text;

//                EntityManager.RemoveComponent<UISetTextCmd>(entity);

//                if (text.Equals(newText))
//                    return;

//                textComp.Text = newText;
//                //textComp.IsNeedUpdate = true;
//                if (EntityManager.HasComponent<UpdatedState>(entity))
//                    EntityManager.RemoveComponent<UpdatedState>(entity);
//            });

//            Entities
//                .WithNone<UpdatedState>()
//                .ForEach((Entity entity, ref UI3DIconText textComp) =>
//            {
//                //if (!textComp.IsNeedUpdate)
//                //    return;
//                //textComp.IsNeedUpdate = false;

//                var font = fontSystem.GetFont(textComp.FontId);
//                //UnityEngine.Debug.Log("UI3DIconTextUpdateSystem.OnUpdate: " + font);
//                if (font.IsValid())
//                {
//                    font.UpdateText(EntityManager, entity);

//                    EntityManager.AddComponent<UpdatedState>(entity);
//                }
//            });

//            Entities
//                .WithNone<Parent>()
//                .ForEach((Entity entity, ref UI3DIconChar charComp) =>
//            {
//                EntityManager.DestroyEntity(entity);
//            });

//        }
//    }
//}
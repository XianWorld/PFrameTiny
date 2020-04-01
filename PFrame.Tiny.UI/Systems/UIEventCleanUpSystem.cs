//using PFrame.Entities;
//using Unity.Entities;

//namespace PFrame.Tiny.SimpleUI
//{
//    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
//    public class UIEventCleanUpSystem : ComponentSystem
//    {
//        protected override void OnUpdate()
//        {
//            Entities.ForEach((Entity entity, ref UIPointerEnterEvent eventComp) =>
//            {
//                LogUtil.Log("UIEventCleanUpSystem: Pointer Enter");
//                EntityManager.RemoveComponent<UIPointerEnterEvent>(entity);
//            });
//            Entities.ForEach((Entity entity, ref UIPointerExitEvent eventComp) =>
//            {
//                LogUtil.Log("UIEventCleanUpSystem: Pointer Exit");
//                EntityManager.RemoveComponent<UIPointerExitEvent>(entity);
//            });
//            Entities.ForEach((Entity entity, ref UIPointerUpEvent eventComp) =>
//            {
//                LogUtil.LogFormat("UIEventCleanUpSystem: Pointer Up: {0}, {1}", entity, eventComp.buttonIndex);
//                EntityManager.RemoveComponent<UIPointerUpEvent>(entity);
//            });
//            Entities.ForEach((Entity entity, ref UIPointerClickEvent eventComp) =>
//            {
//                LogUtil.LogFormat("UIEventCleanUpSystem: Pointer Click: {0}, {1}", entity, eventComp.buttonIndex);
//                EntityManager.RemoveComponent<UIPointerClickEvent>(entity);
//            });
//        }
//    }
//}
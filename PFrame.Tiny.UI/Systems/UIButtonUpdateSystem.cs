using PFrame.Entities;
using PFrame.Tiny.Tweens;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace PFrame.Tiny.UI
{
    [UpdateBefore(typeof(ServiceUpdateSystemGroup))]
    [UpdateAfter(typeof(PointerInteractSystem))]
    public class UIButtonUpdateSystem : ComponentSystem
    {
        private TweenSystem tweenSystem;

        protected override void OnStartRunning()
        {
            base.OnStartRunning();
            tweenSystem = World.GetExistingSystem<TweenSystem>();
        }

        protected override void OnUpdate()
        {
            Entities.ForEach((Entity entity, ref UIButton button, ref PointerClickEvent clickEvent) =>
            {
                EntityManager.AddComponent<UIButtonClickEvent>(entity, false, true);
            });

            Entities.ForEach((Entity entity, ref UIButton button, ref TweenScaleTransition transition, ref PointerEnterEvent enterEvent) =>
            {
                var scale = transition.OverScale;
                var duration = transition.Duration;

                //LogUtil.LogFormat("UIButtonUpdateSystem: {0}", scale);

                var one = new float3(1f);
                if (!EntityManager.HasComponent<NonUniformScale>(entity))
                    EntityManager.AddComponentData(entity, new NonUniformScale { Value = one });

                var typeInfo = TypeManager.GetTypeInfo<NonUniformScale>();
                var info = TweenSystem.GetFieldArgs(typeInfo.TypeIndex, (int)PrimitiveFieldTypes.Float3, 0);

                tweenSystem.AddTween<float3>(
                    entity,
                    info,//EntityUtil.FieldInfo_Scale,
                    one,
                    new float3(scale),
                    duration
                    );
            });

            Entities.ForEach((Entity entity, ref UIButton button, ref TweenScaleTransition transition, ref PointerExitEvent exitEvent) =>
            {
                var scale = transition.OverScale;
                var duration = transition.Duration;

                var one = new float3(1f);
                if (!EntityManager.HasComponent<NonUniformScale>(entity))
                    EntityManager.AddComponentData(entity, new NonUniformScale { Value = one });

                var typeInfo = TypeManager.GetTypeInfo<NonUniformScale>();
                var info = TweenSystem.GetFieldArgs(typeInfo.TypeIndex, (int)PrimitiveFieldTypes.Float3, 0);

                tweenSystem.AddTween<float3>(
                    entity,
                    info,//EntityUtil.FieldInfo_Scale,
                    new float3(scale),
                    one,
                    duration
                    );
            });

            Entities.ForEach((Entity entity, ref UIButton button, ref TweenScaleTransition transition, ref PointerDownEvent downEvent) =>
            {
                var scale = transition.PressedScale;
                EntityManager.SetOrAddComponentData(entity, new NonUniformScale { Value = scale });
            });

            Entities.ForEach((Entity entity, ref UIButton button, ref TweenScaleTransition transition, ref PointerUpEvent upEvent) =>
            {
                var scale = transition.OverScale;
                EntityManager.SetOrAddComponentData(entity, new NonUniformScale { Value = scale });
            });
        }
    }
}
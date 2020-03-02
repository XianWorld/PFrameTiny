using PFrame.Entities;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Tiny.Rendering;

namespace PFrame.Tiny.SimpleUI
{
    [UpdateBefore(typeof(TransformSystemGroup))]
    public class UIRootUpdateSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((Entity entity, ref UIRoot uiRootComp, ref Translation pos, ref Rotation rot) =>
            {
                var cameraEntity = uiRootComp.CameraEntity;
                var offset = uiRootComp.Offset;
                var type = uiRootComp.LocationType;

                if (type == 0)
                    return;

                if (cameraEntity == Entity.Null)
                    cameraEntity = GetSingletonEntity<Camera>();

                //float3 cameraPos = float3.zero;
                //quaternion cameraRot = quaternion.identity;
#if UNITY_DOTSPLAYER
                var cameraPos = EntityManager.GetComponentData<Translation>(cameraEntity).Value;
                var cameraRot = EntityManager.GetComponentData<Rotation>(cameraEntity).Value;
#else
                var cameraTransform = UnityEngine.Camera.main.transform;
                var cameraPos = (float3)cameraTransform.position;
                var cameraRot = (quaternion)cameraTransform.rotation;
#endif

                pos.Value = cameraPos + math.mul(cameraRot, offset);
                rot.Value = cameraRot;
            });
        }
    }
}
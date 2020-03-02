using PFrame.Entities;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Tiny;
using Unity.Tiny.Input;
using Unity.Transforms;
#if UNITY_DOTSPLAYER
using Unity.Tiny.Rendering;
#else
using UnityEngine;
#endif

namespace PFrame.Tiny.SimpleUI
{
    [UpdateBefore(typeof(ServiceUpdateSystemGroup))]
    public class UIInteractSystem : ComponentSystem
    {
        private float2[] poly = new float2[4];

        protected override void OnCreate()
        {
            base.OnCreate();
#if UNITY_DOTSPLAYER
            RequireSingletonForUpdate<Camera>();
#endif
        }

        protected override void OnUpdate()
        {
            var inputSystem = World.GetExistingSystem<InputSystem>();

#if UNITY_DOTSPLAYER
            var cameraEntity = GetSingletonEntity<Camera>();
            var camera = EntityManager.GetComponentData<Camera>(cameraEntity);
            if (!EntityManager.HasComponent<CameraMatrices>(cameraEntity))
                return;

            var cameraMatrices = EntityManager.GetComponentData<CameraMatrices>(cameraEntity);
            var p = cameraMatrices.projection;
            var v = cameraMatrices.view;

            var vp = math.mul(p, v);
            var fov = camera.fov;
            var aspect = camera.aspect;

            TinyEnvironment env = World.TinyEnvironment();
            DisplayInfo di = env.GetConfigData<DisplayInfo>();
            var pixelHeight = di.height;
            var pixelWidth = di.width;

            //var Input = World.GetExistingSystem<InputSystem>();
            var inputPos = inputSystem.GetInputPosition();
#else

            var camera = Camera.main;
            float4x4 p = camera.projectionMatrix;
            float4x4 v = camera.worldToCameraMatrix;

            var vp = math.mul(p, v);
            var fov = camera.fieldOfView;
            var aspect = camera.aspect;

            var pixelHeight = camera.pixelHeight;
            var pixelWidth = camera.pixelWidth;

            var inputPos = Input.mousePosition;
#endif

            var downButtonIndex = InputUtil.GetButtonDown(inputSystem);
            var upButtonIndex = InputUtil.GetButtonUp(inputSystem);
            var pressedButtonIndex = InputUtil.GetButton(inputSystem);

            var pixelHeight2 = pixelHeight / 2f;
            var pixelWidth2 = pixelWidth / 2f;

            var inputPosX = inputPos.x;
            var inputPosY = inputPos.y;
            var inputPos2 = new float2(inputPosX, inputPosY);

            Entities
                .WithAll<UIInteractable>()
                .ForEach((Entity entity, ref UIElement uiElementComp, ref UIInteractable interactableComp, ref LocalToWorld localToWorld) =>
            {
                //var rect = uiElementComp.Rect;

                var m = localToWorld.Value;

                //var mvp = vp * m;

                //var p0 = uiElementComp.Point0;
                //var p1 = math.transform(m, p0);
                //var p2 = math.transform(v, p1);
                //var p3 = math.transform(p, p2);

                //var p4 = math.transform(vp, p1);

                //var p5 = math.transform(mvp, p0);

                var mvp = math.mul(vp, m);

                poly[0] = localToScreen(mvp, uiElementComp.Point0, pixelWidth2, pixelHeight2);
                poly[1] = localToScreen(mvp, uiElementComp.Point1, pixelWidth2, pixelHeight2);
                poly[2] = localToScreen(mvp, uiElementComp.Point2, pixelWidth2, pixelHeight2);
                poly[3] = localToScreen(mvp, uiElementComp.Point3, pixelWidth2, pixelHeight2);

                bool isIn = MathUtil.IsInPolygon(poly, inputPos2);

                //var pos = localToWorld.Position;
                ////var pos3 = mul(vp, pos);
                //var pos3 = math.transform(vp, pos);

                //LogUtil.LogFormat("UIInteractSystem: {0},{1},{2},{3},{4}", inputPos2, poly[0], poly[1], poly[2], poly[3]);

                ////1.798722
                //var x = pos3.x * 0.5f + 1f; //0.9629341
                //var y = pos3.y * 0.5f + 1f; //1.732051
                //var sox = x * pixelWidth2;
                //var soy = y * pixelHeight2;

                ////var dis = -mul(v, pos).z;
                //var pp = math.transform(v, pos);
                //var dis = -pp.z;
                //var rate = dis * math.tan(math.radians(fov * 0.5f));
                //var sx = sox + rect.x / rate * pixelHeight2;
                //var sy = soy + rect.y / rate * pixelHeight2;
                //var sw = (rect.width / rate) * pixelHeight2;
                //var sh = (rect.height / rate) * pixelHeight2;

                //bool isIn = (inputPosX >= sx && inputPosX <= sx + sw
                //    && inputPosY >= sy && inputPosY <= sy + sh);

                //LogUtil.Log("UIInteractSystem: " + inputPos2 + ", " + isIn);

                var isEnter = interactableComp.IsPointerEnter;
                if (isIn)
                {
                    //LogUtil.Log("UIInteractSystem: " + entity);
                    if (!isEnter)
                    {
                        interactableComp.IsPointerEnter = true;
                        EntityManager.AddComponent<UIPointerEnterEvent>(entity);

                        if (pressedButtonIndex >= 0)
                        {
                            interactableComp.DownButtonIndex = pressedButtonIndex;
                            interactableComp.IsPointerDown = true;
                        }
                    }
                    if (downButtonIndex >= 0)
                    {
                        var downEvent = new UIPointerDownEvent
                        {
                            buttonIndex = downButtonIndex
                        };
                        EntityManager.AddComponentData(entity, downEvent);
                        interactableComp.DownButtonIndex = downButtonIndex;
                        interactableComp.IsPointerDown = true;
                    }
                    if (upButtonIndex >= 0)
                    {
                        var upEvent = new UIPointerUpEvent
                        {
                            buttonIndex = upButtonIndex
                        };
                        EntityManager.AddComponentData(entity, upEvent);
                        if (interactableComp.DownButtonIndex == upButtonIndex)
                        {
                            var clickEvent = new UIPointerClickEvent
                            {
                                buttonIndex = upButtonIndex
                            };
                            EntityManager.AddComponentData(entity, clickEvent);
                        }
                        interactableComp.IsPointerDown = false;
                        interactableComp.DownButtonIndex = -1;
                    }
                }
                else
                {
                    if (isEnter)
                    {
                        interactableComp.DownButtonIndex = -1;
                        interactableComp.IsPointerEnter = false;
                        interactableComp.IsPointerDown = false;
                        EntityManager.AddComponent<UIPointerExitEvent>(entity);
                    }
                }
            });
        }

        //public static float3 mul(float4x4 matrix, float3 pos)
        //{
        //    float4 pos4 = new float4(pos, 1.0f);
        //    //pos4 = math.mul(matrix, pos4);
        //    pos4 = math.mul(pos4, matrix);
        //    var pos3 = new float3(pos4.x, pos4.y, pos4.z) / pos4.w;
        //    return pos3;
        //}

        private float2 localToScreen(float4x4 mvp, float3 pos, float screenWidth2, float screenHeight2)
        {
            var pos4 = math.mul(mvp, new float4(pos, 1f));
            var x = pos4.x / pos4.w + 1f; //0.9629341
            var y = pos4.y / pos4.w + 1f; //1.732051
            //var pos3 = math.transform(mvp, pos);
            //var x = pos3.x / pos3.z + 1f; //0.9629341
            //var y = pos3.y / pos3.z + 1f; //1.732051
            var sox = x * screenWidth2;
            var soy = y * screenHeight2;
            return new float2(sox, soy);
        }
    }
}
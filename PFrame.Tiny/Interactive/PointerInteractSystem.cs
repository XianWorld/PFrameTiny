using PFrame.Entities;
using System;
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

namespace PFrame.Tiny
{
    public struct InteractableData : IComparable<InteractableData>
    {
        public Entity Entity;
        public PointerInteractable Interactable;
        public float4x4 Matrix;

        public int CompareTo(InteractableData other)
        {
            return Interactable.CompareTo(other.Interactable);
        }
    }

    [UpdateBefore(typeof(ServiceUpdateSystemGroup))]
    public class PointerInteractSystem : ComponentSystem
    {
        //private float2[] poly = new float2[4];
        //private EntityQuery interactableQuery;
        private NativeList<InteractableData> dataList;

        protected override void OnCreate()
        {
            base.OnCreate();
#if UNITY_DOTSPLAYER
            RequireSingletonForUpdate<Camera>();
#endif

            //interactableQuery = GetEntityQuery(typeof(PointerInteractable));
            dataList = new NativeList<InteractableData>(Allocator.Persistent);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            dataList.Dispose();
        }

        protected override void OnUpdate()
        {
            //var num = interactableQuery.CalculateEntityCount();
            //if (num == 0)
            //    return;
            Entities.ForEach((Entity entity, ref PointerInteractable interactable, ref LocalToWorld localToWorld) =>
            {
                var data = new InteractableData
                {
                    Entity = entity,
                    Interactable = interactable,
                    Matrix = localToWorld.Value
                };
                dataList.Add(data);
            });

            var dataNum = dataList.Length;
            if (dataNum == 0)
                return;

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

            dataList.Sort<InteractableData>();

            bool isInteracted = false;
            for (int j = 0; j < dataNum; j++)
            {
                var data = dataList[j];
                var entity = data.Entity;
                var interactable = data.Interactable;
                var m = data.Matrix;

                bool isIn = false;
                if (!isInteracted)
                {
                    var mvp = math.mul(vp, m);

                    ref var pointArray = ref interactable.PolyPointsAssetRef.Value.Array;
                    var pointNum = pointArray.Length;
                    var points = new NativeArray<float2>(pointNum, Allocator.Temp);
                    for (int i = 0; i < pointNum; i++)
                    {
                        points[i] = localToScreen(mvp, pointArray[i], pixelWidth2, pixelHeight2);
                    }

                    isIn = MathUtil.IsInPolygon(points, inputPos2);
                    points.Dispose();

                    isInteracted = isIn;
                }

                var isEnter = interactable.IsPointerEnter;
                if (isIn)
                {
                    //LogUtil.Log("UIInteractSystem: " + entity);
                    if (!isEnter)
                    {
                        interactable.IsPointerEnter = true;
                        EntityManager.AddComponent<PointerEnterEvent>(entity, false, true);

                        if (pressedButtonIndex >= 0)
                        {
                            interactable.DownButtonIndex = pressedButtonIndex;
                            interactable.IsPointerDown = true;
                        }
                    }
                    if (downButtonIndex >= 0)
                    {
                        var downEvent = new PointerDownEvent
                        {
                            buttonIndex = downButtonIndex
                        };
                        EntityManager.AddComponentData(entity, downEvent, false, true);
                        interactable.DownButtonIndex = downButtonIndex;
                        interactable.IsPointerDown = true;
                    }
                    if (upButtonIndex >= 0)
                    {
                        var upEvent = new PointerUpEvent
                        {
                            buttonIndex = upButtonIndex
                        };
                        EntityManager.AddComponentData(entity, upEvent, false, true);
                        if (interactable.DownButtonIndex == upButtonIndex)
                        {
                            var clickEvent = new PointerClickEvent
                            {
                                buttonIndex = upButtonIndex
                            };
                            EntityManager.AddComponentData(entity, clickEvent, false, true);
                        }
                        interactable.IsPointerDown = false;
                        interactable.DownButtonIndex = -1;
                    }
                }
                else
                {
                    if (isEnter)
                    {
                        interactable.DownButtonIndex = -1;
                        interactable.IsPointerEnter = false;
                        interactable.IsPointerDown = false;
                        EntityManager.AddComponent<PointerExitEvent>(entity, false, true);
                    }
                }

                EntityManager.SetComponentData(entity, interactable);
            }

            dataList.Clear();
            //Entities
            //    //.WithAll<PointerInteractable>()
            //    .ForEach((Entity entity, ref PointerInteractable interactable, ref LocalToWorld localToWorld) =>
            //{
            //    var m = localToWorld.Value;

            //    //var mvp = vp * m;

            //    //var p0 = uiElementComp.Point0;
            //    //var p1 = math.transform(m, p0);
            //    //var p2 = math.transform(v, p1);
            //    //var p3 = math.transform(p, p2);

            //    //var p4 = math.transform(vp, p1);

            //    //var p5 = math.transform(mvp, p0);

            //    var mvp = math.mul(vp, m);

            //    //if (!interactable.ShortArrayAssetRef.IsCreated)
            //    //    return;
            //    //LogUtil.LogFormat("IsCreated: {0}\r\n", interactable.ShortArrayAssetRef.IsCreated);
            //    //ref var shortArray = ref interactable.ShortArrayAssetRef.Value.Array;
            //    //LogUtil.LogFormat("short value: {0}, {1}\r\n", shortArray[1], interactable.ShortArrayAssetRef.Value.Count);

            //    //if (!interactable.PolyPointsAssetRef.IsCreated)
            //    //    return;

            //    ref var pointArray = ref interactable.PolyPointsAssetRef.Value.Array;
            //    var pointNum = pointArray.Length;
            //    var points = new NativeArray<float2>(pointNum, Allocator.Temp);
            //    for (int i = 0; i < pointNum; i++)
            //    {
            //        points[i] = localToScreen(mvp, pointArray[i], pixelWidth2, pixelHeight2);

            //        //LogUtil.LogFormat("PointerInteractSystem: {0}, {1}, {2}, {3}\r\n", pointArray[i], points[i], pixelWidth2, pixelHeight2);
            //    }

            //    //poly[0] = localToScreen(mvp, uiElementComp.Point0, pixelWidth2, pixelHeight2);
            //    //poly[1] = localToScreen(mvp, uiElementComp.Point1, pixelWidth2, pixelHeight2);
            //    //poly[2] = localToScreen(mvp, uiElementComp.Point2, pixelWidth2, pixelHeight2);
            //    //poly[3] = localToScreen(mvp, uiElementComp.Point3, pixelWidth2, pixelHeight2);

            //    bool isIn = MathUtil.IsInPolygon(points, inputPos2);

            //    //LogUtil.LogFormat("PointerInteractSystem: {0}, {1}\r\n", isIn, inputPos2);

            //    points.Dispose();
            //    //bool isIn = false;

            //    //var pos = localToWorld.Position;
            //    ////var pos3 = mul(vp, pos);
            //    //var pos3 = math.transform(vp, pos);

            //    //LogUtil.LogFormat("UIInteractSystem: {0},{1},{2},{3},{4}", inputPos2, poly[0], poly[1], poly[2], poly[3]);

            //    ////1.798722
            //    //var x = pos3.x * 0.5f + 1f; //0.9629341
            //    //var y = pos3.y * 0.5f + 1f; //1.732051
            //    //var sox = x * pixelWidth2;
            //    //var soy = y * pixelHeight2;

            //    ////var dis = -mul(v, pos).z;
            //    //var pp = math.transform(v, pos);
            //    //var dis = -pp.z;
            //    //var rate = dis * math.tan(math.radians(fov * 0.5f));
            //    //var sx = sox + rect.x / rate * pixelHeight2;
            //    //var sy = soy + rect.y / rate * pixelHeight2;
            //    //var sw = (rect.width / rate) * pixelHeight2;
            //    //var sh = (rect.height / rate) * pixelHeight2;

            //    //bool isIn = (inputPosX >= sx && inputPosX <= sx + sw
            //    //    && inputPosY >= sy && inputPosY <= sy + sh);

            //    //LogUtil.Log("UIInteractSystem: " + inputPos2 + ", " + isIn);

            //    var isEnter = interactable.IsPointerEnter;
            //    if (isIn)
            //    {
            //        //LogUtil.Log("UIInteractSystem: " + entity);
            //        if (!isEnter)
            //        {
            //            interactable.IsPointerEnter = true;
            //            EntityManager.AddComponent<PointerEnterEvent>(entity, false, true);

            //            if (pressedButtonIndex >= 0)
            //            {
            //                interactable.DownButtonIndex = pressedButtonIndex;
            //                interactable.IsPointerDown = true;
            //            }
            //        }
            //        if (downButtonIndex >= 0)
            //        {
            //            var downEvent = new PointerDownEvent
            //            {
            //                buttonIndex = downButtonIndex
            //            };
            //            EntityManager.AddComponentData(entity, downEvent, false, true);
            //            interactable.DownButtonIndex = downButtonIndex;
            //            interactable.IsPointerDown = true;
            //        }
            //        if (upButtonIndex >= 0)
            //        {
            //            var upEvent = new PointerUpEvent
            //            {
            //                buttonIndex = upButtonIndex
            //            };
            //            EntityManager.AddComponentData(entity, upEvent, false, true);
            //            if (interactable.DownButtonIndex == upButtonIndex)
            //            {
            //                var clickEvent = new PointerClickEvent
            //                {
            //                    buttonIndex = upButtonIndex
            //                };
            //                EntityManager.AddComponentData(entity, clickEvent, false, true);
            //            }
            //            interactable.IsPointerDown = false;
            //            interactable.DownButtonIndex = -1;
            //        }
            //    }
            //    else
            //    {
            //        if (isEnter)
            //        {
            //            interactable.DownButtonIndex = -1;
            //            interactable.IsPointerEnter = false;
            //            interactable.IsPointerDown = false;
            //            EntityManager.AddComponent<PointerExitEvent>(entity, false, true);
            //        }
            //    }
            //});
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
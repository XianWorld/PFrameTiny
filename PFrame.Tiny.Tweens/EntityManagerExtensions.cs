//using System;
//using System.Runtime.CompilerServices;
//using Unity.Entities;
//using Unity.Tiny.Assertions;

//namespace Unity.Tiny
//{
//    public static class TinyInternalEntityManagerExtensions1
//    {
//        public static unsafe byte* GetComponentDataWithTypeRW1(this EntityManager manager, Entity entity, int typeIndex)
//        {
//            var ptr = manager.GetComponentDataWithTypeRW(entity, typeIndex);
//            return (byte*)ptr;
//        }

//        public static unsafe byte* GetComponentDataWithTypeRO1(this EntityManager manager, Entity entity, int typeIndex)
//        {
//            var ptr = manager.GetComponentDataWithTypeRO(entity, typeIndex);
//            return (byte*)ptr;
//        }

//        public static bool HasComponentRaw1(this EntityManager manager, Entity entity, int typeIndex)
//        {
//            return manager.HasComponentRaw(entity, typeIndex);
//        }
//    }
//}

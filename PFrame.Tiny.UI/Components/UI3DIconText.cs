//using Unity.Collections;
//using Unity.Entities;

//namespace PFrame.Tiny.SimpleUI
//{
//    public enum EHAlignType
//    {
//        Left,
//        Middle,
//        Right
//    }
//    public enum EVAlignType
//    {
//        Bottom,
//        Middle,
//        Top
//    }

//    public struct UI3DIconText : IComponentData
//    {
//        public int FontId;
//        public NativeString128 Text;
//        //public bool IsNeedUpdate;
//        public int HAlignType;
//        public int VAlignType;

//        public float HSpacing;
//        public float VSpacing;
//    }

//    public struct UI3DIconChar : IComponentData
//    {
//        public char Char;
//    }

//    public struct UISetTextCmd : IComponentData
//    {
//        public NativeString128 Text;
//    }

//    public struct UI3DIconCharElement : IBufferElementData
//    {
//        public char Char;

//        public Entity CharEntity;
//    }
//}

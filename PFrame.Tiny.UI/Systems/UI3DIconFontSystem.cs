//using PFrame.Entities;
//using System;
//using Unity.Collections;
//using Unity.Entities;
//using Unity.Mathematics;
//using Unity.Transforms;

//namespace PFrame.Tiny.SimpleUI
//{
//    public struct UI3DIconFontData : IDisposable
//    {
//        public NativeArray<UI3DIconFontChar> iconElements;
//        public Entity fontEntity;
//        public int startCharCode;
//        public float charWidth;
//        public float charHeight;
//        public int iconElementNum;
//        public bool IsSupportLowerCase;

//        public UI3DIconFontData(EntityManager entityManager, Entity entity)
//        {
//            fontEntity = entity;
//            var fontComp = entityManager.GetComponentData<UI3DIconFont>(fontEntity);
//            startCharCode = (int)fontComp.StartChar;
//            charWidth = fontComp.CharWidth;
//            charHeight = fontComp.CharHeight;
//            var nBuffer = entityManager.GetBuffer<UI3DIconFontChar>(fontEntity);
//            iconElements = nBuffer.ToNativeArray(Allocator.Persistent);
//            iconElementNum = iconElements.Length;
//            IsSupportLowerCase = fontComp.IsSupportLowerCase;
//        }

//        public void Dispose()
//        {
//            if (fontEntity == Entity.Null)
//                return;
//            iconElements.Dispose();
//        }

//        public bool IsValid()
//        {
//            return fontEntity != Entity.Null;
//        }

//        public Entity CreateCharEntity(EntityManager entityManager, Entity textEntity, char textChar)
//        {
//            var charEntity = Entity.Null;

//            var charCode = (int)textChar;
//            var index = charCode - startCharCode;
//            if (index < 0 || index >= iconElementNum)
//                return charEntity;

//            var iconElement = iconElements[index];
//            var prefab = iconElement.CharPrefab;
//            if (prefab == Entity.Null)
//                return charEntity;

//            charEntity = CreateCharEntity(entityManager, prefab);

//            entityManager.AddComponentData(charEntity, new UI3DIconChar { Char = textChar });

//            var parent = new Parent { Value = textEntity };
//            if (!entityManager.HasComponent<Parent>(charEntity))
//            {
//                entityManager.AddComponentData<Parent>(charEntity, parent);
//                entityManager.AddComponent<LocalToParent>(charEntity);
//            }
//            else
//            {
//                entityManager.SetComponentData<Parent>(charEntity, parent);
//            }

//            return charEntity;
//        }

//        public Entity CreateCharEntity(EntityManager entityManager, Entity prefab)
//        {
//            return entityManager.Instantiate(prefab);
//        }

//        public void DestroyCharEntity(EntityManager entityManager, Entity charEntity, char textChar)
//        {
//            entityManager.DestroyEntity(charEntity);
//        }

//        public bool UpdateText(EntityManager entityManager, Entity textEntity)
//        {
//            if (fontEntity == Entity.Null)
//                return false;

//            var textComp = entityManager.GetComponentData<UI3DIconText>(textEntity);
//            var text = textComp.Text;
//            var hAlignType = textComp.HAlignType;
//            var vAlignType = textComp.VAlignType;
//            var hSpacing = textComp.HSpacing;
//            var vSpacing = textComp.VSpacing;

//            var buffer = entityManager.GetBuffer<UI3DIconCharElement>(textEntity);

//            //remove all char elements
//            foreach (var element in buffer)
//            {
//                if (element.CharEntity != Entity.Null)
//                {
//                    DestroyCharEntity(entityManager, element.CharEntity, element.Char);
//                }
//            }

//            //add new char elements
//            //var textStr = text.ToString();
//            //if (!IsSupportLowerCase)
//            //    textStr = textStr.ToUpper();
//            //var chars = textStr.ToCharArray();
//            //var len = chars.Length;
//            var len = text.LengthInBytes;

//            if (len > 0)
//            {
//                //calculate x, y 
//                var minX = 0f;
//                var minY = 0f;
//                var textWidth = len * charWidth;
//                var textHeight = charHeight;
//                if (hAlignType == (int)EHAlignType.Middle)
//                {
//                    minX = -(textWidth + hSpacing * (len - 1)) * 0.5f;
//                }
//                else if (hAlignType == (int)EHAlignType.Right)
//                {
//                    minX = -(textWidth + hSpacing * (len - 1));
//                }

//                if (vAlignType == (int)EVAlignType.Middle)
//                {
//                    minY = -(textHeight + vSpacing * (len - 1)) * 0.5f;
//                }
//                else if (vAlignType == (int)EVAlignType.Top)
//                {
//                    minY = -(textHeight + vSpacing * (len - 1));
//                }

//                unsafe
//                {
//                    byte* b = &text.buffer.byte0000;

//                    var buf = new NativeArray<UI3DIconCharElement>(len, Allocator.Temp);
//                    for (int i = 0; i < len; i++)
//                    {
//                        var cc = (char)b[i];
//                        if (!IsSupportLowerCase)
//                        {
//                            if (cc >= 97 && cc <= 122)
//                                cc = (char)(cc - 32);
//                        }

//                        var charEntity = CreateCharEntity(entityManager, textEntity, cc);
//                        buf[i] = (new UI3DIconCharElement { Char = cc, CharEntity = charEntity });

//                        if (charEntity == Entity.Null)
//                            continue;

//                        var pos = entityManager.GetComponentData<Translation>(charEntity);
//                        pos.Value = new float3(minX + i * charWidth + hSpacing * i, minY, 0f);
//                        entityManager.SetComponentData(charEntity, pos);
//                    }

//                    buffer = entityManager.GetBuffer<UI3DIconCharElement>(textEntity);
//                    buffer.Clear();
//                    buffer.AddRange(buf);

//                    buf.Dispose();
//                }

//                //var buf = new NativeArray<UI3DIconCharElement>(len, Allocator.Temp);
//                //for (int i = 0; i < len; i++)
//                //{
//                //    var charEntity = CreateCharEntity(entityManager, textEntity, chars[i]);
//                //    buf[i] = (new UI3DIconCharElement { Char = chars[i], CharEntity = charEntity });

//                //    if (charEntity == Entity.Null)
//                //        continue;

//                //    var pos = entityManager.GetComponentData<Translation>(charEntity);
//                //    pos.Value = new float3(minX + i * charWidth + hSpacing * i, minY, 0f);
//                //    entityManager.SetComponentData(charEntity, pos);
//                //}

//                //buffer = entityManager.GetBuffer<UI3DIconCharElement>(textEntity);
//                //buffer.Clear();
//                //buffer.AddRange(buf);

//                //buf.Dispose();
//            }

//            return true;
//        }
//    }

//    //[UpdateAfter(typeof(InitializationSystemGroup))]
//    public class UI3DIconFontSystem : ComponentSystem
//    {
//        //private NativeArray<UI3DIconFontChar> iconElements;
//        //private Entity iconManagerEntity;
//        //private int startCharCode;
//        //private float charWidth = 1f;
//        //private float charHeight = 1f;
//        //private int iconElementNum;
//        //private NativeArray<UI3DIconFont> fonts;
//        private UI3DIconFontData[] fonts;
//        private static int MAX_FONT_NUM = 4;

//        public UI3DIconFontData GetFont(int fontId)
//        {
//            if (fontId >= MAX_FONT_NUM)
//                fontId = 0;

//            var font = fonts[fontId];
//            if (font.fontEntity == Entity.Null)
//                font = fonts[0];

//            return font;
//        }

//        protected override void OnCreate()
//        {
//            base.OnCreate();
//            //RequireSingletonForUpdate<UI3DIconFontComponent>();
//            fonts = new UI3DIconFontData[MAX_FONT_NUM];
//        }

//        protected override void OnDestroy()
//        {
//            base.OnDestroy();

//            for (int i = 0; i < MAX_FONT_NUM; i++)
//            {
//                fonts[i].Dispose();
//            }
//            //fonts.dispose();
//        }

//        //protected override void OnStartRunning()
//        //{
//        //    base.OnStartRunning();
//        //    //fonts = new NativeArray<UI3DIconFont>(MAX_FONT_NUM, Allocator.Persistent);
//        //    fonts = new UI3DIconFont[MAX_FONT_NUM];

//        //    Entities.ForEach((Entity entity, ref UI3DIconFontComponent fontComp) =>
//        //    {
//        //        int id = fontComp.Id;
//        //        if (id >= MAX_FONT_NUM)
//        //            return;
//        //        var font = new UI3DIconFont(EntityManager, entity);
//        //        fonts[id] = font;
//        //    });

//        //    //var iconManagerEntity = GetSingletonEntity<UI3DIconFontComponent>();
//        //    //font = new UI3DIconFont(EntityManager, iconManagerEntity);

//        //    //var iconManagerComp = EntityManager.GetComponentData<UI3DIconFontComponent>(iconManagerEntity);
//        //    //startCharCode = (int)iconManagerComp.StartChar;
//        //    //charWidth = iconManagerComp.CharWidth;
//        //    //charHeight = iconManagerComp.CharHeight;
//        //    //var nBuffer = EntityManager.GetBuffer<UI3DIconFontChar>(iconManagerEntity);
//        //    //iconElements = nBuffer.ToNativeArray(Allocator.Persistent);
//        //    //iconElementNum = iconElements.Length;
//        //}

//        //protected override void OnStopRunning()
//        //{
//        //    //iconElements.Dispose();
//        //    for(int i =0;i< MAX_FONT_NUM;i++)
//        //    {
//        //        fonts[i].Dispose();
//        //    }
//        //    //fonts.Dispose();
//        //}

//        protected override void OnUpdate()
//        {
//            //UnityEngine.Debug.Log("UI3DIconFontSystem.OnUpdate!");

//            Entities
//                .WithNone<InitedState>()
//                .ForEach((Entity entity, ref UI3DIconFont fontComp) =>
//            {
//                EntityManager.AddComponent<InitedState>(entity);

//                int id = fontComp.Id;
//                if (id >= MAX_FONT_NUM)
//                    return;

//                var font = new UI3DIconFontData(EntityManager, entity);
//                fonts[id] = font;

//                //UnityEngine.Debug.Log("UI3DIconFontSystem.OnUpdate: " + font);
//            });
//        }
//    }
//}
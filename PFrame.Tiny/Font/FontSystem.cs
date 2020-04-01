using PFrame.Entities;
using Unity.Collections;
using Unity.Entities;

namespace PFrame.Tiny
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [UpdateAfter(typeof(GameDataManagerSystem))]
    public class FontSystem : ComponentSystem
    {
        GameDataManagerSystem gameDataManagerSystem;
        //NativeHashMap<ushort, Font> fontMap;
        Font[] fonts;

        protected override void OnCreate()
        {
            base.OnCreate();
            RequireSingletonForUpdate<GameDataManager>();
            //fontMap = new NativeHashMap<ushort, Font>(1, Allocator.Persistent);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            //using (var array = fontMap.GetValueArray(Allocator.Temp))
            //{
            //    foreach (var font in array)
            //    {
            //        font.Dispose();
            //    }
            //}
            //fontMap.Dispose();
        }
        protected override void OnStartRunning()
        {
            base.OnStartRunning();
            gameDataManagerSystem = World.GetExistingSystem<GameDataManagerSystem>();
            if(gameDataManagerSystem.TryGetGameDataArray<FontData>(out var fontArray))
            {
                fonts = new Font[fontArray.Length];
                for (int i =0; i< fontArray.Length;i++)
                {
                    var fontData = fontArray[i];
                    var font = new Font();
                    font.Initialize(fontData);

                    fonts[i] = font;
                    //fontMap.Add(font.DataId, font);
                }
                fontArray.Dispose();
            }
        }

        protected override void OnStopRunning()
        {
            base.OnStopRunning();
            if(fonts != null)
            {
                foreach (var font in fonts)
                {
                    font.Dispose();
                }
                fonts = null;
            }
        }

        public bool IsInited()
        {
            return fonts != null;
        }

        public bool TryGetFont(ushort id, out Font font)
        {
            //return fontMap.TryGetValue(id, out font);

            font = default;
            if (fonts == null)
                return false;

            for (int i = 0; i < fonts.Length; i++)
            {
                if(fonts[i].DataId == id)
                {
                    font = fonts[i];
                    return true;
                }
            }
            return false;
        }

        protected override void OnUpdate()
        {
            //Entities.ForEach((Entity entity, ref Service serviceComp, ref UnloadStageCmd unloadStageCmdComp) =>
            //{
            //    var stageId = serviceComp.StageId;
            //    var stageEntity = serviceComp.StageEntity;

            //    if (EntityManager.HasComponent<LoadState>(entity))
            //        return;
            //    if (EntityManager.HasComponent<UnloadState>(entity))
            //        return;

            //    if (stageEntity != Entity.Null)
            //    {
            //        EntityManager.AddComponent<UnloadCmd>(stageEntity);
            //        EntityUtil.SetState<UnloadState, EnterUnloadStateEvent, LoadedState, ExitLoadedStateEvent>(EntityManager, entity);
            //        //CommonUtil.SetState<UnloadState, UnloadEvent, LoadedState>(World, entity);
            //    }

            //    EntityManager.RemoveComponent<UnloadStageCmd>(entity);
            //});
        }
    }
}

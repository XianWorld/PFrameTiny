using PFrame.Entities;
using PFrame.Entities.Authoring;
using PFrame.Tiny.Hybrid;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Runtime.Build;
using UnityEngine;

namespace PFrame.Tiny.Authoring
{
    [GameDataAuthoring(Name = "FontData")]
    public class FontDataSOListConverter : AGameDataSOListConverter<FontData, FontDataSO>
    {
        public override byte TypeId => (byte)ECommonGameDataType.Font;

        public override FontData ConvertGameData(GameObject gameObject, Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem, FontDataSO dataSO)
        {
            bool isRuntime = conversionSystem.TryGetBuildConfigurationComponent<DotsRuntimeBuildProfile>(out var _);

            var data = new FontData();
            data.Id = dataSO.Id;
            data.Name = dataSO.Name;

            var font = dataSO.Font;

            data.Ascent = font.ascent;
            data.Dynamic = font.dynamic;

            if (isRuntime)
            {
                data.MaterialEntity = conversionSystem.GetPrimaryEntity(font.material);
                Debug.LogFormat("[R]FontDataSOListConverter.ConvertGameData: {0}", font.material);
            }
            else
            {
                var materialEntity = conversionSystem.CreateAdditionalEntity(gameObject);
                dstManager.AddSharedComponentData(materialEntity, new FontMaterial { Material = font.material });
                data.MaterialEntity = materialEntity;
                Debug.LogFormat("FontDataSOListConverter.ConvertGameData: {0}", font.material);
            }

            data.LineHeight = font.lineHeight;
            data.FontSize = font.fontSize;

            data.CharacterInfoArrayAssetRef = CreateArrayAssetRef(font.characterInfo);

            return data;
        }

        public override void DeclareReferencedPrefab(List<GameObject> referencedPrefabs, FontDataSO dataSO)
        {
        }

        protected override void DeclareReferencedObject(GameObjectConversionSystem conversionSystem, FontDataSO dataSO)
        {
            bool isRuntime = conversionSystem.TryGetBuildConfigurationComponent<DotsRuntimeBuildProfile>(out var _);
            if (isRuntime)
            {
                var font = dataSO.Font;
                conversionSystem.DeclareReferencedAsset(font.material);
                Debug.LogFormat("[R]FontDataSOListConverter.DeclareReferencedObject: {0}", font.material);
            }
        }

        private BlobAssetReference<CharacterInfoArrayAsset> CreateArrayAssetRef(UnityEngine.CharacterInfo[] datas)
        {
            var builder = new BlobBuilder(Allocator.Temp);
            ref var root = ref builder.ConstructRoot<CharacterInfoArrayAsset>();
            var len = datas.Length;
            var array = builder.Allocate(ref root.CharacterInfoArray, len);
            root.Count = (ushort)len;
            for (int i = 0; i < len; i++)
            {
                var ucharacterInfo = datas[i];
                var characterInfo = TinyAuthoringUtil.Convert(ucharacterInfo);
                //var characterInfo = new PFrame.Tiny.CharacterInfo();
                //characterInfo.index = ucharacterInfo.index;
                //characterInfo.uv = (ucharacterInfo.uv);
                //characterInfo.vert = (ucharacterInfo.vert);
                //characterInfo.width = ucharacterInfo.advance;
                //characterInfo.size = ucharacterInfo.size;
                //characterInfo.style = (FontStyle)((int)ucharacterInfo.style);
                //characterInfo.flipped = ucharacterInfo.flipped;

                array[i] = characterInfo;
            }
            var assetRef = builder.CreateBlobAssetReference<CharacterInfoArrayAsset>(Allocator.Persistent);
            builder.Dispose();
            return assetRef;
        }

    }

    public class FontDataSOListAuthoring : AGameDataSOListAuthoring<FontData, FontDataSO, FontDataSOListConverter>
    {
    }
}

using PFrame.Tiny.Hybrid;
using Unity.Collections;
using Unity.Entities;
using Unity.Rendering;
using UnityEngine;

namespace PFrame.Tiny.Authoring
{
    [UpdateInGroup(typeof(GameObjectConversionGroup))]
    public class TextMeshConversionSystem : GameObjectConversionSystem
    {
        public override bool ShouldRunConversionSystem()
        {
            ////Workaround for running the tiny conversion systems only if the BuildSettings have the DotsRuntimeBuildProfile component, so these systems won't run in play mode
            //if (!TryGetBuildConfigurationComponent<DotsRuntimeBuildProfile>(out _))
            //    return false;
            return base.ShouldRunConversionSystem();
        }

        protected override void OnUpdate()
        {
            Entities.ForEach((UnityEngine.TextMesh utextMesh, MeshRenderer meshRenderer) =>
            {
                var entity = GetPrimaryEntity(utextMesh);

                Debug.LogFormat("TextMeshConversionSystem: {0}, {1}", utextMesh.name, entity);

                var textMesh = new TextMesh();
                textMesh.Text = utextMesh.text;

                //utextMesh.font
                //if (FontData != null)
                //    textMesh.FontId = FontData.Id;
                textMesh.FontId = 1;
                textMesh.CharSize = utextMesh.characterSize;
                textMesh.Color = TinyAuthoringUtil.Convert(utextMesh.color);

                DstEntityManager.AddComponentData(entity, textMesh);

                //var renderMesh = new RenderMesh();
                //renderMesh.material = 
                //DstEntityManager.AddComponentData(entity, textMesh);

            });

        }
    }
}

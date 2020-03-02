using PFrame.Entities;
using Unity.Collections;
using Unity.Entities;

namespace PFrame.Tiny.SimpleUI
{
    public class SimpleUIUtil
    {
        public static void SetText(EntityManager entityManager, Entity entity, string text)
        {
            var uiText = entityManager.GetComponentData<UI3DIconText>(entity);
            uiText.Text = text.ToString();
            entityManager.SetComponentData<UI3DIconText>(entity, uiText);

            if (entityManager.HasComponent<UpdatedState>(entity))
                entityManager.RemoveComponent<UpdatedState>(entity);
        }
    }
}
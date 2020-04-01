using PFrame.Entities;
using PFrame.Entities.Authoring;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Tiny;
using UnityEngine;

namespace PFrame.Tiny.Authoring
{
	public class PointerInteractableAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        public short Layer = 0;
        public float3[] Points = new float3[3];

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            var interactable = new PointerInteractable();

            interactable.Layer = Layer;

            if (Points != null && Points.Length > 0)
            {
                interactable.PolyPointsAssetRef = EntityUtil.CreateArrayAssetRef(Points);
            }

            //var values = new short[]
            //{
            //    11,
            //    15
            //};
            //interactable.ShortArrayAssetRef = EntityUtil.CreateArrayAssetRef(values);

            dstManager.AddComponentData(entity, interactable);
        }

        private void OnDrawGizmos()
        {
            if (Points == null || Points.Length < 3)
                return;

            Gizmos.color = UnityEngine.Color.red;

            var len = Points.Length;
            Vector3 p0, p1;
            for (int i = 1; i < len; i++)
            {
                p0 = transform.TransformPoint(Points[i-1]);
                p1 = transform.TransformPoint(Points[i]);
                Gizmos.DrawLine(p0, p1);
            }

            p0 = transform.TransformPoint(Points[len - 1]);
            p1 = transform.TransformPoint(Points[0]);
            Gizmos.DrawLine(p0, p1);

            //var p0 = transform.TransformPoint(Point0);
            //var p1 = transform.TransformPoint(Point1);
            //var p2 = transform.TransformPoint(Point2);
            //var p3 = transform.TransformPoint(Point3);
            //Gizmos.DrawLine(p0, p1);
            //Gizmos.DrawLine(p1, p2);
            //Gizmos.DrawLine(p2, p3);
            //Gizmos.DrawLine(p3, p0);
        }
    }
}

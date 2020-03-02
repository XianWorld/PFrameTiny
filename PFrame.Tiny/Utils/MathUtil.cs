//using Unity.Collections;
//using Unity.Entities;
//using Unity.Mathematics;

//namespace PFrame.Tiny
//{
//    public class MathUtil
//    {
//        public static bool IsInPolygon(float2[] poly, float2 p)
//        {
//            float2 p1, p2;
//            bool inside = false;

//            if (poly.Length < 3)
//            {
//                return inside;
//            }

//            var oldPoint = new float2(
//                poly[poly.Length - 1].x, poly[poly.Length - 1].y);

//            for (int i = 0; i < poly.Length; i++)
//            {
//                var newPoint = new float2(poly[i].x, poly[i].y);

//                if (newPoint.x > oldPoint.x)
//                {
//                    p1 = oldPoint;
//                    p2 = newPoint;
//                }
//                else
//                {
//                    p1 = newPoint;
//                    p2 = oldPoint;
//                }

//                if ((newPoint.x < p.x) == (p.x <= oldPoint.x)
//                    && (p.y - (long)p1.y) * (p2.x - p1.x)
//                    < (p2.y - (long)p1.y) * (p.x - p1.x))
//                {
//                    inside = !inside;
//                }

//                oldPoint = newPoint;
//            }

//            return inside;
//        }
//    }
//}
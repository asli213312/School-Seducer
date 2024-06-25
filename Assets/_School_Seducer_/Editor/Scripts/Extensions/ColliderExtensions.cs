using UnityEngine;

namespace _School_Seducer_.Editor.Scripts.Extensions
{
    public static class ColliderExtensions
    {
        public static bool IsIntersects2D(this Transform checker, Transform target)
        {
            Collider2D colChecker = checker.GetComponent<Collider2D>();
            Collider2D colTarget = target.GetComponent<Collider2D>();

            if (colChecker && colTarget == null) return false;

            return colChecker.bounds.Intersects(colTarget.bounds);
        }
    }
}
using UnityEngine;

namespace Bipolar.CablePhysics
{
    public class Cable2D : Cable<Vector2, RaycastHit2D, Collider2D, Ray2D>
    {
        public override Vector3 GetPosition(int pointIndex) => points[pointIndex];

        protected override Vector2 Add(Vector2 lhs, Vector2 rhs) => lhs + rhs;

        protected override float Distance(Vector2 lhs, Vector2 rhs) => Vector2.Distance(lhs, rhs);

        protected override void DrawGizmosLine(Vector2 start, Vector2 end) => Gizmos.DrawLine(start, end);

        protected override Collider2D GetCollider(RaycastHit2D raycastHit) => raycastHit.collider;

        protected override Vector2 GetNormal(RaycastHit2D raycastHit) => raycastHit.normal;

        protected override Vector2 GetPoint(RaycastHit2D raycastHit) => raycastHit.point;

        protected override Vector2 GetPoint(Ray2D ray, float distance) => ray.GetPoint(distance);

        protected override Vector2 GetPosition(Transform transform) => transform.position;

        protected override Ray2D GetRay(Vector2 origin, Vector2 direction) => new Ray2D(origin, direction);

        protected override bool Linecast(Vector2 start, Vector2 end, out RaycastHit2D raycastHit, LayerMask layerMask)
        {
            raycastHit = Physics2D.Linecast(end, start, layerMask);
            return raycastHit.collider != null;
        }

        protected override Vector2 Multiply(Vector2 point, float number) => point * number;

        protected override void Normalize(ref Vector2 point) => point.Normalize();

        protected override float SqrMagnitude(Vector2 point) => point.sqrMagnitude;

        protected override Vector2 Subtract(Vector2 lhs, Vector2 rhs) => lhs - rhs;
    }
}

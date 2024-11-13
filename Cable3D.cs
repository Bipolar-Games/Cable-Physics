using UnityEngine;

namespace Bipolar.CablePhysics
{
    public class Cable3D : Cable<Vector3, RaycastHit, Collider, Ray>
    {
        protected override Vector3 Add(Vector3 lhs, Vector3 rhs) => lhs + rhs;

        protected override float Distance(Vector3 lhs, Vector3 rhs) => (lhs - rhs).magnitude;

        protected override Collider GetCollider(RaycastHit raycastHit) => raycastHit.collider;

        protected override Vector3 GetNormal(RaycastHit raycastHit) => raycastHit.normal;

        protected override Vector3 GetPoint(RaycastHit raycastHit) => raycastHit.point;

        protected override Vector3 GetPosition(Transform transform) => transform.position;

        protected override bool Linecast(Vector3 end, Vector3 start, out RaycastHit raycastHit, LayerMask layerMask) => 
            Physics.Linecast(start, end, out raycastHit, layerMask);

        protected override float SqrMagnitude(Vector3 point) => point.sqrMagnitude;

        protected override Vector3 Multiply(Vector3 point, float number) => point * number;

        protected override Vector3 Subtract(Vector3 lhs, Vector3 rhs) => lhs - rhs;

        protected override void DrawGizmosLine(Vector3 start, Vector3 end) => Gizmos.DrawLine(start, end);

        protected override Ray GetRay(Vector3 origin, Vector3 direction) => new Ray(origin, direction);

        protected override Vector3 GetPoint(Ray ray, float distance) => ray.GetPoint(distance);

        protected override void Normalize(ref Vector3 point) => point.Normalize();

        public override Vector3 GetPosition(int pointIndex) => points[pointIndex];
    }
}

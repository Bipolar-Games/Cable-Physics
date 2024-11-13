using UnityEngine;

namespace Bipolar.CablePhysics
{
    [RequireComponent(typeof(Cable3D))]
    public class CableLengthConstraint3D : CableLengthConstraint<Cable3D, Rigidbody>
    {
        protected override void PullToCable(Rigidbody body, float forceValue, bool isEnding)
        {
            if (body)
            {
                int tipPointIndex = isEnding ? cable.Points.Count - 1 : 0;
                int neighbourIndex = tipPointIndex + (isEnding ? -1 : 1);

                var tipPoint = cable.Points[tipPointIndex];
                var neighbourPoint = cable.Points[neighbourIndex];
                var cableDirection = neighbourPoint - tipPoint;
                cableDirection.Normalize();

                var velocityAlongCable = Vector3.Project(body.velocity, cableDirection);
                Debug.DrawRay(body.position, velocityAlongCable);

                body.AddForceAtPosition(cableDirection * forceValue, tipPoint);
                body.AddForceAtPosition(-velocityAlongCable * Damping, tipPoint);
            }
        }
    }


}

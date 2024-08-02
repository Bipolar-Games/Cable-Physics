using UnityEngine;

namespace Bipolar.CablePhysics
{
    [RequireComponent(typeof(Cable))]
    public class CableLengthConstraint : MonoBehaviour
    {
        private Cable cable;

        [field: SerializeField]
        public Rigidbody OriginRigidbody { get; set; }

        [field: SerializeField]
        public Rigidbody EndingRigidbody { get; set; }

        [field: SerializeField]  
        public float MaxLength { get; set; }

        [field: SerializeField]  
        public float BaseForce { get; set; }

        [field: SerializeField]  
        public float ForceModifier { get; set; }

        [field: SerializeField]  
        public float Damping { get; set; }

        private void Awake()
        {
            cable = GetComponent<Cable>();
        }

        private void OnEnable()
        {
            //thread.Origin.TryGetComponent(out originRigidbody);
            //thread.Ending.TryGetComponent(out endingRigidbody);
        }

        private void FixedUpdate()
        {
            ConstrainLength();
        }

        private void ConstrainLength()
        {
            float overlength = cable.Length - MaxLength;
            if (overlength < 0)
                return;

            float forceValue = BaseForce + overlength * ForceModifier;
            PullToCable(OriginRigidbody, forceValue, isEnding: false);
            PullToCable(EndingRigidbody, forceValue, isEnding: true);
        }

        private void PullToCable(Rigidbody body, float forceValue, bool isEnding)
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

using UnityEngine;

namespace Bipolar.CablePhysics
{
    public abstract class CableLengthConstraint : MonoBehaviour
    {
        public abstract Cable Cable { get; }

        [field: SerializeField]
        public float MaxLength { get; set; }

        [field: SerializeField]
        public float BaseForce { get; set; }

        [field: SerializeField]
        public float ForceModifier { get; set; }

        [field: SerializeField]
        public float Damping { get; set; }

        private void FixedUpdate()
        {
            ConstrainLength();
        }

        protected abstract void ConstrainLength();
    }

    public abstract class CableLengthConstraint<TCable, TRigidbody> : CableLengthConstraint
        where TCable : Cable
        where TRigidbody : Component
    {
        protected TCable cable;
        public override Cable Cable => cable;

        [field: SerializeField]
        public TRigidbody OriginRigidbody { get; set; }

        [field: SerializeField]
        public TRigidbody EndingRigidbody { get; set; }

        protected abstract void PullToCable(TRigidbody body, float forceValue, bool isEnding);

        private void Awake()
        {
            cable = GetComponent<TCable>();
        }

        private void OnEnable()
        {
            //thread.Origin.TryGetComponent(out originRigidbody);
            //thread.Ending.TryGetComponent(out endingRigidbody);
        }

        protected sealed override void ConstrainLength()
        {
            float overlength = cable.Length - MaxLength;
            if (overlength < 0)
                return;

            float forceValue = BaseForce + overlength * ForceModifier;
            PullToCable(OriginRigidbody, forceValue, isEnding: false);
            PullToCable(EndingRigidbody, forceValue, isEnding: true);
        }
    }


}

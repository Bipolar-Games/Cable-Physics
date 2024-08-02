using UnityEngine;

namespace Bipolar.CablePhysics
{
    public class CableVisualThicknessToLenghtRelation : MonoBehaviour
    {
        [SerializeField]
        private Cable cable;
        [SerializeField]
        private LineRenderer cableLineRenderer;
        [SerializeField]
        private CableLengthConstraint lengthConstraint;

        [SerializeField]
        private AnimationCurve thicknessCurve;
        [SerializeField]
        private float thicknessMultiplier;

        private void Update()
        {
            float relativeLength = cable.Length / lengthConstraint.MaxLength;
            float thickness = thicknessMultiplier * thicknessCurve.Evaluate(relativeLength);
            cableLineRenderer.widthMultiplier = thickness;
        }
    }
}

using UnityEngine;

namespace Bipolar.CablePhysics
{
    [RequireComponent(typeof(LineRenderer))]
    public class CableLineRenderer : MonoBehaviour
    {
        private LineRenderer lineRenderer;

        [SerializeField]
        private Cable cable;

        private void Awake()
        {
            lineRenderer = GetComponent<LineRenderer>();
        }

        private void Update()
        {
            if (cable == null)
				return;

            int count = cable.PointsCount;
            lineRenderer.positionCount = count;
            for (int i = 0; i < count; i++)
                lineRenderer.SetPosition(i, cable.GetPosition(i));
        }
    }
}

using UnityEngine;
using UnityEngine.Serialization;

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
            var points = cable.Points;
            lineRenderer.positionCount = points.Count;
            for (int i = 0; i < points.Count; i++)
                lineRenderer.SetPosition(i, points[i]);
        }
    }
}

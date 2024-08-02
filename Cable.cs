using System.Collections.Generic;
using UnityEngine;

namespace Bipolar.CablePhysics
{
    public class Cable : MonoBehaviour
    {
        private readonly List<Vector3> points = new List<Vector3>();
        public IReadOnlyList<Vector3> Points => points;

        [SerializeField]
        private Transform origin;
        public Transform Origin
        {
            get => origin;
            set
            {
                origin = value;
            }
        }

        [SerializeField]
        private Transform ending;
        public Transform Ending
        {
            get => ending;
            set
            {
                ending = value;
            }
        }

        [SerializeField, Min(0.01f)]
        private float thickness = 0.1f;
        public float Thickness
        {
            get => thickness;
            set => thickness = value;
        }

        [SerializeField]
        private LayerMask detectedLayers = -5;

#if NAUGHTY_ATTRIBUTES
        [SerializeField, NaughtyAttributes.ReadOnly]
#endif
        private float length;
        private bool isLengthCalculated;
        public float Length
        {
            get
            {
                if (isLengthCalculated == false)
                    CalculateLength();
                return length;
            }
        }

        private const float preferredChecksDistance = 0.1f;

        private void OnEnable()
        {
            PopulateList();
        }

        private void PopulateList()
        {
            if (origin == null || ending == null)
            {
                enabled = false;
                return;
            }

            points.Clear();
            points.Add(origin.position);
            points.Add(ending.position);
        }


        private void FixedUpdate()
        {
            DetectCollisionEnter(points.Count - 1, -1, ending);
            DetectCollisionEnter(0, +1, origin);

            DetectCollisionExit(points.Count - 1, -1);
            DetectCollisionExit(0, +1);
            isLengthCalculated = false;

#if UNITY_EDITOR
            CalculateLength();
#endif
        }

        private void CalculateLength()
        {
            float distanceSum = 0;
            for (int i = 1; i < points.Count; i++)
            {
                Vector3 start = points[i - 1];
                Vector3 end = points[i];
                distanceSum += (start - end).magnitude;
            }
            length = distanceSum;
            isLengthCalculated = true;
        }

        private void DetectCollisionEnter(int tipIndex, int direction, Transform tipTransform)
        {
            Vector3 previousPosition = points[tipIndex];
            points[tipIndex] = tipTransform.position;

            Vector3 previousTipPosition = previousPosition;
            Vector3 currentTipPosition = points[tipIndex];

            if (currentTipPosition == previousTipPosition)
                return;

            Vector3 reversedPositionDelta = previousTipPosition - currentTipPosition;
            var reversedPositionDeltaRay = new Ray(currentTipPosition, reversedPositionDelta);
            float movedDistance = reversedPositionDelta.magnitude;
            int checksResolution = 1 + Mathf.Max(1, Mathf.CeilToInt(movedDistance / preferredChecksDistance));
            float checkBaseDistance = movedDistance / checksResolution;

            int neighbourPointIndex = tipIndex + direction;
            Vector3 neighbourPosition = points[neighbourPointIndex];
            for (int j = checksResolution - 1; j >= 0; j--)
            {
                Vector3 checkedPoint = reversedPositionDeltaRay.GetPoint(j * checkBaseDistance);
                if (DoubleLinecast(checkedPoint, neighbourPosition, out var hit1, out var hit2, detectedLayers))
                {
                    bool hit1Valid = hit1.collider;
                    bool hit2Valid = hit2.collider;

                    Vector3 hitPoint1 = hit1Valid ? hit1.point : hit2.point;
                    Vector3 hitPoint2 = hit2Valid ? hit2.point : hit1.point;

                    Vector3 hitNormal1 = hit1Valid ? hit1.normal : hit2.normal;
                    Vector3 hitNormal2 = hit2Valid ? hit2.normal : hit1.normal;

                    Vector3 hitCenter = (hitPoint1 + hitPoint2) / 2f;
                    Vector3 hitNormal = (hitNormal1 + hitNormal2) / 2f;
                    if (hitNormal.sqrMagnitude < 0.001f)
                        hitNormal = reversedPositionDelta;
                    hitNormal.Normalize();

                    bool wasSafeHit = false;
                    for (int i = 1; i < 10; i++)
                    {
                        Vector3 safePointDetectionLineStart = hitCenter + i * thickness * hitNormal;
                        if (Physics.Linecast(safePointDetectionLineStart, hitCenter, out var safePointDetectionHit, detectedLayers) == false)
                            continue;

                        Vector3 safePoint = safePointDetectionHit.point + safePointDetectionHit.normal * thickness;
                        points.Insert(Mathf.Max(neighbourPointIndex, tipIndex), safePoint);
                        wasSafeHit = true;
                        break;
                    }

                    if (wasSafeHit == false)
                        Debug.LogError("!!!");

                    break;
                }
            }
        }

        private void DetectCollisionExit(int tipIndex, int direction)
        {
            if (points.Count > 2)
            {
                Vector3 secondNeighbour = points[tipIndex + 2 * direction];
                Vector3 tipPoint = points[tipIndex];

                if (DoubleLinecast(tipPoint, secondNeighbour, out _, out _, detectedLayers) == false)
                {
                    float distance = (tipPoint - secondNeighbour).magnitude;
                    var hypotenuseRay = new Ray(tipPoint, secondNeighbour - tipPoint);
                    int triangleChecksResolution = 1 + Mathf.CeilToInt(distance * 5);
                    float rayStartBaseDistance = distance / triangleChecksResolution;
                    int neighbourIndex = tipIndex + direction;
                    Vector3 neighbourPoint = points[neighbourIndex];
                    for (int i = 1; i <= triangleChecksResolution; i++)
                    {
                        Debug.DrawLine(tipPoint, secondNeighbour, new Color(0.1f, 0.1f, 0.3f, 0.3f), 1f);
                        Vector3 lineEnd = hypotenuseRay.GetPoint(rayStartBaseDistance * i);
                        if (Physics.Linecast(neighbourPoint, lineEnd, detectedLayers))
                            return;
                    }

                    points.RemoveAt(neighbourIndex);
                }
            }
        }

        public static bool DoubleLinecast(Vector3 point1, Vector3 point2, out RaycastHit fromPoint1Info, out RaycastHit fromPoint2Info, LayerMask layerMask)
        {
            bool fromPoint1 = Physics.Linecast(point1, point2, out fromPoint1Info, layerMask);
            bool fromPoint2 = Physics.Linecast(point2, point1, out fromPoint2Info, layerMask);
            bool wasHit = fromPoint1 || fromPoint2;
            return wasHit;
        }

        private void OnDrawGizmos()
        {
            if (points == null || points.Count <= 0)
                return;

            Gizmos.color = Color.yellow;
            if (points.Count > 1)
            {
                for (int i = 1; i < points.Count; i++)
                {
                    Vector3 start = points[i - 1];
                    Vector3 end = points[i];
                    Gizmos.DrawLine(start, end);
                }
            }
        }
    }
}

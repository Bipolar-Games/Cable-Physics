using System;
using System.Collections.Generic;
using UnityEngine;

namespace Bipolar.CablePhysics
{
    public abstract class Cable : MonoBehaviour
    {
        [SerializeField]
        protected Transform origin;
        public Transform Origin
        {
            get => origin;
            set
            {
                origin = value;
            }
        }

        [SerializeField]
        protected Transform ending;
        public Transform Ending
        {
            get => ending;
            set
            {
                ending = value;
            }
        }

        [SerializeField, Min(0.01f)]
        protected float thickness = 0.1f;
        public float Thickness
        {
            get => thickness;
            set => thickness = value;
        }

        [SerializeField]
        protected LayerMask detectedLayers = -5;

#if NAUGHTY_ATTRIBUTES
        [SerializeField, NaughtyAttributes.ReadOnly]
#endif
        protected float length;
        protected bool isLengthCalculated;
        public float Length
        {
            get
            {
                if (isLengthCalculated == false)
                    CalculateLength();
                return length;
            }
        }

        protected const float preferredChecksDistance = 0.1f;

        public abstract int PointsCount { get; }
        public abstract Vector3 GetPosition(int pointIndex);

        protected abstract void CalculateLength();
    }

    public abstract class Cable<TPoint, TRaycastHit, TCollider, TRay> : Cable
        where TPoint : struct, IEquatable<TPoint>   
        where TRaycastHit : struct
        where TCollider : Component
        where TRay : struct
    {
        protected readonly List<TPoint> points = new List<TPoint>();
        public IReadOnlyList<TPoint> Points => points;

        protected abstract float SqrMagnitude(TPoint point);
        protected abstract float Distance(TPoint lhs, TPoint rhs);
        protected abstract void Normalize(ref TPoint point);

        protected abstract bool Linecast(TPoint start, TPoint end, out TRaycastHit raycastHit, LayerMask layerMask);

        protected abstract TPoint GetPosition(Transform transform);

        protected abstract TPoint GetPoint(TRaycastHit raycastHit);
        protected abstract TPoint GetNormal(TRaycastHit raycastHit);
        protected abstract TCollider GetCollider(TRaycastHit raycastHit);

        protected abstract TPoint Multiply(TPoint point, float number);
        protected abstract TPoint Add(TPoint lhs, TPoint rhs);
        protected abstract TPoint Subtract(TPoint lhs, TPoint rhs);

        protected abstract TRay GetRay(TPoint origin, TPoint direction);
        protected abstract TPoint GetPoint(TRay ray, float distance);

        public sealed override int PointsCount => points.Count;

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
            points.Add(GetPosition(origin));
            points.Add(GetPosition(ending));
        }

        public bool DoubleLinecast(TPoint point1, TPoint point2, out TRaycastHit fromPoint1Info, out TRaycastHit fromPoint2Info, LayerMask layerMask)
        {
            bool fromPoint1 = Linecast(point1, point2, out fromPoint1Info, layerMask);
            bool fromPoint2 = Linecast(point2, point1, out fromPoint2Info, layerMask);
            bool wasHit = fromPoint1 || fromPoint2;
            return wasHit;
        }

        protected sealed override void CalculateLength()
        {
            float distanceSum = 0;
            for (int i = 1; i < points.Count; i++)
            {
                var start = points[i - 1];
                var end = points[i];
                distanceSum += Distance(start, end);
            }
            length = distanceSum;
            isLengthCalculated = true;
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

        private void DetectCollisionEnter(int tipIndex, int direction, Transform tipTransform)
        {
            var previousPosition = points[tipIndex];
            points[tipIndex] = GetPosition(tipTransform);

            var previousTipPosition = previousPosition;
            var currentTipPosition = points[tipIndex];

            if (currentTipPosition.Equals(previousTipPosition))
                return;

            var reversedPositionDelta = Subtract(previousTipPosition, currentTipPosition);
            var reversedPositionDeltaRay = GetRay(currentTipPosition, reversedPositionDelta);
            float movedDistance = Mathf.Sqrt(SqrMagnitude(reversedPositionDelta));
            int checksResolution = 1 + Mathf.Max(1, Mathf.CeilToInt(movedDistance / preferredChecksDistance));
            float checkBaseDistance = movedDistance / checksResolution;

            int neighbourPointIndex = tipIndex + direction;
            var neighbourPosition = points[neighbourPointIndex];
            for (int j = checksResolution - 1; j >= 0; j--)
            {
                var checkedPoint = GetPoint(reversedPositionDeltaRay, j * checkBaseDistance);
                if (DoubleLinecast(checkedPoint, neighbourPosition, out var hit1, out var hit2, detectedLayers))
                {
                    bool hit1Valid = GetCollider(hit1);
                    bool hit2Valid = GetCollider(hit2);

                    var hitPoint1 = hit1Valid ? GetPoint(hit1) : GetPoint(hit2);
                    var hitPoint2 = hit2Valid ? GetPoint(hit2) : GetPoint(hit1);

                    var hitNormal1 = hit1Valid ? GetNormal(hit1): GetNormal(hit2);
                    var hitNormal2 = hit2Valid ? GetNormal(hit2): GetNormal(hit1);

                    var hitCenter = Average(hitPoint1, hitPoint2);
                    var hitNormal = Average(hitNormal1, hitNormal2);
                    if (SqrMagnitude(hitNormal) < 0.001f)
                        hitNormal = reversedPositionDelta;
                    Normalize(ref hitNormal);

                    bool wasSafeHit = false;
                    for (int i = 1; i < 10; i++)
                    {
                        var safePointDetectionLineStart = Add(hitCenter, Multiply(hitNormal, i * thickness));
                        if (Linecast(hitCenter, safePointDetectionLineStart, out var safePointDetectionHit, detectedLayers) == false)
                            continue;

                        var safePoint = Add(GetPoint(safePointDetectionHit), Multiply(GetNormal(safePointDetectionHit), thickness));
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

        protected TPoint Average(TPoint first, TPoint second)
        {
            var sum = Add(first, second);
            return Multiply(sum, 0.5f);
        }

        private void DetectCollisionExit(int tipIndex, int direction)
        {
            if (points.Count > 2)
            {
                var secondNeighbour = points[tipIndex + 2 * direction];
                var tipPoint = points[tipIndex];

                if (DoubleLinecast(tipPoint, secondNeighbour, out _, out _, detectedLayers) == false)
                {
                    float distance = Distance(tipPoint, secondNeighbour);
                    var hypotenuseRay = GetRay(tipPoint, Subtract(secondNeighbour, tipPoint));
                    int triangleChecksResolution = 1 + Mathf.CeilToInt(distance * 5);
                    float rayStartBaseDistance = distance / triangleChecksResolution;
                    int neighbourIndex = tipIndex + direction;
                    var neighbourPoint = points[neighbourIndex];
                    for (int i = 1; i <= triangleChecksResolution; i++)
                    {
                        //Debug.DrawLine(tipPoint, secondNeighbour, new Color(0.1f, 0.1f, 0.3f, 0.3f), 1f);
                        var lineEnd = GetPoint(hypotenuseRay, rayStartBaseDistance * i);
                        if (Linecast(neighbourPoint, lineEnd, out _, detectedLayers))
                            return;
                    }

                    points.RemoveAt(neighbourIndex);
                }
            }
        }

        protected abstract void DrawGizmosLine(TPoint start, TPoint end);

        private void OnDrawGizmos()
        {
            if (points == null || points.Count <= 0)
            {
                Gizmos.DrawLine(origin.position, ending.position);
            }
            else
            {
                Gizmos.color = Color.yellow;
                if (points.Count > 1)
                {
                    for (int i = 1; i < points.Count; i++)
                    {
                        var start = points[i - 1];
                        var end = points[i];
                        DrawGizmosLine(start, end);
                    }
                }
            }
        }
    }
}

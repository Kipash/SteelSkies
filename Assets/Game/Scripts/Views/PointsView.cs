using UnityEngine;
using System.Collections;

public class PointsView : MonoBehaviour
{
    [SerializeField] Color color;
    Transform lastPoint;
    private void OnDrawGizmos()
    {
        lastPoint = null;
        Gizmos.color = color;
        foreach (var t in transform)
        {
            if (lastPoint != null)
            {
                Gizmos.DrawLine(lastPoint.position, ((Transform)t).position);
            }
            lastPoint = (Transform)t;
        }
    }
}

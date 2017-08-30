using UnityEngine;
using System.Collections;

public class PointView : MonoBehaviour
{
    [SerializeField] Color color;
    [SerializeField] float size;
    private void OnDrawGizmos()
    {
        Gizmos.color = color;
        Gizmos.DrawLine(transform.position, transform.position + transform.right * size);
        Gizmos.DrawLine(transform.position, transform.position - transform.right * size);
        Gizmos.DrawLine(transform.position, transform.position + transform.up * size);
        Gizmos.DrawLine(transform.position, transform.position - transform.up * size);
    }
}

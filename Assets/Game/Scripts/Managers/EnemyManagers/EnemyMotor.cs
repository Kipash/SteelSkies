using UnityEngine;
using System;
using System.Collections;

[Serializable]
public class EnemyMotor
{
    internal enum MovementType { none, Drag, Forward}

    [Header("References")]
    [SerializeField] Transform transform;

    [Header("Movement")]
    [SerializeField] float movementSpeed;
    [SerializeField] float rotationSpeed;
    [SerializeField] float minDistance;
    [SerializeField] PathType pathType;
    [SerializeField] MovementType type;
    public bool Move;
    public bool Rotate;

    [Header("Deactivation")]
    [SerializeField] bool destroyOnEndOfRail;
    int currIndex;

    Vector3 posLastFrame;
    float velocity;

    Transform[] path;

    public Action OnDisable;

    public void Start()
    {
        path = GameServices.Instance.WayPointManager.Paths[pathType].Points;
        posLastFrame = transform.position;
    }

    public void Update()
    {
        if (path.Length != 0)
        {
            var targetRotation = Quaternion.LookRotation(path[currIndex].position - transform.position);

            if(Rotate)
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            if (Move)
            {
                if (type == MovementType.Forward)
                    transform.position += transform.forward * movementSpeed * Time.deltaTime;
                else if (type == MovementType.Drag)
                    transform.position = Vector3.Lerp(transform.position, path[currIndex].position, movementSpeed / 10 * Time.deltaTime);
            }

            if(Vector3.Distance(transform.position, path[currIndex].position) < minDistance)
            {
                currIndex++;
                if (currIndex == path.Length)
                {
                    if (!destroyOnEndOfRail)
                        currIndex = 0;
                    else
                        OnDisable();
                }
            }
        }

        velocity = (transform.position - posLastFrame).magnitude / Time.deltaTime;

        posLastFrame = transform.position;
    }
}

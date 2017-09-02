using UnityEngine;
using System;
using System.Collections;

[Serializable]
public class EnemyMotor
{
    [Header("References")]
    [SerializeField] Transform transform;

    [Header("Movement")]
    [SerializeField] float movementSpeed;
    [SerializeField] float rotationSpeed;
    [SerializeField] float minDistance;

    public PathSettings pathSettings;

    public bool Move;
    public bool Rotate;

    int currIndex;

    Vector3 posLastFrame;
    float velocity;

    Transform[] path;

    public Action OnDisable;

    public void Start()
    {
        posLastFrame = transform.position;
        currIndex = 0;
    }

    public void GetPath()
    {
        if (GameServices.Instance != null)
        {
            if (!GameServices.Instance.WayPointManager.Paths.ContainsKey(pathSettings.PathType))
                Debug.LogErrorFormat("No PathType for '{0}' was not found!", pathSettings.PathType);
            path = GameServices.Instance.WayPointManager.Paths[pathSettings.PathType].Points;
        }
    }

    public void Update()
    {
        if (path == null)
            return;

        if (path.Length != 0)
        {
            Transform point;
            if (pathSettings.PathStyle == PathStyle.Circle || pathSettings.PathStyle == PathStyle.Line)
                point = path[currIndex];
            else if (pathSettings.PathStyle == PathStyle.Random)
                point = path[UnityEngine.Random.Range(0, path.Length)];
            else
                point = path[0];

            var targetRotation = Quaternion.LookRotation(point.position - transform.position);

            if(Rotate)
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            if (Move)
            {
                if (pathSettings.MovementType == MovementType.Forward)
                    transform.position += transform.forward * movementSpeed * Time.deltaTime;
                else if (pathSettings.MovementType == MovementType.Drag)
                    transform.position = Vector3.Lerp(transform.position, path[currIndex].position, movementSpeed / 10 * Time.deltaTime);
            }

            if (pathSettings.PathStyle != PathStyle.Random)
            {
                if (Vector3.Distance(transform.position, path[currIndex].position) < minDistance)
                {
                    currIndex++;
                    if (currIndex == path.Length)
                    {
                        if (pathSettings.PathStyle == PathStyle.Circle)
                            currIndex = 0;
                        else
                            OnDisable();
                    }
                }
            }
        }

        velocity = (transform.position - posLastFrame).magnitude / Time.deltaTime;

        posLastFrame = transform.position;
    }
}

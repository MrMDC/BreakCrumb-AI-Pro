using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class waypointController : MonoBehaviour
{
    public List<Transform> waypoints = new List<Transform>();
    private Transform targetWaypoint;
    private int targetWaypointIndex = 0;

    private float minDistance = 0.1f;
    private float movementSpeed = 3.0f;
    private float rotationSpeed = 2.0f;

    #region breadcrumb variable
    public enum EnemyState
    {
        PATROLLING,
        FOLLOWING_PLAYER,
        FOLLOWING_BREADCRUMBS
    };

    EnemyState state = EnemyState.PATROLLING;

    List<Transform> crumbs = new List<Transform>();
    public GameObject crumb;
    public Transform player;
    float minCrumbDistance = 3.0f;
    private Transform lastKnownWaypoint;
    private float inRange = 2.5f;
    private float escapeDistance= 3.0f;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        targetWaypoint = waypoints[targetWaypointIndex];
    }

    // Update is called once per frame
    void Update()
    {
        
        UpdateTransform();
        ControlEnemyState();
    }

    void ControlEnemyState()
    {
        CheckDistanceToPlayer();

        switch (state)
        {
            case EnemyState.PATROLLING:
                float distance = Vector3.Distance(transform.position, targetWaypoint.position);
                checkDistance(distance);
                break;

            case EnemyState.FOLLOWING_PLAYER:

                //when drop?
                if (crumbs.Count >= 1 ) // we have drop the first crumb
                {
                    //check to see if we should drop another crumb
                    if (ShouldPlaceCrumb())
                    {
                        DropBreadCrumb();
                    }
                }
                else
                {
                    DropBreadCrumb();
                }
               
                break;

            case EnemyState.FOLLOWING_BREADCRUMBS:
                ReturnToStartingPoint();
                break;
        }
    }

    bool ShouldPlaceCrumb()
    {
        if (Vector3.Distance(transform.position, crumbs[crumbs.Count - 1].transform.position) > minCrumbDistance)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void CheckDistanceToPlayer()
    {
        switch (state)
        {
            case EnemyState.PATROLLING:
                if (Vector3.Distance(transform.position , player.position) < inRange)
                {
                    lastKnownWaypoint = targetWaypoint;
                    targetWaypoint = player.transform;
                    state = EnemyState.FOLLOWING_PLAYER;
                }
                break;
            case EnemyState.FOLLOWING_PLAYER:
                if (Vector3.Distance(transform.position,player.position) > escapeDistance)//Player has escaped. Follow breadcrumbs back
                {
                    state = EnemyState.FOLLOWING_BREADCRUMBS;
                }
                break;
            case EnemyState.FOLLOWING_BREADCRUMBS:
                if (Vector3.Distance(transform.position,player.position) < inRange)
                {
                    targetWaypoint = player.transform;
                    state = EnemyState.FOLLOWING_PLAYER;
                }
                break;
            default:

                break;
        }
    }

    /// <summary>
    /// Updating rotation and position values
    /// </summary>
    /// 
    private void DropBreadCrumb()
    {
        GameObject droppedCrumb = Instantiate(crumb, transform.position, Quaternion.identity, null);
        crumbs.Add(droppedCrumb.transform);
    }
    /// <summary>
    /// Called when the enemy is following breadcrumbs
    /// </summary>
    void ReturnToStartingPoint()
    {
        if (crumbs.Count >= 1) // There are still crumbs left to follow...
        {
            Transform lastCrumb = crumbs[crumbs.Count - 1];
            targetWaypoint = lastCrumb;

            if (Vector3.Distance(transform.position,lastKnownWaypoint.position)<Vector3.Distance(transform.position,targetWaypoint.position))
            {
                targetWaypoint = lastKnownWaypoint;
                state = EnemyState.PATROLLING;
                foreach (Transform breadcrumbs in crumbs)
                {
                    Destroy(breadcrumbs.gameObject);
                }
                crumbs.Clear();
            }

            if (Vector3.Distance(transform.position , targetWaypoint.position)<0.3f)//reached the crumbs
            {
                crumbs.Remove(lastCrumb.transform);
                Destroy(lastCrumb.gameObject);
                ReturnToStartingPoint();
            }
        }
        else
        {//no crumbs left
            state = EnemyState.PATROLLING;
            targetWaypoint = lastKnownWaypoint;
        }
    }
    void UpdateTransform()
    {
        float movementStep = movementSpeed * Time.deltaTime;
        float rotationStep = rotationSpeed * Time.deltaTime;

        Vector3 directionToTarget = targetWaypoint.position - transform.position;
        Quaternion rotationToTarget = Quaternion.LookRotation(directionToTarget);

        transform.rotation = Quaternion.Slerp(transform.rotation, rotationToTarget, rotationStep);

        Debug.DrawRay(transform.position, transform.forward * 50f, Color.green, 0f);
        Debug.DrawRay(transform.position, directionToTarget, Color.red, 0f);


        transform.position = Vector3.MoveTowards(transform.position, targetWaypoint.position, movementStep);
    }


    void checkDistance(float currentDistance)
    {
        if (currentDistance <= minDistance)
        {
            targetWaypointIndex++;
            updateTargetWaypoints();
        }
    }

    void updateTargetWaypoints()
    {
        if (targetWaypointIndex > waypoints.Count -1)
        {
            targetWaypointIndex = 0;
        }

        targetWaypoint = waypoints[(int)targetWaypointIndex];
    }

    
}

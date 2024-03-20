using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointControllerPractice : MonoBehaviour
{
    // Start is called before the first frame update

    /*todo for Section One - Patrolling
     1. declare waypoints list , targetWaypoint,targetWaypointIndex, moving speed , min Distance
     2. init the first target waypoint
     3. moving
     
    4. rotation 
     */

    //1. public waypoint list for transform
    public List<Transform> waypoints = new List<Transform>();
    private Transform targetWaypoint;
    private int targetWaypointIndex = 0;
    private float moveSpeed = 3.0f;
    private float rotationSpeed = 3.0f;


    /*todo for Section Two - BreadCrumb
     1. declare chasing range and return range , crumb game object to instantiate , crumbs list for check , player gameobject for chasing and last known point
     2. tidy up the section one code
     3. set region for three condition
     4. check state
     if state is in patrolling , check distance with player inrange or not
     if state is in player follow , check distance with player outrange or not
     if state is in breakcrumb , chk inrange
     
     5. if player follow , then chase player and put crumb

     */
    private float inRange = 3.0f;
    private float outRange = 6.0f;
    public GameObject crumb;
    public GameObject player;
    private List<GameObject> crumbs= new List<GameObject>();
    private Transform lastKnownPoint ;
    private float minCrumbsDistance = 2.0f;

    #region breadcrumbs
    public enum EnemyState {
        PATROLLING,
        FOLLOWING_PLAYER,
        FOLLOWING_BREAKCRUMB
    }

    public EnemyState state = EnemyState.PATROLLING;
    #endregion

    void Start()
    {
        //2. init
        targetWaypoint = waypoints[targetWaypointIndex];
    }

    // Update is called once per frame
    /*
     1. Go to target then ++
     2. keep update to target
     */
    void Update()
    {
        stateController();
        updateTransform();

    }

    void updateTransform()
    {
        float moveStep = moveSpeed * Time.deltaTime;
        float rotationStep = rotationSpeed * Time.deltaTime;

        Vector3 directionToTarget = targetWaypoint.position - transform.position;
        Quaternion RotationToTarget = Quaternion.LookRotation(directionToTarget);
        transform.rotation = Quaternion.Slerp(transform.rotation, RotationToTarget, rotationStep);

        transform.position = Vector3.MoveTowards(transform.position, targetWaypoint.position, moveStep);
        
    }

    void CheckState()
    {
        switch (state)
        {
            case EnemyState.PATROLLING:
                
                if (Vector3.Distance(transform.position,player.transform.position) < inRange)
                {
                    lastKnownPoint = targetWaypoint;
                    state = EnemyState.FOLLOWING_PLAYER;
                }
                break;
            case EnemyState.FOLLOWING_PLAYER:
                
                if (Vector3.Distance(transform.position, player.transform.position) > outRange)
                {
                    state = EnemyState.FOLLOWING_BREAKCRUMB;
                }
                break;
            case EnemyState.FOLLOWING_BREAKCRUMB:

                
                if (Vector3.Distance(transform.position, player.transform.position) < inRange)
                {
                    state = EnemyState.FOLLOWING_PLAYER;
                }
                break;
            default:
                break;
        }
    }

    void stateController()
    {
        CheckState();

        switch (state)
        {
            case EnemyState.PATROLLING:
                //check distance


                if (Vector3.Distance(transform.position, targetWaypoint.position) <= 0)
                {

                    targetWaypointIndex++;

                    if (targetWaypointIndex > waypoints.Count - 1)
                    {
                        targetWaypointIndex = 0;
                    }

                    targetWaypoint = waypoints[targetWaypointIndex];

                }

                break;
            case EnemyState.FOLLOWING_PLAYER:
                crumbPut();
                targetWaypoint = player.transform;

                break;
            case EnemyState.FOLLOWING_BREAKCRUMB:
                targetWaypoint = crumbs[crumbs.Count - 1].transform;
                
                if (Vector3.Distance(transform.position, targetWaypoint.position) <= 0)
                {
                    if (crumbs.Count == 1)
                    {
                        GameObject.Destroy(crumbs[crumbs.Count - 1]);
                        crumbs.Clear();
                        targetWaypoint = lastKnownPoint;
                        state = EnemyState.PATROLLING;
                    }
                    else
                    {
                        GameObject.Destroy(crumbs[crumbs.Count - 1]);
                        crumbs.Remove(crumbs[crumbs.Count - 1]);
                        targetWaypoint = crumbs[crumbs.Count - 1].transform;
                    }

                }

                

                break;
            default:
                break;
        }
    }

    void crumbPut()
    {
        if (crumbs.Count > 0)
        {
            if (Vector3.Distance(transform.position, crumbs[crumbs.Count - 1].transform.position) > minCrumbsDistance)
            {
                GameObject droppedCrumb = Instantiate(crumb, transform.position, Quaternion.identity, null);
                crumbs.Add(droppedCrumb);
            }
        }
        else {
            GameObject droppedCrumb = Instantiate(crumb, transform.position, Quaternion.identity, null);
            crumbs.Add(droppedCrumb);
        }
        
        
    }


}

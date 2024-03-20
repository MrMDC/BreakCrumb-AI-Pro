using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class crumbController : MonoBehaviour
{
    // Start is called before the first frame update

    /*
     todo 
    1. declare variable crumb to put , player , enemy , targetCrumb
    2. instantiate function , destroy function , shouldPut function , targetCrumbUpdate
    3. moving state and idle state
    4. if player move then enemy move until the last crumb although player is stopped

    5. chk which one of crumb is nearest to 
     */
    public GameObject crumb;
    public GameObject player;
    public GameObject enemy;

    private List<GameObject> crumbs = new List<GameObject>();
    private float minCrumbDistance = 2.0f;
    private Transform targetCrumb;
    private float moveSpeed = 1.0f;
    private float movingConfirmation = 1.0f;

    public enum PlayerState
    {
        Moving,
        Idle
    }

    PlayerState state = PlayerState.Idle;

    
    // Update is called once per frame
    void Update()
    {
        PlayerDo();
        UpdateTransformEnemy();
    }

    void UpdateTransformEnemy()
    {
        float moveStep = moveSpeed * Time.deltaTime;
        if (state == PlayerState.Moving )
        {
            targetCrumb = crumbs[0].transform;
            enemy.transform.position = Vector3.MoveTowards(enemy.transform.position, targetCrumb.position, moveStep);

            if (Vector3.Distance(enemy.transform.position, targetCrumb.position) <= 0)
            {
                GameObject.Destroy(crumbs[0]);
                crumbs.RemoveAt(0);
            }
        }

        
    }

    void CheckPlayerState()
    {
        switch (state)
        {
            case PlayerState.Idle:
                if (Vector3.Distance(player.transform.position,enemy.transform.position) > movingConfirmation)
                {
                    state = PlayerState.Moving;
                }
                break;
            
            case PlayerState.Moving:

                if (Vector3.Distance(player.transform.position, enemy.transform.position) < movingConfirmation)
                {
                    state = PlayerState.Idle;
                }

                break;
        }
    }

    void PlayerDo()
    {
        CheckPlayerState();
        switch (state)
        {
            case PlayerState.Moving:

                DropCrumb();

                break;
            case PlayerState.Idle:

                break;
            default:
                break;
        }
    }

    void DropCrumb()
    {
        if (crumbs.Count > 0)
        {
            if (Vector3.Distance(player.transform.position, crumbs[crumbs.Count - 1].transform.position) > minCrumbDistance)
            {
                GameObject droppedCrumb = Instantiate(crumb, player.transform.position, Quaternion.identity, null);
                crumbs.Add(droppedCrumb);
            }
        }
        else
        {
            GameObject droppedCrumb = Instantiate(crumb, player.transform.position, Quaternion.identity, null);
            crumbs.Add(droppedCrumb);
        }

    }
}

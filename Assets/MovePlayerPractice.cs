using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlayerPractice : MonoBehaviour
{
    // Update is called once per frame
    private float move = 0.25f;
    void Update()
    {
        if (Input.GetKey(KeyCode.UpArrow))
        {
            transform.position = new Vector3(transform.position.x, transform.position.y,transform.position.z - move);
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + move);
        }
        if(Input.GetKey(KeyCode.LeftArrow))
        {
            transform.position = new Vector3(transform.position.x - move, transform.position.y, transform.position.z);
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.position = new Vector3(transform.position.x + move, transform.position.y, transform.position.z );
        }
    }
}

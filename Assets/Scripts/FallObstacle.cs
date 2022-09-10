using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallObstacle : MonoBehaviour
{
    private ObstacleManager obstacleScript;
    private Rigidbody rig;
    private Vector3 initPos;
    public void Init()
    {
        rig = this.GetComponent<Rigidbody>();
        initPos = rig.position;
    }

    public void ManagedUpdate()
    {

    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.tag == "Ground")
        {
            rig.position = initPos;
        }

        if (collision.transform.tag == "Legion")
        {
            Destroy(collision.gameObject);
        }

    }
}

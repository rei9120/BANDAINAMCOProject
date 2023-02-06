using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallObstacle : MonoBehaviour
{
    private ObstacleManager obstacleScript;
    private Rigidbody rig;
    private Vector3 speed;

    public void Start()
    {
        rig = this.GetComponent<Rigidbody>();
        speed = new Vector3(0.0f, 0.0f, -500.0f);
        GameObject obj = GameObject.Find("ObstacleManager");
        obstacleScript = obj.GetComponent<ObstacleManager>();
        obstacleScript.SetBallObjects(this);
    }

    public void Init()
    {
        rig = this.GetComponent<Rigidbody>();
    }

    public void ManagedUpdate(float deltaTime)
    {
        rig.AddForce(speed * deltaTime, ForceMode.Acceleration);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Legion")
        {
            Destroy(collision.gameObject);
        }
    }
}

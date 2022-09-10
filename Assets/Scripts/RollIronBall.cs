using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollIronBall : MonoBehaviour
{
    Rigidbody rb;
    Vector3 speed;
    float startPos;
    float distanceTraveled;
    const float overDistance = 100.0f;

    public void Init()
    {
        rb = this.GetComponent<Rigidbody>();
        speed = new Vector3(0.0f, 0.0f, -400.0f);
        startPos = rb.position.z;
        distanceTraveled = 0.0f;
    }
    public void ManagedUpdate(float deltaTime)
    {
        rb.AddForce(speed * deltaTime, ForceMode.Force);

        distanceTraveled = rb.position.z - startPos;

        if(distanceTraveled < 0)
        {
            distanceTraveled = distanceTraveled * -1f;
        }

        if(distanceTraveled > overDistance)
        {
            Destroy(rb.gameObject);
            Destroy(this);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Legion")
        {
            Destroy(collision.gameObject);
        }
    }
}

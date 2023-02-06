using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarObstacle : MonoBehaviour
{
    public void Start()
    {
    }

    public void Init()
    {
    }

    public void ManagedUpdate(float deltaTime)
    {
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Legion")
        {
            Destroy(collision.gameObject);
        }
    }
}

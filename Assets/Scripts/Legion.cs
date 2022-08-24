using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Legion : MonoBehaviour
{
    private GameObject player;
    private Rigidbody rig;
    private Vector3 velocity = new Vector3(0.0f, 0.0f, 0.0f);
    private Vector3 jumpForce = new Vector3(0.0f, 5.0f, 0.0f);
    private float speed = 10f;
    private float pDistance = 3.0f;
    private float lDistance = 1.5f;

    public void Init(GameObject pl)
    {
        rig = this.GetComponent<Rigidbody>();
        player = pl;
        this.rig.position = player.GetComponent<Rigidbody>().position + new Vector3(0.0f, 0.0f, -4.0f);
    }

    public void ManagedUpdate(Legion le)
    {
        FollowPlayer();
        Control();
        AnotherLegion(le);

        rig.position += velocity;
        velocity = Vector3.zero;
    }

    private void FollowPlayer()
    {
        this.transform.LookAt(player.transform);

        float pos1 = player.transform.position.x - rig.position.x;
        float pos2 = player.transform.position.z - rig.position.z;
        float dis = Mathf.Sqrt(pos1 * pos1 + pos2 * pos2);
        if (dis > pDistance)
        {
            velocity += transform.forward * speed * Time.deltaTime;
        }
    }

    private void Control()
    {
        if (Input.GetMouseButtonDown(1))
        {
            rig.AddForce(jumpForce, ForceMode.Impulse);
        }
    }

    private void AnotherLegion(Legion le)
    {
        if(le != null)
        {
        }
    }
}

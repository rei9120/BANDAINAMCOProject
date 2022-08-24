using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    private GameObject player;
    private Rigidbody playerRig;
    private Transform tf;
    private Vector3 pos;
    private Vector3 distance;

    public void Init(GameObject pl)
    {
        player = pl;
        if(player != null)
        {
            playerRig = player.GetComponent<Rigidbody>();
        }
        tf = this.transform;
        distance = tf.position;
        pos = tf.position;
    }

    public void ManagedUpdate()
    {
        if (player != null)
        {
            FollowPlayer();
        }
    }

    private void FollowPlayer()
    {
        if (player != null)
        {
            tf.position = pos + playerRig.position;
        }
    }
}

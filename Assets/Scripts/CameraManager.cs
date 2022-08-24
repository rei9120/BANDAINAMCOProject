using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    private Legion obj;
    private Rigidbody objRb;
    private Transform tf;
    private Vector3 pos;
    private Vector3 distance;

    public void Init(Legion gameObj)
    {
        obj = gameObj;
        if(obj != null)
        {
            objRb = obj.GetComponent<Rigidbody>();
        }
        tf = this.transform;
        distance = tf.position;
        pos = tf.position;
    }

    public void ManagedUpdate(Legion gameObj)
    {
        if(obj != gameObj && gameObj != null)
        {
            obj = gameObj;
            objRb = obj.GetComponent<Rigidbody>();
        }

        if (obj != null)
        {
            FollowTarget();
        }
    }

    private void FollowTarget()
    {
        if (obj != null)
        {
            tf.position = pos + objRb.position;
        }
    }
}

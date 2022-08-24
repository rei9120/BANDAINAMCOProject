using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointManager : MonoBehaviour
{
    private Transform tf;
    // Start is called before the first frame update
    public void Init()
    {
        tf = this.transform;
    }

    // Update is called once per frame
    public void ManagedUpdate()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo = new RaycastHit();
        if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity))
        {
            tf.position = hitInfo.point;
            Vector3 pos = tf.position;
            pos.y = 1.0f;
            tf.position = pos;
        }
    }
}

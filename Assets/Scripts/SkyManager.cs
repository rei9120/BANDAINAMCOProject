using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyManager : MonoBehaviour
{
    private GameObject camera;
    private Transform tf;
    private Transform cTf;

    private Vector3 letGoPos;

    public void Init(GameObject c)
    {
        camera = c;
        cTf = camera.transform;
        tf = this.transform;
        letGoPos = new Vector3(0.0f, 0.0f, 10.0f);
        tf.position = cTf.position + letGoPos;
    }

    public void ManagedUpdate()
    {
        tf.position = cTf.position + letGoPos;
    }
}

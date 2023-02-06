using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallSensor : MonoBehaviour
{
    [SerializeField] private GameObject ballObject;
    [SerializeField] private float init_lx;
    [SerializeField] private float init_rx;
    [SerializeField] private Vector3 pos;
    private bool ballFlag;

    private const float time = 5.0f;
    private float count = 0.0f;
    // Start is called before the first frame update
    void Start()
    {
        ballFlag = false;
    }

    // Update is called once per frame
    void Update()
    {
        count += Time.deltaTime;
        if(ballFlag && count > time)
        {
            GameObject obj = GameObject.Instantiate(ballObject);
            float x = Random.Range(init_lx, init_rx);
            obj.transform.localPosition = new Vector3(x, pos.y, pos.z);
            count = 0.0f;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Legion")
        {
            ballFlag = true;
        }
        if(other.transform.tag == "Obstacle")
        {
            Destroy(other.gameObject);
        }
    }
}

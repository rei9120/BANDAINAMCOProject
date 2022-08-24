using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Legion : MonoBehaviour
{
    private PointManager pointScript;
    private Rigidbody rig;
    private Transform pTf;
    private Rigidbody pRb;
    private Vector3 velocity = new Vector3(0.0f, 0.0f, 0.0f);
    private Vector3 jumpForce = new Vector3(0.0f, 5.0f, 0.0f);
    private float speed = 10f;
    private float pDistance = 3.0f;
    private float lDistance = 1.5f;

    public enum Item
    {
        none,
        r,
        normal,
    };
    Item itemType;

    public void Init(GameObject p, Vector3 pos)
    {
        pTf = p.transform;
        pRb = p.GetComponent<Rigidbody>();
        pointScript = p.GetComponent<PointManager>();
        rig = this.GetComponent<Rigidbody>();
        rig.position = pos + new Vector3(0.0f, rig.position.y, -2.0f);
    }

    public void ManagedUpdate(Legion le)
    {
        FollowPoint();
    }

    private void FollowPoint()
    {
        if (pointScript.GetMoveFlag())
        {
            float pos1 = pRb.position.x - rig.position.x;
            float pos2 = pRb.position.z - rig.position.z;
            float dis = Mathf.Sqrt(pos1 * pos1 + pos2 * pos2);
            if (dis > pDistance)
            {
                velocity += transform.forward * speed * Time.deltaTime;
            }

            rig.position += velocity;
            velocity = Vector3.zero;

            this.transform.LookAt(pTf);
        }

        if (pointScript.GetJumpFlag())
        {
            rig.AddForce(jumpForce, ForceMode.Impulse);
        }
    }

    private void OnCollisionEnter(Collision col)
    {
        Transform cTf = col.transform;
        if (cTf.tag == "NormalItem")
        {
            itemType = Item.normal;
            col.gameObject.SetActive(false);
        }

        if(cTf.tag == "Ground")
        {
            if (pointScript != null)
            {
                pointScript.SetJumpFlag(false);
            }
        }
    }

    public Item FindItem()
    {
        return itemType;
    }
    public void SetItemType(Item type)
    {
        itemType = type;
    }
}

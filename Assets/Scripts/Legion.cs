using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Legion : MonoBehaviour
{
    private PointManager pointScript;
    private MouseLineRenderer lineScript;
    private Rigidbody rig;
    private Transform pTf;
    private Rigidbody pRb;
    private Vector3 velocity = new Vector3(0.0f, 0.0f, 0.0f);
    private Vector3 lineVelocity = new Vector3(0.0f, 0.0f, 0.0f);
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

    public void Init(GameObject p, MouseLineRenderer lineRenderer, Vector3 pos)
    {
        pTf = p.transform;
        pRb = p.GetComponent<Rigidbody>();
        pointScript = p.GetComponent<PointManager>();
        rig = this.GetComponent<Rigidbody>();
        rig.position = pos + new Vector3(0.0f, rig.position.y, -2.0f);
        lineScript = lineRenderer;
    }

    public void ManagedUpdate(Legion le)
    {
        FollowPoint();
        LineFormation(le);
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

    private void LineFormation(Legion le)
    {
        Vector3 pos = rig.position;
        Vector3 lePos = le.transform.position;
        Vector3 sPos = lineScript.GetStartLinePos();
        Vector3 ePos = lineScript.GetEndLinePos();

        if(sPos.x <= pos.x && ePos.x >= pos.x)
        {
            MoveInLine(pos, lePos, sPos, ePos);
        }
        else
        {
            MoveOutLine(pos, sPos);
        }
    }

    private void MoveInLine(Vector3 pos, Vector3 lePos, Vector3 sPos, Vector3 ePos)
    {

    }

    private void MoveOutLine(Vector3 pos, Vector3 sPos)
    {
        float pos1 = sPos.x - pos.x;
        float pos2 = sPos.z - pos.z;
        float divideSpeed = 2.0f;

        if(pos1 < 0)
        {
            lineVelocity.x = -1 * speed / divideSpeed * Time.deltaTime;
        }
        else
        {
            lineVelocity.x = speed / divideSpeed * Time.deltaTime;
        }

        if(pos2 < 0)
        {
            lineVelocity.z = -1 * speed / divideSpeed * Time.deltaTime;
        }
        else
        {
            lineVelocity.z = speed / divideSpeed * Time.deltaTime;
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

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
    private Vector3 velocity = Vector3.zero;
    private Vector3 lineVelocity = Vector3.zero;
    private Vector3 jumpForce = new Vector3(0.0f, 5.0f, 0.0f);
    private float speed = 10f;
    private float pDistance = 3.0f;
    private float lDistance = 0.5f;
    private float tDistance = 0.5f;
    private bool legionFlag = false;

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

        rig.position += velocity + lineVelocity;
    }

    private void FollowPoint()
    {
        velocity = Vector3.zero;
        if (pointScript.GetMoveFlag() && !legionFlag)
        {
            float pos1 = pRb.position.x - rig.position.x;
            float pos2 = pRb.position.z - rig.position.z;
            float dis = Mathf.Sqrt(pos1 * pos1 + pos2 * pos2);
            if (dis > pDistance)
            {
                velocity = transform.forward * speed * Time.deltaTime;
            }

            this.transform.LookAt(pTf);
        }

        if (pointScript.GetJumpFlag())
        {
            rig.AddForce(jumpForce, ForceMode.Impulse);
        }
    }

    private void LineFormation(Legion le)
    {
        lineVelocity = Vector3.zero;
        // ë‡óÒÇ∑ÇÈîÕàÕê¸Ç™à¯Ç©ÇÍÇΩÇÁ
        if (lineScript.GetSetLineFlag())
        {
            legionFlag = true;
            Vector3 pos = rig.position;
            Vector3 lePos = Vector3.zero;
            if (le != null)
            {
                lePos = le.transform.position;
            }
            Vector3 sPos = lineScript.GetStartLinePos();
            Vector3 ePos = lineScript.GetEndLinePos();

            // îÕàÕê¸ÇÃíÜÇæÇ¡ÇΩÇÁ
            if (sPos.x <= pos.x && ePos.x >= pos.x)
            {
                MoveInLine(pos, lePos, sPos, ePos);
            }
            else  // îÕàÕê¸ÇÃäOÇæÇ¡ÇΩÇÁ
            {
                // îÕàÕÇÃê^ÇÒíÜÇìæÇÈ
                Vector3 dPos = Vector3.zero;
                Vector3 tPos = Vector3.Lerp(sPos, ePos, 0.5f);
                dPos.y = sPos.y;
                dPos.x = tPos.x;
                dPos.z = tPos.z;

                MoveOutLine(pos, dPos);
            }
        }
    }

    private void MoveInLine(Vector3 pos, Vector3 lePos, Vector3 sPos, Vector3 ePos)
    {
        if(lePos == Vector3.zero && legionFlag)
        {
            Vector3 targetPos = new Vector3(MinValue(sPos.x, ePos.x), pos.y, MaxValue(sPos.z, ePos.z));
            this.transform.LookAt(targetPos);
            lineVelocity = transform.forward * speed * Time.deltaTime;
            legionFlag = false;
        }
        else if(legionFlag)
        {

        }
    }

    private void MoveOutLine(Vector3 pos, Vector3 dPos)
    {
        float pos1 = dPos.x - pos.x;
        float pos2 = dPos.z - pos.z;
        float distance = Mathf.Sqrt(pos1 * pos1 + pos2 * pos2);

        this.transform.LookAt(dPos);

        if(distance > 0)
        {
            lineVelocity = transform.forward * speed * Time.deltaTime;
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

    private Vector3 AddVector(Vector3 pos1, Vector3 pos2)
    {
        float maxX = MaxValue(pos1.x, pos2.x);
        float minX = MinValue(pos1.x, pos2.x);
        float maxZ = MaxValue(pos1.z, pos2.z);
        float minZ = MinValue(pos1.z, pos2.z);
        float x = 0.0f;
        float y = 0.0f;
        float z = 0.0f;

        if (maxX > 0 && minX < 0 || maxX < 0 && minX > 0)
        {
            x = (float)maxX - (float)minX;
        }
        else
        {
            x = (float)minX + (float)maxX;
        }

        if (maxZ > 0 && minZ < 0 || maxZ < 0 && minZ > 0)
        {
            z = (float)maxZ - (float)minZ;
        }
        else
        {
            z = (float)minZ + (float)minZ;
        }

        y = (float)pos1.y;

        Vector3 pos = new Vector3(x, y, z);
        return pos;
    }

    private float MaxValue(float value1, float value2)
    {
        float tmp1 = value1;
        float tmp2 = value2;

        if (tmp1 < 0.0f)
        {
            tmp1 = -1 * tmp1;
        }
        if (tmp2 < 0.0f)
        {
            tmp2 = -1 * tmp2;
        }

        if (tmp1 > tmp2)
        {
            if (value1 > 0)
            {
                return value1;
            }
            else
            {
                return value2;
            }
        }
        else
        {
            if (value2 > 0)
            {
                return value2;
            }
            else
            {
                return value1;
            }
        }
    }

    private float MinValue(float value1, float value2)
    {
        float tmp1 = value1;
        float tmp2 = value2;

        if (tmp1 < 0.0f)
        {
            tmp1 = -1 * tmp1;
        }
        if (tmp2 < 0.0f)
        {
            tmp2 = -1 * tmp2;
        }

        if (tmp1 > tmp2)
        {
            if (value1 > 0)
            {
                return value2;
            }
            else
            {
                return value1;
            }
        }
        else
        {
            if (value2 > 0)
            {
                return value1;
            }
            else
            {
                return value2;
            }
        }
    }
}

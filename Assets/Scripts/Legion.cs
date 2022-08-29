using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Legion : MonoBehaviour
{
    private PointManager pointScript;
    private MouseLineRenderer lineScript;
    private GameObject lineRenderer;
    private Transform tf;
    private Rigidbody rig;
    private Transform pTf;
    private Rigidbody pRb;
    private Transform lTf;
    private Vector3 velocity = Vector3.zero;
    private Vector3 lineVelocity = Vector3.zero;
    private Vector3 jumpForce = new Vector3(0.0f, 5.0f, 0.0f);
    private float speed = 10f;
    private float pDistance = 3.0f;
    private float lWidthDistance = 1.5f;
    private float lHeightDistance = 3f;
    private float tWidthDistance = 0.3f;
    private float tHeightDistance = 0.3f;
    private bool moveFlag = false;
    private bool jumpFlag = false;
    private bool setLineFlag = false;
    private bool legionFlag = false;
    private bool followLineFlag = false;

    public enum Item
    {
        none,
        r,
        normal,
    };
    Item itemType;

    public void Init(GameObject p, GameObject l, Vector3 pos)
    {
        pTf = p.transform;
        pRb = p.GetComponent<Rigidbody>();
        pointScript = p.GetComponent<PointManager>();
        tf = this.transform;
        rig = this.GetComponent<Rigidbody>();
        rig.position = pos + new Vector3(0.0f, rig.position.y, -2.0f);
        lineRenderer = l;
        lineScript = lineRenderer.GetComponent<MouseLineRenderer>();
        lTf = lineRenderer.transform;
    }

    public void ManagedUpdate(Legion le)
    {
        // 必要な値を更新または初期化する
        UpdateValue();

        // ラインについていく
        if (followLineFlag)
        {
            FollowLine();
        }
        else
        {
            // ラインの中で隊列する
            if(setLineFlag)
            {
                LineFormation(le);
            }
            // プレイヤーについていく
            else
            {
                FollowPoint();
            }
        }

        if (jumpFlag)
        {
            rig.AddForce(jumpForce, ForceMode.Impulse);
        }

        rig.position += velocity + lineVelocity;

        Debug.Log("setLineFlag = " + setLineFlag);
        Debug.Log("followLineFlag = " + followLineFlag);
    }

    // プレイヤーについていく処理
    private void FollowPoint()
    {
        if (moveFlag)
        {
            float pos1 = pRb.position.x - rig.position.x;
            float pos2 = pRb.position.z - rig.position.z;
            float dis = Mathf.Sqrt(pos1 * pos1 + pos2 * pos2);
            if (dis > pDistance)
            {
                velocity = tf.forward * speed * Time.deltaTime;
            }
        }
        tf.LookAt(pTf);
    }

    private void FollowLine()
    {
        tf.LookAt(pTf);
        if (moveFlag)
        {
            Vector3 sPos = lineScript.GetStartLinePos();
            Vector3 ePos = lineScript.GetEndLinePos();
            Vector3 tPos = Vector3.Lerp(sPos, ePos, 0.5f);
            tPos.y = sPos.y;

            tf.localRotation = lTf.localRotation;
            velocity = tf.forward * speed * Time.deltaTime;
        }
    }

    // 隊列する処理
    private void LineFormation(Legion le)
    {
        // 隊列する範囲線が引かれたら
        if (setLineFlag)
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

            // 範囲線の中だったら
            if (le == null)
            {
                if (sPos.x <= pos.x && ePos.x >= pos.x)
                {
                    MoveInLine(pos, lePos, sPos, ePos);
                }
                else  // 範囲線の外だったら
                {
                    // 範囲の真ん中を得る
                    Vector3 dPos = Vector3.zero;
                    Vector3 tPos = Vector3.Lerp(sPos, ePos, 0.5f);
                    dPos.y = sPos.y;
                    dPos.x = tPos.x;
                    dPos.z = tPos.z;

                    MoveOutLine(pos, dPos);
                }
            }
            else
            {
                FollowAnotherLegion(le, pos, sPos, ePos);
            }
        }
        else
        {
            legionFlag = false;
        }
    }

    private void MoveInLine(Vector3 pos, Vector3 lePos, Vector3 sPos, Vector3 ePos)
    {
        if(lePos == Vector3.zero && legionFlag && !followLineFlag)
        {
            Vector3 targetPos = new Vector3(MinValue(sPos.x, ePos.x), pos.y, MaxValue(sPos.z, ePos.z));

            if (targetPos.x - tWidthDistance < pos.x && targetPos.x + tWidthDistance > pos.x &&
                targetPos.z + tHeightDistance > pos.z && targetPos.z - tHeightDistance < pos.z)
            {
                followLineFlag = true;
            }
            else
            {
                tf.LookAt(targetPos);
                lineVelocity = tf.forward * speed * Time.deltaTime;
            }
            Debug.Log("targetPos = " + targetPos);
        }
    }

    private void MoveOutLine(Vector3 pos, Vector3 dPos)
    {
        float pos1 = dPos.x - pos.x;
        float pos2 = dPos.z - pos.z;
        float distance = Mathf.Sqrt(pos1 * pos1 + pos2 * pos2);

        tf.LookAt(dPos);

        if(distance > 0)
        {
            lineVelocity = tf.forward * speed * Time.deltaTime;
        }
    }

    private void FollowAnotherLegion(Legion le, Vector3 pos, Vector3 sPos, Vector3 ePos)
    {
        if (legionFlag && !followLineFlag)
        {
            Vector3 lePos = le.transform.position;
            lePos.x = lePos.x + lWidthDistance;
            Vector3 targetPos = lePos;

            if ((targetPos.x < pos.x && targetPos.x + lWidthDistance > pos.x) && 
                le.GetFollowLineFlag() && ePos.x >= pos.x)
            {
                followLineFlag = true;
            }
            else
            {
                if(ePos.x < pos.x)
                {
                    targetPos.z = targetPos.z - lHeightDistance;
                }
                tf.LookAt(targetPos);
                lineVelocity = tf.forward * speed * Time.deltaTime;
            }
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

    private void UpdateValue()
    {
        moveFlag = false;
        jumpFlag = false;
        setLineFlag = false;
        moveFlag = pointScript.GetMoveFlag();
        jumpFlag = pointScript.GetJumpFlag();
        setLineFlag = lineScript.GetSetLineFlag();
        velocity = Vector3.zero;
        lineVelocity = Vector3.zero;
    }

    public Item FindItem()
    {
        return itemType;
    }
    public void SetItemType(Item type)
    {
        itemType = type;
    }

    public void SetFollowLineFlag(bool flag)
    {
        followLineFlag = flag;
    }

    public bool GetFollowLineFlag()
    {
        return followLineFlag;
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

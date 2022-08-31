using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Legion : MonoBehaviour
{
    private PointManager pointScript;
    private LegionManager legionScript;

    private Animator anim;

    private Transform tf;
    private Transform pTf;

    private Rigidbody rig;
    private Rigidbody pRb;

    private Vector3 velocity = Vector3.zero;
    private Vector3 jumpForce = new Vector3(0.0f, 5.0f, 0.0f);

    private float speed = 10f;
    
    private bool moveFlag = false;
    private bool jumpFlag = false;
    private bool legionFlag = false;
    private bool arrivalFlag = false;
    private bool lineFlag = false;
    private bool allLegionFlag = false;

    public enum Item
    {
        none,
        r,
        normal,
    };
    Item itemType;

    public void Init(GameObject p, GameObject le,　Vector3 pos)
    {
        pTf = p.transform;
        pRb = p.GetComponent<Rigidbody>();
        pointScript = p.GetComponent<PointManager>();
        tf = this.transform;
        rig = this.GetComponent<Rigidbody>();
        rig.position = pos + new Vector3(0.0f, rig.position.y, -2.0f);
        legionScript = le.GetComponent<LegionManager>();
        anim = this.GetComponent<Animator>();
    }

    public void ManagedUpdate(Vector3 targetPos)
    {
        // 必要な値を更新または初期化する
        UpdateValue();

        if (!arrivalFlag && targetPos != Vector3.zero)
        {
            Move(targetPos);
        }
        else
        {
            if (legionFlag)
            {
                tf.localRotation = Quaternion.LookRotation(pTf.position - targetPos);
            }
            else
            {
                tf.LookAt(pTf);
            }
        }

        if (jumpFlag)
        {
            rig.AddForce(jumpForce, ForceMode.Impulse);
        }

        Animation();
        rig.position += velocity;
    }

    private void Move(Vector3 tPos)
    {
        if(tPos != rig.position)
        {
            // 隊列している場合
            if (legionFlag)
            {
                tf.localRotation = Quaternion.LookRotation(pTf.position - tPos);
            }
            else
            {
                tPos.y = rig.position.y;
                tf.LookAt(tPos);
            }

            if(moveFlag)
            {
                velocity = tf.forward * speed * Time.deltaTime;
            }
            else
            {
                if (lineFlag && !legionFlag)
                {
                    velocity = tf.forward * speed * Time.deltaTime;
                }
            }
        }
    }

    private void Animation()
    {
        if (moveFlag)
        {
            anim.SetBool("ArrivalFlag", arrivalFlag);
        }
        else
        {
            anim.SetBool("ArrivalFlag", true);
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

        if(cTf.tag == "Obstacle")
        {
            Destroy(this);
        }

        if(cTf.tag == "Fall")
        {
            Destroy(this);
        }
    }

    private void UpdateValue()
    {
        jumpFlag = false;
        jumpFlag = pointScript.GetJumpFlag();
        velocity = Vector3.zero;
    }

    public Item FindItem()
    {
        return itemType;
    }
    public void SetItemType(Item type)
    {
        itemType = type;
    }

    public void SetMoveFlag(bool flag)
    {
        moveFlag = flag;
    }

    public void SetLineFlag(bool flag)
    {
        lineFlag = flag;
    }

    public bool GetLegionFlag()
    {
        return legionFlag;
    }

    public void SetLegionFlag(bool flag)
    {
        legionFlag = flag;
    }

    public void SetArrivalFlag(bool flag)
    {
        arrivalFlag = flag;
    }

    public void SetAllLegionFlag(bool flag)
    {
        allLegionFlag = flag;
    }

    public float CompareTheDistanceYouAndOther(Vector3 tPos)
    {
        Vector3 mPos = rig.position;
        float p1 = tPos.x - mPos.x;
        float p2 = tPos.z - mPos.z;
        float distance = Mathf.Sqrt(p1 * p1 + p2 * p2);
        return distance;
    }

    public Vector3 GetLegionPosition()
    {
        return rig.position;
    }
}

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

    public void ManagedUpdate(Vector3 targetPos, float deltaTime)
    {
        // 必要な値を更新または初期化する
        UpdateValue();

        if (jumpFlag)
        {
            rig.AddForce(jumpForce, ForceMode.Impulse);
        }

        Move(targetPos, deltaTime);

        Animation();
    }

    private void Move(Vector3 tPos, float deltaTime)
    {
        switch (legionFlag)
        {
            case true:
                MoveFollowLegion(tPos, deltaTime);
                break;
            case false:
                if (legionScript.GetLineFlag())
                {
                    MoveFollowLine(tPos, deltaTime);
                }
                else
                {
                    MoveFollowPlayer(deltaTime);
                }
                break;
        }

        if (legionScript.GetMoveFlag())
        {
            rig.position += velocity;
        }
    }

    public void MoveFollowPlayer(float deltaTime)
    {
        tf.LookAt(pTf);
        velocity = tf.forward * speed * deltaTime;
    }

    public void MoveFollowLegion(Vector3 tPos, float deltaTime)
    {
        tf.localRotation = Quaternion.LookRotation(pTf.position - tPos);
        velocity = tf.forward * speed * deltaTime;
    }

    public void MoveFollowLine(Vector3 tPos, float deltaTime)
    {
        tf.LookAt(tPos);
        velocity = tf.forward * speed * deltaTime;
    }

    private void Animation()
    {
        if (legionScript.GetMoveFlag())
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

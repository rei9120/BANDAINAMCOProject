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

    private Rigidbody aimLegionRig;

    private float speed = 10f;
    private float distance = 0.03f;

    private bool moveFlag = false;
    private bool jumpFlag = false;
    private bool legionFlag = false;
    private bool arrivalFlag = false;

    int layer = 0;

    public enum LegionType
    {
        Individual,  // 個々
        Gather,      // 集合中
        Legion,      // 軍団
        StandBy,     // 待機中
        Chase,       // 追跡中
    }
    private LegionType legionType;  // Legionの状態

    public enum Item
    {
        none,
        r,
        normal,
    };
    private Item itemType;

    public void Awake()
    {
        layer = LayerMask.NameToLayer("Legion");
    }

    public void Init(GameObject p, GameObject le,　Vector3 pos)
    {
        pTf = p.transform;
        pRb = p.GetComponent<Rigidbody>();
        pointScript = p.GetComponent<PointManager>();
        tf = this.transform;
        rig = this.GetComponent<Rigidbody>();
        rig.position = pos + new Vector3(0.0f, 1f, -2.0f);
        legionScript = le.GetComponent<LegionManager>();
        anim = this.GetComponent<Animator>();
        legionType = LegionType.Individual;
    }

    public void ManagedUpdate(Vector3 targetPos, Vector3 anchorPos, float deltaTime)
    {
        // 必要な値を更新または初期化する
        UpdateValue();

        if (jumpFlag)
        {
            rig.AddForce(jumpForce, ForceMode.Impulse);
        }

        Move(targetPos, anchorPos, deltaTime);

        Animation();
    }

    private void Move(Vector3 tPos, Vector3 aPos, float deltaTime)
    {
        switch (legionType)
        {
            case LegionType.Individual:
                MoveFollowPlayer(deltaTime);
                break;
            case LegionType.Gather:
                Physics.IgnoreLayerCollision(layer, layer, true);
                MoveGatherLine(tPos, deltaTime);
                break;
            case LegionType.Legion:
                Physics.IgnoreLayerCollision(layer, layer, false);
                MoveFollowLegion(aPos, deltaTime);
                break;
            case LegionType.Chase:
                Physics.IgnoreLayerCollision(layer, layer, true);
                MoveChaseLegion(tPos, deltaTime);
                break;
            case LegionType.StandBy:
                break;
        }

        if (moveFlag)
        {
            rig.position += velocity;
        }
    }

    private void MoveFollowPlayer(float deltaTime)
    {
        float dis = CompareTheDistanceYouAndOther(pTf.position);
        tf.LookAt(pTf);
        Quaternion rota = tf.localRotation;
        rota.x = 0.0f;
        rota.z = 0.0f;
        tf.localRotation = rota;
        if (dis < distance)
        {
            arrivalFlag = true;
        }
        else
        {
            Vector3 forward = tf.forward;
            forward.y = 0.0f;
            velocity = forward * speed * deltaTime;
            arrivalFlag = false;
        }
    }

    private void MoveFollowLegion(Vector3 aPos, float deltaTime)
    {
        float dis = CompareTheDistanceYouAndOther(pTf.position);
        tf.localRotation = Quaternion.LookRotation(pTf.position - aPos);
        Quaternion rota = tf.localRotation;
        rota.x = 0.0f;
        rota.z = 0.0f;
        tf.localRotation = rota;
        if (dis < distance)
        {
            arrivalFlag = true;
        }
        else
        {
            Vector3 forward = tf.forward;
            forward.y = 0.0f;
            velocity = forward * speed * deltaTime;
            arrivalFlag = false;
        }
    }

    private void MoveGatherLine(Vector3 tPos, float deltaTime)
    {
        float dis = CompareTheDistanceYouAndOther(tPos);
        tf.LookAt(tPos);
        Quaternion rota = tf.localRotation;
        rota.x = 0.0f;
        rota.z = 0.0f;
        tf.localRotation = rota;
        if (dis < distance)
        {
            arrivalFlag = true;
            if (legionScript.GetAllLegionFlag())
            {
                legionType = LegionType.Legion;
            }
            else
            {
                legionType = LegionType.StandBy;  // 待機中に変更
            }
        }
        else
        {
            Vector3 forward = tf.forward;
            forward.y = 0.0f;
            velocity = forward * speed * deltaTime;
            arrivalFlag = false;
        }
    }

    private void MoveChaseLegion(Vector3 tPos, float deltaTime)
    {
        float dis = CompareTheDistanceYouAndOther(tPos);
        tf.LookAt(tPos);
        Quaternion rota = tf.localRotation;
        rota.x = 0.0f;
        rota.z = 0.0f;
        tf.localRotation = rota;
        if (dis < distance)
        {
            arrivalFlag = true;
            legionType = LegionType.Legion;
        }
        else
        {
            Vector3 forward = tf.forward;
            forward.y = 0.0f;
            velocity = forward * (speed * 2f) * deltaTime;
            arrivalFlag = false;
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
        jumpFlag = pointScript.GetJumpFlag();
        velocity = Vector3.zero;
    }

    public Item FindItem()
    {
        return itemType;
    }

    public void SetLegionType(LegionType type)
    {
        legionType = type;
    }

    public LegionType GetLegionType()
    {
        return legionType;
    }

    public void SetItemType(Item type)
    {
        itemType = type;
    }

    public bool GetMoveFlag()
    {
        return moveFlag;
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
        if (this != null)
        {
            return rig.position;
        }
        return Vector3.zero;
    }
}

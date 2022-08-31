using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegionManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> legionPrefab;
    private GameObject point;
    private GameObject lineRenderer;
    private PointManager pointScript;
    private MouseLineRenderer lineScript;

    private Rigidbody pRig;

    private List<Legion> legion;
    private Legion leftSideStartLegion;
    private Legion leftSidelegion;
    private Legion rightSidelegion;
    private Rigidbody lSRb;
    private Rigidbody lRb;
    private Rigidbody rRb;
    private int GyaarKunNo;

    private float distance = 0.1f;
    private float pDistance = 1.5f;
    private float lWidthDistance = 2.0f;
    private float lHeightDistance = 2.0f;

    private bool moveFlag = false;
    private bool setLineFlag = false;
    private bool arrivalFlag = false;
    private bool legionFlag = false;
    private bool sideFlag = false;
    private bool allLegionFlag = false;
    private bool startLegionFlag = false;

    /// <summary>
    /// ������(GameSceneManager�ŌĂ�ł���)
    /// </summary>
    /// <param name="p">Point�I�u�W�F�N�g</param>
    /// <param name="l">LineRenderer�I�u�W�F�N�g</param>
    public void Init(GameObject p, GameObject l)
    {
        // Point�I�u�W�F�N�g
        point = p;
        pointScript = point.GetComponent<PointManager>();
        pRig = point.GetComponent<Rigidbody>();
        // LineRenderer�I�u�W�F�N�g
        lineRenderer = l;
        lineScript = lineRenderer.GetComponent<MouseLineRenderer>();
        // Legion�I�u�W�F�N�g
        legion = new List<Legion>();
        GyaarKunNo = 0;
        CreateLegion(3);
        leftSideStartLegion = legion[0];
        leftSidelegion = legion[0];
        rightSidelegion = legion[0];
        lSRb = leftSideStartLegion.GetComponent<Rigidbody>();
        lRb = leftSidelegion.GetComponent<Rigidbody>();
        rRb = rightSidelegion.GetComponent<Rigidbody>();
    }

    /// <summary>
    /// �X�V����(GameSceneManager�ɌĂ΂��)
    /// </summary>
    public void ManagedUpdate()
    {
        UpdateValue();

        bool flag = CheckAllLegionFlag();

        // Legion�I�u�W�F�N�g�̍s�����X�V����
        for (int i = 0; i < legion.Count; i++)
        {
            int num = (int)legion[i].FindItem();
            // �N�����A�C�e�����E���Ă����炻�̐����M���[�N�𐶐�
            if (num > 0)
            {
                CreateLegion(num);
                legion[i].SetItemType(Legion.Item.none);  // �A�C�e�����E���Ă��Ȃ���Ԃɖ߂�
            }

            if (legion[i] != null)
            {
                // �s���p�^�[�������߂�
                legion[i].SetAllLegionFlag(flag);
                legionFlag = legion[i].GetLegionFlag();
                legion[i].SetMoveFlag(moveFlag);
                Vector3 tPos = DecideLegionMove(legion[i], i);
                legion[i].ManagedUpdate(tPos);
            }
        }

        if(Input.GetKeyDown(KeyCode.Backspace))
        {
            LegionDestroy(legion[0]);
        }
    }

    /// <summary>
    /// Legion�I�u�W�F�N�g�̍s���p�^�[�������߂鏈��
    /// </summary>
    /// <param name="le">legion�I�u�W�F�N�g</param>
    /// <param name="leNo">����Ă���legion�I�u�W�F�N�g�̔ԍ�</param>
    /// <returns>�ړ�����ʒu</returns>
    private Vector3 DecideLegionMove(Legion le, int leNo)
    {
        Vector3 tPos = Vector3.zero;
        // ���C����������Ă��邩
        switch(setLineFlag)
        {
            case true:
                tPos = LineMove(le, leNo);  // ���C���ɑ΂��Ă̍s�����s��
                break;
            case false:
                sideFlag = false;
                le.SetLegionFlag(false);
                if (moveFlag)
                {
                    tPos = FollowPlayer(le);  // �v���C���[�ɂ��Ă���
                }
                break;
        }
        le.SetArrivalFlag(arrivalFlag);
        return tPos;
    }

    /// <summary>
    /// Player�ɂ��Ă�������
    /// </summary>
    /// <param name="le">legion�I�u�W�F�N�g</param>
    /// <returns>�ړ�����ʒu</returns>
    private Vector3 FollowPlayer(Legion le)
    {
        float dis = le.CompareTheDistanceYouAndOther(pRig.position);
        if (dis < pDistance)
        {
            arrivalFlag = true;
            return le.GetLegionPosition();
        }
        return pRig.position;
    }

    /// <summary>
    /// ���C���ɑ΂��Ăǂ������������߂�
    /// </summary>
    /// <param name="le"></param>
    /// <param name="leNo"></param>
    /// <returns></returns>
    private Vector3 LineMove(Legion le, int leNo)
    {
        // ����o���Ă��邩
        if(legionFlag)
        {
            if (moveFlag)
            {
                return FollowLine();
            }
            arrivalFlag = true;

            return Vector3.zero;
        }
        else
        {
            return LineFormation(le, leNo);
        }
    }

    /// <summary>
    /// ���񂵂Ȃ���Player�ɂ��Ă�������
    /// </summary>
    /// <returns>�ړ�����ʒu</returns>
    private Vector3 FollowLine()
    {
        float dis = legion[0].CompareTheDistanceYouAndOther(pRig.position);
        Vector3 pos = new Vector3(Mathf.Lerp(lSRb.position.x, lSRb.position.x, 0.5f)
                                           , pRig.position.y
                                           , Mathf.Lerp(lRb.position.z, rRb.position.z, 0.5f));
        if(dis < pDistance)
        {
            arrivalFlag = true;
            return pos;
        }
        return pos;
    }

    /// <summary>
    /// �����ꂽ���C���ɑ��񂷂鏈��
    /// </summary>
    /// <param name="le">legion�I�u�W�F�N�g</param>
    /// <param name="leNo">������Ă���legion�I�u�W�F�N�g�̔ԍ�</param>
    /// <returns>�ړ�����ʒu</returns>
    private Vector3 LineFormation(Legion le, int leNo)
    {
        Vector3 pos = le.GetLegionPosition();
        Vector3 sPos = lineScript.GetStartLinePos();
        Vector3 ePos = lineScript.GetEndLinePos();
        
        // legion�I�u�W�F�N�g���擪�����̑��ŏ�����ς���
        switch (leNo)
        {
            case 0:  // �擪��������
                // �͈͐��̒���������
                if (sPos.x <= pos.x && ePos.x >= pos.x)
                {
                    return MoveInLine(le, pos, sPos, ePos);
                }
                else  // �͈͐��̊O��������
                {
                    // �͈͂̐^�񒆂𓾂�
                    Vector3 dPos = Vector3.Lerp(sPos, ePos, 0.5f);
                    dPos.y = sPos.y;
                    return MoveOutLine(le, pos, dPos);
                }
            default:  // ����ȊO��������
                Vector3 tPos = legion[leNo - 1].GetLegionPosition();  // �ЂƂO��legion�I�u�W�F�N�g�ʒu�����炤
                tPos.x += lWidthDistance;  // ���̉��Ɉʒu�Â���
                return AnotherLegionFormation(le, tPos, MaxValue(sPos.x, ePos.x), leNo);
        }
    }

    /// <summary>
    /// ���C���ɓ��������̏���
    /// </summary>
    /// <param name="le">legion�I�u�W�F�N�g</param>
    /// <param name="pos">������Ă���legion�I�u�W�F�N�g�̈ʒu</param>
    /// <param name="sPos">���C���̈����n�߂��ʒu</param>
    /// <param name="ePos">���C���̈����I������ʒu</param>
    /// <returns>�ړ�����ʒu</returns>
    private Vector3 MoveInLine(Legion le, Vector3 pos, Vector3 sPos, Vector3 ePos)
    {
        // ���������C���̍���𓾂�
        Vector3 targetPos = new Vector3(MinValue(sPos.x, ePos.x), pos.y, MaxValue(sPos.z, ePos.z));

        // ���C���̍���ɂ��ǂ蒅������
        float dis = le.CompareTheDistanceYouAndOther(targetPos);
        if (dis < distance)
        {
            arrivalFlag = true;
            le.SetLegionFlag(true);
            return targetPos;
        }
        else
        {
            return targetPos;
        }
    }

    /// <summary>
    /// ���C���̊O�ɂ���Ƃ��̏���
    /// </summary>
    /// <param name="le">legion�I�u�W�F�N�g</param>
    /// <param name="pos">������Ă���legion�I�u�W�F�N�g�̈ʒu</param>
    /// <param name="dPos">���C���̐^�񒆂̈ʒu</param>
    /// <returns>�ړ�����ʒu</returns>
    private Vector3 MoveOutLine(Legion le, Vector3 pos, Vector3 dPos)
    {
        // ���C���̐^�񒆂ɂ��ǂ蒅������
        float dis = le.CompareTheDistanceYouAndOther(dPos);
        if (dis <= 0)
        {
            return pos;
        }

        return dPos;
    }

    /// <summary>
    /// �擪�ȊO��legion�I�u�W�F�N�g�̍s������
    /// </summary>
    /// <param name="le">legion�I�u�W�F�N�g</param>
    /// <param name="tPos">�������ЂƂO��legion�I�u�W�F�N�g�̈ʒu�̉�</param>
    /// <param name="overPos">���C���̉E�ʒu</param>
    /// <param name="leNo">������Ă���legion�I�u�W�F�N�g�̔ԍ�</param>
    /// <returns>�ړ�����ʒu</returns>
    private Vector3 AnotherLegionFormation(Legion le, Vector3 tPos, float overPos, int leNo)
    {
        // ���^�[�Q�b�g�ɂ��Ă���ʒu�����C���̉E���𒴂��Ă�����
        if (tPos.x > overPos)
        {
            if (rightSidelegion != legion[leNo - 1])
            {
                rightSidelegion = legion[leNo - 1];
                rRb = rightSidelegion.GetComponent<Rigidbody>();
            }
            // �ł�������legion�I�u�W�F�N�g�̌��Ƀ^�[�Q�b�g�̈ʒu��ύX
            tPos = leftSidelegion.GetLegionPosition();
            tPos.z -= lHeightDistance;
        }

        // �^�[�Q�b�g�̈ʒu�ɂ��ǂ蒅������
        float dis = le.CompareTheDistanceYouAndOther(tPos);
        if (dis < distance)
        {
            if (!sideFlag)
            {
                sideFlag = true;
                leftSidelegion = le;
                lRb = leftSidelegion.GetComponent<Rigidbody>();
            }
            arrivalFlag = true;
            le.SetLegionFlag(true);
        }
        return tPos;
    }

    /// <summary>
    /// �X�V�����̍ŏ��ɌĂԕK�v�Ȓl�̏���������
    /// </summary>
    private void UpdateValue()
    {
        moveFlag = false;
        setLineFlag = false;
        legionFlag = false;
        arrivalFlag = false;
        allLegionFlag = false;
        moveFlag = pointScript.GetMoveFlag();
        setLineFlag = lineScript.GetSetLineFlag();
    }

    /// <summary>
    /// legion�I�u�W�F�N�g�𐔕��������鏈��
    /// </summary>
    /// <param name="num">�������Ăق�����</param>
    public void CreateLegion(int num)
    {
        for (int i = 0; i < num; i++)
        {
            // prefab����I�u�W�F�N�g�𐶐�����
            GameObject obj = Instantiate(legionPrefab[GyaarKunNo]);
            // ���������I�u�W�F�N�g����Legion�X�N���v�g�̏������炤
            legion.Add(obj.GetComponent<Legion>());
            if (legion.Count == 1)  // ��̖ڂ�������
            {
                legion[legion.Count - 1].Init(point,  this.gameObject, point.transform.position);
            }
            else  // ����ȊO��������
            {
                legion[legion.Count - 1].Init(point, this.gameObject, legion[legion.Count - 2].transform.position);
            }
            legion[legion.Count - 1].name = "legion" + (legion.Count - 1);  // ���O��ύX
            GyaarKunNo++;
            if (GyaarKunNo > 5)  // �Ăяo��prefab�̎�ނ�ς���
            {
                GyaarKunNo = 0;
            }
        }
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

    public void LegionDestroy(Legion le)
    {
        Destroy(le.gameObject);
        Destroy(le);
        legion.Remove(le);
    }

    public bool CheckAllLegionFlag()
    {
        for (int i = 0; i < legion.Count; i++)
        {
            if (!legion[i].GetLegionFlag())
            {
                break;
            }
        }
        startLegionFlag = true;

        return true;
    }
    
    public bool GetStartLegionFlag()
    {
        return startLegionFlag;
    }

    public Legion GetStartLegionPtr()
    {
        if (legion.Count > 0)
        {
            for (int i = 0; i < legion.Count; i++)
            {
                if (legion[i] != null)
                {
                    return legion[i];
                }
            }
            return null;
        }
        else
        {
            return null;
        }
    }

    public bool CheckCanMove()
    {
        if(!allLegionFlag && !startLegionFlag)
        {
            moveFlag = false;
        }

        return moveFlag;
    }
}

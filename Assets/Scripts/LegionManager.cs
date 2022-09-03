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

    private Vector3 aimPos = Vector3.zero;
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
        CreateLegion(5);
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
    public void ManagedUpdate(float deltaTime)
    {
        UpdateValue();

        if (setLineFlag)
        {
            allLegionFlag = CheckAllLegionFlag();
        }
        else
        {
            ReleaseLegion();
        }

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
                legionFlag = legion[i].GetLegionFlag();
                legion[i].SetMoveFlag(CheckMoveFlag());
                DecideLegionMove(legion[i], i);
                legion[i].ManagedUpdate(aimPos, deltaTime);
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
    private void DecideLegionMove(Legion le, int leNo)
    {
        // ���C����������Ă��邩
        switch(setLineFlag)
        {
            case true:
                LineMove(le, leNo);  // ���C���ɑ΂��Ă̍s�����s��
                break;
            case false:
                sideFlag = false;
                FollowPlayer(le);  // �v���C���[�ɂ��Ă���
                break;
        }
        le.SetArrivalFlag(arrivalFlag);
    }

    /// <summary>
    /// Player�ɂ��Ă�������
    /// </summary>
    /// <param name="le">legion�I�u�W�F�N�g</param>
    /// <returns>�ړ�����ʒu</returns>
    private void FollowPlayer(Legion le)
    {
        float dis = le.CompareTheDistanceYouAndOther(pRig.position);
        if (dis < pDistance)
        {
            arrivalFlag = true;
            aimPos = le.GetLegionPosition();
        }
        aimPos = pRig.position;
    }

    /// <summary>
    /// ���C���ɑ΂��Ăǂ������������߂�
    /// </summary>
    /// <param name="le"></param>
    /// <param name="leNo"></param>
    /// <returns></returns>
    private void LineMove(Legion le, int leNo)
    {
        // ����o���Ă��邩
        if(legionFlag)
        {
            FollowLine();
            arrivalFlag = true;
        }
        else
        {
            LineFormation(le, leNo);
        }
    }

    /// <summary>
    /// ���񂵂Ȃ���Player�ɂ��Ă�������
    /// </summary>
    /// <returns>�ړ�����ʒu</returns>
    private void FollowLine()
    {
        float dis = legion[0].CompareTheDistanceYouAndOther(pRig.position);
        aimPos = new Vector3(Mathf.Lerp(lSRb.position.x, lSRb.position.x, 0.5f)
                                           , pRig.position.y
                                           , Mathf.Lerp(lRb.position.z, rRb.position.z, 0.5f));
        if (dis < pDistance)
        {
            arrivalFlag = true;
        }
    }

    /// <summary>
    /// �����ꂽ���C���ɑ��񂷂鏈��
    /// </summary>
    /// <param name="le">legion�I�u�W�F�N�g</param>
    /// <param name="leNo">������Ă���legion�I�u�W�F�N�g�̔ԍ�</param>
    /// <returns>�ړ�����ʒu</returns>
    private void LineFormation(Legion le, int leNo)
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
                    MoveInLine(le, pos, sPos, ePos);
                }
                else  // �͈͐��̊O��������
                {
                    // �͈͂̐^�񒆂𓾂�
                    Vector3 dPos = Vector3.Lerp(sPos, ePos, 0.5f);
                    dPos.y = sPos.y;
                    MoveOutLine(le, pos, dPos);
                }
                break;
            default:  // ����ȊO��������
                Vector3 tPos = legion[leNo - 1].GetLegionPosition();  // �ЂƂO��legion�I�u�W�F�N�g�ʒu�����炤
                tPos.x += lWidthDistance;  // ���̉��Ɉʒu�Â���
                AnotherLegionFormation(le, tPos, MaxValue(sPos.x, ePos.x), leNo);
                break;
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
    private void MoveInLine(Legion le, Vector3 pos, Vector3 sPos, Vector3 ePos)
    {
        // ���������C���̍���𓾂�
        aimPos = new Vector3(MinValue(sPos.x, ePos.x), pos.y, MaxValue(sPos.z, ePos.z));

        // ���C���̍���ɂ��ǂ蒅������
        float dis = le.CompareTheDistanceYouAndOther(aimPos);
        if (dis < distance)
        {
            arrivalFlag = true;
            le.SetLegionFlag(true);
        }
    }

    /// <summary>
    /// ���C���̊O�ɂ���Ƃ��̏���
    /// </summary>
    /// <param name="le">legion�I�u�W�F�N�g</param>
    /// <param name="pos">������Ă���legion�I�u�W�F�N�g�̈ʒu</param>
    /// <param name="dPos">���C���̐^�񒆂̈ʒu</param>
    /// <returns>�ړ�����ʒu</returns>
    private void MoveOutLine(Legion le, Vector3 pos, Vector3 dPos)
    {
        // ���C���̐^�񒆂ɂ��ǂ蒅������
        float dis = le.CompareTheDistanceYouAndOther(dPos);
        if (dis <= 0)
        {
            aimPos = pos;
        }

        aimPos = dPos;
    }

    /// <summary>
    /// �擪�ȊO��legion�I�u�W�F�N�g�̍s������
    /// </summary>
    /// <param name="le">legion�I�u�W�F�N�g</param>
    /// <param name="tPos">�������ЂƂO��legion�I�u�W�F�N�g�̈ʒu�̉�</param>
    /// <param name="overPos">���C���̉E�ʒu</param>
    /// <param name="leNo">������Ă���legion�I�u�W�F�N�g�̔ԍ�</param>
    /// <returns>�ړ�����ʒu</returns>
    private void AnotherLegionFormation(Legion le, Vector3 tPos, float overPos, int leNo)
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
            if (legion[leNo - 1].GetLegionFlag())
            {
                le.SetLegionFlag(true);
                arrivalFlag = false;
            }
        }
        aimPos = tPos;
    }

    /// <summary>
    /// �X�V�����̍ŏ��ɌĂԕK�v�Ȓl�̏���������
    /// </summary>
    private void UpdateValue()
    {
        aimPos = Vector3.zero;
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

    public void LegionDestroy(Legion le)
    {
        Destroy(le.gameObject);
        Destroy(le);
        legion.Remove(le);
    }

    private void ReleaseLegion()
    {
        if (allLegionFlag)
        {
            for (int i = 0; i < legion.Count; i++)
            {
                if (legion[i].GetLegionFlag())
                {
                    legion[i].SetLegionFlag(false);
                }
            }
            allLegionFlag = false;
            startLegionFlag = false;
        }
    }

    private bool CheckMoveFlag()
    {
        bool flag = pointScript.GetMoveFlag();
        if(setLineFlag)
        {
            flag = true;
            pointScript.SetMoveFlag(flag);
            if (legionFlag && !allLegionFlag)
            {
                flag = false;
                pointScript.SetMoveFlag(flag);
            }
        }

        return flag;
    }

    private bool CheckAllLegionFlag()
    {
        for (int i = 0; i < legion.Count; i++)
        {
            if (!legion[i].GetLegionFlag())
            {
                return false;
            }
        }
        startLegionFlag = true;

        return true;
    }

    public bool GetMoveFlag()
    {
        return moveFlag;
    }

    public bool GetLineFlag()
    {
        return setLineFlag;
    }

    public bool GetAllLegionFlag()
    {
        return allLegionFlag;
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

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

    private List<Vector3> legionPos;
    private Vector3 middleLegionPos;
    private Vector3 aimPos = Vector3.zero;
    private float distance = 0.1f;
    private float pDistance = 1.5f;
    private float lineWidthDistance = 1.0f;
    private float lineHeightDistance = 1.0f;
    private float lWidthDistance = 2.0f;
    private float lHeightDistance = 2.0f;

    private int legionNum = 0;
    private int rightNo = 0;

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
        legionPos = new List<Vector3>();
        GyaarKunNo = 0;
        CreateLegion(1);
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
            if (legionPos.Count != 0)
            {
                Vector3 lPos = legion[0].GetLegionPosition();
                Vector3 rPos = legion[rightNo].GetLegionPosition();
                // ��ԍ���ƉE�ォ�炻�̐^�񒆂𓾂�
                middleLegionPos = new Vector3(Mathf.Lerp(lPos.x, rPos.x, 0.5f), lPos.y, Mathf.Lerp(lPos.z, rPos.z, 0.5f));
            }
        }
        else
        {
            ReleaseLegion();
            legionNum = 0;
            rightNo = 0;
        }

        Debug.Log(allLegionFlag);

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
                CheckLegionType(legion[i]);
                Vector3 pos = Vector3.zero;
                if(legionPos.Count != 0)
                {
                    pos = legionPos[i];
                }
                legion[i].ManagedUpdate(pos, middleLegionPos, deltaTime);
            }
        }

        if(Input.GetKeyDown(KeyCode.Backspace))
        {
            LegionDestroy(legion[0], 0);
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

    private void CheckLegionType(Legion le)
    {
        Legion.LegionType type = le.GetLegionType();
        switch(type)
        {
            case Legion.LegionType.Individual:  // �X
                if (lineScript.GetSetLineFlag())
                {
                    if (type == Legion.LegionType.Individual)
                    {
                        DecideLegionPosition();
                    }
                    le.SetMoveFlag(true);
                    le.SetLegionType(Legion.LegionType.Gather);
                }
                le.SetMoveFlag(pointScript.GetMoveFlag());
                break;
            case Legion.LegionType.Gather:  // �W����
                le.SetMoveFlag(true);  // �W�����Ȃ̂Ńv���C���[�ɂ�����炸�����悤�ɂ���
                break;
            case Legion.LegionType.Legion:  // �R�c
                le.SetMoveFlag(pointScript.GetMoveFlag());
                break;
            case Legion.LegionType.Chase:  // �ǐՒ�
                le.SetMoveFlag(true);  // �R�c�ɒǂ������Ƃ��Ă���̂œ����悤�ɂ���
                break;
            case Legion.LegionType.StandBy:  // �ҋ@��
                le.SetMoveFlag(false);
                if(allLegionFlag)
                {
                    le.SetLegionType(Legion.LegionType.Legion);
                    pointScript.SetMoveFlag(false);  // �R�c��������������ɓ����Ȃ��悤�ɂ���
                }
                break;
        }
    }

    private void DecideLegionPosition()
    {
        Vector3 sPos = lineScript.GetStartLinePos();
        Vector3 ePos = lineScript.GetEndLinePos();
        // ��ԍ���̃|�W�V����
        Vector3 leftUpPos = new Vector3(MinValue(sPos.x, ePos.x), legion[0].GetLegionPosition().y, MaxValue(sPos.z, ePos.z));
        // �E�̃��C��x�ʒu
        float rightLinePos = MaxValue(sPos.x, ePos.x);

        Vector3 leftLegionPos = Vector3.zero;

        if (legionPos.Count == 0)
        {
            // ��ԍ���Legion�̈ʒu(�����ł͐擪)
            leftLegionPos = new Vector3(leftUpPos.x + lineWidthDistance, leftUpPos.y, leftUpPos.z - lineHeightDistance);
            legionPos.Add(leftLegionPos);
        }

        Vector3 nextLeftPos = Vector3.zero;
        if (legionNum != 0)
        {
            nextLeftPos = legion[legionNum - 1].GetLegionPosition();
        }

        // �����n�߂ɐ��������Ɛ����ς���Ă�����
        if (legionNum != legion.Count)
        {
            // �擪�͌��܂����̂Ŏ���Legion����ʒu���߂��s��
            for (int i = 1; i < legion.Count - legionNum; i++)
            {
                // ���̈ʒu�͍���Legion�̗�
                Vector3 nextLegionPos = nextLeftPos + new Vector3(lWidthDistance, 0.0f, 0.0f);
                // ���Ԉʒu���E�̃��C���𒴂��Ă�����
                if (nextLegionPos.x > rightLinePos)
                {
                    // ��ԍ��ɂ���Legion�̌��ɕύX
                    nextLegionPos = leftLegionPos - new Vector3(0.0f, 0.0f, lHeightDistance);
                    // ���ԗ񂪕ς�����̂ň�ԍ��ɂ���Legion�����̗��Legion�ɕύX
                    leftLegionPos = nextLegionPos;
                    if(rightNo == 0)
                    {
                        rightNo = i - 1;
                    }
                }
                nextLeftPos = nextLegionPos;
                legionPos.Add(nextLegionPos);
            }
            legionNum = legion.Count;  // �R�c��������Ƃ��ɐ��𐔂��Ă���

            Vector3 lPos = legionPos[0];
            Vector3 rPos = legionPos[rightNo];
            // ��ԍ���ƉE�ォ�炻�̐^�񒆂𓾂�
            middleLegionPos = new Vector3(Mathf.Lerp(lPos.x, rPos.x, 0.5f), lPos.y, Mathf.Lerp(lPos.z, rPos.z, 0.5f));
        }
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

        Legion.LegionType type = legion[0].GetLegionType();
        if (type != Legion.LegionType.Individual)
        {
            DecideLegionPosition();
        }
    }

    public void LegionDestroy(Legion le, int leNo)
    {
        Destroy(le.gameObject);
        Destroy(le);
        legion.Remove(le);
        legionPos.RemoveAt(leNo);
    }

    private void ReleaseLegion()
    {
        if (allLegionFlag)
        {
            for (int i = 0; i < legion.Count; i++)
            { 
                legion[i].SetLegionType(Legion.LegionType.Individual);
            }
            allLegionFlag = false;
        }

        if(legionPos.Count != 0)
        {
            legionPos.Clear();
            middleLegionPos = Vector3.zero;
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
        if (legion[0].GetLegionType() != Legion.LegionType.Legion)
        {
            for (int i = 0; i < legion.Count; i++)
            {
                // �ҋ@���ɂȂ��Ă��邩�A�ǐՒ�����Ȃ���ΌR�c�ɂȂ��Ă��Ȃ�
                if (legion[i].GetLegionType() == Legion.LegionType.Gather)
                {
                    return false;
                }
            }
        }
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

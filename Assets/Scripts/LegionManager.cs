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
    private Vector3 middleLegionPos = Vector3.zero;
    private Vector3 leftLegionPos = Vector3.zero;
    private float overLinePos = 0.0f;
    private float rightLineDis = 0.0f;
    private float lineWidthDistance = 0.5f;
    private float lineHeightDistance = 0.5f;
    private float lWidthDistance = 0.1f;
    private float lHeightDistance = 0.1f;

    private int legionNum = 0;
    private int rightNo = -1;
    private int leftNo = 0;
    private int chaseleftNo = 0;

    private bool moveFlag = false;
    private bool setLineFlag = false;
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
            if (rightNo != -1)
            {
                Vector3 lPos = legion[0].GetLegionPosition();
                Vector3 rPos = legion[rightNo].GetLegionPosition();
                // ��ԍ���ƉE�ォ�炻�̐^�񒆂𓾂�
                middleLegionPos = new Vector3(Mathf.Lerp(lPos.x, rPos.x, 0.5f), lPos.y, Mathf.Lerp(lPos.z, rPos.z, 0.5f));
            }
            else
            {
                Vector3 lPos = legion[0].GetLegionPosition();
                Vector3 rPos = legion[legion.Count - 1].GetLegionPosition();
                // ��ԍ���ƉE�ォ�炻�̐^�񒆂𓾂�
                middleLegionPos = new Vector3(Mathf.Lerp(lPos.x, rPos.x, 0.5f), lPos.y, Mathf.Lerp(lPos.z, rPos.z, 0.5f));
            }
        }
        else
        {
            if (allLegionFlag)
            {
                ReleaseLegion();
                legionNum = 0;
                rightNo = -1;
                middleLegionPos = Vector3.zero;
                leftLegionPos = Vector3.zero;
                rightLineDis = 0.0f;
                overLinePos = 0.0f;
            }
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
                CheckLegionType(legion[i], i);
                Vector3 pos = Vector3.zero;
                legionNum = legion.Count;
               
                if (legionPos.Count != 0)
                {
                    pos = legionPos[i];
                }
                legion[i].ManagedUpdate(pos, middleLegionPos, deltaTime);
            }
        }

        if (legionPos.Count != 0)
        {
            overLinePos = legion[0].GetLegionPosition().x + rightLineDis;
        }

        if (setLineFlag && legion[legion.Count - 1].GetLegionType() == Legion.LegionType.Chase)
        {
            chaseleftNo = leftNo;
        }
        else if(setLineFlag && legion[legion.Count - 1].GetLegionType() != Legion.LegionType.Chase)
        {
            leftNo = chaseleftNo;
        }

        if(Input.GetKeyDown(KeyCode.Backspace))
        {
            LegionDestroy(legion[0], 0);
        }
    }

    private void CheckLegionType(Legion le, int leNo)
    {
        Legion.LegionType type = le.GetLegionType();
        switch(type)
        {
            case Legion.LegionType.Individual:  // �X
                if (lineScript.GetSetLineFlag())
                {
                    DecideGatherLegionPosition(leNo);
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
                DecideChaseLegionPosition(leNo);
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

    private void DecideGatherLegionPosition(int leNo)
    {
        Vector3 sPos = lineScript.GetStartLinePos();
        Vector3 ePos = lineScript.GetEndLinePos();
        // ��ԍ���̃|�W�V����
        Vector3 leftUpPos = new Vector3(MinValue(sPos.x, ePos.x), legion[0].GetLegionPosition().y, MaxValue(sPos.z, ePos.z));
        // �E�̃��C��x�ʒu
        float rightLinePos = MaxValue(sPos.x, ePos.x);

        // �擪�̈ʒu����
        if (leNo == 0)
        {
            // ��ԍ���Legion�̈ʒu(�����ł͐擪)
            leftLegionPos = new Vector3(leftUpPos.x + lineWidthDistance, leftUpPos.y, leftUpPos.z - lineHeightDistance);
            rightLineDis = rightLinePos - leftLegionPos.x;
            legionPos.Add(leftLegionPos);
            leftNo = -1;
            overLinePos = leftLegionPos.x + rightLineDis;
        }
        else
        {
            Vector3 nextLeftPos = legionPos[leNo - 1];

            // ���̈ʒu�͍���Legion�̗�
            Vector3 nextLegionPos = nextLeftPos + new Vector3(lWidthDistance, 0.0f, 0.0f);
            // ���Ԉʒu���E�̃��C���𒴂��Ă�����
            if (nextLegionPos.x > overLinePos - lineWidthDistance)
            {
                // ��ԍ��ɂ���Legion�̌��ɕύX
                nextLegionPos = leftLegionPos - new Vector3(0.0f, 0.0f, lHeightDistance);
                // ���ԗ񂪕ς�����̂ň�ԍ��ɂ���Legion�����̗��Legion�ɕύX
                leftLegionPos = nextLegionPos;
                if (rightNo == -1)
                {
                    rightNo = leNo - 1;
                }
                leftNo = leNo;
            }
            legionPos.Add(nextLegionPos);
        }

        chaseleftNo = leftNo;

        Vector3 lPos = legionPos[0];
        Vector3 rPos = Vector3.zero;
        if (rightNo != -1)
        {
            rPos = legionPos[rightNo];
        }
        else
        {
            rPos = legionPos[0];
        }
        // ��ԍ���ƉE�ォ�炻�̐^�񒆂𓾂�
        middleLegionPos = new Vector3(Mathf.Lerp(lPos.x, rPos.x, 0.5f), lPos.y, Mathf.Lerp(lPos.z, rPos.z, 0.5f));
    }

    private void DecideChaseLegionPosition(int leNo)
    {
        Vector3 nextLeftPos = Vector3.zero;
        if (legion[leNo - 1].GetLegionType() != Legion.LegionType.Chase)
        {
            nextLeftPos = legion[leNo - 1].GetLegionPosition();
        }
        else
        {
            nextLeftPos = legionPos[leNo - 1];
        }

        // ���̈ʒu�͍���Legion�̗�
        Vector3 nextLegionPos = nextLeftPos + new Vector3(lWidthDistance, 0.0f, 0.0f);

        // ���Ԉʒu���E�̃��C���𒴂��Ă�����
        if (nextLegionPos.x > overLinePos - lineWidthDistance)
        {
            // ������n�߂�O�̗�̂����Ƃ����̃L�����̈ʒu���Ƃ�
            if (chaseleftNo <= 0)
            {
                leftLegionPos = legion[0].GetLegionPosition();
            }
            else
            {
                leftLegionPos = legion[chaseleftNo].GetLegionPosition();
            }
            // ��ԍ��ɂ���Legion�̌��ɕύX
            nextLegionPos = leftLegionPos - new Vector3(0.0f, 0.0f, lHeightDistance);
            if (rightNo == -1)
            {
                rightNo = leNo - 1;
            }
            chaseleftNo = leNo;
        }

        legionPos[leNo] = nextLegionPos;
    }

    /// <summary>
    /// �X�V�����̍ŏ��ɌĂԕK�v�Ȓl�̏���������
    /// </summary>
    private void UpdateValue()
    {
        moveFlag = false;
        setLineFlag = false;
        moveFlag = pointScript.GetMoveFlag();
        setLineFlag = lineScript.GetSetLineFlag();
    }

    /// <summary>
    /// legion�I�u�W�F�N�g�𐔕��������鏈��
    /// </summary>
    /// <param name="num">�������Ăق�����</param>
    public void CreateLegion(int num)
    {
        Legion.LegionType type = Legion.LegionType.Individual;
        if (legion.Count != 0)
        {
            type = legion[0].GetLegionType();
        }

        int beforeCount = legion.Count;
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

            if (type != Legion.LegionType.Individual)
            {
                legion[beforeCount + i].SetLegionType(Legion.LegionType.Chase);
                legionPos.Add(legion[beforeCount + i].GetLegionPosition());
            }
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
        for (int i = 0; i < legion.Count; i++)
        {
            legion[i].SetLegionType(Legion.LegionType.Individual);
        }
        allLegionFlag = false;

        if(legionPos.Count != 0)
        {
            legionPos.Clear();
            middleLegionPos = Vector3.zero;
        }
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
                    chaseleftNo = leftNo;
                    return false;
                }
            }
        }
        leftNo = chaseleftNo;
        return true;
    }

    public void AddLegionPos(Vector3 pos)
    {
        legionPos.Add(pos);
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

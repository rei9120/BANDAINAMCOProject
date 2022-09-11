using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LegionManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> legionPrefab;
    private GameObject point;
    private GameObject lineRenderer;
    private PointManager pointScript;
    private MouseLineRenderer lineScript;

    private List<Legion> legion;
    private List<Legion> chaseLegion;
    private int GyaarKunNo;

    private List<Vector3> legionPos;
    private Vector3 middleLegionPos;
    private Vector3 leftLegionPos;
    private float overLinePos;
    private float rightLineDis;
    private float lineWidthDistance;
    private float lineHeightDistance;
    private float lWidthDistance;
    private float lHeightDistance;

    private int rightNo;
    private int leftNo;
    private int chaseleftNo;

    private bool moveFlag;
    private bool setLineFlag;
    private bool allLegionFlag;
    private bool startLegionFlag;

    [SerializeField] private int deleteNum;

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
        // LineRenderer�I�u�W�F�N�g
        lineRenderer = l;
        lineScript = lineRenderer.GetComponent<MouseLineRenderer>();
        // Legion�I�u�W�F�N�g
        legion = new List<Legion>();
        chaseLegion = new List<Legion>();
        legionPos = new List<Vector3>();
        GyaarKunNo = 0;
        CreateLegion(1);

        middleLegionPos = Vector3.zero;
        leftLegionPos = Vector3.zero;
        overLinePos = 1.0f;
        rightLineDis = 0.0f;
        lineWidthDistance = 0.5f;
        lineHeightDistance = 0.5f;
        lWidthDistance = 2.0f;
        lHeightDistance = 2.0f;

        rightNo = -1;
        leftNo = 0;
        chaseleftNo = 0;
        
        moveFlag = false;
        setLineFlag = false;
        allLegionFlag = false;
        startLegionFlag = false;
    }

    /// <summary>
    /// �X�V����(GameSceneManager�ɌĂ΂��)
    /// </summary>
    public void ManagedUpdate(float deltaTime)
    {
        UpdateValue();

        for (int i = 0; i < legion.Count; i++)
        {
            if (legion[i] == null)
            {
                LegionDestroy(legion[i], i);
            }
        }

        if (setLineFlag && legion.Count != 0)
        {
            allLegionFlag = CheckAllLegionFlag();
            if (rightNo > 0)  // �L�������E�̃��C���ɒB�����ς����Ƃ�
            {
                Vector3 lPos = legion[0].GetLegionPosition();  // �őO���̂����Ƃ����̃L����
                Vector3 rPos = legion[rightNo].GetLegionPosition();  // �őO���̂����Ƃ��E�̃L����
                // ��ԍ���ƉE�ォ�炻�̐^�񒆂𓾂�
                middleLegionPos = new Vector3(Mathf.Lerp(lPos.x, rPos.x, 0.5f), lPos.y, Mathf.Lerp(lPos.z, rPos.z, 0.5f));
            }
            else  // ���C���̉E�[�ɃL�������B���Ă��Ȃ��Ƃ�
            {
                Vector3 lPos = legion[0].GetLegionPosition();  // �őO���̂����Ƃ����̃L����
                Vector3 rPos = legion[legion.Count - 1].GetLegionPosition();  // �őO���̂����Ƃ��E�̃L����
                // ��ԍ���ƉE�ォ�炻�̐^�񒆂𓾂�
                middleLegionPos = new Vector3(Mathf.Lerp(lPos.x, rPos.x, 0.5f), lPos.y, Mathf.Lerp(lPos.z, rPos.z, 0.5f));
            }
        }
        else
        {
            if (legion.Count != 0)
            {
                if(allLegionFlag || legion[0].GetLegionType() != Legion.LegionType.Individual)
                ReleaseLegion();
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
                legion[i].SetItemType(Legion.Item.None);  // �A�C�e�����E���Ă��Ȃ���Ԃɖ߂�
            }

            if (legion[i] != null)
            {
                Vector3 pos = legion[i].GetLegionPosition();
                if (pos.y < -3.0f)
                {
                    LegionDestroy(legion[i], i);
                    continue;
                }
                pos = Vector3.zero;
                CheckLegionType(legion[i], i);

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

        CheckEndChase();

        if(Input.GetKeyDown(KeyCode.RightArrow))
        {
            deleteNum++;
            if(deleteNum > legion.Count - 1)
            {
                deleteNum = 0;
            }
        }
        else if(Input.GetKeyDown(KeyCode.LeftArrow))
        {
            deleteNum--;
            if(deleteNum < 0)
            {
                deleteNum = legion.Count - 1;
            }
        }

        if (Input.GetKeyDown(KeyCode.Backspace) && SceneManager.GetActiveScene().name == "Debug")
        {
            LegionDestroy(legion[deleteNum], deleteNum);
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
                chaseLegion.Add(legion[beforeCount + i]);
            }
        }
    }

    public void LegionDestroy(Legion le, int leNo)
    {
        if (le != null)
        {
            Destroy(le.gameObject);
            Destroy(le);
        }
        legion.Remove(le);
        if (legionPos.Count != 0)
        {
            legionPos.RemoveAt(leNo);
        }

        // �w�W�ɂ���E���̃L�����������Ă�����1���̃L����������
        if (rightNo == leNo)
        {
            rightNo -= 1;
        }
        else
        {
            rightNo--;
        }
        chaseleftNo--;
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

    private bool CheckEndChase()
    {
        for(int i = 0; i < chaseLegion.Count; i++)
        {
            if(chaseLegion[i].GetLegionType() == Legion.LegionType.Chase)
            {
                chaseleftNo = leftNo;
                return false;
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

    public Legion.LegionType GetLegionType()
    {
        if(legion.Count == 0)
        {
            return Legion.LegionType.StandBy;
        }
        return legion[0].GetLegionType();
    }

    public bool GetLegionMoveFlag()
    {
        if(legion.Count == 0)
        {
            return false;
        }
        return legion[0].GetMoveFlag();
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

    public int GetNowLegionNum()
    {
        return legion.Count;
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

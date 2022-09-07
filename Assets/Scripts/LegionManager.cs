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
    /// 初期化(GameSceneManagerで呼んでいる)
    /// </summary>
    /// <param name="p">Pointオブジェクト</param>
    /// <param name="l">LineRendererオブジェクト</param>
    public void Init(GameObject p, GameObject l)
    {
        // Pointオブジェクト
        point = p;
        pointScript = point.GetComponent<PointManager>();
        pRig = point.GetComponent<Rigidbody>();
        // LineRendererオブジェクト
        lineRenderer = l;
        lineScript = lineRenderer.GetComponent<MouseLineRenderer>();
        // Legionオブジェクト
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
    /// 更新処理(GameSceneManagerに呼ばれる)
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
                // 一番左上と右上からその真ん中を得る
                middleLegionPos = new Vector3(Mathf.Lerp(lPos.x, rPos.x, 0.5f), lPos.y, Mathf.Lerp(lPos.z, rPos.z, 0.5f));
            }
            else
            {
                Vector3 lPos = legion[0].GetLegionPosition();
                Vector3 rPos = legion[legion.Count - 1].GetLegionPosition();
                // 一番左上と右上からその真ん中を得る
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

        // Legionオブジェクトの行動を更新する
        for (int i = 0; i < legion.Count; i++)
        {
            int num = (int)legion[i].FindItem();
            // 誰かがアイテムを拾っていたらその数分ギャー君を生成
            if (num > 0)
            {
                CreateLegion(num);
                legion[i].SetItemType(Legion.Item.none);  // アイテムを拾っていない状態に戻す
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
            case Legion.LegionType.Individual:  // 個々
                if (lineScript.GetSetLineFlag())
                {
                    DecideGatherLegionPosition(leNo);
                    le.SetMoveFlag(true);
                    le.SetLegionType(Legion.LegionType.Gather);
                }
                le.SetMoveFlag(pointScript.GetMoveFlag());
                break;
            case Legion.LegionType.Gather:  // 集合中
                le.SetMoveFlag(true);  // 集合中なのでプレイヤーにかかわらず動くようにする
                break;
            case Legion.LegionType.Legion:  // 軍団
                le.SetMoveFlag(pointScript.GetMoveFlag());
                break;
            case Legion.LegionType.Chase:  // 追跡中
                DecideChaseLegionPosition(leNo);
                le.SetMoveFlag(true);  // 軍団に追いつこうとしているので動くようにする
                break;
            case Legion.LegionType.StandBy:  // 待機中
                le.SetMoveFlag(false);
                if(allLegionFlag)
                {
                    le.SetLegionType(Legion.LegionType.Legion);
                    pointScript.SetMoveFlag(false);  // 軍団が完成した直後に動かないようにする
                }
                break;
        }
    }

    private void DecideGatherLegionPosition(int leNo)
    {
        Vector3 sPos = lineScript.GetStartLinePos();
        Vector3 ePos = lineScript.GetEndLinePos();
        // 一番左上のポジション
        Vector3 leftUpPos = new Vector3(MinValue(sPos.x, ePos.x), legion[0].GetLegionPosition().y, MaxValue(sPos.z, ePos.z));
        // 右のラインx位置
        float rightLinePos = MaxValue(sPos.x, ePos.x);

        // 先頭の位置決め
        if (leNo == 0)
        {
            // 一番左のLegionの位置(ここでは先頭)
            leftLegionPos = new Vector3(leftUpPos.x + lineWidthDistance, leftUpPos.y, leftUpPos.z - lineHeightDistance);
            rightLineDis = rightLinePos - leftLegionPos.x;
            legionPos.Add(leftLegionPos);
            leftNo = -1;
            overLinePos = leftLegionPos.x + rightLineDis;
        }
        else
        {
            Vector3 nextLeftPos = legionPos[leNo - 1];

            // 次の位置は左のLegionの隣
            Vector3 nextLegionPos = nextLeftPos + new Vector3(lWidthDistance, 0.0f, 0.0f);
            // 並ぶ位置が右のラインを超えていたら
            if (nextLegionPos.x > overLinePos - lineWidthDistance)
            {
                // 一番左にいるLegionの後ろに変更
                nextLegionPos = leftLegionPos - new Vector3(0.0f, 0.0f, lHeightDistance);
                // 並ぶ列が変わったので一番左にいるLegionもその列のLegionに変更
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
        // 一番左上と右上からその真ん中を得る
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

        // 次の位置は左のLegionの隣
        Vector3 nextLegionPos = nextLeftPos + new Vector3(lWidthDistance, 0.0f, 0.0f);

        // 並ぶ位置が右のラインを超えていたら
        if (nextLegionPos.x > overLinePos - lineWidthDistance)
        {
            // 隊列を始める前の列のもっとも左のキャラの位置をとる
            if (chaseleftNo <= 0)
            {
                leftLegionPos = legion[0].GetLegionPosition();
            }
            else
            {
                leftLegionPos = legion[chaseleftNo].GetLegionPosition();
            }
            // 一番左にいるLegionの後ろに変更
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
    /// 更新処理の最初に呼ぶ必要な値の初期化処理
    /// </summary>
    private void UpdateValue()
    {
        moveFlag = false;
        setLineFlag = false;
        moveFlag = pointScript.GetMoveFlag();
        setLineFlag = lineScript.GetSetLineFlag();
    }

    /// <summary>
    /// legionオブジェクトを数分生成する処理
    /// </summary>
    /// <param name="num">生成してほしい数</param>
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
            // prefabからオブジェクトを生成する
            GameObject obj = Instantiate(legionPrefab[GyaarKunNo]);

            // 生成したオブジェクトからLegionスクリプトの情報をもらう
            legion.Add(obj.GetComponent<Legion>());
            if (legion.Count == 1)  // 一体目だったら
            {
                legion[legion.Count - 1].Init(point,  this.gameObject, point.transform.position);
            }
            else  // それ以外だったら
            {
                legion[legion.Count - 1].Init(point, this.gameObject, legion[legion.Count - 2].transform.position);
            }
            legion[legion.Count - 1].name = "legion" + (legion.Count - 1);  // 名前を変更
            GyaarKunNo++;
            if (GyaarKunNo > 5)  // 呼び出すprefabの種類を変える
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
                // 待機中になっているか、追跡中じゃなければ軍団になっていない
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

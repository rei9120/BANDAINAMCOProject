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
    /// 初期化(GameSceneManagerで呼んでいる)
    /// </summary>
    /// <param name="p">Pointオブジェクト</param>
    /// <param name="l">LineRendererオブジェクト</param>
    public void Init(GameObject p, GameObject l)
    {
        // Pointオブジェクト
        point = p;
        pointScript = point.GetComponent<PointManager>();
        // LineRendererオブジェクト
        lineRenderer = l;
        lineScript = lineRenderer.GetComponent<MouseLineRenderer>();
        // Legionオブジェクト
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
    /// 更新処理(GameSceneManagerに呼ばれる)
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
            if (rightNo > 0)  // キャラが右のラインに達し列を変えたとき
            {
                Vector3 lPos = legion[0].GetLegionPosition();  // 最前線のもっとも左のキャラ
                Vector3 rPos = legion[rightNo].GetLegionPosition();  // 最前線のもっとも右のキャラ
                // 一番左上と右上からその真ん中を得る
                middleLegionPos = new Vector3(Mathf.Lerp(lPos.x, rPos.x, 0.5f), lPos.y, Mathf.Lerp(lPos.z, rPos.z, 0.5f));
            }
            else  // ラインの右端にキャラが達していないとき
            {
                Vector3 lPos = legion[0].GetLegionPosition();  // 最前線のもっとも左のキャラ
                Vector3 rPos = legion[legion.Count - 1].GetLegionPosition();  // 最前線のもっとも右のキャラ
                // 一番左上と右上からその真ん中を得る
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

        // Legionオブジェクトの行動を更新する
        for (int i = 0; i < legion.Count; i++)
        {
            int num = (int)legion[i].FindItem();
            // 誰かがアイテムを拾っていたらその数分ギャー君を生成
            if (num > 0)
            {
                CreateLegion(num);
                legion[i].SetItemType(Legion.Item.None);  // アイテムを拾っていない状態に戻す
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

        // 指標にする右側のキャラが消えていたら1つ左のキャラを見る
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

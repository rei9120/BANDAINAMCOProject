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
    /// 更新処理(GameSceneManagerに呼ばれる)
    /// </summary>
    public void ManagedUpdate()
    {
        UpdateValue();

        bool flag = CheckAllLegionFlag();

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
                // 行動パターンを決める
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
    /// Legionオブジェクトの行動パターンを決める処理
    /// </summary>
    /// <param name="le">legionオブジェクト</param>
    /// <param name="leNo">貰ってきたlegionオブジェクトの番号</param>
    /// <returns>移動する位置</returns>
    private Vector3 DecideLegionMove(Legion le, int leNo)
    {
        Vector3 tPos = Vector3.zero;
        // ラインが引かれているか
        switch(setLineFlag)
        {
            case true:
                tPos = LineMove(le, leNo);  // ラインに対しての行動を行う
                break;
            case false:
                sideFlag = false;
                le.SetLegionFlag(false);
                if (moveFlag)
                {
                    tPos = FollowPlayer(le);  // プレイヤーについていく
                }
                break;
        }
        le.SetArrivalFlag(arrivalFlag);
        return tPos;
    }

    /// <summary>
    /// Playerについていく処理
    /// </summary>
    /// <param name="le">legionオブジェクト</param>
    /// <returns>移動する位置</returns>
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
    /// ラインに対してどう動くかを決める
    /// </summary>
    /// <param name="le"></param>
    /// <param name="leNo"></param>
    /// <returns></returns>
    private Vector3 LineMove(Legion le, int leNo)
    {
        // 隊列出来ているか
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
    /// 隊列しながらPlayerについていく処理
    /// </summary>
    /// <returns>移動する位置</returns>
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
    /// 引かれたラインに隊列する処理
    /// </summary>
    /// <param name="le">legionオブジェクト</param>
    /// <param name="leNo">もらってきたlegionオブジェクトの番号</param>
    /// <returns>移動する位置</returns>
    private Vector3 LineFormation(Legion le, int leNo)
    {
        Vector3 pos = le.GetLegionPosition();
        Vector3 sPos = lineScript.GetStartLinePos();
        Vector3 ePos = lineScript.GetEndLinePos();
        
        // legionオブジェクトが先頭かその他で処理を変える
        switch (leNo)
        {
            case 0:  // 先頭だったら
                // 範囲線の中だったら
                if (sPos.x <= pos.x && ePos.x >= pos.x)
                {
                    return MoveInLine(le, pos, sPos, ePos);
                }
                else  // 範囲線の外だったら
                {
                    // 範囲の真ん中を得る
                    Vector3 dPos = Vector3.Lerp(sPos, ePos, 0.5f);
                    dPos.y = sPos.y;
                    return MoveOutLine(le, pos, dPos);
                }
            default:  // それ以外だったら
                Vector3 tPos = legion[leNo - 1].GetLegionPosition();  // ひとつ前のlegionオブジェクト位置をもらう
                tPos.x += lWidthDistance;  // その横に位置づける
                return AnotherLegionFormation(le, tPos, MaxValue(sPos.x, ePos.x), leNo);
        }
    }

    /// <summary>
    /// ラインに入った時の処理
    /// </summary>
    /// <param name="le">legionオブジェクト</param>
    /// <param name="pos">もらってきたlegionオブジェクトの位置</param>
    /// <param name="sPos">ラインの引き始めた位置</param>
    /// <param name="ePos">ラインの引き終わった位置</param>
    /// <returns>移動する位置</returns>
    private Vector3 MoveInLine(Legion le, Vector3 pos, Vector3 sPos, Vector3 ePos)
    {
        // 引いたラインの左上を得る
        Vector3 targetPos = new Vector3(MinValue(sPos.x, ePos.x), pos.y, MaxValue(sPos.z, ePos.z));

        // ラインの左上にたどり着いたら
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
    /// ラインの外にいるときの処理
    /// </summary>
    /// <param name="le">legionオブジェクト</param>
    /// <param name="pos">もらってきたlegionオブジェクトの位置</param>
    /// <param name="dPos">ラインの真ん中の位置</param>
    /// <returns>移動する位置</returns>
    private Vector3 MoveOutLine(Legion le, Vector3 pos, Vector3 dPos)
    {
        // ラインの真ん中にたどり着いたら
        float dis = le.CompareTheDistanceYouAndOther(dPos);
        if (dis <= 0)
        {
            return pos;
        }

        return dPos;
    }

    /// <summary>
    /// 先頭以外のlegionオブジェクトの行動処理
    /// </summary>
    /// <param name="le">legionオブジェクト</param>
    /// <param name="tPos">自分よりひとつ前のlegionオブジェクトの位置の横</param>
    /// <param name="overPos">ラインの右位置</param>
    /// <param name="leNo">もらってきたlegionオブジェクトの番号</param>
    /// <returns>移動する位置</returns>
    private Vector3 AnotherLegionFormation(Legion le, Vector3 tPos, float overPos, int leNo)
    {
        // 今ターゲットにしている位置がラインの右側を超えていたら
        if (tPos.x > overPos)
        {
            if (rightSidelegion != legion[leNo - 1])
            {
                rightSidelegion = legion[leNo - 1];
                rRb = rightSidelegion.GetComponent<Rigidbody>();
            }
            // 最も左側のlegionオブジェクトの後ろにターゲットの位置を変更
            tPos = leftSidelegion.GetLegionPosition();
            tPos.z -= lHeightDistance;
        }

        // ターゲットの位置にたどり着いたら
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
    /// 更新処理の最初に呼ぶ必要な値の初期化処理
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
    /// legionオブジェクトを数分生成する処理
    /// </summary>
    /// <param name="num">生成してほしい数</param>
    public void CreateLegion(int num)
    {
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

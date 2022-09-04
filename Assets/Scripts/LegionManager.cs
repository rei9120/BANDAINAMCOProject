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
        CreateLegion(1);
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
            if (legionPos.Count != 0)
            {
                Vector3 lPos = legion[0].GetLegionPosition();
                Vector3 rPos = legion[rightNo].GetLegionPosition();
                // 一番左上と右上からその真ん中を得る
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
    /// Legionオブジェクトの行動パターンを決める処理
    /// </summary>
    /// <param name="le">legionオブジェクト</param>
    /// <param name="leNo">貰ってきたlegionオブジェクトの番号</param>
    /// <returns>移動する位置</returns>
    private void DecideLegionMove(Legion le, int leNo)
    {
        // ラインが引かれているか
        switch(setLineFlag)
        {
            case true:
                LineMove(le, leNo);  // ラインに対しての行動を行う
                break;
            case false:
                sideFlag = false;
                FollowPlayer(le);  // プレイヤーについていく
                break;
        }
        le.SetArrivalFlag(arrivalFlag);
    }

    private void CheckLegionType(Legion le)
    {
        Legion.LegionType type = le.GetLegionType();
        switch(type)
        {
            case Legion.LegionType.Individual:  // 個々
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
            case Legion.LegionType.Gather:  // 集合中
                le.SetMoveFlag(true);  // 集合中なのでプレイヤーにかかわらず動くようにする
                break;
            case Legion.LegionType.Legion:  // 軍団
                le.SetMoveFlag(pointScript.GetMoveFlag());
                break;
            case Legion.LegionType.Chase:  // 追跡中
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

    private void DecideLegionPosition()
    {
        Vector3 sPos = lineScript.GetStartLinePos();
        Vector3 ePos = lineScript.GetEndLinePos();
        // 一番左上のポジション
        Vector3 leftUpPos = new Vector3(MinValue(sPos.x, ePos.x), legion[0].GetLegionPosition().y, MaxValue(sPos.z, ePos.z));
        // 右のラインx位置
        float rightLinePos = MaxValue(sPos.x, ePos.x);

        Vector3 leftLegionPos = Vector3.zero;

        if (legionPos.Count == 0)
        {
            // 一番左のLegionの位置(ここでは先頭)
            leftLegionPos = new Vector3(leftUpPos.x + lineWidthDistance, leftUpPos.y, leftUpPos.z - lineHeightDistance);
            legionPos.Add(leftLegionPos);
        }

        Vector3 nextLeftPos = Vector3.zero;
        if (legionNum != 0)
        {
            nextLeftPos = legion[legionNum - 1].GetLegionPosition();
        }

        // もし始めに数えた時と数が変わっていたら
        if (legionNum != legion.Count)
        {
            // 先頭は決まったので次のLegionから位置決めを行う
            for (int i = 1; i < legion.Count - legionNum; i++)
            {
                // 次の位置は左のLegionの隣
                Vector3 nextLegionPos = nextLeftPos + new Vector3(lWidthDistance, 0.0f, 0.0f);
                // 並ぶ位置が右のラインを超えていたら
                if (nextLegionPos.x > rightLinePos)
                {
                    // 一番左にいるLegionの後ろに変更
                    nextLegionPos = leftLegionPos - new Vector3(0.0f, 0.0f, lHeightDistance);
                    // 並ぶ列が変わったので一番左にいるLegionもその列のLegionに変更
                    leftLegionPos = nextLegionPos;
                    if(rightNo == 0)
                    {
                        rightNo = i - 1;
                    }
                }
                nextLeftPos = nextLegionPos;
                legionPos.Add(nextLegionPos);
            }
            legionNum = legion.Count;  // 軍団を作ったときに数を数えておく

            Vector3 lPos = legionPos[0];
            Vector3 rPos = legionPos[rightNo];
            // 一番左上と右上からその真ん中を得る
            middleLegionPos = new Vector3(Mathf.Lerp(lPos.x, rPos.x, 0.5f), lPos.y, Mathf.Lerp(lPos.z, rPos.z, 0.5f));
        }
    }

    /// <summary>
    /// Playerについていく処理
    /// </summary>
    /// <param name="le">legionオブジェクト</param>
    /// <returns>移動する位置</returns>
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
    /// ラインに対してどう動くかを決める
    /// </summary>
    /// <param name="le"></param>
    /// <param name="leNo"></param>
    /// <returns></returns>
    private void LineMove(Legion le, int leNo)
    {
        // 隊列出来ているか
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
    /// 隊列しながらPlayerについていく処理
    /// </summary>
    /// <returns>移動する位置</returns>
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
    /// 引かれたラインに隊列する処理
    /// </summary>
    /// <param name="le">legionオブジェクト</param>
    /// <param name="leNo">もらってきたlegionオブジェクトの番号</param>
    /// <returns>移動する位置</returns>
    private void LineFormation(Legion le, int leNo)
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
                    MoveInLine(le, pos, sPos, ePos);
                }
                else  // 範囲線の外だったら
                {
                    // 範囲の真ん中を得る
                    Vector3 dPos = Vector3.Lerp(sPos, ePos, 0.5f);
                    dPos.y = sPos.y;
                    MoveOutLine(le, pos, dPos);
                }
                break;
            default:  // それ以外だったら
                Vector3 tPos = legion[leNo - 1].GetLegionPosition();  // ひとつ前のlegionオブジェクト位置をもらう
                tPos.x += lWidthDistance;  // その横に位置づける
                AnotherLegionFormation(le, tPos, MaxValue(sPos.x, ePos.x), leNo);
                break;
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
    private void MoveInLine(Legion le, Vector3 pos, Vector3 sPos, Vector3 ePos)
    {
        // 引いたラインの左上を得る
        aimPos = new Vector3(MinValue(sPos.x, ePos.x), pos.y, MaxValue(sPos.z, ePos.z));

        // ラインの左上にたどり着いたら
        float dis = le.CompareTheDistanceYouAndOther(aimPos);
        if (dis < distance)
        {
            arrivalFlag = true;
            le.SetLegionFlag(true);
        }
    }

    /// <summary>
    /// ラインの外にいるときの処理
    /// </summary>
    /// <param name="le">legionオブジェクト</param>
    /// <param name="pos">もらってきたlegionオブジェクトの位置</param>
    /// <param name="dPos">ラインの真ん中の位置</param>
    /// <returns>移動する位置</returns>
    private void MoveOutLine(Legion le, Vector3 pos, Vector3 dPos)
    {
        // ラインの真ん中にたどり着いたら
        float dis = le.CompareTheDistanceYouAndOther(dPos);
        if (dis <= 0)
        {
            aimPos = pos;
        }

        aimPos = dPos;
    }

    /// <summary>
    /// 先頭以外のlegionオブジェクトの行動処理
    /// </summary>
    /// <param name="le">legionオブジェクト</param>
    /// <param name="tPos">自分よりひとつ前のlegionオブジェクトの位置の横</param>
    /// <param name="overPos">ラインの右位置</param>
    /// <param name="leNo">もらってきたlegionオブジェクトの番号</param>
    /// <returns>移動する位置</returns>
    private void AnotherLegionFormation(Legion le, Vector3 tPos, float overPos, int leNo)
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
            if (legion[leNo - 1].GetLegionFlag())
            {
                le.SetLegionFlag(true);
                arrivalFlag = false;
            }
        }
        aimPos = tPos;
    }

    /// <summary>
    /// 更新処理の最初に呼ぶ必要な値の初期化処理
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
                // 待機中になっているか、追跡中じゃなければ軍団になっていない
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSManager : MonoBehaviour
{
    // ManagedUpdate()が呼ばれた回数をカウントします。
    int frameCount;
    // 前回フレームレートを表示してからの経過時間です。
    float elapsedTime;

    float deltaTime = 0.0f;

    bool calculatedFlag = false;

    public void Init()
    {

    }

    public void Update()
    {
        // 呼ばれた回数を加算します。
        frameCount++;

        // 前のフレームからの経過時間を加算します。
        elapsedTime += Time.deltaTime;

        if (elapsedTime >= 1.0f)
        {
            // 経過時間が1秒を超えていたら、フレームレートを計算します。
            float fps = 1.0f * frameCount / elapsedTime;
            deltaTime = (float)(elapsedTime / frameCount);

            // フレームのカウントと経過時間を初期化します。
            frameCount = 0;
            elapsedTime = 0f;

            calculatedFlag = true;
        }
    }

    public float GetDeltaTime()
    {
        return deltaTime;
    }

    public bool GetCalculatedFlag()
    {
        return calculatedFlag;
    }
}

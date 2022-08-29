using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointManager : MonoBehaviour
{
    [SerializeField] private LegionManager legionScript;
    [SerializeField] private GameObject point;
    private Transform tf;
    private int pressButtonCount = 0;
    private const int longPressCount = 100;
    private bool moveFlag = true;
    private bool jumpFlag = false;

    enum Mouse
    {
        None,
        Left,
        Right,
        Middle,
        LongLeft,
        LongRight,
        LongMiddle,
        End
    }
    private Mouse buttonType;

    public void Init()
    {
        tf = this.transform;
    }

    public RaycastHit ManagedUpdate()
    {
        Control();

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo = new RaycastHit();
        if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity))
        {
            if (hitInfo.transform.tag == "Ground")
            {
                tf.position = hitInfo.point;
                Vector3 pos = tf.position;
                pos.y = 0.5f + hitInfo.point.y;
                tf.position = pos;
            }
        }
        Debug.Log("moveFlag = " + moveFlag);

        return hitInfo;
    }

    // プレイヤーが操作する処理
    private void Control()
    {
        CheckMouseButton();
    }

    private void CheckMouseButton()
    {
        Mouse type = Mouse.None;
        // 左のボタンをクリックしたら
        if (Input.GetMouseButtonDown(0))
        {
            type = Mouse.Left;
            LeftMouseButton();
        }
        // 右のボタンをクリックしたら
        else if(Input.GetMouseButtonDown(1))
        {
            type = Mouse.Right;
            RightMouseButton();
        }
        // 真ん中のボタンをクリックしたら
        else if(Input.GetMouseButtonDown(2))
        {
            type = Mouse.Middle;
            MiddleMouseButton();
        }

        // 押されたボタンが前回と違うか、何も押されなかったら
        if (buttonType != type || buttonType == Mouse.None)
        {
            pressButtonCount = 0;
        }
        else  // そうじゃなかったら
        {
            pressButtonCount++;
        }
    }

    private void LeftMouseButton()
    {
        // 長押しなら
        if (pressButtonCount > longPressCount)
        {
            buttonType = Mouse.LongLeft;
            // キャラクターの動きを止める
            if (moveFlag)
            {
                moveFlag = false;
            }
        }
        else
        {
            // キャラクターを動かすかどうかを変える
                if (moveFlag)
                {
                    moveFlag = false;
                }
                else
                {
                    moveFlag = true;
                }
        }
    }

    private void RightMouseButton()
    {
        // 長押しなら
        if (pressButtonCount > longPressCount)
        {
            buttonType = Mouse.LongRight;
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                // キャラクターをジャンプさせる
                if (buttonType == Mouse.Right && !jumpFlag)
                {
                    jumpFlag = true;
                }
            }
        }
    }

    private void MiddleMouseButton()
    {
        // 長押しなら
        if (pressButtonCount > longPressCount)
        {
            buttonType = Mouse.LongMiddle;
        }
    }

    public bool GetMoveFlag()
    {
        return moveFlag;
    }
    public bool GetJumpFlag()
    {
        return jumpFlag;
    }
    public void SetJumpFlag(bool flag)
    {
        jumpFlag = flag;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointManager : MonoBehaviour
{
    [SerializeField] private LegionManager legionScript;
    [SerializeField] private GameObject point;
    private Transform tf;
    private float pressButtonCount = 0;
    private const float longPressCount = 0.4f;
    private bool moveFlag = true;
    private bool jumpFlag = false;

    enum Mouse
    {
        None = -1,
        Left,
        Right,
        Middle,
        LongLeft,
        LongRight,
        LongMiddle,
    }
    private Mouse buttonType;
    private Mouse oldButtonType;

    public void Init()
    {
        tf = this.transform;
        oldButtonType = Mouse.None;
    }

    public RaycastHit ManagedUpdate(float deltaTime)
    {
        Control(deltaTime);

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo = new RaycastHit();
        if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity))
        {
            if (hitInfo.transform.tag == "Ground")
            {
                tf.position = hitInfo.point;
                Vector3 pos = tf.position;
                pos.y = 1.5f + hitInfo.point.y;
                tf.position = pos;
            }
        }

        return hitInfo;
    }

    // プレイヤーが操作する処理
    private void Control(float deltaTime)
    {
        CheckMouseButton(deltaTime);
    }

    private void CheckMouseButton(float deltaTime)
    {
        buttonType = Mouse.None;
        // 左のボタンをクリックしたら
        if (Input.GetMouseButton(0))
        {
            buttonType = Mouse.Left;
            LeftMouseButton();
        }
        // 右のボタンをクリックしたら
        else if(Input.GetMouseButton(1))
        {
            buttonType = Mouse.Right;
            RightMouseButton();
        }
        // 真ん中のボタンをクリックしたら
        else if(Input.GetMouseButton(2))
        {
            buttonType = Mouse.Middle;
            MiddleMouseButton();
        }

        if(buttonType == Mouse.None)
        {
            pressButtonCount = 0;
        }
        else
        {
            pressButtonCount += deltaTime;
        }
        oldButtonType = buttonType;

        Debug.Log(pressButtonCount);
    }

    private void LeftMouseButton()
    {
        // 長押しなら
        if (pressButtonCount > longPressCount)
        {
            buttonType = Mouse.LongLeft;
            // キャラクターの動きを止める
            moveFlag = false;
        }
        else
        {
            // キャラクターを動かすかどうかを変える
            if (moveFlag)
            {
                moveFlag = false;
                Debug.Log(false);
            }
            else
            {
                moveFlag = true;
                Debug.Log(true);
            }
        }
    }

    private void RightMouseButton()
    {
        // 長押しなら
        if (pressButtonCount > longPressCount)
        {
        }
        else if (buttonType != oldButtonType)
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
        }
        else if (buttonType != oldButtonType)
        {
        }
    }

    public bool GetMoveFlag()
    {
        return moveFlag;
    }

    public void SetMoveFlag(bool flag)
    {
        moveFlag = flag;
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

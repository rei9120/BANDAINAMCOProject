using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointManager : MonoBehaviour
{
    [SerializeField] private LegionManager legionScript;
    [SerializeField] private GameObject point;
    private Transform tf;
    private int pressButtonCount = 0;
    private const int longPressCount = 70;
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
            Debug.Log("hitInfo = " + hitInfo.point);
        }

        return hitInfo;
    }

    private void Control()
    {
        buttonType = CheckMouseButton();

        if (buttonType == Mouse.Left)
        {
            if (moveFlag)
            {
                moveFlag = false;
            }
            else
            {
                moveFlag = true;
            }
        }

        if (buttonType == Mouse.Right && !jumpFlag)
        {
            jumpFlag = true;
        }
    }

    private Mouse CheckMouseButton()
    {
        Mouse type = Mouse.None;
        if (Input.GetMouseButton(0))
        {
            type = Mouse.Left;
        }
        else if(Input.GetMouseButton(1))
        {
            type = Mouse.Right;
        }
        else if(Input.GetMouseButton(2))
        {
            type = Mouse.Middle;
        }

        if (buttonType != type && buttonType == Mouse.None)
        {
            pressButtonCount = 0;
        }
        else
        {
            pressButtonCount++;
            switch (buttonType)
            {
                case Mouse.Left:
                    if (pressButtonCount > longPressCount)
                    {
                        buttonType = Mouse.LongLeft;
                        if(moveFlag)
                        {
                            moveFlag = false;
                        }
                    }
                    break;
                case Mouse.Right:
                    if (pressButtonCount > longPressCount)
                    {
                        buttonType = Mouse.LongRight;
                    }
                    break;
                case Mouse.Middle:
                    if (pressButtonCount > longPressCount)
                    {
                        buttonType = Mouse.LongMiddle;
                    }
                    break;
                default:
                    break;
            }
        }

        return type;
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

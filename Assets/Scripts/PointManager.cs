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

    // �v���C���[�����삷�鏈��
    private void Control()
    {
        CheckMouseButton();
    }

    private void CheckMouseButton()
    {
        Mouse type = Mouse.None;
        // ���̃{�^�����N���b�N������
        if (Input.GetMouseButtonDown(0))
        {
            type = Mouse.Left;
            LeftMouseButton();
        }
        // �E�̃{�^�����N���b�N������
        else if(Input.GetMouseButtonDown(1))
        {
            type = Mouse.Right;
            RightMouseButton();
        }
        // �^�񒆂̃{�^�����N���b�N������
        else if(Input.GetMouseButtonDown(2))
        {
            type = Mouse.Middle;
            MiddleMouseButton();
        }

        // �����ꂽ�{�^�����O��ƈႤ���A����������Ȃ�������
        if (buttonType != type || buttonType == Mouse.None)
        {
            pressButtonCount = 0;
        }
        else  // ��������Ȃ�������
        {
            pressButtonCount++;
        }
    }

    private void LeftMouseButton()
    {
        // �������Ȃ�
        if (pressButtonCount > longPressCount)
        {
            buttonType = Mouse.LongLeft;
            // �L�����N�^�[�̓������~�߂�
            if (moveFlag)
            {
                moveFlag = false;
            }
        }
        else
        {
            // �L�����N�^�[�𓮂������ǂ�����ς���
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
        // �������Ȃ�
        if (pressButtonCount > longPressCount)
        {
            buttonType = Mouse.LongRight;
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                // �L�����N�^�[���W�����v������
                if (buttonType == Mouse.Right && !jumpFlag)
                {
                    jumpFlag = true;
                }
            }
        }
    }

    private void MiddleMouseButton()
    {
        // �������Ȃ�
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointManager : MonoBehaviour
{
    [SerializeField] private LegionManager legionScript;
    [SerializeField] private GameObject point;
    private Transform tf;
    private float pressButtonCount = 0;
    private const float longPressCount = 100f;
    private bool moveFlag = true;
    private bool jumpFlag = false;

    private int debugCount = 0;

    enum Mouse
    {
        None = -1,
        Left,
        Right,
        Middle,
    }
    private Mouse buttonType;
    private Mouse oldButtonType;

    public void Init()
    {
        tf = this.transform;
        oldButtonType = Mouse.None;
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
                pos.y = 1.5f + hitInfo.point.y;
                tf.position = pos;
            }
        }

        return hitInfo;
    }

    // �v���C���[�����삷�鏈��
    private void Control()
    {
        CheckMouseButton();
    }

    private void CheckMouseButton()
    {
        buttonType = Mouse.None;
        // ���̃{�^�����N���b�N������
        if (Input.GetMouseButton(0))
        {
            buttonType = Mouse.Left;
            LeftMouseButton();
        }
        // �E�̃{�^�����N���b�N������
        else if(Input.GetMouseButton(1))
        {
            buttonType = Mouse.Right;
            RightMouseButton();
        }
        // �^�񒆂̃{�^�����N���b�N������
        else if(Input.GetMouseButton(2))
        {
            buttonType = Mouse.Middle;
            MiddleMouseButton();
        }

        if(buttonType == Mouse.None || buttonType != oldButtonType)
        {
            pressButtonCount = 0;
        }
        else
        {
            pressButtonCount += Time.deltaTime;
        }
        oldButtonType = buttonType;
    }

    private void LeftMouseButton()
    {
        // �������Ȃ�
        if (pressButtonCount > longPressCount)
        {
            // �L�����N�^�[�̓������~�߂�
            if (moveFlag)
            {
                moveFlag = false;
            }
        }
        else if(buttonType != oldButtonType)
        {
            // �L�����N�^�[�𓮂������ǂ�����ς���
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
        // �������Ȃ�
        if (pressButtonCount > longPressCount)
        {
        }
        else if (buttonType != oldButtonType)
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

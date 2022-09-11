using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextManager : MonoBehaviour
{
    [SerializeField] private GameObject title;
    [SerializeField] private GameObject gameStart;
    [SerializeField] private GameObject manual;
    [SerializeField] private GameObject exit;
    private TextMeshPro titleText;
    private TextMeshPro gameStartText;
    private TextMeshPro manualText;
    private TextMeshPro exitText;
    private Transform gTf;
    private Transform mTf;
    private Transform eTf;
    private string titleTextDetail;
    private List<string> titleColor;

    private Vector3 gameStartSize;
    private Vector3 manualSize;
    private Vector3 exitSize;

    private bool gameStartFlag = false;
    private bool manualFlag = false;
    private bool exitFlag = false;

    public void Init()
    {
        titleColor = new List<string>();
        titleColor.Add("<color=#FF0082>");  // 桃色ギャー
        titleColor.Add("<color=#00FFEC>");  // 水色ギャー
        titleColor.Add("<color=#3DFF00>");  // 黄緑色ギャー
        titleColor.Add("<color=#FFB200>");  // 黄色ギャー
        titleColor.Add("<color=#FF4D00>");  // オレンジ色ギャー
        titleColor.Add("<color=#7600A6>");  // 紫色ギャー

        titleText = title.GetComponent<TextMeshPro>();
        ChangeTitleDetailColor();
        titleText.text = titleTextDetail;

        gameStartText = gameStart.GetComponent<TextMeshPro>();
        manualText = manual.GetComponent<TextMeshPro>();
        exitText = exit.GetComponent<TextMeshPro>();

        gTf = gameStart.transform;
        mTf = manual.transform;
        eTf = exit.transform;

        gameStartSize = gTf.localScale;
        manualSize = mTf.localScale;
        exitSize = eTf.localScale;
    }

    public void ManagedUpdate()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo = new RaycastHit();
        if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity))
        {
            Transform rTf = hitInfo.transform;
            if (rTf.tag == "GameStart")
            {
                gTf.localScale = ChangeTextBigSize(gTf.localScale, gameStartSize);
                gameStartText.text = titleColor[0] + "ゲームスタート</color>";
                if (Input.GetMouseButtonDown(0))
                {
                    gameStartFlag = true;
                }
            }
            else
            {
                gTf.localScale = gameStartSize;
                gameStartText.text = "<color=#FFFFFF>ゲームスタート</color>";
            }
            if (rTf.tag == "Manual")
            {
                mTf.localScale = ChangeTextBigSize(mTf.localScale, manualSize);
                manualText.text = titleColor[2] + "操作説明</color>";
                if (Input.GetMouseButtonDown(0))
                {
                    manualFlag = true;
                }
            }
            else
            {
                mTf.localScale = manualSize;
                manualText.text = "<color=#FFFFFF>操作説明</color>";
            }
            if (rTf.tag == "Exit")
            {
                eTf.localScale = ChangeTextBigSize(eTf.localScale, exitSize);
                exitText.text = titleColor[1] + "ゲーム終了</color>";
                if (Input.GetMouseButtonDown(0))
                {
                    exitFlag = true;
                }
            }
            else
            {
                eTf.localScale = exitSize;
                exitText.text = "<color=#FFFFFF>ゲーム終了</color>";
            }
        }
    }

    private void ChangeTitleDetailColor()
    {
        string tDetail = "Induction Legion";
        int colorNum = 0;
        for (int i = 0; i < tDetail.Length; i++)
        {
            if (tDetail[i] == ' ')
            {
                titleTextDetail += tDetail[i];
                continue;
            }
            titleTextDetail += titleColor[colorNum] + tDetail[i] + "</color>";
            colorNum++;
            if (colorNum > 5)
            {
                colorNum = 0;
            }
        }
    }

    private Vector3 ChangeTextBigSize(Vector3 size, Vector3 defaultSize)
    {
        Vector3 changeSize = new Vector3(0.3f, 0.3f, 0.3f);
        if (size.x < defaultSize.x + changeSize.x)
        {
            size.x += 0.1f;
            size.y += 0.1f;
            size.z += 0.1f;
        }

        return size;
    }

    public bool GetGameStartFlag()
    {
        return gameStartFlag;
    }

    public bool GetManualFlag()
    {
        return manualFlag;
    }

    public bool GetExitFlag()
    {
        return exitFlag;
    }
}

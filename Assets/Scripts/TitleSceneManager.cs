using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[DefaultExecutionOrder(-1)]
public class TitleSceneManager : MonoBehaviour
{
    [SerializeField] private GameObject fade;
    [SerializeField] private GameObject option;
    [SerializeField] private TextManager textScript;
    private FadeManager fadeScript;
    private Image fadeImage;
    private Color fadeColor;
    private float alphaSpeed = 0.03f;
    private bool sceneFlag = false;
    private bool fadeFlag = true;


    private void Start()
    {
        fadeImage = fade.GetComponent<Image>();
        fadeColor = fadeImage.color;
        fadeScript = fade.GetComponent<FadeManager>();
        textScript.Init();
    }

    // Update is called once per frame
    void Update()
    {
        if (!sceneFlag && option.activeSelf)
        {
            if (fadeFlag)
            {
                fadeColor.a = fadeScript.OnFade(fadeColor.a, alphaSpeed, false);
                fadeImage.color = fadeColor;
                if (fadeImage.color.a < 0)
                {
                    fadeFlag = false;
                }
            }

            textScript.ManagedUpdate();

            //if (textScript.GetManualFlag())
            //{
            //    manualCanvas.SetActive(true);
            //}

            if (textScript.GetExitFlag() || Input.GetKeyDown(KeyCode.Escape))
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;//ゲームプレイ終了
#else
    Application.Quit();//ゲームプレイ終了
#endif
            }
        }

        if (textScript.GetGameStartFlag() || sceneFlag)
        {
            sceneFlag = true;
            fadeFlag = true;
            fadeColor.a = fadeScript.OnFade(fadeColor.a, alphaSpeed, true);
            fadeImage.color = fadeColor;
            if (fadeImage.color.a > 1)
            {
                fadeFlag = false;
                SceneUpdate();
            }
        }
    }

    private void SceneUpdate()
    {
        SceneManager.LoadScene("Game");
    }
}

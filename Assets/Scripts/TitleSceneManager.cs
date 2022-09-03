using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[DefaultExecutionOrder(-1)]
public class TitleSceneManager : MonoBehaviour
{
    [SerializeField] private GameObject fade;
    [SerializeField] private TextManager textScript;
    [SerializeField] private GameObject manualCanvas;
    private ManualManager manualScript;
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
        manualScript = manualCanvas.GetComponent<ManualManager>();
        if (manualCanvas.activeSelf)
        {
            manualCanvas.SetActive(false);
        }
        textScript.Init();
    }

    // Update is called once per frame
    void Update()
    {
        if (!sceneFlag && !manualCanvas.activeSelf)
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

            if (textScript.GetManualFlag())
            {
                manualCanvas.SetActive(true);
            }

            if (textScript.GetExitFlag())
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;//ゲームプレイ終了
#else
    Application.Quit();//ゲームプレイ終了
#endif
            }
        }
        else
        {
            manualScript.ManagedUpdate();
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

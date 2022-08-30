using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[DefaultExecutionOrder(-1)]
public class TitleSceneManager : MonoBehaviour
{
    [SerializeField] private FadeManager fadeScript;
    [SerializeField] private GameObject fade;
    private Image fadeImage;
    private Color fadeColor;
    private float alphaSpeed = 0.03f;
    private bool sceneFlag = false;
    private bool fadeFlag = true;


    private void Start()
    {
        fadeImage = fade.GetComponent<Image>();
        fadeColor = fadeImage.color;
    }

    // Update is called once per frame
    void Update()
    {
        if (!sceneFlag)
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
        }

        if (Input.GetKeyDown(KeyCode.Space) || sceneFlag)
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

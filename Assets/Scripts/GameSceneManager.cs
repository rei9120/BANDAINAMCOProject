using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[DefaultExecutionOrder(-1)]
public class GameSceneManager : MonoBehaviour
{
    [SerializeField] private GameObject fpsManager;
    [SerializeField] private GameObject fade;
    [SerializeField] private GameObject point;
    [SerializeField] private GameObject legionManager;
    [SerializeField] private GameObject gameCamera;
    [SerializeField] private GameObject lineRenderer;
    [SerializeField] private GameObject anchor;
    [SerializeField] private GameObject obstacleManager;
    private FPSManager fpsScript;
    private FadeManager fadeScript;
    private PointManager pointScript;
    private LegionManager legionScript;
    private CameraManager cameraScript;
    private MouseLineRenderer mouseLineScript;
    private ObstacleManager obstacleScript;

    private Image fadeImage;
    private Color fadeColor;
    private float alphaSpeed = 0.03f;

    private RaycastHit hitInfo;
    private bool fadeFlag = false;
    private bool sceneFlag = false;

    enum GameState
    {
        Begin,
        Game,
    }
    GameState gState = GameState.Begin;


    private void Start()
    {
        fpsScript = fpsManager.GetComponent<FPSManager>();
        fadeScript = fade.GetComponent<FadeManager>();
        pointScript = point.GetComponent<PointManager>();
        legionScript = legionManager.GetComponent<LegionManager>();
        cameraScript = gameCamera.GetComponent<CameraManager>();
        mouseLineScript = lineRenderer.GetComponent<MouseLineRenderer>();
        obstacleScript = obstacleManager.GetComponent<ObstacleManager>();
        fpsScript.Init();
        pointScript.Init();
        legionScript.Init(point, lineRenderer);
        cameraScript.Init(legionScript.GetStartLegionPtr());
        mouseLineScript.Init(point, legionManager);
        obstacleScript.Init();

        fadeImage = fade.GetComponent<Image>();
        fadeColor = fadeImage.color;
    }

    // Update is called once per frame
    private void Update()
    {
        switch (gState)
        {
            case GameState.Begin:
                // デルタタイムの測定,フェードインなどを行う
                GamePreparation();
                break;
            case GameState.Game:
                float deltaTime = fpsScript.GetDeltaTime();
                Game(deltaTime);
                break;
        }
    }

    private void Game(float deltaTime)
    {
        hitInfo = pointScript.ManagedUpdate();
        mouseLineScript.ManagedUpdate(hitInfo);
        obstacleScript.ManagedUpdate();
        legionScript.ManagedUpdate(deltaTime);
        cameraScript.ManagedUpdate(legionScript.GetStartLegionPtr());

        if (Input.GetKeyDown(KeyCode.Space))
        {
            SceneUpdate();
        }
    }

    private void SceneUpdate()
    {
        SceneManager.LoadScene("Title");
    }

    private void GamePreparation()
    {
        if (fpsScript.GetCalculatedFlag())
        {
            if (fadeFlag)
            {
                gState = GameState.Game;
            }
            else
            {
                fadeColor.a = fadeScript.OnFade(fadeColor.a, alphaSpeed, false);
                fadeImage.color = fadeColor;
                if (fadeImage.color.a < 0)
                {
                    fadeFlag = true;
                }
            }
        }
    }
}
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
    [SerializeField] private GameObject obstacleManager;
    [SerializeField] private GameObject gameUI;
    [SerializeField] private GameObject goal;
    private FPSManager fpsScript;
    private FadeManager fadeScript;
    private PointManager pointScript;
    private LegionManager legionScript;
    private CameraManager cameraScript;
    private MouseLineRenderer mouseLineScript;
    private ObstacleManager obstacleScript;
    private GameUIManager gameUIScript;
    private GoalManager goalScript;

    private Image fadeImage;
    private Color fadeColor;
    private float alphaSpeed = 0.03f;

    private RaycastHit hitInfo;
    private bool fadeFlag = false;

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
        gameUIScript = gameUI.GetComponent<GameUIManager>();
        goalScript = goal.GetComponent<GoalManager>();
        fpsScript.Init();
        pointScript.Init();
        obstacleScript.Init();
        legionScript.Init(point, lineRenderer);
        cameraScript.Init(legionScript.GetStartLegionPtr());
        mouseLineScript.Init(point, legionManager);
        gameUIScript.Init(legionScript);

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
        hitInfo = pointScript.ManagedUpdate(deltaTime);
        mouseLineScript.ManagedUpdate(hitInfo);
        obstacleScript.ManagedUpdate(deltaTime);
        legionScript.ManagedUpdate(deltaTime);
        cameraScript.ManagedUpdate(legionScript.GetStartLegionPtr());
        gameUIScript.ManagedUpdate();

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Escape)
            || goalScript.GetGoalFlag())
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
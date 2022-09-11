using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugSceneManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> objects;
    [SerializeField] private GameObject fpsManager;
    [SerializeField] private GameObject point;
    [SerializeField] private GameObject legionManager;
    [SerializeField] private GameObject gameCamera;
    [SerializeField] private GameObject lineRenderer;
    [SerializeField] private GameObject obstacleManager;
    [SerializeField] private GameObject gameUI;
    private FPSManager fpsScript;
    private PointManager pointScript;
    private LegionManager legionScript;
    private CameraManager cameraScript;
    private MouseLineRenderer mouseLineScript;
    private ObstacleManager obstacleScript;
    private GameUIManager gameUIScript;

    private GameObject obj;
    [SerializeField] private int debugCount;

    private RaycastHit hitInfo;

    enum GameState
    {
        Begin,
        Game,
    }
    GameState gState = GameState.Begin;

    // Start is called before the first frame update
    void Start()
    {
        fpsScript = fpsManager.GetComponent<FPSManager>();
        pointScript = point.GetComponent<PointManager>();
        legionScript = legionManager.GetComponent<LegionManager>();
        cameraScript = gameCamera.GetComponent<CameraManager>();
        mouseLineScript = lineRenderer.GetComponent<MouseLineRenderer>();
        obstacleScript = obstacleManager.GetComponent<ObstacleManager>();
        gameUIScript = gameUI.GetComponent<GameUIManager>();
        fpsScript.Init();
        pointScript.Init();
        obstacleScript.Init();
        legionScript.Init(point, lineRenderer);
        cameraScript.Init(legionScript.GetStartLegionPtr());
        mouseLineScript.Init(point, legionManager);
        gameUIScript.Init(legionScript);
    }

    // Update is called once per frame
    void Update()
    {
        if (fpsScript.GetCalculatedFlag())
        {
            float deltaTime = fpsScript.GetDeltaTime();
            hitInfo = pointScript.ManagedUpdate(deltaTime);
            mouseLineScript.ManagedUpdate(hitInfo);
            obstacleScript.ManagedUpdate(deltaTime);
            legionScript.ManagedUpdate(deltaTime);
            cameraScript.ManagedUpdate(legionScript.GetStartLegionPtr());
            gameUIScript.ManagedUpdate();
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            debugCount++;
            if (debugCount > objects.Count - 1)
            {
                debugCount = 0;
            }
        }
        else if(Input.GetKeyDown(KeyCode.LeftArrow))
        {
            debugCount--;
            if(debugCount < 0)
            {
                debugCount = objects.Count - 1;
            }
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            if(obj != null)
            {
                Destroy(obj.gameObject);
                Destroy(obj);
            }

            obj = Instantiate(objects[debugCount]);
            if(obj.tag == "Obstacle")
            {
                Rigidbody rig = obj.GetComponent<Rigidbody>();
                rig.position = new Vector3(0.0f, 20.0f, 10.0f);
            }
            else
            {
                obj.transform.position = new Vector3(0.0f, 1.0f, 10.0f);
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;//ゲームプレイ終了
#else
    Application.Quit();//ゲームプレイ終了
#endif
        }
    }
}

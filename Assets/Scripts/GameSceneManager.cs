using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(-1)]
public class GameSceneManager : MonoBehaviour
{
    [SerializeField] private GameObject point;
    [SerializeField] private GameObject legionManager;
    [SerializeField] private GameObject gameCamera;
    [SerializeField] private GameObject lineRenderer;
    [SerializeField] private GameObject anchor;
    [SerializeField] private GameObject obstacleManager;
    private PointManager pointScript;
    private LegionManager legionScript;
    private CameraManager cameraScript;
    private MouseLineRenderer mouseLineScript;
    private ObstacleManager obstacleScript;

    private RaycastHit hitInfo;

    private void Start()
    {
        pointScript = point.GetComponent<PointManager>();
        legionScript = legionManager.GetComponent<LegionManager>();
        cameraScript = gameCamera.GetComponent<CameraManager>();
        mouseLineScript = lineRenderer.GetComponent<MouseLineRenderer>();
        obstacleScript = obstacleManager.GetComponent<ObstacleManager>();
        pointScript.Init();
        legionScript.Init(point, lineRenderer, anchor);
        cameraScript.Init(legionScript.GetStartLegionPtr());
        mouseLineScript.Init(point, legionManager);
        obstacleScript.Init();
    }

    // Update is called once per frame
    private void Update()
    {
        hitInfo = pointScript.ManagedUpdate();
        legionScript.ManagedUpdate();
        cameraScript.ManagedUpdate(legionScript.GetStartLegionPtr());
        mouseLineScript.ManagedUpdate(hitInfo);
        obstacleScript.ManagedUpdate();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            SceneUpdate();
        }
    }

    private void SceneUpdate()
    {
        SceneManager.LoadScene("Title");
    }
}

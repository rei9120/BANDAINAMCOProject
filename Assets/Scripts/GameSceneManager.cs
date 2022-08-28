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
    private PointManager pointScript;
    private LegionManager legionScript;
    private CameraManager cameraScript;
    private MouseLineRenderer mouseLineScript;

    private RaycastHit hitInfo;

    private void Start()
    {
        pointScript = point.GetComponent<PointManager>();
        legionScript = legionManager.GetComponent<LegionManager>();
        cameraScript = gameCamera.GetComponent<CameraManager>();
        mouseLineScript = lineRenderer.GetComponent<MouseLineRenderer>();
        pointScript.Init();
        legionScript.Init(point, lineRenderer);
        cameraScript.Init(legionScript.GetLegionPtr());
        mouseLineScript.Init(point);
    }

    // Update is called once per frame
    private void Update()
    {
        hitInfo = pointScript.ManagedUpdate();
        legionScript.ManagedUpdate();
        cameraScript.ManagedUpdate(legionScript.GetLegionPtr());
        mouseLineScript.ManagedUpdate(hitInfo);

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

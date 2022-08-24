using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneManager : MonoBehaviour
{
    [SerializeField] private GameObject gameCamera;
    [SerializeField] private GameObject playerManager;
    [SerializeField] private GameObject legionManager;
    [SerializeField] private GameObject lineRenderer;
    [SerializeField] private GameObject point;
    private PlayerManager playerScript;
    private LegionManager legionScript;
    private CameraManager cameraScript;
    private MouseLineRenderer mouseLineScript;
    private PointManager pointScript;

    private void Start()
    {
        pointScript = point.GetComponent<PointManager>();
        cameraScript = gameCamera.GetComponent<CameraManager>();
        mouseLineScript = lineRenderer.GetComponent<MouseLineRenderer>();
        playerScript = playerManager.GetComponent<PlayerManager>();
        legionScript = legionManager.GetComponent<LegionManager>();
        pointScript.Init();
        playerScript.Init();
        legionScript.Init();
        cameraScript.Init(playerScript.GetPlayerPtr());
        mouseLineScript.Init();
    }

    // Update is called once per frame
    void Update()
    {
        pointScript.ManagedUpdate();
        cameraScript.ManagedUpdate();
        mouseLineScript.ManagedUpdate();
        playerScript.ManagedUpdate();
        legionScript.ManagedUpdate();

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

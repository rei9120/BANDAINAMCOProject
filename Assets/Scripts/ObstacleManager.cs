using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleManager : MonoBehaviour
{
    [SerializeField] private List<FallObstacle> fallObjects;
    [SerializeField] private List<BallObstacle> ballObjects;
    public void Init()
    {
        for(int i = 0; i < fallObjects.Count; i++)
        {
            fallObjects[i].Init();
        }
    }

    public void ManagedUpdate(float deltaTime)
    {
        for(int i = 0; i < fallObjects.Count; i++)
        {
            fallObjects[i].ManagedUpdate();
        }
        for (int i = 0; i < ballObjects.Count; i++)
        {
            if (ballObjects[i] != null)
            {
                ballObjects[i].ManagedUpdate(deltaTime);
            }
            else
            {
                ballObjects.RemoveAt(i);
            }
        }
    }

    public void SetBallObjects(BallObstacle script)
    {
        ballObjects.Add(script);
    }
}

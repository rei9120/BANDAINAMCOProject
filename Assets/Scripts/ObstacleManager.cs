using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleManager : MonoBehaviour
{
    [SerializeField] private List<FallObstacle> fallObjects;
    [SerializeField] private List<RollIronBall> rollIronBalls;
    public void Init()
    {
        for(int i = 0; i < fallObjects.Count; i++)
        {
            fallObjects[i].Init();
        }
        for(int i = 0; i < rollIronBalls.Count; i++)
        {
            rollIronBalls[i].Init();
        }
    }

    public void ManagedUpdate(float deltaTime)
    {
        for(int i = 0; i < fallObjects.Count; i++)
        {
            fallObjects[i].ManagedUpdate();
        }
        for(int i = 0; i < rollIronBalls.Count; i++)
        {
            if (rollIronBalls[i] != null)
            {
                rollIronBalls[i].ManagedUpdate(deltaTime);
            }
            else
            {
                rollIronBalls.RemoveAt(i);
            }
        }
    }
}

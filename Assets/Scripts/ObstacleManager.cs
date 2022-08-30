using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleManager : MonoBehaviour
{
    [SerializeField] private List<FallObstacle> fallObjects;
    public void Init()
    {
        for(int i = 0; i < fallObjects.Count; i++)
        {
            fallObjects[i].Init();
        }
    }

    public void ManagedUpdate()
    {
        for(int i = 0; i < fallObjects.Count; i++)
        {
            fallObjects[i].ManagedUpdate();
        }
    }
}

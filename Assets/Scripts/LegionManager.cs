using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegionManager : MonoBehaviour
{
    [SerializeField] private GameObject legionPrefab;
    private List<Legion> legion;

    public void Init()
    {
        legion = new List<Legion>();
    }

    public void CreateLegion(GameObject pl)
    {
        GameObject obj = Instantiate(legionPrefab);
        legion.Add(obj.GetComponent<Legion>());
        legion[legion.Count - 1].Init(pl);
    }

    public void ManagedUpdate()
    {
        for(int i = 0; i < legion.Count; i++)
        {
            if (legion[i] != null)
            {
                if (i - 1 >= 0)
                {
                    legion[i].ManagedUpdate(legion[i - 1]);
                }
                else
                {
                    legion[i].ManagedUpdate(null);
                }
            }
        }
    }
}

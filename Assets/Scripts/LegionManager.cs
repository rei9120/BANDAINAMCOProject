using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegionManager : MonoBehaviour
{
    [SerializeField] private GameObject legionPrefab;
    private MouseLineRenderer lineScript;
    private GameObject point;
    private GameObject lineRenderer;
    private List<Legion> legion;

    public void Init(GameObject p, GameObject l)
    {
        point = p;
        lineRenderer = l;
        lineScript = lineRenderer.GetComponent<MouseLineRenderer>();
        legion = new List<Legion>();
        CreateLegion(3);
    }

    public void CreateLegion(int num)
    {
        for (int i = 0; i < num; i++)
        {
            GameObject obj = Instantiate(legionPrefab);
            legion.Add(obj.GetComponent<Legion>());
            if (legion.Count == 1)
            {
                legion[legion.Count - 1].Init(point, lineRenderer, point.transform.position);
            }
            else
            {
                legion[legion.Count - 1].Init(point, lineRenderer, legion[legion.Count - 2].transform.position);
            }
        }
    }

    public void ManagedUpdate()
    {
        for (int i = 0; i < legion.Count; i++)
        {
            int num = (int)legion[i].FindItem();
            if (num > 0)
            {
                CreateLegion(num);
                legion[i].SetItemType(Legion.Item.none);
            }

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

    public Legion GetLegionPtr()
    {
        return legion[0];
    }

    public bool CheckFollowLineFlag()
    {
        for(int i = 0; i < legion.Count; i++)
        {
            bool flag = false;
            flag = legion[i].GetFollowLineFlag();
            if(!flag)
            {
                return false;
            }
        }
        return true;
    }

    public void FollowLineFlag(bool flag)
    {
        for(int i = 0; i < legion.Count; i++)
        {
            legion[i].SetFollowLineFlag(flag);
        }
    }
}

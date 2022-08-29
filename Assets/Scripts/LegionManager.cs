using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegionManager : MonoBehaviour
{
    [SerializeField] private GameObject legionPrefab;
    private GameObject anchor;
    private GameObject point;
    private GameObject lineRenderer;
    private List<Legion> legion;
    private Legion sidelegion;
    private int createLegion = 0;

    public void Init(GameObject p, GameObject l, GameObject a)
    {
        point = p;
        lineRenderer = l;
        anchor = a;
        legion = new List<Legion>();
        CreateLegion(5);
        sidelegion = legion[0];
    }

    public void CreateLegion(int num)
    {
        for (int i = 0; i < num; i++)
        {
            GameObject obj = Instantiate(legionPrefab);
            legion.Add(obj.GetComponent<Legion>());
            if (legion.Count == 1)
            {
                legion[legion.Count - 1].Init(point, lineRenderer, this.gameObject, anchor, point.transform.position);
            }
            else
            {
                legion[legion.Count - 1].Init(point, lineRenderer, this.gameObject, anchor,legion[legion.Count - 2].transform.position);
            }
            legion[legion.Count - 1].name = "legion" + (legion.Count - 1);
            createLegion++;
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
            Debug.Log("legion = " + legion[i]);
        }
    }

    public Legion GetStartLegionPtr()
    {
        return legion[0];
    }
    public Legion GetEndLegionPtr()
    {
        return legion[legion.Count - 1];
    }

    public Legion GetSideLegion()
    {
        return sidelegion;
    }

    public void SetSideLegion(Legion le)
    {
        sidelegion = le;
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

    public void SetAnchorPosition(Vector3 pos)
    {
        anchor.transform.localPosition = pos;
    }
}

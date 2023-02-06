using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class OptionManager : MonoBehaviour
{
    [SerializeField] private GameObject operation;
    private bool operationFlag = false;
    public void ManagedUpdate()
    {
        if(operationFlag)
        {
            operation.SetActive(true);
        }
        else
        {
            operation.SetActive(false);
        }
    }

    public void SetOperationFlag(bool flag)
    {
        operationFlag = flag;
    }
}

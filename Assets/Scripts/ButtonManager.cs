using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonManager : MonoBehaviour
{
    [SerializeField] private GameObject manualManager;

    public void OnClick()
    {
        GameObject obj = GameObject.Find("ManualCanvas");
        obj.gameObject.SetActive(false);
    }
}

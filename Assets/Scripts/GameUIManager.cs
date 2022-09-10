using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour
{
    [SerializeField] private GameObject gyaarKunTypeImage;
    [SerializeField] private GameObject gyaarKunNum;
    private GyaarTypeImage gyaTypeScript;
    private GyaarNum gyaNumScript;

    public void Init(LegionManager le)
    {
        gyaTypeScript = gyaarKunTypeImage.GetComponent<GyaarTypeImage>();
        gyaNumScript = gyaarKunTypeImage.GetComponent<GyaarNum>();
        gyaTypeScript.Init(le);
        gyaNumScript.Init(le);
    }

    public void ManagedUpdate()
    {
        gyaTypeScript.ManagedUpdate();
        gyaNumScript.ManagedUpdate();
    }
}

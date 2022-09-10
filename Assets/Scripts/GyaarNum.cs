using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GyaarNum : MonoBehaviour
{
    [SerializeField] private GameObject gyaNumObject;
    private LegionManager legionScript;
    private TextMeshProUGUI tmPro;
    private string num;

    public void Init(LegionManager le)
    {
        legionScript = le;
        tmPro = gyaNumObject.GetComponent<TextMeshProUGUI>();
        num = le.GetNowLegionNum().ToString();
        tmPro.text = num;
    }

    public void ManagedUpdate()
    {
        num = legionScript.GetNowLegionNum().ToString();
        tmPro.text = num;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

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
        if(legionScript.GetNowLegionNum() == 0)
        {
            SceneManager.LoadScene("Title");
        }
        num = legionScript.GetNowLegionNum().ToString();
        tmPro.text = num;
    }
}

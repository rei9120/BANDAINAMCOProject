using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GyaarTypeImage : MonoBehaviour
{
    [SerializeField] private List<Sprite> sprite;
    [SerializeField] GameObject gyaTypeImageObject;
    private LegionManager legionScript;
    private Image image;
    public void Init(LegionManager le)
    {
        image = gyaTypeImageObject.GetComponent<Image>();
        legionScript = le;
        image.sprite = sprite[0];
    }

    public void ManagedUpdate()
    {
        if(legionScript.GetLegionType() == Legion.LegionType.Legion)
        {
            if(legionScript.GetLegionMoveFlag())
            {
                image.sprite = sprite[3];
            }
            else
            {
                image.sprite = sprite[2];
            }
        }
        else
        {
            if (legionScript.GetLegionMoveFlag())
            {
                image.sprite = sprite[1];
            }
            else
            {
                image.sprite = sprite[0];
            }
        }
    }
}

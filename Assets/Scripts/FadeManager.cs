using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeManager : MonoBehaviour
{
    public float OnFade(float alpha, float speed, bool fade)
    {
        if(fade)
        {
            alpha = alpha + speed;
            return alpha;
        }
        else
        {
            alpha = alpha - speed;
            return alpha;
        }
    }
}

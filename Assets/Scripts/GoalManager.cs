using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalManager : MonoBehaviour
{
    private bool goalFlag;
    public void Init()
    {
        goalFlag = false;
    }

    public bool GetGoalFlag()
    {
        return goalFlag;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Legion")
        {
            goalFlag = true;
        }
    }
}

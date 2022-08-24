using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private GameObject legionManager;
    [SerializeField] private GameObject playerPrefab;
    private LegionManager legionScript;
    private GameObject player;
    private Player playerScript;

    public void Init()
    {
        CreatePlayer();
        legionScript = legionManager.GetComponent<LegionManager>();
    }

    public void CreatePlayer()
    {
        player = Instantiate(playerPrefab);
        playerScript = player.GetComponent<Player>();
        playerScript.Init(new Vector3(0.0f, 0.5f, 0.0f));
    }

    public void ManagedUpdate()
    {
        if(playerScript.FindItem() != Player.Item.none)
        {
            for(int i = 0; i < (int)playerScript.FindItem(); i++)
            {
                legionScript.CreateLegion(player);
            }
        }
        playerScript.ManagedUpdate();
    }

    public GameObject GetPlayerPtr()
    {
        return player;
    }
}

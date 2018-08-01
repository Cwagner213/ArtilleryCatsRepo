//***********************************************************************************************************************************************************************************
// BombNumberPlus controls the powerup which allows players to place more bombs at once
// Last Updated: 7/26/18 12pm
//***********************************************************************************************************************************************************************************

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BombNumberPlus : NetworkBehaviour
{
    //***********************************************************************************************************************************************************************************
    // OnTriggerEnter2D will give the player who picks up the powerup the ability to place one more bomb at a time for each powerup achieved
    // It will also destroy itself when a player picks it up 
    //***********************************************************************************************************************************************************************************
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag == "Player")
        {

            if (!(collision.GetComponent<Player>().bombNumber > 4))
            {
                collision.GetComponent<Player>().bombMax++;
                collision.GetComponent<Player>().bombsAvailable++;
            }
            NetworkServer.Destroy(gameObject);

        }
    }
}

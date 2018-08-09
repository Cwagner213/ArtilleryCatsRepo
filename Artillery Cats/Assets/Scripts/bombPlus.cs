//***********************************************************************************************************************************************************************************
// bombPlus increases the bomb power of the bombs that the player who get this powerup
// Last Updated: 7/26/18 12pm
//***********************************************************************************************************************************************************************************

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class bombPlus : NetworkBehaviour {


    //***********************************************************************************************************************************************************************************
    // OnTriggerEnter2D will give the player more bomb power and destroy the powerup when a player takes it
    //***********************************************************************************************************************************************************************************
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.transform.tag == "Player")
        {

            collision.GetComponent<Player>().bombPower++;
            NetworkServer.Destroy(gameObject);
        }
    }
}

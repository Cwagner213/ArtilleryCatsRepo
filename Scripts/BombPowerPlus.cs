//***********************************************************************************************************************************************************************************
// BombPowerPlus controls a powerup that will increase the distance that a player can send a bomb flying 
// Last Updated: 7/26/18 12pm
//***********************************************************************************************************************************************************************************

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BombPowerPlus : NetworkBehaviour
{

    //***********************************************************************************************************************************************************************************
    // OnTriggerEnter2D will give the player the ability to shoot a bomb one more square distance for every powerup picked up 
    // it will also destroy itself when a player picks it up
    //***********************************************************************************************************************************************************************************
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag == "Player")
        {

            if (!(collision.GetComponent<Player>().bombNumber > 5))
            {
                collision.GetComponent<Player>().bombAvailableDistance++;     
            }
            NetworkServer.Destroy(gameObject);

        }
    }
}

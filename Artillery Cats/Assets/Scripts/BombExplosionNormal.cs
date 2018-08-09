//***********************************************************************************************************************************************************************************
// BombExplosionNormal controls the actual explosion objects made when a bomb goes off checking to see if they hit players
// Last Updates: 7/26/18 12pm
//***********************************************************************************************************************************************************************************

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BombExplosionNormal : NetworkBehaviour {

    // Use this for initialization
    void Start() {
        StartCoroutine(timerTilDead()); //Starts timer for explosion to fizzle out
    }

    //***********************************************************************************************************************************************************************************
    //  timerTilDead kills the explosion off once one second has passed
    //***********************************************************************************************************************************************************************************
    public IEnumerator timerTilDead()
    {
        yield return new WaitForSeconds(1);
        NetworkServer.Destroy(gameObject);
    }

    //***********************************************************************************************************************************************************************************
    // OnTriggerEnter2D checks to see if a player enters the explosions BoxCollider2D and kills them if they do
    //***********************************************************************************************************************************************************************************
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag == "Player" && collision.transform.gameObject.GetComponent<Player>().phasedOut == false)
        {        
            collision.GetComponent<Player>().killSelf(); //kills a player who walks into the explosion
        }
    }  
}

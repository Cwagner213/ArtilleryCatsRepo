//***********************************************************************************************************************************************************************************
//  Quicksand controls the sand spawned on the litterbox maps that kills players when they walk over it
//  Last Updated: 7/27/18 11:39am
//***********************************************************************************************************************************************************************************

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class quicksand : NetworkBehaviour
{

    void Start()
    {
        StartCoroutine(killtimer()); //Starts timer for the lifespan of the quicksand
    }

    //***********************************************************************************************************************************************************************************
    // killtimer is the timer until the sand deletes itself
    //***********************************************************************************************************************************************************************************
    public IEnumerator killtimer()
    {
        yield return new WaitForSeconds(3);
        NetworkServer.Destroy(gameObject);       
    }

    //***********************************************************************************************************************************************************************************
    //  OnTriggerEnter2D will be activated wehenever an object enters the quicksand, if the object is a player the player is killed
    //***********************************************************************************************************************************************************************************
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag == "Player" && collision.transform.gameObject.GetComponent<Player>().phasedOut == false)
        {
            collision.GetComponent<Player>().killSelf();          
        }
    }
}

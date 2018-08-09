//***********************************************************************************************************************************************************************************
//WaterPatch controls the effect of water patches left by water balloonos
// Last updated" 7/26/18 11:38am
//***********************************************************************************************************************************************************************************

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class WaterPatch : NetworkBehaviour {

	// Use this for initialization
	void Start () {
        StartCoroutine(killSelf()); //starts timer for water patch to go away
	}

    //***********************************************************************************************************************************************************************************
    // killSelf deletes the waterpatch after 2 seconds
    //***********************************************************************************************************************************************************************************
    public IEnumerator killSelf()
    {
        yield return new WaitForSeconds(2);
        NetworkServer.Destroy(gameObject);
    }

    //***********************************************************************************************************************************************************************************
    // OnTriggerStay2d detects any objects within the water patches BoxCollider2D and slowos them down
    //***********************************************************************************************************************************************************************************
    private void OnTriggerStay2D(Collider2D collision)
    {


        if (collision.transform.tag == "Player")
        {

            collision.GetComponent<Player>().startspeed();
        }
    }

}

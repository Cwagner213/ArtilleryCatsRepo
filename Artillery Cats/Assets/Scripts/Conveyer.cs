//***********************************************************************************************************************************************************************************
//  Conveyer controls the response of bombs that land on conveyeer belts int he industry maps
//  Last Updated: 7/25/18 12pm
//***********************************************************************************************************************************************************************************

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class Conveyer : MonoBehaviour{

    //***********************************************************************************************************************************************************************************
    //  OnTriggerStay2D is called when any object enters the BoxCollider2D 
    //  It takes one parameter which is the object that collided with it
    //***********************************************************************************************************************************************************************************
    private void OnTriggerStay2D(Collider2D collision)
    {

        //checks to see if the object that entered is a bomb
        if (collision.transform.tag == "Bomb" && collision.GetComponent<Bomb>().bStopMove == true)
        {
            //sets the bomb to move down the belt
            collision.GetComponent<Bomb>().bStopMove = false;
            collision.GetComponent<Bomb>().startSlowMove(new Vector3(collision.transform.position.x + .64f, collision.transform.position.y));
        }
    }
}

//***********************************************************************************************************************************************************************************
// ConveyerR is the same as Conveyer but moves the bomb in the opposite direction 
// LastUpdated: 7/26/18 12pm
//***********************************************************************************************************************************************************************************

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyerR : MonoBehaviour {

    //***********************************************************************************************************************************************************************************
    // OnTriggerStay2D will move any bomb in the conveyer belts BoxCollider2D so long as it remains inside of the BoxCollider
    //***********************************************************************************************************************************************************************************
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.transform.tag == "Bomb" && collision.GetComponent<Bomb>().bStopMove == true)
        {
           
            collision.GetComponent<Bomb>().bStopMove = false;
            collision.GetComponent<Bomb>().startSlowMove(new Vector3(collision.transform.position.x - .64f, collision.transform.position.y));
          
        }
    }
}

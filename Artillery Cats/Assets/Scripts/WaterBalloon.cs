//***********************************************************************************************************************************************************************************
// WaterBalloon controls the actions of spawned water balloons from the DLC beach cat
// Last Updated: 7/26/18 12pm
//***********************************************************************************************************************************************************************************

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class WaterBalloon : NetworkBehaviour {

   public  GameObject water; //store water patch object to spawn when the balloon bursts

	// Use this for initialization
	void Start () {
        StartCoroutine(balloonPop()); //starts the timer to pop the balloon once its been thrown
	}

    //***********************************************************************************************************************************************************************************
    // balloonPop pops the balloon one sec after its been thrown
    //***********************************************************************************************************************************************************************************
    public IEnumerator balloonPop()
    {
        yield return new WaitForSeconds(1);
        explode();
    }

    //***********************************************************************************************************************************************************************************
    //  explode explodes the waterballoon spawning water all around it which will slow down players who walk into it
    //***********************************************************************************************************************************************************************************
    public void explode()
    {
        GameObject ttt = (GameObject)Instantiate(water, new Vector3(transform.position.x, transform.position.y), Quaternion.identity);
        NetworkServer.Spawn(ttt);

        GameObject ttt2 = (GameObject)Instantiate(water, new Vector3(transform.position.x + 1.28f, transform.position.y), Quaternion.identity);
        NetworkServer.Spawn(ttt2);

        GameObject ttt3 = (GameObject)Instantiate(water, new Vector3(transform.position.x + .64f, transform.position.y), Quaternion.identity);
        NetworkServer.Spawn(ttt3);

        GameObject ttt4 = (GameObject)Instantiate(water, new Vector3(transform.position.x - .64f, transform.position.y), Quaternion.identity);
        NetworkServer.Spawn(ttt4);

        GameObject ttt5 = (GameObject)Instantiate(water, new Vector3(transform.position.x - 1.28f, transform.position.y), Quaternion.identity);
        NetworkServer.Spawn(ttt5);

        GameObject ttt6 = (GameObject)Instantiate(water, new Vector3(transform.position.x - .64f, transform.position.y + .64f), Quaternion.identity);
        NetworkServer.Spawn(ttt6);

        GameObject ttt7 = (GameObject)Instantiate(water, new Vector3(transform.position.x - .64f, transform.position.y - .64f), Quaternion.identity);
        NetworkServer.Spawn(ttt7);

        GameObject ttt8 = (GameObject)Instantiate(water, new Vector3(transform.position.x +.64f, transform.position.y -.64f) , Quaternion.identity);
        NetworkServer.Spawn(ttt8);

        GameObject ttt9 = (GameObject)Instantiate(water, new Vector3(transform.position.x + .64f, transform.position.y + .64f), Quaternion.identity);
        NetworkServer.Spawn(ttt9);

        GameObject tt = (GameObject)Instantiate(water, new Vector3(transform.position.x, transform.position.y + .64f), Quaternion.identity);
        NetworkServer.Spawn(tt);

        GameObject tt2 = (GameObject)Instantiate(water, new Vector3(transform.position.x, transform.position.y + 1.28f), Quaternion.identity);
        NetworkServer.Spawn(tt2);

        GameObject tt3 = (GameObject)Instantiate(water, new Vector3(transform.position.x, transform.position.y - .64f), Quaternion.identity);
        NetworkServer.Spawn(tt3);

        GameObject tt4 = (GameObject)Instantiate(water, new Vector3(transform.position.x, transform.position.y - 1.28f), Quaternion.identity);
        NetworkServer.Spawn(tt4);
        NetworkServer.Destroy(gameObject);
    }

    
}

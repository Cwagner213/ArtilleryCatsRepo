//***********************************************************************************************************************************************************************************
// Bomb class to control in game bomb behavior, will be attached to bomb prefab
// Last Updated:  7/13/18 11:23 - Added use of different types of bombs IE fireball bombs from lava map
//***********************************************************************************************************************************************************************************

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class Bomb : NetworkBehaviour
{
    Vector2 gridPosition;

    public GameObject bombExplosion, bPlus, pPlus, dPlus, sand; //Gameobjects that the bomb spawns from its location

    public int bombCD = 3; //Seconds it takes a bomb to explode
    public int bombPower = 1; //Power bomb will explode with (tiles covered)
    public bool stop0 = false, stop1 = false, stop2 = false, stop3 = false, stop4 = false; //Used in explode function to track bomb time, should probably be local >.>
    public List<GameObject> explosions = new List<GameObject>();  //Keeps track of explosion objects created in bomb blast
    public GameObject homePlayer; // reference to the player who shot the bomb
    public AudioClip bombexplode;
    public int rGen = 0;

    public bool bStopMove = false;

    //***********************************************************************************************************************************************************************************
    //  Explode is called after the bomb has existed for 3 seconds,  this will destroy the bomb and spawn  bombexposion prefabs that can kill the player and burn down the crates
    //***********************************************************************************************************************************************************************************
    public void explode()
    {
        GetComponent<AudioSource>().PlayOneShot(bombexplode);

       

        explosions.Add((GameObject)Instantiate(bombExplosion, new Vector3(transform.position.x, transform.position.y), Quaternion.identity));

        //checks to see if the bomb  exploded on the litter box level, if so spawns quicksand under the blast
        if(GameObject.Find("DataStore").GetComponent<DataStore>().level == "4PLitter")
        {
            homePlayer.GetComponent<Player>().CmdSpawnQS(sand, transform.position.x, transform.position.y);
        }


        //Raycast array to store and find the location of objects around the bomb to see if they need to be destroyed
        RaycastHit2D[] hitCheck;
        hitCheck = Physics2D.RaycastAll(new Vector2(transform.position.x , transform.position.y), new Vector2(transform.position.x , transform.position.y), 0f);


        //Loop that checks the space under the initial bomb blast
        for (int j = 0; j < hitCheck.Length; j++)
        {
            if (hitCheck[j].transform.tag == "Wall" || hitCheck[j].transform.tag == "Crate")
            {

                if (hitCheck[j].transform.tag == "Crate" && stop0== false)
                {
                    rGen = Random.Range(0, 100);

                    //Block of code that spawns power ups if crates are destroyed in bomb blast, homeplayer is used to make the call to spawn the objects
                    if (rGen > 50 && rGen < 75)
                    {
                        homePlayer.GetComponent<Player>().CmdSpawnBuff(bPlus, hitCheck[j].transform.position.x, hitCheck[j].transform.position.y);
                    }
                    else if (rGen > 75)
                    {
                        homePlayer.GetComponent<Player>().CmdSpawnBuff(dPlus, hitCheck[j].transform.position.x, hitCheck[j].transform.position.y);
                    }
                    else if (rGen > 30 && rGen < 50)
                    {
                        homePlayer.GetComponent<Player>().CmdSpawnBuff(pPlus, hitCheck[j].transform.position.x, hitCheck[j].transform.position.y);
                    }
                    if (homePlayer.GetComponent<Player>().isServer)
                    {
                        homePlayer.GetComponent<Player>().CmdDestroyCrate(hitCheck[j].transform.gameObject);
                    }
                }
                stop0 = true;
            }
        }
        //Spawns the necessary explosion for the center
        if (stop0 == false && homePlayer.GetComponent<Player>().isServer)
        {
            explosions.Add((GameObject)Instantiate(bombExplosion, new Vector3(transform.position.x, transform.position.y), Quaternion.identity));
            NetworkServer.Spawn(explosions[explosions.Count - 1]);
            explosions.Clear();
        }

        //Limiter on the bomb power the player can achieve
        if (bombPower > 7 )
        {
            bombPower = 7;
        }

        //Loop for the bombs East row
        for(int i = 1; i < bombPower; i++)
        {
            hitCheck = Physics2D.RaycastAll(new Vector2(transform.position.x + (i * .64f), transform.position.y), new Vector2(transform.position.x + (i * .64f), transform.position.y), 0f);
            for(int j = 0; j < hitCheck.Length; j++)
            {
                if(hitCheck[j].transform.tag == "Wall" || hitCheck[j].transform.tag == "Crate")
                {
                  
                    if(hitCheck[j].transform.tag == "Crate" && stop1 == false)
                    {                   
                        rGen = Random.Range(0, 100);

                        if (rGen > 50 && rGen < 75)
                        {
                            homePlayer.GetComponent<Player>().CmdSpawnBuff(bPlus, hitCheck[j].transform.position.x, hitCheck[j].transform.position.y);
                        }
                        else if (rGen > 75)
                        {
                            homePlayer.GetComponent<Player>().CmdSpawnBuff(dPlus, hitCheck[j].transform.position.x, hitCheck[j].transform.position.y);

                        }
                        else if (rGen > 30 && rGen < 50)
                        {
                            homePlayer.GetComponent<Player>().CmdSpawnBuff(pPlus, hitCheck[j].transform.position.x, hitCheck[j].transform.position.y);
                        }
                        if (homePlayer.GetComponent<Player>().isServer)
                        {
                            homePlayer.GetComponent<Player>().CmdDestroyCrate(hitCheck[j].transform.gameObject);
                        }
                    }
                    stop1 = true;
                }
            }
            if (stop1 == false && homePlayer.GetComponent<Player>().isServer)
            {
                explosions.Add((GameObject)Instantiate(bombExplosion, new Vector3(transform.position.x + (i * .64f), transform.position.y), Quaternion.identity));
                NetworkServer.Spawn(explosions[explosions.Count -1]);
                explosions.Clear();
            }
        }
        stop1 = false;

        //West bomb explosion loop
        for (int i = 1; i < bombPower; i++)
        {
            hitCheck = Physics2D.RaycastAll(new Vector2(transform.position.x - (i * .64f), transform.position.y), new Vector2(transform.position.x - (i * .64f), transform.position.y), 0f);
            for (int j = 0; j < hitCheck.Length; j++)
            {
                if (hitCheck[j].transform.tag == "Wall" || hitCheck[j].transform.tag == "Crate")
                {
                    
                    if (hitCheck[j].transform.tag == "Crate" && stop2 == false)
                    {
                        rGen = Random.Range(0, 100);
                        if (rGen > 50 && rGen < 75)
                        {
                            homePlayer.GetComponent<Player>().CmdSpawnBuff(bPlus, hitCheck[j].transform.position.x, hitCheck[j].transform.position.y);
                        }
                        else if (rGen > 75)
                        {
                            homePlayer.GetComponent<Player>().CmdSpawnBuff(dPlus, hitCheck[j].transform.position.x, hitCheck[j].transform.position.y);

                        }
                        else if (rGen > 30 && rGen < 50)
                        {
                            homePlayer.GetComponent<Player>().CmdSpawnBuff(pPlus, hitCheck[j].transform.position.x, hitCheck[j].transform.position.y);
                        }
                        if (homePlayer.GetComponent<Player>().isServer)
                        {
                            homePlayer.GetComponent<Player>().CmdDestroyCrate(hitCheck[j].transform.gameObject);
                        }
                    }
                    stop2 = true;
                }
            }
            if (stop2 == false && homePlayer.GetComponent<Player>().isServer)
            {
                explosions.Add((GameObject)Instantiate(bombExplosion, new Vector3(transform.position.x - (i * .64f), transform.position.y), Quaternion.identity));
                NetworkServer.Spawn(explosions[explosions.Count - 1]);
                explosions.Clear();
            }
        }
        stop2 = false;

        //Noth bomb explosion loop
        for (int i = 1; i < bombPower; i++)
        {
            hitCheck = Physics2D.RaycastAll(new Vector2(transform.position.x, transform.position.y + (i *.64f)), new Vector2(transform.position.x, transform.position.y + (i * .64f)), 0f);
            for (int j = 0; j < hitCheck.Length; j++)
            {
                if (hitCheck[j].transform.tag == "Wall" || hitCheck[j].transform.tag == "Crate")
                {
                   
                    if (hitCheck[j].transform.tag == "Crate" && stop3 == false)
                    {
                        rGen = Random.Range(0, 100);
                        if (rGen > 50 && rGen < 75)
                        {                                                    
                            homePlayer.GetComponent<Player>().CmdSpawnBuff(bPlus, hitCheck[j].transform.position.x, hitCheck[j].transform.position.y);
                        }
                        else if (rGen > 75)
                        {                                                 
                            homePlayer.GetComponent<Player>().CmdSpawnBuff(dPlus, hitCheck[j].transform.position.x, hitCheck[j].transform.position.y);
                          
                        }
                        else if (rGen > 30 && rGen < 50)
                        {                         
                            homePlayer.GetComponent<Player>().CmdSpawnBuff(pPlus, hitCheck[j].transform.position.x, hitCheck[j].transform.position.y);
                        }
                        if (homePlayer.GetComponent<Player>().isServer)
                        {
                            homePlayer.GetComponent<Player>().CmdDestroyCrate(hitCheck[j].transform.gameObject);
                        }
                    }
                    stop3 = true;
                }
            }
            if (stop3 == false && homePlayer.GetComponent<Player>().isServer)
            {
                explosions.Add((GameObject)Instantiate(bombExplosion, new Vector3(transform.position.x, transform.position.y + (i * .64f)), Quaternion.identity));
                NetworkServer.Spawn(explosions[explosions.Count -1]);
                explosions.Clear();
            }
        }
        stop3 = false;

        //Southern Bomb explosion loop
        for (int i = 1; i < bombPower; i++)
        {
            hitCheck = Physics2D.RaycastAll(new Vector2(transform.position.x, transform.position.y - (i * .64f)), new Vector2(transform.position.x, transform.position.y - (i * .64f)), 0f);
            for (int j = 0; j < hitCheck.Length; j++)
            {
                if (hitCheck[j].transform.tag == "Wall" || hitCheck[j].transform.tag == "Crate")
                {                  
                    if (hitCheck[j].transform.tag == "Crate" && stop4 == false)
                    {
                        rGen = Random.Range(0, 100);
                        if (rGen > 50 && rGen < 75)
                        {
                            homePlayer.GetComponent<Player>().CmdSpawnBuff(bPlus, hitCheck[j].transform.position.x, hitCheck[j].transform.position.y);
                        }
                        else if (rGen > 75)
                        {
                            homePlayer.GetComponent<Player>().CmdSpawnBuff(dPlus, hitCheck[j].transform.position.x, hitCheck[j].transform.position.y);

                        }
                        else if (rGen > 30 && rGen < 50)
                        {
                            homePlayer.GetComponent<Player>().CmdSpawnBuff(pPlus, hitCheck[j].transform.position.x, hitCheck[j].transform.position.y);
                        }
                        if (homePlayer.GetComponent<Player>().isServer)
                        {
                            homePlayer.GetComponent<Player>().CmdDestroyCrate(hitCheck[j].transform.gameObject);
                        }
                    }
                    stop4 = true;
                }
            }
            if (stop4 == false && homePlayer.GetComponent<Player>().isServer)
            {
                explosions.Add((GameObject)Instantiate(bombExplosion, new Vector3(transform.position.x, transform.position.y - (i * .64f)), Quaternion.identity));
                NetworkServer.Spawn(explosions[explosions.Count - 1]);
                explosions.Clear();
            }
           
        }
        stop4 = false;
        homePlayer.GetComponent<Player>().CmdRestoreBomb(homePlayer);
        StartCoroutine(deleteSelf());
    }


    //***********************************************************************************************************************************************************************************
    // CoRoutine that kills the bomb off one second after the routine is called
    //***********************************************************************************************************************************************************************************
    public IEnumerator deleteSelf()
    {
        yield return new WaitForSeconds(1);

        NetworkServer.Destroy(gameObject);
    }

    //***********************************************************************************************************************************************************************************
    // Helper function to start the CoRoutine to move the bomb 
    //***********************************************************************************************************************************************************************************
    public void startMove(Vector3 end)
    {
        StartCoroutine(move(end));
    }

    //***********************************************************************************************************************************************************************************
    // Helper function to start the CoRoutine to the slow move function
    //***********************************************************************************************************************************************************************************
    public void startSlowMove(Vector3 end)
    {
        StartCoroutine(slowMove(end));
    }

    //***********************************************************************************************************************************************************************************
    // Moves the bomb at a slow rate to its destination, takes one Vector3 as an argument which is the bombs intended end point on the grid
    // The Vector3 represents the gamespace grid and not the map grid
    //***********************************************************************************************************************************************************************************
    public IEnumerator slowMove(Vector3 end)
    {

        float dif = (transform.position - end).sqrMagnitude;
        while (dif > float.Epsilon)
        {
            Vector3 newPos = Vector3.MoveTowards(transform.position, end, Time.deltaTime * (float).8);
            transform.position = newPos;
            dif = (transform.position - end).sqrMagnitude;
            yield return null;
        }
        GetComponent<BoxCollider2D>().enabled = true;
        bStopMove = true;
    }

    //***********************************************************************************************************************************************************************************
    // Moves the bomb at a normal rate to its destination, takes one Vector3 as an argument which is the bombs intended end point on the grid
    // The Vector3 represents the gamespace grid and not the map grid
    //***********************************************************************************************************************************************************************************
    public IEnumerator move(Vector3 end)
    {
      
        float dif = (transform.position - end).sqrMagnitude;
        while (dif > float.Epsilon)
        {
            Vector3 newPos = Vector3.MoveTowards(transform.position, end, Time.deltaTime * (float)3);
            transform.position = newPos;
            dif = (transform.position - end).sqrMagnitude;
            yield return null;
        }
        GetComponent<BoxCollider2D>().enabled = true;
        bStopMove = true;
    }


    //***********************************************************************************************************************************************************************************
    // This is the extended timer used for the fireball bombs on the fire map
    //***********************************************************************************************************************************************************************************
    public IEnumerator bombTimer2()
    {

        yield return new WaitForSeconds(1);
        GetComponent<BoxCollider2D>().enabled = true; //Sets the bomb as a physical game object when it starts travelling to its location (can hit players)
        yield return new WaitForSeconds(9);
        explode(); //The fire bomb takes longer to explode because of the large distance it needs to travel from out of map to its intended detonation point 
    }
    //***********************************************************************************************************************************************************************************
    //This is the normal timer for player shot bombs, they will explode in 3 seconds
    //***********************************************************************************************************************************************************************************
    public IEnumerator bombTimer()
    {

        yield return new WaitForSeconds(1);
        GetComponent<BoxCollider2D>().enabled = true; //Sets the bomb as a physical game object when it starts travelling to its location (can hit players)
        yield return new WaitForSeconds(2);
        explode();
    }

    //***********************************************************************************************************************************************************************************
    // Calculate Trajectory is called from the player who shoots the bomb(or gamecontroller) in order to find out where the bomb should land relative to where the player is
    // 
    // Calculate Trajectory takes 5 parameters, A vector2 start which represents the tile the player is standing on currently, facing which is the direction the bomb is being shot from 
    // start, power which is the length it will travel from the player that shot it, bpower which is the length of the explosion the bomb will create, and bType which distinguishes 
    // whether it is a fireball or normal bomb.  If bType is normal it will read as "normal" if its a fireball it will read as "fireball"
    //
    // Returns a Vector2 which represents the tile that the bomb should land in
    //***********************************************************************************************************************************************************************************
    public Vector2 CalculateTrajectory(Vector2 start, char facing, int power, int bpower, string bType)
    {
        bombPower = bombPower + bpower + 3; //Sets the bomb explosion length for this instance of bomb

        Vector2 endLocation = new Vector2(); //creates Vector2 to hold the end location of the bomb

        //This block of code figures out the location of the tile the bomb will land on given the facing of the player
        if(facing == 'N')
        {
            endLocation = new Vector2(start.x+power, start.y);
        }
        else if (facing == 'E')
        {
            endLocation = new Vector2(start.x, start.y + power);
        }
        else if(facing == 'S')
        {
            endLocation = new Vector2(start.x - power, start.y);
        }
        else if(facing == 'W')
        {
            endLocation = new Vector2(start.x, start.y - power);
        }

        //Finds the map limits of the current map 
        int tX = GameObject.Find("DataStore").GetComponent<DataStore>().x;
        int tY = GameObject.Find("DataStore").GetComponent<DataStore>().y;

        //Makes sure the bmob does not cross those limits and if it does sets the bomb to the closest legal spot to where it intended to go
        if (endLocation.y > tY)
        {
            endLocation.y = tY;
        }
        if(endLocation.y < 0)
        {
            endLocation.y = 0;
        }
        if(endLocation.x > tX)
        {
            endLocation.x = tX;
        }
        if(endLocation.x < 0)
        {
            endLocation.x = 0;
        }

        //updated return value containing the location the bomb should travel to 
        Vector2 returnValue = new Vector2(GameObject.Find("" + endLocation.x +"_"+ endLocation.y).transform.position.x, GameObject.Find("" + endLocation.x + "_" + endLocation.y).transform.position.y);

        //checks to see if the bomb is a fire bomb or not
        if (bType == "normal")
        {
            StartCoroutine(bombTimer());
        }
        else
        {
            StartCoroutine(bombTimer2());
        }
       
        return returnValue;
    }
}

//***********************************************************************************************************************************************************************************
// Volcano controller is nearly identical to GameController except it spews random fireballs onto the play field, this should probably be just an if statement in 
// gamecontrollers update function but origally there were a lot more maps intended and I thought it would get messy to have it all in one class. Only the fireball spawning is
// commented for other reference look at GameController
// Last Updated: 7/26/18 12pm
//***********************************************************************************************************************************************************************************

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class VolcanoController : NetworkBehaviour {

    [SyncVar]
    public int p1Score, p2Score, p3Score, p4Score, p5Score, p6Score, p7Score, p8Score;
    public int[] scores = new int[8];
    int maxScore = 3;
    public int done = 0;
    public int deadCount = 0;
    public int locked = 0;

    public int xRan, yRan;
    public bool recentScore = false;
    public int FireBallTimer = 0;
    public GameObject FireBall, Target;
    // Use this for initialization
    void Start()
    {
        //Server only
        if (isServer)
        {


          
            StartCoroutine(LoadInIDs());
        }
        StartCoroutine(startMatch());
        //Run on all clients
        GameObject[] temp = GameObject.FindGameObjectsWithTag("Player");
        int tempx = 1;
        for (int i = 0; i < temp.Length; i++)
        {
            temp[i].transform.position = GameObject.Find("Spawn Position " + tempx).transform.position;
            temp[i].GetComponent<SpriteRenderer>().enabled = true;
            temp[i].GetComponent<BoxCollider2D>().enabled = true;
            temp[i].GetComponent<Player>().dead = false;
            temp[i].GetComponent<Player>().bombPower = 0;
            temp[i].GetComponent<Player>().bombNumber = 1;
            temp[i].GetComponent<Player>().bombDistance = 0;
            temp[i].GetComponent<Player>().bombAvailableDistance = 0;
            temp[i].GetComponent<Player>().bombsAvailable = 1;
            tempx++;
        }
    }



    public IEnumerator startMatch()
    {
        yield return new WaitForSeconds(3);
        GameObject.Find("LoadingCanvas").GetComponent<Canvas>().enabled = false;

    }
    //***********************************************************************************************************************************************************************************
    //  Controls the timimg of the random fireballs being shot into the match, Shoots a fireball every 2 seconds
    //***********************************************************************************************************************************************************************************
    private IEnumerator fireCount()
    {
        if (isServer)
        {
         
            GameObject fire = null;
            GameObject[] list = GameObject.FindGameObjectsWithTag("Player");


            yield return new WaitForSeconds(2); //Controls the timing of the fireballs being shot

            //generates the random numbers for fireball placement depending on the size of the map
            if (GameObject.Find("DataStore").GetComponent<DataStore>().level == "8PVolcano")
            {
                Debug.Log("VOCLANO");
                xRan = Random.Range(1, 10);
                yRan = Random.Range(1, 16);
            }
            else if (GameObject.Find("DataStore").GetComponent<DataStore>().level == "4PVolcano")
            {
                xRan = Random.Range(1, 10);
                yRan = Random.Range(1, 8);
            }

            //Uses the player who is hosting the game to shoot the fireball
            for (int j = 0; j < list.Length; j++)
            {
                if (list[j].GetComponent<Player>().isServer)
                {
                    list[j].GetComponent<Player>().ShootBombNPC(0, 0, new Vector3(xRan, yRan + 13), xRan, yRan);
                }
            }
        }

        FireBallTimer = 0;
        
    }

   
    public IEnumerator returnToMapSelect()
    {
        yield return new WaitForSeconds(10);

        GameObject[] temp = GameObject.FindGameObjectsWithTag("Player");

        for (int i = 0; i < temp.Length; i++)
        {
            if (temp[i].GetComponent<Player>().isServer)
            {
                temp[i].GetComponent<Player>().runMapSelector();
            }
        }

    }

    private IEnumerator LoadInIDs()
    {
        yield return new WaitForSeconds(1);

        RpcSetScore();
        RpcSetID();

        GameObject[] temp3 = GameObject.FindGameObjectsWithTag("Player");
        Debug.Log("Number of players in game is " + temp3.Length);
        for (int i = 0; i < temp3.Length; i++)
        {
            if (temp3[i].GetComponent<Player>().getPlayerId() == 0)
            {
                temp3[i].GetComponent<Player>().setPlayerId(i + 1);
            }
        }
    }

    [ClientRpc]
    public void RpcSetID()
    {
        GameObject[] temp3 = GameObject.FindGameObjectsWithTag("Player");
        Debug.Log("Number of players in game is " + temp3.Length);
        for (int i = 0; i < temp3.Length; i++)
        {
            if (temp3[i].GetComponent<Player>().getPlayerId() == 0)
            {
                temp3[i].GetComponent<Player>().setPlayerId(i + 1);
            }
        }
    }

    [ClientRpc]
    public void RpcSetScore()
    {
        scores[0] = p1Score;
        scores[1] = p2Score;
        scores[2] = p3Score;
        scores[3] = p4Score;
        scores[4] = p5Score;
        scores[5] = p6Score;
        scores[6] = p7Score;
        scores[7] = p8Score;
        Debug.Log("Is this even happening");
        for (int i = 0; i < 8; i++)
        {
            if (scores[i] == 1)
            {
                GameObject.Find("P" + (i + 1) + "B1").GetComponent<Image>().color = Color.green;
            }
            else if (scores[i] == 2)
            {
                GameObject.Find("P" + (i + 1) + "B1").GetComponent<Image>().color = Color.green;
                GameObject.Find("P" + (i + 1) + "B2").GetComponent<Image>().color = Color.green;
            }
        }
    }


    // Update is called once per frame
    void Update()
    {
      

        if (FireBallTimer == 0)
        {
            StartCoroutine(fireCount());
            FireBallTimer = 1;
        }
    }

   
    public void CheckForVicotry()
    {
        if (isServer)
        {
            GameObject[] temp = GameObject.FindGameObjectsWithTag("Player");
            int aliveCount = 0;
            GameObject winningPlayer = null;

            for (int i = 0; i < temp.Length; i++)
            {
                if (temp[i].GetComponent<Player>().dead == false)
                {
                    winningPlayer = temp[i];
                    aliveCount++;
                }
            }

            if (aliveCount <= 1)
            {
                PlayerWon(winningPlayer);
                StartCoroutine(waitToReset());
            }
        }
    }

    public void PlayerWon(GameObject player)
    {
        Debug.Log("UniqueID of player won is " + player.GetComponent<Player>().getPlayerId());
      
        RpcPlayerWon(player);
        locked = 1;
    }
    public IEnumerator scoreTimerFix()
    {
        yield return new WaitForSeconds(1);
        recentScore = false;
    }
    [ClientRpc]
    public void RpcPlayerWon(GameObject player)
    {
        if (recentScore == false)
        {
            recentScore = true;
            StartCoroutine(scoreTimerFix());
            scores[player.GetComponent<Player>().getPlayerId() - 1]++;
            Debug.Log("Howmanytimesdoesthishappen");
            //canvas popup with score here

            for (int i = 0; i < 8; i++)
            {

                if (scores[i] == 1)
                {
                    GameObject.Find("Canvas").GetComponent<Canvas>().enabled = true;
                    GameObject.Find("P" + (i + 1) + "B1").GetComponent<Image>().color = Color.green;
                }
                else if (scores[i] == 2)
                {
                    GameObject.Find("Canvas").GetComponent<Canvas>().enabled = true;
                    GameObject.Find("P" + (i + 1) + "B1").GetComponent<Image>().color = Color.green;
                    GameObject.Find("P" + (i + 1) + "B2").GetComponent<Image>().color = Color.green;
                }
                //If player wins match
                else if (scores[i] == 3)
                {
                    GameObject.Find("Canvas").GetComponent<Canvas>().enabled = false;

                    scores[0] = 0;
                    scores[1] = 0;
                    scores[2] = 0;
                    scores[3] = 0;
                    scores[4] = 0;
                    scores[5] = 0;
                    scores[6] = 0;
                    scores[7] = 0;
                    p1Score = scores[0];
                    p2Score = scores[1];
                    p3Score = scores[2];
                    p4Score = scores[3];
                    p5Score = scores[4];
                    p6Score = scores[5];
                    p7Score = scores[6];
                    p8Score = scores[7];
                    GameObject.Find("P" + (i + 1) + "B1").GetComponent<Image>().color = Color.green;
                    GameObject.Find("P" + (i + 1) + "B2").GetComponent<Image>().color = Color.green;
                    GameObject.Find("P" + (i + 1) + "B3").GetComponent<Image>().color = Color.green;
                    PlayerPrefs.DeleteAll();
                    GameObject.Find("NextMatchText").GetComponent<Text>().text = "Player " + player.GetComponent<Player>().getPlayerId() + " is the winner!";
                    locked = 3;
                    GameObject.Find("Main Camera").GetComponent<Camera>().orthographicSize = 1;


                    GameObject.Find("Main Camera").transform.position = new Vector3(player.transform.position.x, player.transform.position.y, -10);

                    GameObject.Find("WinCanvas").GetComponent<Canvas>().enabled = true;
                    if (isServer)
                    {
                        StartCoroutine(returnToMapSelect());
                    }
                }
            }

        }
        locked = 1;
    }

 


    public IEnumerator waitToReset()
    {
        yield return new WaitForSeconds(6);
        GameObject.Find("Canvas").GetComponent<Canvas>().enabled = false;
        resetGame();

    }


    public void resetGame()
    {

        GameObject[] powerList = GameObject.FindGameObjectsWithTag("PowerUp");
        for (int i = 0; i < powerList.Length; i++)
        {
            NetworkServer.Destroy(powerList[i]);
        }

   
        RpcResetBoard();
        //NetworkManager.singleton.ServerChangeScene("MainScene");
    }

    [ClientRpc]
    public void RpcResetBoard()
    {

        GameObject.Find("Canvas").GetComponent<Canvas>().enabled = false;
        GameObject[] list = GameObject.FindGameObjectsWithTag("Crate");
        for (int i = 0; i < list.Length; i++)
        {
            list[i].GetComponent<SpriteRenderer>().enabled = true;
            list[i].GetComponent<BoxCollider2D>().enabled = true;
        }

        GameObject[] pList = GameObject.FindGameObjectsWithTag("Player");
        int tempx = 1;
        for (int i = 0; i < pList.Length; i++)
        {
            pList[i].transform.position = GameObject.Find("Spawn Position " + tempx).transform.position;
            pList[i].GetComponent<SpriteRenderer>().enabled = true;
            pList[i].GetComponent<BoxCollider2D>().enabled = true;
            pList[i].GetComponent<Player>().dead = false;
            pList[i].GetComponent<Player>().bombPower = 0;
            pList[i].GetComponent<Player>().bombNumber = 1;
            pList[i].GetComponent<Player>().bombDistance = 1;
            pList[i].GetComponent<Player>().bombAvailableDistance = 1;
            pList[i].GetComponent<Player>().bombsAvailable = 1;
            tempx++;
        }
        tempx = 0;
        PlayerPrefs.SetInt("p1Score", scores[0]);
        PlayerPrefs.SetInt("p2Score", scores[1]);
        PlayerPrefs.SetInt("p3Score", scores[2]);
        PlayerPrefs.SetInt("p4Score", scores[3]);
        PlayerPrefs.SetInt("p5Score", scores[4]);
        PlayerPrefs.SetInt("p6Score", scores[5]);
        PlayerPrefs.SetInt("p7Score", scores[6]);
        PlayerPrefs.SetInt("p8Score", scores[7]);
    }
}


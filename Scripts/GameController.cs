//***********************************************************************************************************************************************************************************
// The Gamecontroller class takes care of tracking who wins rounds and controlling the flow of the game 
// Last Updated:  7/13/18 1:28 - Made it look less bad 
//***********************************************************************************************************************************************************************************

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class GameController : NetworkBehaviour {

    [SyncVar]
    public int p1Score, p2Score, p3Score, p4Score, p5Score, p6Score, p7Score, p8Score;
    public int[] scores = new int[8];
    int maxScore = 3;
    public int done = 0;
    public int deadCount = 0;
    public int locked = 0;
    public bool recentScore = false;

	// Use this for initialization
	void Start () {
        //Server only
        if (isServer)
        {
           
         
            StartCoroutine(LoadInIDs());
        }

        //Run on all clients, sets initial game settings for all players
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
            temp[i].GetComponent<Player>().bombDistance = 1;
            temp[i].GetComponent<Player>().bombAvailableDistance = 1;
            temp[i].GetComponent<Player>().bombsAvailable = 1;
            tempx++;
        }
    }


    //***********************************************************************************************************************************************************************************
    // returnToMapSelect returns all of the clients to map select after a match is over
    //***********************************************************************************************************************************************************************************
    public IEnumerator returnToMapSelect()
    {
        yield return new WaitForSeconds(10);
        
        GameObject[] playerList= GameObject.FindGameObjectsWithTag("Player"); //Makes a list of all players in the game
    
        for (int i = 0; i < playerList.Length; i++)
        {
            if(playerList[i].GetComponent<Player>().isServer)
            {
                playerList[i].GetComponent<Player>().runMapSelector();
            }
        }

    }

    //***********************************************************************************************************************************************************************************
    // LoadInIds Sets all of the players uniqueIDs in the order they joined the game the host starting with the value 1
    //***********************************************************************************************************************************************************************************
    private IEnumerator LoadInIDs()
    {
        yield return new WaitForSeconds(1);

        RpcSetScore();
        RpcSetID();//sets the ID of the local cat on all clients

        GameObject[] playerList = GameObject.FindGameObjectsWithTag("Player");

        for (int i = 0; i < playerList.Length; i++)
        {
            if (playerList[i].GetComponent<Player>().getPlayerId() == 0)
            {
                playerList[i].GetComponent<Player>().setPlayerId(i+1);
            }
        }
    }


    //***********************************************************************************************************************************************************************************
    // RpcSetID sets the uniqueID of all players on all clients
    //***********************************************************************************************************************************************************************************
    [ClientRpc]
    public void RpcSetID()
    {
        GameObject[] playerList = GameObject.FindGameObjectsWithTag("Player");

        for (int i = 0; i < playerList.Length; i++)
        {
            if (playerList[i].GetComponent<Player>().getPlayerId() == 0)
            {
                playerList[i].GetComponent<Player>().setPlayerId(i+1);
            }
        }
    }


    //***********************************************************************************************************************************************************************************
    //  RpcSetScore Sets the scores on all clients and updates the score boards
    //***********************************************************************************************************************************************************************************
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

        //Sets the scoreboards on all clients
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

    //***********************************************************************************************************************************************************************************
    // CheckForVictory checks to see if there is a winner after every cat death, if there is the game will announce the winner and go back to map select
    //***********************************************************************************************************************************************************************************
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

    //***********************************************************************************************************************************************************************************
    // PlayerWon is to be ran if a player wins a round
    //***********************************************************************************************************************************************************************************
    public void PlayerWon(GameObject player)
    {  
        RpcPlayerWon(player);      
        locked = 1; //Makes sure the game doesnt continue after someone wins
    }

    //***********************************************************************************************************************************************************************************
    // scoreTimerFix prevents double counting scores by introducing a timer of 1 second inbetween being able to win a round
    //***********************************************************************************************************************************************************************************
    public IEnumerator scoreTimerFix()
    {
        yield return new WaitForSeconds(1);
        recentScore = false;
    }

    //***********************************************************************************************************************************************************************************
    //  RpcPlayerWon updates all of clients scoreboards and checks to see if someone won the whole match
    //
    //  Takes one parameter which is the gameobject for the player that won
    //***********************************************************************************************************************************************************************************
    [ClientRpc]
    public void RpcPlayerWon(GameObject player)
    {
        if (recentScore == false)
        {
            recentScore = true;
            StartCoroutine(scoreTimerFix());
            scores[player.GetComponent<Player>().getPlayerId() - 1]++;
      
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
                    //resets all the points if someone wins a match
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

    //***********************************************************************************************************************************************************************************
    //  waitToReset is a timer to reset the map after a round ends
    //***********************************************************************************************************************************************************************************
    public IEnumerator waitToReset()
    {
        yield return new WaitForSeconds(6);
        GameObject.Find("Canvas").GetComponent<Canvas>().enabled = false;
        resetGame();

    }

    //***********************************************************************************************************************************************************************************
    // resetGame resets the game after the timer goes off at round end
    //***********************************************************************************************************************************************************************************
    public void resetGame()
    {
        //Destroys all spare powerups on the board
        GameObject[] powerList = GameObject.FindGameObjectsWithTag("PowerUp");
        for(int i = 0; i < powerList.Length; i++)
        {
            NetworkServer.Destroy(powerList[i]);
        }
    
        RpcResetBoard(); //Resets the rest of the objects on individual clients
    }

    //***********************************************************************************************************************************************************************************
    //  RpcResetBoard Resets all gameobjects on the gameboard for all clients, including crates and player status
    //***********************************************************************************************************************************************************************************
    [ClientRpc]
    public void RpcResetBoard()
    {
        GameObject.Find("Canvas").GetComponent<Canvas>().enabled = false;
        //Finds all the crates and sets them all to alive and active
        GameObject[] crateList = GameObject.FindGameObjectsWithTag("Crate");
        for (int i = 0; i < crateList.Length; i++)
        {
            crateList[i].GetComponent<SpriteRenderer>().enabled = true;
            crateList[i].GetComponent<BoxCollider2D>().enabled = true;
        }

        //Finds all players and sets them to alive and active
        GameObject[] pList = GameObject.FindGameObjectsWithTag("Player");
        
        for (int i = 0; i < pList.Length; i++)
        {
            pList[i].transform.position = GameObject.Find("Spawn Position " + (i+1)).transform.position;
            pList[i].GetComponent<SpriteRenderer>().enabled = true;
            pList[i].GetComponent<BoxCollider2D>().enabled = true;
            pList[i].GetComponent<Player>().dead = false;
            pList[i].GetComponent<Player>().bombPower = 0;
            pList[i].GetComponent<Player>().bombNumber = 1;
            pList[i].GetComponent<Player>().bombDistance = 1;
            pList[i].GetComponent<Player>().bombAvailableDistance = 1;
            pList[i].GetComponent<Player>().bombsAvailable = 1;
        }
        //Resets all scores
        for(int i = 0; i < 8; i++)
        {
            PlayerPrefs.SetInt("p" + (i + 1) + "Score", scores[i]);
        }
     
    }
   
}

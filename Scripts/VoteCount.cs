//***********************************************************************************************************************************************************************************
// VoteCount handles the voting done from the map selector menu to select the next map to play on
// Last Updated: 7/25/2018 1:05am
//***********************************************************************************************************************************************************************************
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using Steamworks;

public class VoteCount : MonoBehaviour {

    public int[] arr = new int[8];
    public bool playerCheckWait = false;
    [HideInInspector]
    public bool gameClosed = true;

    public int countDown = 15;

    //***********************************************************************************************************************************************************************************
    //  Update is ran once every frame, it checks to make sure the menu is correctly updated showing how many players are in the game
    //***********************************************************************************************************************************************************************************
    void Update()
    {

        if (playerCheckWait == false)
        {
            StartCoroutine(playerCheck()); //Checks to see how amny players are in the lobby
            playerCheckWait = true;
        }


    }

    //***********************************************************************************************************************************************************************************
    // openCloseGame is attached to the opengame button in the map select menu, it will either open or close the game to the public depending on which option is already selected
    //***********************************************************************************************************************************************************************************
    public void openCloseGame()
    {
        //Opens the game to the public
        if(gameClosed == true)
        {
            GameObject.Find("Network Manager").GetComponent<networkManage>().openGame();
            GameObject.Find("OpenGameText").GetComponent<Text>().text = "Close Game";
            gameClosed = false;
        }
        //Closes the game to the public
        else
        {
            GameObject.Find("Network Manager").GetComponent<networkManage>().closeGame();
            GameObject.Find("OpenGameText").GetComponent<Text>().text = "Open Game";
            gameClosed = true;
        }
    }

    //***********************************************************************************************************************************************************************************
    //  playerCheck checks to see how many players are in the game and updated the lobby to show how many people are in game
    //***********************************************************************************************************************************************************************************
    public IEnumerator playerCheck()
    {
        yield return new WaitForSeconds(1);
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        for(int i = 0; i < 8; i++)
        {
            GameObject.Find("PlayerName" + (i + 1)).GetComponent<Text>().text = "";
        }
        for (int i = 0; i < players.Length; i++)
        {
            GameObject.Find("PlayerName" + (i + 1)).GetComponent<Text>().text = "Cat";
;        }

        playerCheckWait = false;
    }

    //***********************************************************************************************************************************************************************************
    //  Countdown is triggered when the host chooses the map to play on, it starts a countdown on all of the players screens to when the match starts
    //***********************************************************************************************************************************************************************************
    public IEnumerator CountDown()
    {
        GameObject.Find("Network Manager").GetComponent<networkManage>().closeGame();
        GameObject[] players2 = GameObject.FindGameObjectsWithTag("Player");
        players2[0].GetComponent<Player>().CmdCountMap();
        GameObject.Find("CountText").GetComponent<Text>().enabled = true;
        for (int i = 0; i < 5; i++)
        {
            yield return new WaitForSeconds(1);  
            //GameObject.Find("CountText").GetComponent<Text>().text = "" + (int.Parse(GameObject.Find("CountText").GetComponent<Text>().text) - 1);
        }

        int maxVal = arr.Max();
        int maxIndex = arr.ToList().IndexOf(maxVal);

       


        //Block of code that checks to see which map the host has chosen to play on, if they chose a 4 player map with more than 4 players it will
        //automaticlaly move them to the 8 player version of that map
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].GetComponent<Player>().isServer)
            {
                if (maxIndex == 0 )
                {
                    if (players.Length > 4)
                    {
                        players[i].GetComponent<Player>().runMap5();
                    }
                    else
                    {
                        players[i].GetComponent<Player>().runMap1();
                    }
                }
                else if(maxIndex == 1)
                {
                    if (players.Length > 4)
                    {
                        players[i].GetComponent<Player>().runMap6();
                    }
                    else
                    {
                        players[i].GetComponent<Player>().runMap2();
                    }
                }
                else if(maxIndex == 2)
                {
                    if (players.Length > 4)
                    {
                        players[i].GetComponent<Player>().runMap7();
                    }
                    else
                    {
                        players[i].GetComponent<Player>().runMap3();
                    }
                }
                else if (maxIndex == 3)
                {
                    if (players.Length > 4)
                    {
                        players[i].GetComponent<Player>().runMap8();
                    }
                    else
                    {
                        players[i].GetComponent<Player>().runMap4();
                    }
                }
                else if (maxIndex == 4)
                {
                    players[i].GetComponent<Player>().runMap5();
                }
                else if (maxIndex == 5)
                {
                    players[i].GetComponent<Player>().runMap6();
                }
                else if (maxIndex == 6)
                {
                    players[i].GetComponent<Player>().runMap7();
                }
                else if (maxIndex == 7)
                {
                    players[i].GetComponent<Player>().runMap8();
                }
            }
        }
    }


    //***********************************************************************************************************************************************************************************
    // selectMap 1 - 8 chooses the map to play on (host only) They are connected to the corresponding map selection buttons on the map selection menu
    //***********************************************************************************************************************************************************************************
    public void selectMap1()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].GetComponent<Player>().isServer)
            {
                players[i].GetComponent<Player>().selectMap1();
                StartCoroutine(CountDown());
            }
        }
    }
    public void selectMap2()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].GetComponent<Player>().isServer)
            {
                players[i].GetComponent<Player>().selectMap2();
                StartCoroutine(CountDown());
            }
        }
    }

    public void selectMap3()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].GetComponent<Player>().isServer)
            {
                players[i].GetComponent<Player>().selectMap3();
                StartCoroutine(CountDown());
            }
        }
    }
    public void selectMap4()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].GetComponent<Player>().isServer)
            {
                players[i].GetComponent<Player>().selectMap4();
                StartCoroutine(CountDown());
            }
        }
    }
    public void selectMap5()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].GetComponent<Player>().isServer)
            {
                players[i].GetComponent<Player>().selectMap5();
                StartCoroutine(CountDown());
            }
        }
    }
    public void selectMap6()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].GetComponent<Player>().isServer)
            {
                players[i].GetComponent<Player>().selectMap6();
                StartCoroutine(CountDown());
            }
        }
    }
    public void selectMap7()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].GetComponent<Player>().isServer)
            {
                players[i].GetComponent<Player>().selectMap7();
                StartCoroutine(CountDown());
            }
        }
    }
    public void selectMap8()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].GetComponent<Player>().isServer)
            {
                players[i].GetComponent<Player>().selectMap8();
                StartCoroutine(CountDown());
            }
        }
    }

}

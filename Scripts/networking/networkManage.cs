
//***********************************************************************************************************************************************************************************
// NetworkManage class to control multiplayer connections, will be permanently on the do not destroy list.  
// Last Updated:  7/13/18 1:28 - Made it look less bad 
//***********************************************************************************************************************************************************************************

using NATTraversal;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.Networking.NetworkSystem;
using UnityEngine.Networking.Types;
using UnityEngine.SceneManagement;

public class networkManage : NATTraversal.NetworkManager {

    public string gameMode = "quickjoin"; //Default gamemode is quickjoin
    public string gameName = ""; //Name of the game that will be created by host by default
    public string gamePass = ""; //Password for a hosted game by default 
    public bool isTheServer = false;  //keeps track of the player who is hosting the match
    public bool privacySetting = false; //Sets the game to listed or unlisted on matchamking server
    public double thisMatch; //The match ID on the matchmaking server

    public override void Start()
    {
        base.Start();
        Time.timeScale = 1;
    }

    //***********************************************************************************************************************************************************************************
    // Called when a player hits the Start button on the hosting menu, lists a match on the Matchmaking server using the default gameName and gamePass
    //***********************************************************************************************************************************************************************************

    public void Host()
    {
        NetworkServer.Reset(); //Makes sure that the potential host's network manager is clear
        isTheServer = true; //If someone hosts the match it is recorded here that they are hosting

        if (matchMaker == null) matchMaker = gameObject.AddComponent<NetworkMatch>();
        StartMatchMaker(); //Opens up the game to recieve connections from the Matchmaking server

        string gamename = GameObject.Find("GameName").GetComponent<InputField>().text + "`" + GameObject.Find("Password").GetComponent<InputField>().text + "`"; //Sets the game name and password into the gamename to be passed to the MM server

        //Block of code that determines whether the game had a password or not and stores that info in the gamename
        if(GameObject.Find("Password").GetComponent<InputField>().text != "")
        {
            gamename = gamename + "p";
        }
        else
        {
            gamename = gamename + "o";
        }

        //Changes the players UI to reflect that they are hosting a match
        GameObject.Find("HostCanvas").GetComponent<Canvas>().enabled = false;
        GameObject.Find("MapSelectCanvas").GetComponent<Canvas>().enabled = true;
        GameObject.Find("GamePasswordCanvas").GetComponent<Canvas>().enabled = false;
        GameObject.Find("Disconnect").GetComponent<Button>().enabled = true;
        GameObject.Find("Disconnect").GetComponent<Image>().enabled = true;

        GameObject.Find("DisconnectText").GetComponent<Text>().enabled = true;

        //Final go on hosting the match, sends game name to the matchmaking server with the maximum size of the match
        StartHostAll(gamename, customConfig ? (uint)(maxConnections + 1) : matchSize);
          
    }

    //***********************************************************************************************************************************************************************************
    // Sets the value of thisMatch to the current matches MatchID when a game is created
    //***********************************************************************************************************************************************************************************
    public override void OnMatchCreate(bool success, string extendedInfo, MatchInfo matchInfo)
    {
        base.OnMatchCreate(success, extendedInfo, matchInfo);
        thisMatch = (double)matchInfo.networkId;
    }

    //***********************************************************************************************************************************************************************************
    // Changes the players UI to go back to the root menu 
    //***********************************************************************************************************************************************************************************
    public void resetToStart()
    {
        GameObject.Find("MainCanvas").GetComponent<Canvas>().enabled = true;
        GameObject.Find("HostCanvas").GetComponent<Canvas>().enabled = false;
        GameObject.Find("Launch").GetComponent<Button>().enabled = false;
        GameObject.Find("Launch").GetComponent<Image>().enabled = false;
        GameObject.Find("LaunchText").GetComponent<Text>().enabled = false;
        GameObject.Find("GamePasswordCanvas").GetComponent<Canvas>().enabled = false;
        GameObject.Find("JoinCanvas").GetComponent<Canvas>().enabled = false;

    }

    //***********************************************************************************************************************************************************************************
    // Changes the players UI to reflect hitting the host button on the root menu
    //***********************************************************************************************************************************************************************************
    public void openHostMenu()
    {
        GameObject.Find("MainCanvas").GetComponent<Canvas>().enabled = false;
        GameObject.Find("OtherCanvas").GetComponent<Canvas>().enabled = false;
        GameObject.Find("HostCanvas").GetComponent<Canvas>().enabled = true;
        GameObject.Find("GamePasswordCanvas").GetComponent<Canvas>().enabled = true;

    }

    //***********************************************************************************************************************************************************************************
    // Changes the players UI to reflect hitting the Join button on the root menu
    //***********************************************************************************************************************************************************************************
    public void openJoinMenu()
    {
        GameObject.Find("MainCanvas").GetComponent<Canvas>().enabled = false;
        GameObject.Find("OtherCanvas").GetComponent<Canvas>().enabled = false;
        GameObject.Find("GamePasswordCanvas").GetComponent<Canvas>().enabled = true;
        GameObject.Find("JoinCanvas").GetComponent<Canvas>().enabled = true;
    }

    //***********************************************************************************************************************************************************************************
    // Connected to the Search button on the Join menu, starts the search for a game with the name and password the player has entered 
    //***********************************************************************************************************************************************************************************
    public void Join()
    {
        if (matchMaker == null) matchMaker = gameObject.AddComponent<NetworkMatch>();
        gameMode = "joinFriend";
        gameName = GameObject.Find("GameName").GetComponent<InputField>().text;
        gamePass = GameObject.Find("Password").GetComponent<InputField>().text;
        StartMatchMaker();
        matchMaker.ListMatches(0, 10, "", true, 0, 0, OnMatchList);
    }

    //***********************************************************************************************************************************************************************************
    // Connected to the quickjoin button on the root menu, starts the process of joining a random online match
    //***********************************************************************************************************************************************************************************
    public void quickJoin()
    {
        GameObject.Find("OtherCanvas").GetComponent<Canvas>().enabled = false;

        if (matchMaker == null) matchMaker = gameObject.AddComponent<NetworkMatch>();
        gameMode = "quickJoin";

        StartMatchMaker();
        matchMaker.ListMatches(0, 10, "", true, 0, 0, OnMatchList);
    }

    //***********************************************************************************************************************************************************************************
    // Literally does nothing
    //***********************************************************************************************************************************************************************************
    public override void OnStopClient()
    {
        base.OnStopClient();

    }

    //***********************************************************************************************************************************************************************************
    // Sets up the network manager to be ready for a clean disconnect 
    //***********************************************************************************************************************************************************************************
    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);
        GameObject.Find("ScriptHolder").GetComponent<LeaveGame>().exitGame();
    }

    //***********************************************************************************************************************************************************************************
    // Upon connection to a game this will set the UI to reflect having joined a hosted match
    //***********************************************************************************************************************************************************************************
    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);

        GameObject.Find("Disconnect").GetComponent<Button>().enabled = true;
        GameObject.Find("Disconnect").GetComponent<Image>().enabled = true;

        GameObject.Find("DisconnectText").GetComponent<Text>().enabled = true;
        
    }


    //***********************************************************************************************************************************************************************************
    // Called when either of the join functions are called, finds an acceptable match for the user to join whether they quick joined or did a normal join and enters them into it
    //
    // Recieves three parameters, success which returns whether or not the matchlisting has been returned without corruption, extendedinfo providing more information on the matchlist, 
    // then the match list itself
    //***********************************************************************************************************************************************************************************
    public override void OnMatchList(bool success, string extendedInfo, List<MatchInfoSnapshot> matchList)
    {
        
        List<MatchInfoSnapshot> publicMatchList = new List<MatchInfoSnapshot>();  //New matchlist to sort with

        //For when the player selected quickoin
        if (gameMode == "quickJoin")
        {
            for (int i = 0; i < matchList.Count; i++)
            {
                char[] matchname = matchList[i].name.ToCharArray(); //gamename passed from gamehost through the gamename to be broken down into the real name and password
                string realName = ""; //Name of game
                string realPassword = ""; //game password
                char isPrivate = 'h';  //is the game private or open to be joined
                int counter = 0; 

                //Loop to break up the name from the matchlist into password name and privacy
                for (int j = 0; j < matchname.Length; j++)
                {

                    if (matchname[j] != '`')
                    {
                        if (counter == 0)
                        {
                            realName = realName + matchname[j];
                        }
                        else if (counter == 1)
                        {
                            realPassword = realPassword + matchname[j];
                        }
                        else if (counter == 2)
                        {
                            isPrivate = matchname[j];
                            break;
                        }


                    }
                    else
                    {
                        counter++;
                    }
                }
                counter = 0; //counter wasnt resetting 

               
                //adds the game to publicmatchlist if it is an open game
                if (isPrivate == 'o')  
                {
                    publicMatchList.Add(matchList[i]);
                }
            }
            //Block of code to show the player if there were no matches fun
            if (publicMatchList.Count == 0)
            {
                GameObject.Find("tryagainI").GetComponent<Image>().enabled = true;
                GameObject.Find("tryagainT").GetComponent<Text>().enabled = true;
                GameObject.Find("ScriptHolder").GetComponent<scirptHolderMenu>().tryAgainWaitStart();
            }
            //Searches through the match list for a random match to join then joins it
            else
            {
                for (int i = 0; i < publicMatchList.Count; i++)
                {
                    Random.Range(0, publicMatchList.Count);
                    GameObject.Find("MainCanvas").GetComponent<Canvas>().enabled = false;
                    GameObject.Find("JoinCanvas").GetComponent<Canvas>().enabled = false;
                    GameObject.Find("GamePasswordCanvas").GetComponent<Canvas>().enabled = false;
                    GameObject.Find("HostCanvas").GetComponent<Canvas>().enabled = false;
                    GameObject.Find("Launch").GetComponent<Button>().enabled = false;
                    GameObject.Find("Launch").GetComponent<Image>().enabled = false;
                    GameObject.Find("LaunchText").GetComponent<Text>().enabled = false;                
                    GameObject.Find("MapSelectCanvas").GetComponent<Canvas>().enabled = true;

                    //Sets the match info of clients to the correct matchID
                    MatchInfoSnapshot match = matchList[i];
                    matchID = match.networkId;

                  

                    StartClientAll(match); //joins the player to the chosen match

                    break;
                }
            }
        }
        //To be used when Search is used from the Join menu
        else if(gameMode == "joinFriend")
        {
            bool successJoin = false; 

            
            for(int i = 0; i < matchList.Count; i++)
            {
                char[] matchname = matchList[i].name.ToCharArray();
                string realName = "";
                string realPassword = "";
                char isPrivate = 'h';
                int counter = 0;
                for (int j = 0; j < matchname.Length; j++)
                {
                    
                    if (matchname[j] != '`')
                    {
                        if (counter == 0)
                        {
                            realName = realName + matchname[j];
                        }
                        else if( counter == 1)
                        {
                            realPassword = realPassword + matchname[j];
                        }
                        else if(counter == 2)
                        {
                            isPrivate = matchname[j];
                            break;
                        }
                       

                    }  
                    else
                    {
                        counter++;
                    }
                }
                counter = 0;

                Debug.Log(realName  + "   " + realPassword);

                //Checks to see if the name and password match if so joins player to the game
                if(realName == gameName && realPassword == gamePass)
                {
                    GameObject.Find("MainCanvas").GetComponent<Canvas>().enabled = false;
                    GameObject.Find("GamePasswordCanvas").GetComponent<Canvas>().enabled = false;
                    GameObject.Find("HostCanvas").GetComponent<Canvas>().enabled = false;
                    GameObject.Find("JoinCanvas").GetComponent<Canvas>().enabled = false;
                    GameObject.Find("Launch").GetComponent<Button>().enabled = false;
                    GameObject.Find("Launch").GetComponent<Image>().enabled = false;
                    GameObject.Find("LaunchText").GetComponent<Text>().enabled = false;
                    MatchInfoSnapshot match = matchList[i];
                    matchID = match.networkId;
                    GameObject.Find("MapSelectCanvas").GetComponent<Canvas>().enabled = true;
                    StartClientAll(match);
                    successJoin = true;
                    break;
                }
            }
            if (successJoin == false)
            {
                GameObject.Find("tryagainI").GetComponent<Image>().enabled = true;
                GameObject.Find("tryagainT").GetComponent<Text>().enabled = true;
                GameObject.Find("ScriptHolder").GetComponent<scirptHolderMenu>().tryAgainWaitStart();
            }
        }
    }


    //***********************************************************************************************************************************************************************************
    // Turns off the popup at starting menu telling players about updates to expect
    //***********************************************************************************************************************************************************************************
    public void killUpdateCanvas()
    {
        GameObject.Find("UpdateCanvas").GetComponent<Canvas>().enabled = false;
    }


    //***********************************************************************************************************************************************************************************
    // Disconnects the game making sure that the network manager gets a reset
    //***********************************************************************************************************************************************************************************
    public void disconnect2()
    {
        NetworkServer.Shutdown();
        NetworkServer.Reset();
        StopClient();
     
    }

    //***********************************************************************************************************************************************************************************
    // Disconnects player from game, figures out if the player is a host or client and acts accordingly 
    //***********************************************************************************************************************************************************************************
    public void Disconnect()
    {
        StopClient();
        StopHost();
        StopMatchMaker();
        //  Disconnect();​
        NetworkServer.Shutdown();
        if (isTheServer == true)
        {
            isTheServer = false;
        }
        GameObject.Find("MapSelectCanvas").GetComponent<Canvas>().enabled = false;
        GameObject.Find("MainCanvas").GetComponent<Canvas>().enabled = true;
        GameObject.Find("HostCanvas").GetComponent<Canvas>().enabled = false;
        GameObject.Find("Launch").GetComponent<Button>().enabled = false;
        GameObject.Find("Launch").GetComponent<Image>().enabled = false;
        GameObject.Find("LaunchText").GetComponent<Text>().enabled = false;
        GameObject.Find("Disconnect").GetComponent<Image>().enabled = false;
        GameObject.Find("Disconnect").GetComponent<Button>().enabled = false;
        GameObject.Find("DisconnectText").GetComponent<Text>().enabled = false;

        NetworkServer.Reset();
    }

    //***********************************************************************************************************************************************************************************
    // Has the server move all clients back to the map select menu on the main menu screen
    //***********************************************************************************************************************************************************************************
    public void RMapSelect()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < players.Length; i++)
        {

            players[i].GetComponent<Player>().CmdSetMenuState(); //Sets the state which the  main menu should load on all clients
            
        }
       
        ServerChangeScene("NewLobby"); //Performs the client mvoes
      
    }

    //***********************************************************************************************************************************************************************************
    // Series of map changes which all switch the clients to a playable level
    //***********************************************************************************************************************************************************************************
    public void RMap1()
    {
        ServerChangeScene("4PlayerForest");
    }
    public void RMap2()
    {
        ServerChangeScene("4PlayerVolcano");
    }
    public void RMap3()
    {
        ServerChangeScene("4PlayerIndustry");
    }
    public void RMap4()
    {
        ServerChangeScene("4PlayerLitter");
    }
    public void RMap5()
    {
        ServerChangeScene("8PlayerForest");
    }
    public void RMap6()
    {
        ServerChangeScene("8PlayerVolcano");
    }
    public void RMap7()
    {
        ServerChangeScene("8PlayerIndustry");
    }
    public void RMap8()
    {
        ServerChangeScene("8PlayerLitter");
    }

    //***********************************************************************************************************************************************************************************
    //  Runs the map selector counting votes of players and choosing the correct map to play on
    //***********************************************************************************************************************************************************************************
    public void launchGame()
    {
        
        matchMaker.SetMatchAttributes((NetworkID)thisMatch, false, 0, OnSetMatchAttributes); //Sets the match to private so people can't join mid game

        //determines which player is the host and has that player start the level change
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        for(int i = 0; i < players.Length; i++)
        {
            if(players[i].GetComponent<Player>().isServer)
            {
                players[i].GetComponent<Player>().runMapSelector();
            }
        }
    }

    //***********************************************************************************************************************************************************************************
    //  Opens the game to be joined from map select
    //***********************************************************************************************************************************************************************************
    public void openGame()
    {
        matchMaker.SetMatchAttributes((NetworkID)thisMatch, true, 0, OnSetMatchAttributes);
    }

    //***********************************************************************************************************************************************************************************
    // Closes the game to be joined from map select
    //***********************************************************************************************************************************************************************************
    public void closeGame()
    {
        matchMaker.SetMatchAttributes((NetworkID)thisMatch, false, 0, OnSetMatchAttributes);
    }

    //***********************************************************************************************************************************************************************************
    //  Sets the game to fullscreen
    //***********************************************************************************************************************************************************************************
    public void fullScreen()
    {
        Screen.fullScreen = true;
    }

    //***********************************************************************************************************************************************************************************
    //  Sets the game to windowed
    //***********************************************************************************************************************************************************************************
    public void windowed()
    {
        Screen.fullScreen = false;
    }

    //***********************************************************************************************************************************************************************************
    // Closes the game application
    //***********************************************************************************************************************************************************************************
    public void exitgame()
    {
        Application.Quit();
    }
}

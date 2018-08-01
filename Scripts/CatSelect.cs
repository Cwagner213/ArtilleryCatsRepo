//***********************************************************************************************************************************************************************************
//  CatSelect finds and sets the correct cat for each player at the start of the match
//***********************************************************************************************************************************************************************************
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class CatSelect : NetworkBehaviour {
    
    public RuntimeAnimatorController c1, c2, c3, c4,c5,c6,c7,c8;
    // Use this for initialization


    //***********************************************************************************************************************************************************************************
    // unityIsDumb seperates out the switching of cat skins out over a maximum of 80 frames in order to not confuse the server and set everyone to the same skin
    //***********************************************************************************************************************************************************************************
    public IEnumerator unityIsDumb()
    {
        yield return new WaitForSeconds(1);
        int UPID= 0;
        //finds the playerpref of the 
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].GetComponent<Player>().isLocalPlayer)
            {
                UPID = players[i].GetComponent<Player>().getPlayerId();
            }
        }
        if (UPID == 1)
        {
            for (int i = 0; i < 10; i++)
            {
                yield return new WaitForEndOfFrame();
            }
        }
        else if (UPID == 2)
        {
            for (int i = 0; i < 20; i++)
            {
                yield return new WaitForEndOfFrame();
            }
        }
        else if (UPID == 3)
        {
            for (int i = 0; i < 30; i++)
            {
                yield return new WaitForEndOfFrame();
            }
        }
        else if (UPID == 4)
        {
            for (int i = 0; i < 40; i++)
            {
                yield return new WaitForEndOfFrame();
            }
        }
        else if (UPID == 5)
        {
            for (int i = 0; i < 50; i++)
            {
                yield return new WaitForEndOfFrame();
            }
        }
        else if (UPID == 6)
        {
            for (int i = 0; i < 60; i++)
            {
                yield return new WaitForEndOfFrame();
            }
        }
        else if (UPID == 7)
        {
            for (int i = 0; i < 70; i++)
            {
                yield return new WaitForEndOfFrame();
            }
        }
        else if (UPID == 8)
        {
            for (int i = 0; i < 80; i++)
            {
                yield return new WaitForEndOfFrame();
            }
        }
        if (!PlayerPrefs.HasKey("CatChoice"))
        {
            PlayerPrefs.SetInt("CatChoice", 1);
        }
        int choice = PlayerPrefs.GetInt("CatChoice");

        if (choice == 1)
        {
            selectCat1();
        }
        if (choice == 2)
        {
            selectCat2();
        }
        if (choice == 3)
        {
            selectCat3();
        }
        if (choice == 4)
        {
            selectCat4();
        }
        if (choice == 5)
        {
            selectCat5();
        }
        if (choice == 6)
        {
            selectCat6();
        }
        if (choice == 7)
        {
            selectCat7();
        }
        if (choice == 8)
        {
            selectCat8();
        }
    }


    //Start runs at the start of the scene or object creation
    //Sets catchoice to 1 before setting cat skins if no cat skin was selected
    void Start()
    {
        if (!PlayerPrefs.HasKey("CatChoice"))
        {
            PlayerPrefs.SetInt("CatChoice", 1);
        }
        StartCoroutine(unityIsDumb());
    }
	
	// Update is called once per frame
    // Opens the escape menu to exit the game or look at controls
	void Update () {

		if(Input.GetKeyUp(KeyCode.Escape))
        {
            GameObject.Find("Canvas4").GetComponent<Canvas>().enabled = !GameObject.Find("Canvas4").GetComponent<Canvas>().enabled;
        }
	}

   

    public void openControlMenu()
    {
        GameObject.Find("ControlCanvas").GetComponent<Canvas>().enabled = !GameObject.Find("ControlCanvas").GetComponent<Canvas>().enabled;
    }

    public void openOptionsMenu()
    {
        GameObject.Find("OptionsCanvas").GetComponent<Canvas>().enabled = !GameObject.Find("OptionsCanvas").GetComponent<Canvas>().enabled;
    }

    public void selectCat1()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player"); 
        for(int i = 0; i < players.Length; i++)
        {
            if(players[i].GetComponent<Player>().isLocalPlayer)
            {
                players[i].GetComponent<Animator>().runtimeAnimatorController = c1;
                players[i].GetComponent<Player>().catNum = 1;
                GameObject.Find("Canvas3").SetActive(false);
                players[i].GetComponent<Player>().CmdSelectCat1(players[i].GetComponent<Player>().getPlayerId());
            }
        }
    }
    public void selectCat2()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].GetComponent<Player>().isLocalPlayer)
            {
                players[i].GetComponent<Animator>().runtimeAnimatorController = c2;
                players[i].GetComponent<Player>().catNum = 2;
                GameObject.Find("Canvas3").SetActive(false);
                players[i].GetComponent<Player>().CmdSelectCat2(players[i].GetComponent<Player>().getPlayerId());
            }
        }
    }
    public void selectCat3()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].GetComponent<Player>().isLocalPlayer)
            {
                players[i].GetComponent<Animator>().runtimeAnimatorController = c4;
                players[i].GetComponent<Player>().catNum = 4;
                GameObject.Find("Canvas3").SetActive(false);
                players[i].GetComponent<Player>().CmdSelectCat4(players[i].GetComponent<Player>().getPlayerId());
            }
        }
    }


    public void selectCat4()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].GetComponent<Player>().isLocalPlayer)
            {
                players[i].GetComponent<Animator>().runtimeAnimatorController = c3;
                players[i].GetComponent<Player>().catNum = 3;
                GameObject.Find("Canvas3").SetActive(false);
                players[i].GetComponent<Player>().CmdSelectCat3(players[i].GetComponent<Player>().getPlayerId());
            }
        }
    }



   

    public void selectCat5()
    {
        Debug.Log("turd");
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].GetComponent<Player>().isLocalPlayer)
            {
                players[i].GetComponent<Animator>().runtimeAnimatorController = c5;
                players[i].GetComponent<Player>().catNum = 5;
                GameObject.Find("Canvas3").SetActive(false);
                players[i].GetComponent<Player>().CmdSelectCat5(players[i].GetComponent<Player>().getPlayerId());
            }
        }
    }
    public void selectCat6()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].GetComponent<Player>().isLocalPlayer)
            {
                players[i].GetComponent<Animator>().runtimeAnimatorController = c6;
                players[i].GetComponent<Player>().catNum = 6;
                GameObject.Find("Canvas3").SetActive(false);
                players[i].GetComponent<Player>().CmdSelectCat6(players[i].GetComponent<Player>().getPlayerId());
            }
        }
    }
    public void selectCat7()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].GetComponent<Player>().isLocalPlayer)
            {
                players[i].GetComponent<Animator>().runtimeAnimatorController = c7;
                players[i].GetComponent<Player>().catNum = 7;
                GameObject.Find("Canvas3").SetActive(false);
                players[i].GetComponent<Player>().CmdSelectCat7(players[i].GetComponent<Player>().getPlayerId());
            }
        }
    }
    public void selectCat8()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].GetComponent<Player>().isLocalPlayer)
            {
                players[i].GetComponent<Animator>().runtimeAnimatorController = c8;
                players[i].GetComponent<Player>().catNum = 8;
                GameObject.Find("Canvas3").SetActive(false);
                players[i].GetComponent<Player>().CmdSelectCat8(players[i].GetComponent<Player>().getPlayerId());
            }
        }
    }


}

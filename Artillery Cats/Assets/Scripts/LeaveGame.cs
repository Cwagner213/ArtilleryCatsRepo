//***********************************************************************************************************************************************************************************
//LeaveGame checks to see how many people are in the game and gives the player a chance to exit the game if there are less than 2
//Last Updated: 7/26/18 9:28am
//***********************************************************************************************************************************************************************************

using NATTraversal;

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class LeaveGame : MonoBehaviour {

    [HideInInspector]
    public bool isChecked = false;
    [HideInInspector]
    public bool wantsToLeave = true;

    public bool disregardCheck = false;

    void Update()
    {
        if (isChecked == false && disregardCheck == false)
        {
            StartCoroutine(checkPlayerCount());
            isChecked = true;
        }
    }

    //***********************************************************************************************************************************************************************************
    // checkPlayerCount checks to see how many players are in the game, if there is only one player then it prompts that player to see if they want to leave or stay
    //***********************************************************************************************************************************************************************************
    public IEnumerator checkPlayerCount()
    {
        disregardCheck = true;
        yield return new WaitForSeconds(1);
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        if (players.Length < 2)
        {
            StartCoroutine(delayedGameExit());
            GameObject.Find("LeaveQuestionCanvas").GetComponent<Canvas>().enabled = true; //Asks the player if they want to stay in the game
        }
        isChecked = false;
    }

    //***********************************************************************************************************************************************************************************
    // delayedGameExit leaves the game after 3 seconds if the player opts to leave agame with only one person
    //***********************************************************************************************************************************************************************************
    public IEnumerator delayedGameExit()
    {
        yield return new WaitForSeconds(3);
        if (wantsToLeave == true)
        {
            exitGame();
        }
    }

    //***********************************************************************************************************************************************************************************
    // interuptLeave is connected to a button that the player can click to express that they want to stay in their one person game
    //***********************************************************************************************************************************************************************************
    public void interuptLeave()
    {
        GameObject.Find("LeaveQuestionCanvas").GetComponent<Canvas>().enabled = false;
        wantsToLeave = false;
      
    }

    //***********************************************************************************************************************************************************************************
    // Leaves the current game and returns to the root menu screen
    //***********************************************************************************************************************************************************************************
    public void exitGame()
    {

        GameObject.Find("Network Manager").GetComponent<networkManage>().disconnect2();
        GameObject.Destroy(GameObject.Find("Network Manager"));
        SceneManager.LoadScene("NewLobby");
        
    }
}

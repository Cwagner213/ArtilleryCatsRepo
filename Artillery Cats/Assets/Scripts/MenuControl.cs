//***********************************************************************************************************************************************************************************
// MenuControl controls the network manager indirectly allowing the network manager to be detroyed and reset without breaking its links to all of the menu buttons
// Last Updated: 7/25/18 11:35am
//***********************************************************************************************************************************************************************************

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Steamworks;

public class MenuControl : MonoBehaviour {



    // Use this for initialization
    void Start() {
        //Looks at the menuKey playerpref to determine which menu to show
        if (!PlayerPrefs.HasKey("menuKey"))
        {
            PlayerPrefs.SetInt("menuKey", 0);
        }
        //Keeps menu at root menu
        if (PlayerPrefs.GetInt("menuKey") == 0)
        {

        }
        //switches menu to map select
        else
        {
            GameObject.Find("OtherCanvas").GetComponent<Canvas>().enabled = false;
            GameObject.Find("MapSelectCanvas").GetComponent<Canvas>().enabled = true;
            GameObject.Find("MainCanvas").GetComponent<Canvas>().enabled = false;
            GameObject.Find("JoinCanvas").GetComponent<Canvas>().enabled = false;
            GameObject.Find("GamePasswordCanvas").GetComponent<Canvas>().enabled = false;
            GameObject.Find("HostCanvas").GetComponent<Canvas>().enabled = false;
            GameObject.Find("Disconnect").GetComponent<Button>().enabled = true;
            GameObject.Find("Disconnect").GetComponent<Image>().enabled = true;

            GameObject.Find("DisconnectText").GetComponent<Text>().enabled = true;
            StartCoroutine(waitForMenuReset());
        }
        if (!PlayerPrefs.HasKey("CatChoice"))
        {
            PlayerPrefs.SetInt("CatChoice", 1);
        }
    }
    //***********************************************************************************************************************************************************************************
    // dlcNo pops up a menu telling the player they do not own the DLC if they try selecting a DLC only cat
    //***********************************************************************************************************************************************************************************
    public IEnumerator dlcNo()
    {
        GameObject.Find("DLCMSG").GetComponent<Image>().enabled = true;
        GameObject.Find("DLCMSGTXT").GetComponent<Text>().enabled = true;
       
        yield return new WaitForSeconds(2);

        GameObject.Find("DLCMSG").GetComponent<Image>().enabled = false;
        GameObject.Find("DLCMSGTXT").GetComponent<Text>().enabled = false;
    }

    //***********************************************************************************************************************************************************************************
    //  Series of set cat buttons controls which cat the player will be when the game is started
    //***********************************************************************************************************************************************************************************
    public void setCat1()
    {

        PlayerPrefs.SetInt("CatChoice", 1);     
        
        for (int i = 0; i < 8; i++)
        {
            GameObject.Find("catButt" + (i + 1)).GetComponent<Image>().color = Color.red;
        }
        GameObject.Find("catButt1").GetComponent<Image>().color = Color.green;
    }
    public void setCat2()
    {

        PlayerPrefs.SetInt("CatChoice", 2);
        for (int i = 0; i < 8; i++)
        {
            GameObject.Find("catButt" + (i + 1)).GetComponent<Image>().color = Color.red;
        }
        GameObject.Find("catButt2").GetComponent<Image>().color = Color.green;
    }
    public void setCat3()
    {

        PlayerPrefs.SetInt("CatChoice", 3);
        for (int i = 0; i < 8; i++)
        {
            GameObject.Find("catButt" + (i + 1)).GetComponent<Image>().color = Color.red;
        }
        GameObject.Find("catButt3").GetComponent<Image>().color = Color.green;

    }
    public void setCat4()
    {

        PlayerPrefs.SetInt("CatChoice", 4);
        for (int i = 0; i < 8; i++)
        {
            GameObject.Find("catButt" + (i + 1)).GetComponent<Image>().color = Color.red;
        }
        GameObject.Find("catButt4").GetComponent<Image>().color = Color.green;
    }
    public void setCat5()
    {
        if (SteamApps.BIsDlcInstalled((AppId_t)863760))
        {
            Debug.Log("dat");

            PlayerPrefs.SetInt("CatChoice", 5);
            for (int i = 0; i < 8; i++)
            {
                GameObject.Find("catButt" + (i + 1)).GetComponent<Image>().color = Color.red;
            }
            GameObject.Find("catButt5").GetComponent<Image>().color = Color.green;
        }
        else
        {
            StartCoroutine(dlcNo());
        }
    }
    public void setCat6()
    {
        if (SteamApps.BIsDlcInstalled((AppId_t)863760))
        {
            PlayerPrefs.SetInt("CatChoice", 6);
            for (int i = 0; i < 8; i++)
            {
                GameObject.Find("catButt" + (i + 1)).GetComponent<Image>().color = Color.red;
            }
            GameObject.Find("catButt6").GetComponent<Image>().color = Color.green;
        }
        else
        {
            StartCoroutine(dlcNo());
        }
    }
    public void setCat7()
    {

        if (SteamApps.BIsDlcInstalled((AppId_t)863760))
        {
            PlayerPrefs.SetInt("CatChoice", 7);
            for (int i = 0; i < 8; i++)
            {
                GameObject.Find("catButt" + (i + 1)).GetComponent<Image>().color = Color.red;
            }
            GameObject.Find("catButt7").GetComponent<Image>().color = Color.green;
        }
        else
        {
            StartCoroutine(dlcNo());
        }
    }
   
    public void setCat8()
    {
        if (SteamApps.BIsDlcInstalled((AppId_t)863760))
        {
            PlayerPrefs.SetInt("CatChoice", 8);
            for (int i = 0; i < 8; i++)
            {
                GameObject.Find("catButt" + (i + 1)).GetComponent<Image>().color = Color.red;
            }
            GameObject.Find("catButt8").GetComponent<Image>().color = Color.green;
        }
        else
        {
            StartCoroutine(dlcNo());
        }
    }


    public void callResetToStart()
    {
        GameObject.Find("Network Manager").GetComponent<networkManage>().resetToStart();
    }

    public void callHost()
    {
        GameObject.Find("Network Manager").GetComponent<networkManage>().Host();
    }

    public void callJoin()
    {
        GameObject.Find("Network Manager").GetComponent<networkManage>().Join();
    }

    public void callQuickJoin()
    {
        GameObject.Find("Network Manager").GetComponent<networkManage>().quickJoin();
    }

    public void callUpdateKill()
    {
        GameObject.Find("Network Manager").GetComponent<networkManage>().killUpdateCanvas();
    }

    public void callDisconnect()
    {
        GameObject.Find("Network Manager").GetComponent<networkManage>().Disconnect();
    }

    public void callFullScreen()
    {
        GameObject.Find("Network Manager").GetComponent<networkManage>().fullScreen();
    }

    public void callWindowed()
    {
        GameObject.Find("Network Manager").GetComponent<networkManage>().windowed();
    }

    public void callExitGame()
    {
        GameObject.Find("Network Manager").GetComponent<networkManage>().exitgame();
    }

    public void callOpenHost()
    {
        GameObject.Find("Network Manager").GetComponent<networkManage>().openHostMenu();
    }

    public void callOpenJoin()
    {
        GameObject.Find("Network Manager").GetComponent<networkManage>().openJoinMenu();
    }

    public IEnumerator waitForMenuReset()
    {
        yield return new WaitForSeconds(1);
        PlayerPrefs.SetInt("menuKey", 0);
    }

	// Update is called once per frame
	void Update () {
		
	}
}

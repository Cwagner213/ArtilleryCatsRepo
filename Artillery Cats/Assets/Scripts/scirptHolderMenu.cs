//***********************************************************************************************************************************************************************************
// ScriptHolderMenu holds the scripts for a few menu options such as changing between EU and NA servers, also the no match found text pop up
// Last Updated: 7/
//***********************************************************************************************************************************************************************************

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class scirptHolderMenu : MonoBehaviour {

    //***********************************************************************************************************************************************************************************
    //  tryAgainWaitStart starts the timer for the try again pop up when someone couldnt find a match aka whenever they look for one <,<
    //***********************************************************************************************************************************************************************************
    public void tryAgainWaitStart()
    {
        StartCoroutine(tryAgainWait());
    }

    //***********************************************************************************************************************************************************************************
    //  tryAgainWait is a timer to get rid of the popup telling someone they failed to find a match
    //***********************************************************************************************************************************************************************************
    public IEnumerator tryAgainWait()
    {
        yield return new WaitForSeconds(3);
        GameObject.Find("tryagainI").GetComponent<Image>().enabled = false;
        GameObject.Find("tryagainT").GetComponent<Text>().enabled = false;
    }

    public void switchToUS()
    {
        GameObject.Find("Network Manager").GetComponent<networkManage>().matchHost = "us1-mm.unet.unity3d.com";
    }

    public void switchToEU()
    {
        GameObject.Find("Network Manager").GetComponent<networkManage>().matchHost = "eu1-mm.unet.unity3d.com";
    }
}

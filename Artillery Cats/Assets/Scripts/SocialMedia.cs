using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SocialMedia : MonoBehaviour {

	public void twitterClick()
    {
        Application.OpenURL("https://twitter.com/MonotoneGameDev");
    }
    public void websiteClick()
    {
        Application.OpenURL("https://www.monotonegamedev.com/");
    }
}

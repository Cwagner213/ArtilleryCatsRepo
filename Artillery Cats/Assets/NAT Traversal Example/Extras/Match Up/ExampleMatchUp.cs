#if MATCH_UP

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using MatchUp;

/**
 * Utilize the MatchUp Matchmaker to perform matchmaking. 
 * Connect to the match host using NAT Traversal
 */
[RequireComponent(typeof(NetworkManager))]
public class ExampleMatchUp : Matchmaker
{
    NATTraversal.NetworkManager netManager;
    Match[] matches;
    bool showDisconnectButton;

	void Awake()
    {
        netManager = GetComponent<NATTraversal.NetworkManager>();
	}

    void OnGUI()
    {
        if (NetworkManagerExtension.externalIP == null) GUI.enabled = false;
        else GUI.enabled = true;

        if (!NetworkServer.active && !NetworkClient.active)
        {
            // Host a match
            if (GUI.Button(new Rect(10, 10, 150, 48), "Host"))
            {
                StartCoroutine(HostAMatch());
                showDisconnectButton = true;
            }

            // List matches
            if (GUI.Button(new Rect(10, 60, 150, 48), "Search matches"))
            {
                Debug.Log("Fetching match list");

                // Get the filtered match list. The results will be received in OnMatchList()
                this.GetMatchList(OnMatchList, 0, 10);
            }
        }

        // Disconnect
        if (showDisconnectButton)
        {
            if (GUI.Button(new Rect(10, 110, 150, 48), "Disconnect"))
            {
                // Stop hosting and destroy the match
                if (NetworkServer.active)
                {
                    Debug.Log("Destroyed match");
                    netManager.StopHost();
                    this.DestroyMatch();
                }

                // Disconnect from the host and leave the match
                else
                {
                    Debug.Log("Left match");
                    netManager.StopClient();
                    this.LeaveMatch();
                }
                showDisconnectButton = false;
            }
        }
        
        if (matches != null && !showDisconnectButton)
        {
            for (int i = 0; i < matches.Length; i++)
            {
                if (GUI.Button(new Rect(170, 10 + i * 26, 600, 25), matches[i].matchData["Match Name"]))
                {
                    showDisconnectButton = true;
                    this.JoinMatch(matches[i], OnJoinMatch);
                }
            }
        }
    }

    IEnumerator HostAMatch()
    {
        string matchName = "Layla's Match";

        // Start the host first so that we connect to the Facilitator and get a GUID
        netManager.StartHostAll(matchName, (uint)netManager.maxConnections + 1);

        // Wait for the Facilitator connection
        while (netManager.natHelper.isConnectingToFacilitator) yield return 0;

        // Make sure we actually connected
        if (!netManager.natHelper.isConnectedToFacilitator) yield break;

        // Add the guid to the match data
        var matchData = new Dictionary<string, MatchData>() {
            { "Match Name", matchName },
            { "guid", netManager.natHelper.guid }
        };

        // Create the Match with the associated MatchData
        this.CreateMatch(netManager.maxConnections + 1, matchData);

        Debug.Log("Created match: " + matchName);
    }

    /**
     * This is called when the match list is received
     */
    void OnMatchList(bool success, Match[] matchesTemp)
    {
        if (!success) return;

        Debug.Log("Received match list.");
        matches = matchesTemp;
    }

    /**
     * This is called when a response is received from a JoinMatch() request
     * Once the match is succesfully joined you have access to all the associated MatchData
     */
    void OnJoinMatch(bool success, Match match)
    {
        if (!success) return;

        Debug.Log("Joined match: " + match.matchData["Match Name"]);
        showDisconnectButton = true;
        
        string externalIP = match.matchData["externalIP"];
        string internalIP = match.matchData["internalIP"];
        int port = match.matchData["port"];
        ulong guid = (ulong)match.matchData["guid"];

        // Connect to the host
        netManager.StartClientAll(externalIP, internalIP, port, guid);
    }
}
#else
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ExampleMatchUp : MonoBehaviour
{
    void Start() 
    { 
        Debug.LogError("This example requires the Match Up plugin. Get it here: http://u3d.as/10eJ\nIf you already have Match Up installed you may just need to re-import this script."); 
    }
}
#endif
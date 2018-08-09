//***********************************************************************************************************************************************************************************
//  The player file has the most control over the game as it is attached to the player prefab and is directly controlled by players in game.  A player prefab is spawned from the 
//  network manager when a player hosts a match or joins a match.  All networking is routed through this class due to player controlled objects being the only objects allowed to 
//  pass information to and from the server
//
//  Last Updated: 6/12/2018 12:13
//***********************************************************************************************************************************************************************************

using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;
using Steamworks;

public class Player : NetworkBehaviour {

    public int lastMoveUsed = -1;
    public string[] playerNames = new string[8];
    public string playerName = "";
    public Font actionJ;

    public RuntimeAnimatorController C1, C2, C3, C4, C5, C6, C7, C8; //animation controllers for all the cats
    public int catNum = 1; //number to determine which cat the player chose

    private Rigidbody2D rgdb;
    private char facing = 'E';
    private bool running = false;

    public GameObject bomb, firebomb, poop, balloon;
    public GameObject portGoingAway, portArrival;
     
    //bomb stats to determine the kind of bombs the player will shoot
    public int bombPower = 3;
    public int bombDistance = 1;
    public int bombAvailableDistance = 1;
    public int bombNumber = 1;
    public int bombsAvailable = 1;
    public int bombMax = 1;
    public int shootingDelay = 0;
    private int timer = 0;

    public Texture2D boxBracket;
    [HideInInspector]
    public float speedMod = .5f;

    public bool idling = true;

    private int abilTimer = 0;
    private int abilTimerC = 0;
    [HideInInspector]
    public bool isReversed = false;


    public AudioClip portalCatFire, trebCatFire, NormalFire;

    private int uniqueID = 0;

    private NetworkIdentity objNetid;
    public bool phasedOut = false;

    private int spawnControl = 0;
  
    private int done = 0;

    [SyncVar]
    private Vector3 syncPos;

    [SerializeField] Transform myTrasform;
    [SerializeField] float lerpRate= 15;

    public bool dead = false;
    public bool timerWaiting = true;
    public int first = 0;


    //***********************************************************************************************************************************************************************************
    //  Is called every frame and generally used for physics calculations, in this case its being used to sync the player position across the clients 
    //***********************************************************************************************************************************************************************************
    void FixedUpdate()
    {
        transmitPosition();
        LerpPosition();
    }

    //***********************************************************************************************************************************************************************************
    //  Syncs the player position from the position it is on the server
    //***********************************************************************************************************************************************************************************
    void LerpPosition()
    {
        if (!isLocalPlayer)
        {
            myTrasform.position = Vector3.Lerp(myTrasform.position, syncPos, Time.deltaTime * lerpRate);
        }
    }

    //***********************************************************************************************************************************************************************************
    //  Tells the server where the character currently is
    //***********************************************************************************************************************************************************************************
    [Command]
    void CmdProvidePositionToServer(Vector3 pos)
    {
        syncPos = pos;
    }

    //***********************************************************************************************************************************************************************************
    //  Called to initiate CmDProvidePositionToServer
    //***********************************************************************************************************************************************************************************
    [ClientCallback] 
    void transmitPosition()
    {
        if (isLocalPlayer)
        {
            CmdProvidePositionToServer(myTrasform.position);
        }
    }

    //***********************************************************************************************************************************************************************************
    // Use this for initialization, called the moment player object is spawned
    //***********************************************************************************************************************************************************************************
    void Start () {
        rgdb = GetComponent<Rigidbody2D>();
        DontDestroyOnLoad(transform.gameObject);

    }

    //***********************************************************************************************************************************************************************************
    // Tells the server that the clients are heading back to the main menu soon and making sure they are going to be in the map select menu
    //***********************************************************************************************************************************************************************************
    [Command]
    public void CmdSetMenuState()
    {
        RpcSetMenuState();
    }

    //***********************************************************************************************************************************************************************************
    //  Tells the clients to set the menu up correctly for heading back to the main menu
    //***********************************************************************************************************************************************************************************
    [ClientRpc]
    public void RpcSetMenuState()
    {
        PlayerPrefs.SetInt("menuKey", 1);
    }

    //***********************************************************************************************************************************************************************************
    //  returns this players uniwueID
    //***********************************************************************************************************************************************************************************
    public int getPlayerId()
    {
        return uniqueID;
    }

    //***********************************************************************************************************************************************************************************
    // Set this players uniqueID
    //***********************************************************************************************************************************************************************************
    public void setPlayerId(int x)
    {
        uniqueID = x;
    }

    //***********************************************************************************************************************************************************************************
    // CoRoutine to countdown timer of the 'Reverse' ability used by hypnocat
    //***********************************************************************************************************************************************************************************
    public IEnumerator reverseCounter()
    {
        yield return new WaitForSeconds(2);
        isReversed = false;
    }

    //***********************************************************************************************************************************************************************************
    //  Counter that resets the ability for the player to use their special
    //***********************************************************************************************************************************************************************************
    public IEnumerator abilCounter()
    {
        GameObject.Find("PowerCount").GetComponent<Text>().text = "" + abilTimer;
        while (abilTimer >= 0)
        {
            yield return new WaitForSeconds(1);

            abilTimer = abilTimer - 1;
            GameObject.Find("PowerCount").GetComponent<Text>().text = "" + abilTimer;
        }
        GameObject.Find("PowerCount").GetComponent<Text>().text = "go";
        abilTimerC = 0;
    }
    //***********************************************************************************************************************************************************************************
    // Update is called once per frame
    //***********************************************************************************************************************************************************************************
    void Update() {

        //Sets the players initial stats
        if (first == 0 && isLocalPlayer)
        {
            GameObject.Find("ID").GetComponent<Text>().text = "" + uniqueID;
            bombPower = 3;
            first = 1;
            abilTimer = 0;
        }

        //moves the player to their initial spawn position
        if (done == 0 && isServer)
        {
            GameObject[] temp = GameObject.FindGameObjectsWithTag("Player");
            int tempx = 1;
            for (int i = 0; i < temp.Length; i++)
            {
                temp[i].transform.position = GameObject.Find("Spawn Position " + tempx).transform.position;
                temp[i].GetComponent<Player>().spawnControl = i + 1;
                tempx++;
            }
            done = 1;
        }

        //Opens/Closes the score screen when the player presses Tab
        if (isLocalPlayer && Input.GetKeyDown(KeyCode.Tab))
        {
            GameObject.Find("Canvas").GetComponent<Canvas>().enabled = !GameObject.Find("Canvas").GetComponent<Canvas>().enabled;
        }

        //{powers up the bomb range and adjusts the bomb range menu
        if(isLocalPlayer && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("g4444ggggg");
            if (bombDistance < bombAvailableDistance)
            {
                Debug.Log("gggggg");
                if (bombDistance < 5)
                {
                    bombDistance++;
                    if (bombDistance == 2)
                    {
                        GameObject.Find("PL1").GetComponent<Image>().color = Color.green;
                        GameObject.Find("PL2").GetComponent<Image>().color = Color.red;
                        GameObject.Find("PL3").GetComponent<Image>().color = Color.red;
                        GameObject.Find("PL4").GetComponent<Image>().color = Color.red;
                    }
                    else if (bombDistance == 3)
                    {
                        GameObject.Find("PL1").GetComponent<Image>().color = Color.green;
                        GameObject.Find("PL2").GetComponent<Image>().color = Color.green;
                        GameObject.Find("PL3").GetComponent<Image>().color = Color.red;
                        GameObject.Find("PL4").GetComponent<Image>().color = Color.red;
                    }
                    else if (bombDistance == 4)
                    {
                        GameObject.Find("PL1").GetComponent<Image>().color = Color.green;
                        GameObject.Find("PL2").GetComponent<Image>().color = Color.green;
                        GameObject.Find("PL3").GetComponent<Image>().color = Color.green;
                        GameObject.Find("PL4").GetComponent<Image>().color = Color.red;
                    }
                    else if (bombDistance == 1)
                    {
                        GameObject.Find("PL1").GetComponent<Image>().color = Color.red;
                        GameObject.Find("PL2").GetComponent<Image>().color = Color.red;
                        GameObject.Find("PL3").GetComponent<Image>().color = Color.red;
                        GameObject.Find("PL4").GetComponent<Image>().color = Color.red;
                    }
                    else if (bombDistance == 5)
                    {
                        GameObject.Find("PL1").GetComponent<Image>().color = Color.green;
                        GameObject.Find("PL2").GetComponent<Image>().color = Color.green;
                        GameObject.Find("PL3").GetComponent<Image>().color = Color.green;
                        GameObject.Find("PL4").GetComponent<Image>().color = Color.green;
                    }
                }
            }
        }
        //powers down the bomb range and adjusts the bomb range menu
        else if(isLocalPlayer && Input.GetKeyDown(KeyCode.R))
        {
            if(bombDistance > 1)
            {
                bombDistance--;
                if (bombDistance == 2)
                {
                    GameObject.Find("PL1").GetComponent<Image>().color = Color.green;
                    GameObject.Find("PL2").GetComponent<Image>().color = Color.red;
                    GameObject.Find("PL3").GetComponent<Image>().color = Color.red;
                    GameObject.Find("PL4").GetComponent<Image>().color = Color.red;
                }
                else if (bombDistance == 3)
                {
                    GameObject.Find("PL1").GetComponent<Image>().color = Color.green;
                    GameObject.Find("PL2").GetComponent<Image>().color = Color.green;
                    GameObject.Find("PL3").GetComponent<Image>().color = Color.red;
                    GameObject.Find("PL4").GetComponent<Image>().color = Color.red;
                }
                else if (bombDistance == 4)
                {
                    GameObject.Find("PL1").GetComponent<Image>().color = Color.green;
                    GameObject.Find("PL2").GetComponent<Image>().color = Color.green;
                    GameObject.Find("PL3").GetComponent<Image>().color = Color.green;
                    GameObject.Find("PL4").GetComponent<Image>().color = Color.red;
                }
                else if (bombDistance == 1)
                {
                    GameObject.Find("PL1").GetComponent<Image>().color = Color.red;
                    GameObject.Find("PL2").GetComponent<Image>().color = Color.red;
                    GameObject.Find("PL3").GetComponent<Image>().color = Color.red;
                    GameObject.Find("PL4").GetComponent<Image>().color = Color.red;
                }
                else if (bombDistance == 5)
                {
                    GameObject.Find("PL1").GetComponent<Image>().color = Color.green;
                    GameObject.Find("PL2").GetComponent<Image>().color = Color.green;
                    GameObject.Find("PL3").GetComponent<Image>().color = Color.green;
                    GameObject.Find("PL4").GetComponent<Image>().color = Color.green;
                }
            }
        }
        //Checks for movement and bomb firing input 
        if (isLocalPlayer && !dead)
        {
            //Makes sure the player isnt reversed
            if (isReversed == false)
            {
                //Moves up
                if (Input.GetKey("w"))
                {
                    idling = false;
                    float moveVertical = Input.GetAxis("Vertical");
                    float moveHorizontal = Input.GetAxis("Horizontal");
                    Vector2 movement = new Vector2(moveHorizontal, moveVertical + speedMod);

                    double remainder = transform.position.x % .64;
                    bool moveHelpCheck = true;
                    RaycastHit2D[] hitCheck;
                    hitCheck = Physics2D.RaycastAll(new Vector2(transform.position.x, transform.position.y + .64f), new Vector2(transform.position.x, transform.position.y + .64f), 0f);

                    for (int i = 0; i < hitCheck.Length; i++)
                    {
                        if (hitCheck[i].transform.tag == "Wall" || hitCheck[i].transform.tag == "Crate")
                        {
                            moveHelpCheck = false;
                        }
                    }

                    //Small block to help the player scoot around corners instead of being frustrated when caught on edges
                    if (remainder > 0 && remainder <= .24 && moveHelpCheck == true)
                    {
                        movement.x = 1;
                    }
                    else if (remainder < .64 && remainder > .40 && moveHelpCheck == true)
                    {
                        movement.x = -1;
                    }

                    rgdb.velocity = movement; //Sets the players movement in this moment

                    //Controls animation
                    if (facing != 'N')
                    {
                        GetComponent<Animator>().SetTrigger("RunBack");
                        running = true;
                    }
                    facing = 'N'; //Sets direction the cat is currently facing
                }
                //Moves downward
                else if (Input.GetKey("s"))
                {
                    idling = false;
                    float moveVertical = Input.GetAxis("Vertical");
                    float moveHorizontal = Input.GetAxis("Horizontal");
                    Vector2 movement = new Vector2(moveHorizontal, moveVertical - speedMod);

                    double remainder = transform.position.x % .64;
                    bool moveHelpCheck = true;
                    RaycastHit2D[] hitCheck;
                    hitCheck = Physics2D.RaycastAll(new Vector2(transform.position.x, transform.position.y - .64f), new Vector2(transform.position.x, transform.position.y - .64f), 0f);

                    for (int i = 0; i < hitCheck.Length; i++)
                    {
                        if (hitCheck[i].transform.tag == "Wall" || hitCheck[i].transform.tag == "Crate")
                        {
                            moveHelpCheck = false;
                        }
                    }

                    if (remainder > 0 && remainder <= .24 && moveHelpCheck == true)
                    {
                        movement.x = 1;
                    }
                    else if (remainder < .64 && remainder > .40 && moveHelpCheck == true)
                    {
                        movement.x = -1;
                    }

                    rgdb.velocity = movement;

                    if (facing != 'S')
                    {
                        GetComponent<Animator>().SetTrigger("RunFront");
                        running = true;
                    }
                    facing = 'S';
                }
                //moves left
                else if (Input.GetKey("a"))
                {
                    idling = false;
                    float moveVertical = Input.GetAxis("Vertical");
                    float moveHorizontal = Input.GetAxis("Horizontal");
                    Vector2 movement = new Vector2(moveHorizontal - speedMod, moveVertical);

                    double remainder = transform.position.y % .64;
                    bool moveHelpCheck = true;
                    RaycastHit2D[] hitCheck;
                    hitCheck = Physics2D.RaycastAll(new Vector2(transform.position.x - .64f, transform.position.y), new Vector2(transform.position.x - .64f, transform.position.y), 0f);

                    for (int i = 0; i < hitCheck.Length; i++)
                    {
                        if (hitCheck[i].transform.tag == "Wall" || hitCheck[i].transform.tag == "Crate")
                        {
                            moveHelpCheck = false;
                        }
                    }

                    if (remainder > 0 && remainder <= .24 && moveHelpCheck == true)
                    {
                        movement.y = 1;
                    }
                    else if (remainder < .64 && remainder > .40 && moveHelpCheck == true)
                    {
                        movement.y = -1;
                    }

                    rgdb.velocity = movement;


                    if (facing != 'W')
                    {
                        GetComponent<Animator>().SetTrigger("RunLeft");
                        running = true;
                    }
                    facing = 'W';
                }
                //moves right
                else if (Input.GetKey("d"))
                {
                    idling = false;
                    float moveVertical = Input.GetAxis("Vertical");
                    float moveHorizontal = Input.GetAxis("Horizontal");
                    Vector2 movement = new Vector2(moveHorizontal + speedMod, moveVertical);

                    double remainder = transform.position.y % .64;
                    bool moveHelpCheck = true;
                    RaycastHit2D[] hitCheck;
                    hitCheck = Physics2D.RaycastAll(new Vector2(transform.position.x + .64f, transform.position.y), new Vector2(transform.position.x + .64f, transform.position.y), 0f);

                    for (int i = 0; i < hitCheck.Length; i++)
                    {
                        if (hitCheck[i].transform.tag == "Wall" || hitCheck[i].transform.tag == "Crate")
                        {
                            moveHelpCheck = false;
                        }
                    }

                    if (remainder > 0 && remainder <= .24 && moveHelpCheck == true)
                    {
                        movement.y = 1;
                    }
                    else if (remainder < .64 && remainder > .40 && moveHelpCheck == true)
                    {
                        movement.y = -1;
                    }

                    rgdb.velocity = movement;

                    if (facing != 'E')
                    {
                        GetComponent<Animator>().SetTrigger("RunRight");
                        running = true;
                    }
                    facing = 'E';
                }
                //Sets cat animation to idle if it isnt moving
                else if (shootingDelay == 0 && idling == false)
                {
                    idling = true;
                    rgdb.velocity = new Vector2(0, 0);
                    running = false;

                    if (facing == 'N')
                    {
                        GetComponent<Animator>().SetTrigger("IdleBack");
                    }
                    else if (facing == 'E')
                    {
                        GetComponent<Animator>().SetTrigger("IdleRight");
                    }
                    else if (facing == 'W')
                    {
                        GetComponent<Animator>().SetTrigger("IdleLeft");
                    }
                    else if (facing == 'S')
                    {
                        GetComponent<Animator>().SetTrigger("IdleFront");
                    }

                }
            }
            //If the cat is reversed it will move in the opposite direction but a little faster
            else
            {
                if (Input.GetKey("w"))
                {
                    idling = false;
                    float moveVertical = Input.GetAxis("Vertical");
                    float moveHorizontal = Input.GetAxis("Horizontal");
                    Vector2 movement = new Vector2(moveHorizontal, moveVertical - 2.5f);

                    double remainder = transform.position.x % .64;
                    bool moveHelpCheck = true;
                    RaycastHit2D[] hitCheck;
                    hitCheck = Physics2D.RaycastAll(new Vector2(transform.position.x, transform.position.y + .64f), new Vector2(transform.position.x, transform.position.y + .64f), 0f);

                    for (int i = 0; i < hitCheck.Length; i++)
                    {
                        if (hitCheck[i].transform.tag == "Wall" || hitCheck[i].transform.tag == "Crate")
                        {
                            moveHelpCheck = false;
                        }
                    }

                    if (remainder > 0 && remainder <= .24 && moveHelpCheck == true)
                    {
                        movement.x = 1;
                    }
                    else if (remainder < .64 && remainder > .40 && moveHelpCheck == true)
                    {
                        movement.x = -1;
                    }

                    rgdb.velocity = movement;

                    if (facing != 'N')
                    {
                        GetComponent<Animator>().SetTrigger("RunBack");
                        running = true;
                    }
                    facing = 'N';
                }
                else if (Input.GetKey("s"))
                {
                    idling = false;
                    float moveVertical = Input.GetAxis("Vertical");
                    float moveHorizontal = Input.GetAxis("Horizontal");
                    Vector2 movement = new Vector2(moveHorizontal, moveVertical + 2.5f);

                    double remainder = transform.position.x % .64;
                    bool moveHelpCheck = true;
                    RaycastHit2D[] hitCheck;
                    hitCheck = Physics2D.RaycastAll(new Vector2(transform.position.x, transform.position.y - .64f), new Vector2(transform.position.x, transform.position.y - .64f), 0f);

                    for (int i = 0; i < hitCheck.Length; i++)
                    {
                        if (hitCheck[i].transform.tag == "Wall" || hitCheck[i].transform.tag == "Crate")
                        {
                            moveHelpCheck = false;
                        }
                    }

                    if (remainder > 0 && remainder <= .24 && moveHelpCheck == true)
                    {
                        movement.x = 1;
                    }
                    else if (remainder < .64 && remainder > .40 && moveHelpCheck == true)
                    {
                        movement.x = -1;
                    }

                    rgdb.velocity = movement;

                    if (facing != 'S')
                    {
                        GetComponent<Animator>().SetTrigger("RunFront");
                        running = true;
                    }
                    facing = 'S';
                }
                else if (Input.GetKey("a"))
                {
                    idling = false;
                    float moveVertical = Input.GetAxis("Vertical");
                    float moveHorizontal = Input.GetAxis("Horizontal");
                    Vector2 movement = new Vector2(moveHorizontal + 2.5f, moveVertical);

                    double remainder = transform.position.y % .64;
                    bool moveHelpCheck = true;
                    RaycastHit2D[] hitCheck;
                    hitCheck = Physics2D.RaycastAll(new Vector2(transform.position.x - .64f, transform.position.y), new Vector2(transform.position.x - .64f, transform.position.y), 0f);

                    for (int i = 0; i < hitCheck.Length; i++)
                    {
                        if (hitCheck[i].transform.tag == "Wall" || hitCheck[i].transform.tag == "Crate")
                        {
                            moveHelpCheck = false;
                        }
                    }

                    if (remainder > 0 && remainder <= .24 && moveHelpCheck == true)
                    {
                        movement.y = 1;
                    }
                    else if (remainder < .64 && remainder > .40 && moveHelpCheck == true)
                    {
                        movement.y = -1;
                    }

                    rgdb.velocity = movement;


                    if (facing != 'W')
                    {
                        GetComponent<Animator>().SetTrigger("RunLeft");
                        running = true;
                    }
                    facing = 'W';
                }
                else if (Input.GetKey("d"))
                {
                    idling = false;
                    float moveVertical = Input.GetAxis("Vertical");
                    float moveHorizontal = Input.GetAxis("Horizontal");
                    Vector2 movement = new Vector2(moveHorizontal - 2.5f, moveVertical);

                    double remainder = transform.position.y % .64;
                    bool moveHelpCheck = true;
                    RaycastHit2D[] hitCheck;
                    hitCheck = Physics2D.RaycastAll(new Vector2(transform.position.x + .64f, transform.position.y), new Vector2(transform.position.x + .64f, transform.position.y), 0f);

                    for (int i = 0; i < hitCheck.Length; i++)
                    {
                        if (hitCheck[i].transform.tag == "Wall" || hitCheck[i].transform.tag == "Crate")
                        {
                            moveHelpCheck = false;
                        }
                    }

                    if (remainder > 0 && remainder <= .24 && moveHelpCheck == true)
                    {
                        movement.y = 1;
                    }
                    else if (remainder < .64 && remainder > .40 && moveHelpCheck == true)
                    {
                        movement.y = -1;
                    }

                    rgdb.velocity = movement;

                    if (facing != 'E')
                    {
                        GetComponent<Animator>().SetTrigger("RunRight");
                        running = true;
                    }
                    facing = 'E';
                }
                else if (shootingDelay == 0&& idling == false)
                {
                    rgdb.velocity = new Vector2(0, 0);
                    running = false;
                    idling = true;
                    if (facing == 'N')
                    {
                        GetComponent<Animator>().SetTrigger("IdleBack");
                    }
                    else if (facing == 'E')
                    {
                        GetComponent<Animator>().SetTrigger("IdleRight");
                    }
                    else if (facing == 'W')
                    {
                        GetComponent<Animator>().SetTrigger("IdleLeft");
                    }
                    else if (facing == 'S')
                    {
                        GetComponent<Animator>().SetTrigger("IdleFront");
                    }

                }
            }
            //Fires bomb if the player has a bomb available 
            if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown("g")) && isLocalPlayer && !dead && bombsAvailable > 0)
            {

                //Sets animatino depending on facing
                if (facing == 'N')
                {
                    GetComponent<Animator>().SetTrigger("ShootBack");
                }
                else if (facing == 'E')
                {
                    GetComponent<Animator>().SetTrigger("ShootRight");
                }
                else if (facing == 'W')
                {
                    GetComponent<Animator>().SetTrigger("ShootLeft");
                }
                else if (facing == 'S')
                {
                    GetComponent<Animator>().SetTrigger("ShootFront");
                }

                shootingDelay = 1;
                StartCoroutine(shootdelay());
                ShootBomb(); //Starts bomb shooting process
            }
           
            //uses special ability depending on which cat was chosen
            if (Input.GetKeyDown("f") && isLocalPlayer)
            {
                if (GameObject.Find("PowerCount").GetComponent<Text>().text == "go") //If text on abilCounter says GO then use ability 
                {
                    if (catNum == 1)
                    {
                        Teleport();
                    }
                    else if (catNum == 2)
                    {
                        phaseOut();
                    }
                    else if (catNum == 3)
                    {
                        lineShot();
                    }
                    else if (catNum == 4)
                    {
                        ScatterShot();
                    }
                    else if (catNum == 5)
                    {
                        waterBalloon();
                    }
                    else if (catNum == 6)
                    {
                        reverse();
                    }
                    else if (catNum == 7)
                    {
                        catPoop();
                    }
                    else if(catNum == 8)
                    {
                        CopyCat();
                    }
                }
            }
        }
       

    }

    //***********************************************************************************************************************************************************************************
    // Fakes the death of a cat to show it has been tagged by a bomb (no cats were harmed in the making of this game)
    //***********************************************************************************************************************************************************************************
    public void killSelf()
    {
        if(isLocalPlayer)
        {
            GetComponent<Player>().dead = true;
            GetComponent<Animator>().SetTrigger("Dead");
            GetComponent<BoxCollider2D>().enabled = false;
            CmdKillSelf(); //Sets the cat to fake dead on server also
        }
    }

    //***********************************************************************************************************************************************************************************
    // Sets cat to fake dead on server and checks to see if that was the last cat needed to go down for a victory
    //***********************************************************************************************************************************************************************************
    [Command]
    public void CmdKillSelf()
    {
        GetComponent<Player>().dead = true;
        GetComponent<Animator>().SetTrigger("Dead");
        GetComponent<BoxCollider2D>().enabled = false;

        //Checks to see if there is a last cat standing
        if (GameObject.Find("DataStore").GetComponent<DataStore>().level == "8PVolcano" || GameObject.Find("DataStore").GetComponent<DataStore>().level == "4PVolcano")
        {
            GameObject.Find("GameController").GetComponent<VolcanoController>().CheckForVicotry();
        }
        else
        {
            GameObject.Find("GameController").GetComponent<GameController>().CheckForVicotry();
        }
    }


    //***********************************************************************************************************************************************************************************
    //  sets player fake dead on all clients
    //***********************************************************************************************************************************************************************************
    [ClientRpc]
    public void RpcKillSelf()
    {
        GetComponent<Player>().dead = true;
        GetComponent<Animator>().SetTrigger("Dead");
        GetComponent<BoxCollider2D>().enabled = false;
    }

    //***********************************************************************************************************************************************************************************
    // Helper method to call speed set coRoutine from other objects
    //***********************************************************************************************************************************************************************************
    public void startspeed()
    {
        StartCoroutine(speedSet());
    }

    //***********************************************************************************************************************************************************************************
    //  Sets the speed for when reverse is used 
    //***********************************************************************************************************************************************************************************
    public IEnumerator speedSet()
    {
        speedMod = -.3f;
        yield return new WaitForSeconds(3);
        speedMod = .5f;
    }

    //***********************************************************************************************************************************************************************************
    // Starts chain to run CountToPlay() on all clients
    //***********************************************************************************************************************************************************************************
    [Command]
    public void CmdCountMap()
    {
        RpcCountMap();
    }

    //***********************************************************************************************************************************************************************************
    // Sends message to all clients to run countToplay()
    //***********************************************************************************************************************************************************************************
    [ClientRpc]
    public void RpcCountMap()
    {
        StartCoroutine(countToPlay());
      
    }

    //***********************************************************************************************************************************************************************************
    //  Couunts the map select screen down to launch map
    //***********************************************************************************************************************************************************************************
    public IEnumerator countToPlay()
    {
        for (int i = 5; i >0; i--)
        {
            yield return new WaitForSeconds(1);
            GameObject.Find("CountText").GetComponent<Text>().enabled = true;
            GameObject.Find("CountText").GetComponent<Text>().text = "" + i;
        }
    }

    //***********************************************************************************************************************************************************************************
    //  Makes sure there is at least a one frame delay between the player shooting bombs
    //***********************************************************************************************************************************************************************************
    public IEnumerator shootdelay()
    {

        for (int i = 0; i < 40; i++)
        {
            yield return new WaitForEndOfFrame();    
        }
        shootingDelay = 0;
    }

    //***********************************************************************************************************************************************************************************
    // Shoots bomb, bomb is shot first on the client that shot it then it is sent to the server which tells the clients to individually shoot the bomb themselves
    // This gets rid of any input lag for the player who shot it making it feel instant 
    //***********************************************************************************************************************************************************************************
    void ShootBomb()
    {
        
        if (timer == 0 && bombsAvailable > 0) // makes sure the player can shoot bombs
        {
          
            RaycastHit2D[] hitCheck;
          
            hitCheck = Physics2D.RaycastAll(transform.position, transform.position, 0f); //checks the tile the player is under 
            GameObject tile = null;

            for (int i = 0; i < hitCheck.Length; i++)
            {
                if (hitCheck[i].transform.tag == "Floor")
                {
                    tile = hitCheck[i].transform.gameObject;
                }
            }

            //block of code that finds the x,y position of the tile the player was under to determine where the bomb should go
            string nameDecode = tile.transform.name;
            char[] nDecode = nameDecode.ToCharArray();
            int x, y;
            string n = char.ToString(nDecode[0]);
            x = int.Parse(n);
            if (nDecode.Length > 3)
            {
                string temp = "" + nDecode[2] + "" + nDecode[3];
                y = int.Parse(temp);
            }
            else
            {
                string nm = char.ToString(nDecode[2]);
                y = int.Parse(nm);
            }

            //plays the sound of the bomb firing depending on which cat the player has chosen
            if(catNum == 1 || catNum == 4)
            {
                GetComponent<AudioSource>().PlayOneShot(NormalFire);
            }
            else if (catNum == 2)
            {
                GetComponent<AudioSource>().PlayOneShot(portalCatFire);
            }
            else if(catNum == 3)
            {
                GetComponent<AudioSource>().PlayOneShot(trebCatFire);
            }

            timer = 1;
           
            StartCoroutine(shootTimer());
            GameObject.Find("ID").GetComponent<Text>().text = ""+uniqueID;
            //if the player is not the host sends the bomb info to the server to be processed on other clients
   
            if (!isServer)
            {
                if(bombDistance == 0)
                {
                    bombDistance = 1;
                }
                GameObject tBomb = (GameObject)Instantiate(bomb, new Vector3(transform.position.x, transform.position.y), Quaternion.identity);
                tBomb.GetComponent<Bomb>().homePlayer = gameObject;
                Vector2 posToGo = tBomb.GetComponent<Bomb>().CalculateTrajectory(new Vector2(x, y), facing, bombDistance, bombPower, "normal");
                tBomb.GetComponent<Bomb>().startMove(posToGo);
                tBomb.GetComponent<Bomb>().homePlayer = gameObject;
                bombsAvailable--;
          
                CmdShootBomb(x, y, facing, bombDistance, tBomb, uniqueID);
            }
            //if the player is the host sends the bomb to the server to be ran there
            else
            {
                GameObject tBomb = null;

                CmdShootBomb(x, y, facing, bombDistance, tBomb, uniqueID);
            }
        }
    }

    //***********************************************************************************************************************************************************************************
    // Shoots a bomb in a random spot in front of the cats facing
    //***********************************************************************************************************************************************************************************
    void ShootBombScatter(int xDir, int yDir)
    {

        if (timer == 0)
        {

            RaycastHit2D[] hitCheck;

            hitCheck = Physics2D.RaycastAll(transform.position, transform.position, 0f);
            GameObject tile = null;

            for (int i = 0; i < hitCheck.Length; i++)
            {
                if (hitCheck[i].transform.tag == "Floor")
                {
                    tile = hitCheck[i].transform.gameObject;
                }
            }
            Debug.Log(tile);
            string nameDecode = tile.transform.name;
            char[] nDecode = nameDecode.ToCharArray();
            int x, y;
            string n = char.ToString(nDecode[0]);
            x = int.Parse(n);
            if (nDecode.Length > 3)
            {
                string temp = "" + nDecode[2] + "" + nDecode[3];
                y = int.Parse(temp);
            }
            else
            {
                string nm = char.ToString(nDecode[2]);
                y = int.Parse(nm);
            }
            if (catNum == 1 || catNum == 3)
            {
                GetComponent<AudioSource>().PlayOneShot(NormalFire);
            }
            else if (catNum == 2)
            {
                GetComponent<AudioSource>().PlayOneShot(portalCatFire);
            }
            else if (catNum == 4)
            {
                GetComponent<AudioSource>().PlayOneShot(trebCatFire);
            }



            StartCoroutine(shootTimer());
            GameObject.Find("ID").GetComponent<Text>().text = "" + uniqueID;
            if (!isServer)
            {
                if (bombDistance == 0)
                {
                    bombDistance = 1;
                }
                GameObject tBomb = (GameObject)Instantiate(bomb, new Vector3(transform.position.x, transform.position.y), Quaternion.identity);
                tBomb.GetComponent<Bomb>().homePlayer = gameObject;
                Vector2 posToGo = tBomb.GetComponent<Bomb>().CalculateTrajectory(new Vector2(x + xDir, y + yDir), facing, bombDistance, bombPower, "normal");
                tBomb.GetComponent<Bomb>().startMove(posToGo);
                tBomb.GetComponent<Bomb>().homePlayer = gameObject;
                bombsAvailable--;

                CmdShootBombScatter(x, y, facing, bombDistance, tBomb, uniqueID,xDir,yDir);
            }
            else
            {
                GameObject tBomb = null;

                CmdShootBombScatter(x, y, facing, bombDistance, tBomb, uniqueID, xDir, yDir);
            }
        }
    }

    //***********************************************************************************************************************************************************************************
    // Used by server to shoot fireballs on lava map
    //***********************************************************************************************************************************************************************************
    public void ShootBombNPC(int xDir, int yDir, Vector3 pos, int x, int y)
    {

        if (timer == 0)
        {                  
            StartCoroutine(shootTimer());
            GameObject.Find("ID").GetComponent<Text>().text = "" + uniqueID;
            if (isServer)
            {
                GameObject tBomb = null;
                CmdShootBombNPC(x, y, facing, 0, tBomb, uniqueID, xDir, yDir, pos);
            }
        }
    }

 

    //***********************************************************************************************************************************************************************************
    // Server side response to a client shooting a bomb
    //
    // Takes 8 parameters, the x and y position of the player shooting it, a char for the facing, the distance the bomb will travel, the bomb gameobject, the spc aka the id of the 
    // player who shot the bomb, amd the x and y values for the bomb offset 
    //***********************************************************************************************************************************************************************************
    [Command]
    void CmdShootBombScatter(int x, int y, char f, int distance, GameObject bombb, int spc, int xDir, int yDir)
    {
        Debug.Log("SPC IS " + spc); //Checking the player that shot the bomb
        //if the bomb is being placed directly under the player, makes sure to reset it so it goes at least one tile away
        if (bombDistance == 0)
        {
            bombDistance = 1;
            distance = 1;
        }

        //Plays the sound of the cat shooting the bomb
        if (catNum == 1 || catNum == 3)
        {
            GetComponent<AudioSource>().PlayOneShot(NormalFire);
        }
        else if (catNum == 2)
        {
            GetComponent<AudioSource>().PlayOneShot(portalCatFire);
        }
        else if (catNum == 4)
        {
            GetComponent<AudioSource>().PlayOneShot(trebCatFire);
        }

        //Old code to drop bomb under the cat like classic bomberman, holding onto it in case I ever want a gamemode like that
        if (bombDistance == 0)
        {
            RaycastHit2D[] hitCheck;
            hitCheck = Physics2D.RaycastAll(new Vector2(transform.position.x, transform.position.y), new Vector2(transform.position.x, transform.position.y), 0f);

            Vector3 tempGameobject;

            for (int i = 0; i < hitCheck.Length; i++)
            {
                if (hitCheck[i].transform.tag == "Floor")
                {
                    tempGameobject = hitCheck[i].transform.position;
                }
            }

            if (bombb == null)
            {
                bombsAvailable--;
                bombb = (GameObject)Instantiate(bomb, new Vector3(transform.position.x, transform.position.y), Quaternion.identity);
                bombb.GetComponent<Bomb>().homePlayer = gameObject;
                bombb.GetComponent<Bomb>().bombPower = bombPower;
                Vector2 posToGo = bombb.GetComponent<Bomb>().CalculateTrajectory(new Vector2(x + xDir, y +yDir), f, distance, bombPower, "normal");
                bombb.GetComponent<Bomb>().startMove(posToGo);
                spc = uniqueID;
            }
            RpcShootBombScatter(x, y, f, distance, bombb.GetComponent<Bomb>().bombPower, spc, xDir, yDir);
            // RpcBombCount(0, gameObject);
        }
        //Sets the bombs position on the server, sends the bomb to the clients to be spawned
        else
        {
            //gives bomb default bomb features if it has none, should never happen
            if (bombb == null)
            {
                bombsAvailable--; //decreases the bombs available to player until they explode
                bombb = (GameObject)Instantiate(bomb, new Vector3(transform.position.x, transform.position.y), Quaternion.identity);
                bombb.GetComponent<Bomb>().homePlayer = gameObject;
                bombb.GetComponent<Bomb>().bombPower = bombPower;
                Vector2 posToGo = bombb.GetComponent<Bomb>().CalculateTrajectory(new Vector2(x +xDir, y+yDir), f, distance, bombPower, "normal");
                bombb.GetComponent<Bomb>().startMove(posToGo);
                spc = uniqueID;
            }
            //sends bomb to clients
            RpcShootBombScatter(x, y, f, distance, bombb.GetComponent<Bomb>().bombPower, spc, xDir, yDir);
           
        }

    }


    //***********************************************************************************************************************************************************************************
    //  Processes a bomb being fired on the server and sends the bomb information to all clients
    //
    //  Takes 6 parameters, the x and y position the bomb will travel to, the facing of the player who shot it, the distance it will travel, the bomb object to use, and the 
    //  spc (uniqueID of player) who shot it
    //***********************************************************************************************************************************************************************************
    [Command]
    void CmdShootBomb(int x, int y, char f, int distance, GameObject bombb, int spc)
    {

        if (bombDistance == 0)
        {
            bombDistance = 1;
            distance = 1;
        }
        if (catNum == 1 || catNum == 3)
        {
            GetComponent<AudioSource>().PlayOneShot(NormalFire);
        }
        else if (catNum == 2)
        {
            GetComponent<AudioSource>().PlayOneShot(portalCatFire);
        }
        else if (catNum == 4)
        {
            GetComponent<AudioSource>().PlayOneShot(trebCatFire);
        }
        if (bombDistance == 0)
        {
            RaycastHit2D[] hitCheck;
            hitCheck = Physics2D.RaycastAll(new Vector2(transform.position.x, transform.position.y), new Vector2(transform.position.x, transform.position.y), 0f);

            Vector3 tempGameobject;

            for (int i = 0; i < hitCheck.Length; i++)
            {
                if (hitCheck[i].transform.tag == "Floor")
                {
                    tempGameobject = hitCheck[i].transform.position;
                }
            }

            if (bombb == null)
            {
                bombsAvailable--;
                bombb = (GameObject)Instantiate(bomb, new Vector3(transform.position.x, transform.position.y), Quaternion.identity);
                bombb.GetComponent<Bomb>().homePlayer = gameObject;
                bombb.GetComponent<Bomb>().bombPower = bombPower;
                Vector2 posToGo = bombb.GetComponent<Bomb>().CalculateTrajectory(new Vector2(x, y), f, distance, bombPower, "normal");
                bombb.GetComponent<Bomb>().startMove(posToGo);
                spc = uniqueID;
            }
            RpcShootBomb(x, y, f, distance, bombb.GetComponent<Bomb>().bombPower, spc);
            // RpcBombCount(0, gameObject);
        }
        else
        {
            if (bombb == null)
            {
                bombsAvailable--;
                bombb = (GameObject)Instantiate(bomb, new Vector3(transform.position.x, transform.position.y), Quaternion.identity);
                bombb.GetComponent<Bomb>().homePlayer = gameObject;
                bombb.GetComponent<Bomb>().bombPower = bombPower;
                Vector2 posToGo = bombb.GetComponent<Bomb>().CalculateTrajectory(new Vector2(x, y), f, distance, bombPower, "normal");
                bombb.GetComponent<Bomb>().startMove(posToGo);
                spc = uniqueID;
            }
            RpcShootBomb(x, y, f, distance, bombb.GetComponent<Bomb>().bombPower, spc);

        }

    }

    //***********************************************************************************************************************************************************************************
    // Used to shoot fireballs on lava map, spawns bombs offscreen and move them onto random locations
    //***********************************************************************************************************************************************************************************
    [Command]
    void CmdShootBombNPC(int x, int y, char f, int distance, GameObject bombb, int spc, int xDir, int yDir, Vector3 pos)
    {
        Debug.Log("SPC IS " + spc);
        if (bombDistance == 0)
        {
            bombDistance = 1;
            distance = 1;
        }

        if (bombDistance == 0)
        {
                  
            RpcShootBombNPC(x, y, f, 0, bombb.GetComponent<Bomb>().bombPower, spc, xDir, yDir, pos);
            
        }
        else
        {
            if (bombb == null && isServer)
            {
               
                bombb = (GameObject)Instantiate(firebomb, new Vector3(pos.x, pos.y), Quaternion.identity);
                bombb.GetComponent<Bomb>().homePlayer = gameObject;
                bombb.GetComponent<Bomb>().bombPower = bombPower;
                Vector2 posToGo = bombb.GetComponent<Bomb>().CalculateTrajectory(new Vector2(x + xDir, y + yDir), f, distance, bombPower, "fireMap");
                bombb.GetComponent<Bomb>().startMove(posToGo);
                spc = uniqueID;
            }
            RpcShootBombNPC(x, y, f, 0, bombb.GetComponent<Bomb>().bombPower, spc, xDir, yDir, pos);
         
        }

    }

    //***********************************************************************************************************************************************************************************
    //  Creates bombs on all the clients, the last phase of shooting a bomb
    //
    // Takes 8 parameters, the x and y position of the player shooting it, a char for the facing, the distance the bomb will travel, the bomb gameobject, the spc aka the id of the 
    // player who shot the bomb, amd the x and y values for the bomb offset 
    //***********************************************************************************************************************************************************************************
    [ClientRpc]
    void RpcShootBombScatter(int x, int y, char face, int bDist, int bPower, int spc, int xDir, int yDir)
    {
        //Plays sound for bomb being shot
        if (catNum == 1 || catNum == 3)
        {
            GetComponent<AudioSource>().PlayOneShot(NormalFire);
        }
        else if (catNum == 2)
        {
            GetComponent<AudioSource>().PlayOneShot(portalCatFire);
        }
        else if (catNum == 4)
        {
            GetComponent<AudioSource>().PlayOneShot(trebCatFire);
        }


        //Shoots the bomb on all clients except the one who shot it orignally and the host
        if (!isServer && spc != int.Parse(GameObject.Find("ID").GetComponent<Text>().text))
        {     
            if (bDist == 0)
            {
                bDist = 1;
            }
            GameObject tBomb = (GameObject)Instantiate(bomb, new Vector3(transform.position.x, transform.position.y), Quaternion.identity);
            tBomb.GetComponent<Bomb>().homePlayer = gameObject;
            Vector2 posToGo = tBomb.GetComponent<Bomb>().CalculateTrajectory(new Vector2(x+xDir, y+yDir), face, bDist, bPower, "normal");
            tBomb.GetComponent<Bomb>().startMove(posToGo);
        }
    }

    //***********************************************************************************************************************************************************************************
    //  Creates bombs on all the clients, the last phase of shooting a bomb
    //
    // Takes 9 parameters, the x and y position of the player shooting it, a char for the facing, the distance the bomb will travel, the bomb gameobject, the spc aka the id of the 
    // player who shot the bomb, amd the x and y values for the bomb offset.  Also has the initial offscreen starting position of fireballs as a vector3
    //***********************************************************************************************************************************************************************************
    [ClientRpc]
    void RpcShootBombNPC(int x, int y, char face, int bDist, int bPower, int spc, int xDir, int yDir, Vector3 pos)
    {
      
        if (!isServer && spc != int.Parse(GameObject.Find("ID").GetComponent<Text>().text))
        {

            if (bDist == 0)
            {
                bDist = 1;
            }
            GameObject tBomb = (GameObject)Instantiate(firebomb, new Vector3(pos.x, pos.y), Quaternion.identity);
            tBomb.GetComponent<Bomb>().homePlayer = gameObject;
            Vector2 posToGo = tBomb.GetComponent<Bomb>().CalculateTrajectory(new Vector2(x + xDir, y + yDir), face, 0, bPower, "fireMap");
            tBomb.GetComponent<Bomb>().startMove(posToGo);
        }
    }

    //***********************************************************************************************************************************************************************************
    //  Creates bombs on all the clients, the last phase of shooting a bomb
    //
    // Takes 6 parameters, the x and y position of the player shooting it, a char for the facing, the distance the bomb will travel, the bomb gameobject, the spc aka the id of the 
    // player who shot the bomb
    //***********************************************************************************************************************************************************************************
    [ClientRpc]
    void RpcShootBomb(int x, int y, char face, int bDist, int bPower, int spc)
    {

        if (!isServer && spc != int.Parse(GameObject.Find("ID").GetComponent<Text>().text))
        {
            if (catNum == 1 || catNum == 3)
            {
                GetComponent<AudioSource>().PlayOneShot(NormalFire);
            }
            else if (catNum == 2)
            {
                GetComponent<AudioSource>().PlayOneShot(portalCatFire);
            }
            else if (catNum == 4)
            {
                GetComponent<AudioSource>().PlayOneShot(trebCatFire);
            }
            Debug.Log("We are shooting on server from client");
            if (bDist == 0)
            {
                bDist = 1;
            }
            GameObject tBomb = (GameObject)Instantiate(bomb, new Vector3(transform.position.x, transform.position.y), Quaternion.identity);
            tBomb.GetComponent<Bomb>().homePlayer = gameObject;
            Vector2 posToGo = tBomb.GetComponent<Bomb>().CalculateTrajectory(new Vector2(x, y), face, bDist, bPower, "normal");
            tBomb.GetComponent<Bomb>().startMove(posToGo);
        }
    }


    //***********************************************************************************************************************************************************************************
    // Soawns the  powerup on the server and all clients
    //
    // Takes 3 parameters, the type of buff to spawn and the x and y value of where to spawn it
    //***********************************************************************************************************************************************************************************
    [Command]
    public void CmdSpawnBuff(GameObject objToSpawn,float x,float y)
    {
        if (isServer)
        {
            GameObject ttt = (GameObject)Instantiate(objToSpawn, new Vector3(x, y), Quaternion.identity);
            NetworkServer.Spawn(ttt); //puts the object on the server for all clients to see
        }
    }

    //***********************************************************************************************************************************************************************************
    // Soawns the quicksand on the server and all clients
    //
    // Takes 3 parameters, the QS object to spawn and the x and y value of where to spawn it
    //***********************************************************************************************************************************************************************************
    [Command]
    public void CmdSpawnQS(GameObject objToSpawn, float x, float y)
    {
        if (isServer)
        {
            GameObject ttt = (GameObject)Instantiate(objToSpawn, new Vector3(x, y), Quaternion.identity);
            NetworkServer.Spawn(ttt);
        }
    }

    //***********************************************************************************************************************************************************************************
    // Moves a bomb on all clients, isnt used anymore but holding on to it for future mechanics
    //
    // Takes two parameters, the bomb object to movoe and where its moving to
    //***********************************************************************************************************************************************************************************
    [ClientRpc]
    void RpcMoveBomb(GameObject tBomb, Vector2 move)
    {
        tBomb.GetComponent<Bomb>().startMove(move);      
    }

    //***********************************************************************************************************************************************************************************
    //  Timer to wait 30 frames between shooting a bomb
    //***********************************************************************************************************************************************************************************
    public IEnumerator shootTimer()
    {
        for(int i = 0; i < 30; i++)
        {
            yield return new WaitForEndOfFrame();
        }
       
        timer = 0;
    }

    //***********************************************************************************************************************************************************************************
    //  Sends a message to the server to add a bomb back to a players bombsavailable, from the server it sends back the okay to the clients for the player to have a bomb back
    //***********************************************************************************************************************************************************************************
    [Command]
    public void CmdRestoreBomb(GameObject player)
    {
        RpcRestoreBomb(player);
    }

    //***********************************************************************************************************************************************************************************
    // Adds a bomb back to the player to use after one of theirs explodes
    //***********************************************************************************************************************************************************************************
    [ClientRpc]
    public void RpcRestoreBomb(GameObject player)
    {
        if (bombsAvailable < bombMax)
        {
            player.GetComponent<Player>().bombsAvailable++;
        }
    }

    //***********************************************************************************************************************************************************************************
    //  Disables a crate on the server after getting hit by a bomb, sends the crate to be disabled to all clients
    //***********************************************************************************************************************************************************************************
    [Command]
    public void CmdDestroyCrate(GameObject crate)
    {
        crate.GetComponent<SpriteRenderer>().enabled = false;
        crate.GetComponent<BoxCollider2D>().enabled = false;
        RpcDestroyCrate(crate);
    }

    //***********************************************************************************************************************************************************************************
    // Disables a crate on all clients
    //***********************************************************************************************************************************************************************************
    [ClientRpc]
    public void RpcDestroyCrate(GameObject crate)
    {
        crate.GetComponent<SpriteRenderer>().enabled = false;
        crate.GetComponent<BoxCollider2D>().enabled = false;
    }




    //***********************************TELEPORT BLOCK***********************************************************************************************

    //***********************************************************************************************************************************************************************************
    //  Serverside Teleport, sends the message to spawn the teleport animations to all the clients
    //***********************************************************************************************************************************************************************************
    [Command]
    public void CmdTeleSpawn()
    {
        GameObject portAway = (GameObject)Instantiate(portGoingAway, new Vector3(transform.position.x, transform.position.y), Quaternion.identity);
        RpcTeleSpawn(); //Sends command to spawn teleport animations to clients
    }

    //***********************************************************************************************************************************************************************************
    //  Spawns teleport animations on clients
    //***********************************************************************************************************************************************************************************
    [ClientRpc]
    public void RpcTeleSpawn()
    {
        GameObject portAway = (GameObject)Instantiate(portGoingAway, new Vector3(transform.position.x, transform.position.y), Quaternion.identity);
    }

    //***********************************************************************************************************************************************************************************
    //  Serverside Teleport, sends the message to spawn the teleport animation for arriving to the clients
    //***********************************************************************************************************************************************************************************
    [Command]
    public void CmdTeleArrive()
    {
        GameObject portArrive = (GameObject)Instantiate(portArrival, new Vector3(transform.position.x, transform.position.y), Quaternion.identity);
        RpcTeleArrive();
    }


    //***********************************************************************************************************************************************************************************
    // Generates teleport animation for arriving on all the clients
    //***********************************************************************************************************************************************************************************
    [ClientRpc]
    public void RpcTeleArrive()
    {
        GameObject portArrive = (GameObject)Instantiate(portArrival, new Vector3(transform.position.x, transform.position.y), Quaternion.identity);
    }




    //***********************************************************************************************************************************************************************************
    //  Teleports the player 3 tiles in the direction they are currently facing, if that tile is blocked then closer tiles will be chosen in the same direction
    //***********************************************************************************************************************************************************************************
    public void Teleport()
    {
        bool floorCheck = false; //bool to record whether there is floor in the intended landing spot
        bool crateCheck = false; //bool to record whether there is a crate in the intended landing spot
        bool wallCheck = false;  //bool to record whether there is a wall at the intended landing spot

        abilTimer = 5; //sets the special ability cooldown timer
        StartCoroutine(abilCounter()); //beginsn counting down til teleport can be used again
        CmdSendCopy(1); //records the last used ability in the game for the vampire cat to copy
        GameObject portAway = (GameObject)Instantiate(portGoingAway, new Vector3(transform.position.x, transform.position.y), Quaternion.identity);

        //To be used if the player teleports north
        if (facing == 'N')
        {
            RaycastHit2D[] hitCheck;
            int tempCheck = 0;
            for (int j = 3; j > 0; j--)
            {
                //block of code to check where the player would land and adjust if needed
                hitCheck = Physics2D.RaycastAll(new Vector2(transform.position.x, transform.position.y + (.64f * j)), new Vector2(transform.position.x, transform.position.y + (.64f * j)), 0f);
                for (int i = 0; i < hitCheck.Length; i++)
                {
                    if (hitCheck[i].transform.tag == "Floor")
                    {
                        floorCheck = true;
                        tempCheck = i;
                    }
                    if (hitCheck[i].transform.tag == "Wall")
                    {
                        wallCheck = true;
                    }
                    if (hitCheck[i].transform.tag == "Crate")
                    {
                        crateCheck = true;
                    }
                }
                if(crateCheck == true || wallCheck == true)
                {
                    floorCheck = false;
                    crateCheck = false;
                    wallCheck = false;
                }
                else if(floorCheck == true)
                {
                    j = -1;
                    transform.position = new Vector3(hitCheck[tempCheck].transform.position.x, hitCheck[tempCheck].transform.position.y);
                    GameObject portArrive = (GameObject)Instantiate(portArrival, new Vector3(transform.position.x, transform.position.y), Quaternion.identity);
                    CmdTeleArrive();
                }
            }
           
        }
        //Player teleporting south
        else if (facing == 'S')
        {
            RaycastHit2D[] hitCheck;
            int tempCheck = 0;
            for (int j = 3; j > 0; j--)
            {

                hitCheck = Physics2D.RaycastAll(new Vector2(transform.position.x, transform.position.y - (.64f * j)), new Vector2(transform.position.x, transform.position.y - (.64f * j)), 0f);

                for (int i = 0; i < hitCheck.Length; i++)
                {
                    if (hitCheck[i].transform.tag == "Floor")
                    {
                        floorCheck = true;
                        tempCheck = i;
                    }
                    if (hitCheck[i].transform.tag == "Wall")
                    {
                        wallCheck = true;
                    }
                    if (hitCheck[i].transform.tag == "Crate")
                    {
                        crateCheck = true;
                    }
                }
                if (crateCheck == true || wallCheck == true)
                {
                    floorCheck = false;
                    crateCheck = false;
                    wallCheck = false;
                }
                else if (floorCheck == true)
                {
                    j = -1;
                    transform.position = new Vector3(hitCheck[tempCheck].transform.position.x, hitCheck[tempCheck].transform.position.y);
                    GameObject portArrive = (GameObject)Instantiate(portArrival, new Vector3(transform.position.x, transform.position.y), Quaternion.identity);
                    CmdTeleArrive();
                }
            }
        }
        //player teleporting east
        else if (facing == 'E')
        {
            RaycastHit2D[] hitCheck;
                int tempCheck = 0;
            for (int j = 3; j > 0; j--)
            {

                hitCheck = Physics2D.RaycastAll(new Vector2(transform.position.x + (.64f * j), transform.position.y), new Vector2(transform.position.x + (.64f * j), transform.position.y), 0f);
                for (int i = 0; i < hitCheck.Length; i++)
                {
                    if (hitCheck[i].transform.tag == "Floor")
                    {
                        floorCheck = true;
                        tempCheck = i;
                    }
                    if (hitCheck[i].transform.tag == "Wall")
                    {
                        wallCheck = true;
                    }
                    if (hitCheck[i].transform.tag == "Crate")
                    {
                        crateCheck = true;
                    }
                }
                if (crateCheck == true || wallCheck == true)
                {
                    floorCheck = false;
                    crateCheck = false;
                    wallCheck = false;
                }
                else if (floorCheck == true)
                {
                    j = -1;
                    transform.position = new Vector3(hitCheck[tempCheck].transform.position.x, hitCheck[tempCheck].transform.position.y);
                    GameObject portArrive = (GameObject)Instantiate(portArrival, new Vector3(transform.position.x, transform.position.y), Quaternion.identity);
                    CmdTeleArrive();
                }
            }
        }
        //player teleporting west
        else if (facing == 'W')
        {

            RaycastHit2D[] hitCheck;
            int tempCheck = 0;
            for (int j = 3; j > 0; j--)
            {

                hitCheck = Physics2D.RaycastAll(new Vector2(transform.position.x - (.64f * j), transform.position.y), new Vector2(transform.position.x - (.64f * j), transform.position.y), 0f);
                for (int i = 0; i < hitCheck.Length; i++)
                {
                    if (hitCheck[i].transform.tag == "Floor")
                    {
                        floorCheck = true;
                        tempCheck = i;
                    }
                    if (hitCheck[i].transform.tag == "Wall")
                    {
                        wallCheck = true;
                    }
                    if (hitCheck[i].transform.tag == "Crate")
                    {
                        crateCheck = true;
                    }
                }
                if (crateCheck == true || wallCheck == true)
                {
                    floorCheck = false;
                    crateCheck = false;
                    wallCheck = false;
                }
                else if (floorCheck == true)
                {
                    j = -1;
                    transform.position = new Vector3(hitCheck[tempCheck].transform.position.x, hitCheck[tempCheck].transform.position.y);
                    GameObject portArrive = (GameObject)Instantiate(portArrival, new Vector3(transform.position.x, transform.position.y), Quaternion.identity);
                    CmdTeleArrive();
                }
            }
        }
        transmitPosition(); //sends new position to the server to be synced
    }

    //***********************************************************************************************************************************************************************************
    //  Tells the server to send the message to update all the clients last ability used
    //***********************************************************************************************************************************************************************************
    [Command]
    public void CmdSendCopy(int num)
    {
        GameObject.Find("DataStore").GetComponent<DataStore>().catNum = num;
        RpcSendCopy((int)num);
    }

    //***********************************************************************************************************************************************************************************
    //  Stores the number value of the last ability used on all of the clients
    //***********************************************************************************************************************************************************************************
    [ClientRpc]
    public void RpcSendCopy(int num)
    {
        GameObject.Find("DataStore").GetComponent<DataStore>().catNum = num;
    }


    //***********************************************************************************************************************************************************************************
    //  Ability that makes the cat immune to damage for 3 seconds
    //***********************************************************************************************************************************************************************************
    public void phaseOut()
    {
        CmdSendCopy(2);
        abilTimer = 10;
        StartCoroutine(abilCounter());
        phasedOut = true;
       
        CmdPhasedOut();
        Color temp = new Color(255, 255, 255);
        temp.a = .5f;
        GetComponent<SpriteRenderer>().color = temp; //Makes the cat a little bit faded to show the use of the special

    }

    //***********************************************************************************************************************************************************************************
    //  timer that is activated when the player uses the phase ability allowing them to be invincible for 3 seconds
    //***********************************************************************************************************************************************************************************
    public IEnumerator phasedTimer()
    {
        yield return new WaitForSeconds(3);
        GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 255);
        phasedOut = false;
    }

    //***********************************************************************************************************************************************************************************
    // Passes on the faded color change of the cat sprite from using phase
    //***********************************************************************************************************************************************************************************
    [Command]
    public void CmdPhasedOut()
    {
        RpcPhasedOut();
    }


    //***********************************************************************************************************************************************************************************
    //  Changes the cat sprite to the faded color on all clients
    //***********************************************************************************************************************************************************************************
    [ClientRpc]
    public void RpcPhasedOut()
    {
        Color temp = new Color(255, 255, 255);
        temp.a = .5f;
        GetComponent<SpriteRenderer>().color = temp;
        StartCoroutine(phasedTimer());
    }


    //***********************************************************************************************************************************************************************************
    //  Uses ability which reverses the movement keys and speeds up all of the cats
    //***********************************************************************************************************************************************************************************
    public void reverse()
    {
        CmdSendCopy(6);
        abilTimer = 10;
        StartCoroutine(abilCounter());
        CmdReverse();
    }

    //***********************************************************************************************************************************************************************************
    // Sends message to server to make all of the clients movement reversed for a few seconds
    //***********************************************************************************************************************************************************************************
    [Command]
    public void CmdReverse()
    {
        RpcReverse();
    }

    //***********************************************************************************************************************************************************************************
    // Starts the counter to undo the reverse
    //***********************************************************************************************************************************************************************************
    public void startReverse()
    {
        StartCoroutine(reverseCounter());
    }

    //***********************************************************************************************************************************************************************************
    //  Sets all the players to be reversed movement and sped up a little
    //***********************************************************************************************************************************************************************************
    [ClientRpc]
    public void RpcReverse()
    {
     
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < players.Length; i++)
        {
            players[i].GetComponent<Player>().isReversed = true;
            players[i].GetComponent<Player>().startReverse(); //Sets timer to undo reverse
        }
    }

    //***********************************************************************************************************************************************************************************
    //  Uses the last used ability in the game
    //***********************************************************************************************************************************************************************************
    public void CopyCat()
    {
        abilTimer = 1;
        StartCoroutine(abilCounter());
        CmdSendCopy(8);
        if (GameObject.Find("PowerCount").GetComponent<Text>().text == "go")
        {
            if (catNum == 1)
            {
                Teleport();
            }
            else if (catNum == 2)
            {
                phaseOut();
            }
            else if (catNum == 3)
            {
                lineShot();
            }
            else if (catNum == 4)
            {
                ScatterShot();
            }
            else if (catNum == 5)
            {
                waterBalloon();
            }
            else if (catNum == 6)
            {
                reverse();
            }
            else if (catNum == 7)
            {
                catPoop();
            }
            else if (catNum == 8)
            {
                CopyCat();
            }
        }

    }

    //***********************************************************************************************************************************************************************************
    //  Spawns a water balloon for the waterballoon special attack
    //***********************************************************************************************************************************************************************************
    [Command]
    public void CmdSpawnBalloon(GameObject objToSpawn, float x, float y)
    {

        GameObject ttt = (GameObject)Instantiate(balloon, new Vector3(x, y), Quaternion.identity);
        NetworkServer.Spawn(ttt);

    }

    //***********************************************************************************************************************************************************************************
    // Uses the waterballoon ability slowing down enemies in a large radius around the balloon spawn
    //***********************************************************************************************************************************************************************************
    public void waterBalloon()
    {
        CmdSendCopy(5);
        abilTimer = 6;
        StartCoroutine(abilCounter());
        RaycastHit2D[] hitCheck;
        //Spawns balloon north
        if (facing == 'N')
        {
            hitCheck = Physics2D.RaycastAll(new Vector3(transform.position.x, transform.position.y + 3.2f), new Vector3(transform.position.x, transform.position.y + 3.2f), 0f);
            for(int i = 0; i < hitCheck.Length; i++)
            {
                if(hitCheck[i].transform.tag == "Floor")
                {
                    CmdSpawnBalloon(balloon, hitCheck[i].transform.position.x, hitCheck[i].transform.position.y);
                }
            }
        }
        //spawns balloon south
        else if(facing == 'S')
        {
            hitCheck = Physics2D.RaycastAll(new Vector3(transform.position.x, transform.position.y - 3.2f), new Vector3(transform.position.x, transform.position.y - 3.2f), 0f);
            for (int i = 0; i < hitCheck.Length; i++)
            {
                if (hitCheck[i].transform.tag == "Floor")
                {
                    CmdSpawnBalloon(balloon, hitCheck[i].transform.position.x, hitCheck[i].transform.position.y);
                }
            }
        }
        //spawns balloon east
        else if(facing == 'E')
        {
            hitCheck = Physics2D.RaycastAll(new Vector3(transform.position.x + 3.2f, transform.position.y), new Vector3(transform.position.x + 3.2f, transform.position.y), 0f);
            for (int i = 0; i < hitCheck.Length; i++)
            {
                if (hitCheck[i].transform.tag == "Floor")
                {
                    CmdSpawnBalloon(balloon, hitCheck[i].transform.position.x, hitCheck[i].transform.position.y);
                }
            }
        }
        //spawns balloon west
        else
        {
            hitCheck = Physics2D.RaycastAll(new Vector3(transform.position.x - 3.2f, transform.position.y), new Vector3(transform.position.x - 3.2f, transform.position.y), 0f);
            for (int i = 0; i < hitCheck.Length; i++)
            {
                if (hitCheck[i].transform.tag == "Floor")
                {
                    CmdSpawnBalloon(balloon, hitCheck[i].transform.position.x, hitCheck[i].transform.position.y);
                }
            }
        }
    }

    //***********************************************************************************************************************************************************************************
    //  Shoots a line of bombs 5 long in front of the cats current facing
    //***********************************************************************************************************************************************************************************
    public void lineShot()
    {
        CmdSendCopy(4);
        abilTimer = 15;
        StartCoroutine(abilCounter());
        if (facing == 'N')
        {
            for (int i = 0; i < 5; i++)
            {
                int bombDist = i;
                int bombPeal = 0;
                ShootBombScatter(bombDist, bombPeal);
            }
        }
        else if (facing == 'E')
        {
            for (int i = 0; i < 5; i++)
            {
                int bombDist = 0;
                int bombPeal = i;
                ShootBombScatter(bombDist, bombPeal);
            }
        }
        else if (facing == 'W')
        {
            for (int i = 0; i > -5; i--)
            {
                int bombDist = 0;
                int bombPeal = i;
                ShootBombScatter(bombDist, bombPeal);
            }
        }
        else if (facing == 'S')
        {
            for (int i = 0; i > -5; i--)
            {
                int bombDist = i;
                int bombPeal = 0;
                ShootBombScatter(bombDist, bombPeal);
            }
        }
    }


    //***********************************************************************************************************************************************************************************
    // Poops a cat turd onto the battlefield in front of the cats current facing, this turd will respawn every round reset 
    //***********************************************************************************************************************************************************************************
    public void catPoop()
    {
        CmdSendCopy(7);
        abilTimer = 2;
        StartCoroutine(abilCounter());
      
        RaycastHit2D[] hitCheck;

        //Poops to the north
        if(facing == 'N')
        {
            hitCheck = Physics2D.RaycastAll(new Vector3(transform.position.x, transform.position.y + .64f),new Vector3 (transform.position.x, transform.position.y + .64f), 0f); //Forms a list of all objects on the tile in front of the cat using carPoop
            //Checks that tile to see if its a valid space
            for(int i = 0; i < hitCheck.Length; i++)
            {
                if(hitCheck[i].transform.tag == "Wall" || hitCheck[i].transform.tag == "Crate") //If the tile in front of the cat is a wall or crate the cat cannot pooop
                {
                    break;
                }
                if(hitCheck[i].transform.tag == "Floor")
                {
                    if(isLocalPlayer)
                    {
                        Debug.Log("AM I THE LOCALCAT");
                        CmdCatPoop(poop, hitCheck[i].transform.position.x, hitCheck[i].transform.position.y);
                    }
                }
            }
        }
        //Poops to the south
        if (facing == 'S')
        {
            hitCheck = Physics2D.RaycastAll(new Vector3(transform.position.x, transform.position.y - .64f), new Vector3(transform.position.x, transform.position.y - .64f), 0f);
            for (int i = 0; i < hitCheck.Length; i++)
            {
                if (hitCheck[i].transform.tag == "Wall" || hitCheck[i].transform.tag == "Crate")
                {
                    break;
                }
                if (hitCheck[i].transform.tag == "Floor")
                {
                    if (isLocalPlayer)
                    {
                        CmdCatPoop(poop, hitCheck[i].transform.position.x, hitCheck[i].transform.position.y);
                    }
                }
            }
        }
        //Poops to the east
        if (facing == 'E')
        {
            hitCheck = Physics2D.RaycastAll(new Vector3(transform.position.x + .64f, transform.position.y), new Vector3(transform.position.x + .64f, transform.position.y), 0f);
            for (int i = 0; i < hitCheck.Length; i++)
            {
                if (hitCheck[i].transform.tag == "Wall" || hitCheck[i].transform.tag == "Crate")
                {
                    break;
                }
                if (hitCheck[i].transform.tag == "Floor")
                {
                    if (isLocalPlayer)
                    {
                        CmdCatPoop(poop, hitCheck[i].transform.position.x, hitCheck[i].transform.position.y);
                    }
                }
            }
        }
        //Poops to the west
        if (facing == 'W')
        {
            hitCheck = Physics2D.RaycastAll(new Vector3(transform.position.x - .64f, transform.position.y), new Vector3(transform.position.x - .64f, transform.position.y), 0f);
            for (int i = 0; i < hitCheck.Length; i++)
            {
                if (hitCheck[i].transform.tag == "Wall" || hitCheck[i].transform.tag == "Crate")
                {
                    break;
                }
                if (hitCheck[i].transform.tag == "Floor")
                {
                    if (isLocalPlayer)
                    {
                        CmdCatPoop(poop, hitCheck[i].transform.position.x, hitCheck[i].transform.position.y);
                    }
                }
            }
        }

    }
    //***********************************************************************************************************************************************************************************
    // Spawns catpoop as a network object on the server
    //
    // Takes three parameters, the poop object and thet x and y values of where to spawn it
    //***********************************************************************************************************************************************************************************
    [Command]
    public void CmdCatPoop(GameObject objToSpawn, float x, float y)
    {
       
            GameObject ttt = (GameObject)Instantiate(poop, new Vector3(x, y), Quaternion.identity);
            NetworkServer.Spawn(ttt);


        
    }
    //***********************************************************************************************************************************************************************************
    //  Shoots a series of bombs equal to the maXBombsAvailable integer into random locations in front of the cats current position
    //***********************************************************************************************************************************************************************************
    public void ScatterShot()
    {
        CmdSendCopy(3);
        abilTimer = 15;
        StartCoroutine(abilCounter());
        int tempBombMax = bombMax;
        if(bombMax > 7)
        {
            tempBombMax = 7; //If the bombmax is higher than 7 set it to 7 for this ability
        }
        
        if (facing == 'N')
        {
            for(int i = 0; i < tempBombMax; i++)
            {
                int bombDist = Random.Range(1, 5); //Random bomb distance traveled
                int bombPeal = Random.Range(-2, 2); //random left and right bomb movement
                ShootBombScatter(bombDist, bombPeal);
            }
        }
        else if(facing == 'E')
        {
            for (int i = 0; i < tempBombMax; i++)
            {
                int bombDist = Random.Range(-2, 2);
                int bombPeal = Random.Range(1, 5);
                ShootBombScatter(bombDist, bombPeal);
            }
        }
        else if(facing == 'W')
        {
            for (int i = 0; i < tempBombMax; i++)
            {
                int bombDist = Random.Range(-2, 2);
                int bombPeal = Random.Range(-1, -5);
                ShootBombScatter(bombDist, bombPeal);
            }
        }
        else if(facing == 'S')
        {
            for (int i = 0; i < tempBombMax; i++)
            {
                int bombDist = Random.Range(-1, -5);
                int bombPeal = Random.Range(-2, 2);
                ShootBombScatter(bombDist, bombPeal);
            }
        }
    }

    //***********************************************************************************************************************************************************************************
    // Selects the first cat (teleport cat)
    //
    // Takes one parameter, the cat num to select
    //***********************************************************************************************************************************************************************************
    [Command]
    public void CmdSelectCat1(int ID)
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].GetComponent<Player>().getPlayerId() == ID)
            {
                players[i].GetComponent<Animator>().runtimeAnimatorController =C1;
                players[i].GetComponent<Player>().catNum = 1;
                RpcSelectCat1(ID);
            }
        }
    }

    [ClientRpc]
    public void RpcSelectCat1(int ID)
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].GetComponent<Player>().getPlayerId() == ID)
            {
                players[i].GetComponent<Animator>().runtimeAnimatorController = C1;
                players[i].GetComponent<Player>().catNum = 1;
            }
        }
    }


    [Command]
    public void CmdSelectCat2(int ID)
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].GetComponent<Player>().getPlayerId() == ID)
            {
                players[i].GetComponent<Animator>().runtimeAnimatorController = C2;
                players[i].GetComponent<Player>().catNum = 2;
                RpcSelectCat2(ID);
            }
        }
    }

    [ClientRpc]
    public void RpcSelectCat2(int ID)
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].GetComponent<Player>().getPlayerId() == ID)
            {
                players[i].GetComponent<Animator>().runtimeAnimatorController = C2;
                players[i].GetComponent<Player>().catNum = 2;
            }
        }
    }



    [Command]
    public void CmdSelectCat3(int ID)
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].GetComponent<Player>().getPlayerId() == ID)
            {
                players[i].GetComponent<Animator>().runtimeAnimatorController = C3;
                players[i].GetComponent<Player>().catNum = 3;
                RpcSelectCat3(ID);
            }
        }
    }

    [ClientRpc]
    public void RpcSelectCat3(int ID)
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].GetComponent<Player>().getPlayerId() == ID)
            {
                players[i].GetComponent<Animator>().runtimeAnimatorController = C3;
                players[i].GetComponent<Player>().catNum = 3;
            }
        }
    }

    [Command]
    public void CmdSelectCat4(int ID)
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].GetComponent<Player>().getPlayerId() == ID)
            {
                players[i].GetComponent<Animator>().runtimeAnimatorController = C4;
                players[i].GetComponent<Player>().catNum = 4;
                RpcSelectCat4(ID);
            }
        }
    }

    [ClientRpc]
    public void RpcSelectCat4(int ID)
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].GetComponent<Player>().getPlayerId() == ID)
            {
                players[i].GetComponent<Animator>().runtimeAnimatorController = C4;
                players[i].GetComponent<Player>().catNum = 4;
            }
        }
    }

    [Command]
    public void CmdSelectCat5(int ID)
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].GetComponent<Player>().getPlayerId() == ID)
            {
                players[i].GetComponent<Animator>().runtimeAnimatorController = C5;
                players[i].GetComponent<Player>().catNum = 5;
                RpcSelectCat5(ID);
            }
        }
    }

    [ClientRpc]
    public void RpcSelectCat5(int ID)
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].GetComponent<Player>().getPlayerId() == ID)
            {
                players[i].GetComponent<Animator>().runtimeAnimatorController = C5;
                players[i].GetComponent<Player>().catNum = 5;
            }
        }
    }


    [Command]
    public void CmdSelectCat6(int ID)
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].GetComponent<Player>().getPlayerId() == ID)
            {
                players[i].GetComponent<Animator>().runtimeAnimatorController = C6;
                players[i].GetComponent<Player>().catNum = 6;
                RpcSelectCat6(ID);
            }
        }
    }

    [ClientRpc]
    public void RpcSelectCat6(int ID)
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].GetComponent<Player>().getPlayerId() == ID)
            {
                players[i].GetComponent<Animator>().runtimeAnimatorController = C6;
                players[i].GetComponent<Player>().catNum = 6;
            }
        }
    }


    [Command]
    public void CmdSelectCat7(int ID)
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].GetComponent<Player>().getPlayerId() == ID)
            {
                players[i].GetComponent<Animator>().runtimeAnimatorController = C7;
                players[i].GetComponent<Player>().catNum = 7;
                RpcSelectCat7(ID);
            }
        }
    }

    [ClientRpc]
    public void RpcSelectCat7(int ID)
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].GetComponent<Player>().getPlayerId() == ID)
            {
                players[i].GetComponent<Animator>().runtimeAnimatorController = C7;
                players[i].GetComponent<Player>().catNum = 7;
            }
        }
    }


    [Command]
    public void CmdSelectCat8(int ID)
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].GetComponent<Player>().getPlayerId() == ID)
            {
                players[i].GetComponent<Animator>().runtimeAnimatorController = C8;
                players[i].GetComponent<Player>().catNum = 8;
                RpcSelectCat8(ID);
            }
        }
    }

    [ClientRpc]
    public void RpcSelectCat8(int ID)
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].GetComponent<Player>().getPlayerId() == ID)
            {
                players[i].GetComponent<Animator>().runtimeAnimatorController = C8;
                players[i].GetComponent<Player>().catNum = 8;
            }
        }
    }


    //***********************************************************************************************************************************************************************************
    // Selects the first map(forest)
    //***********************************************************************************************************************************************************************************
    public void selectMap1()
    {
        if(isServer)
        {
            GameObject.Find("Voting").GetComponent<VoteCount>().arr[0]++;
            Debug.Log("IAMSELECT");
        }
        else
        {
            CmdSelectMap1();
        }
    }

    [Command]
    public void CmdSelectMap1()
    {
        GameObject.Find("Voting").GetComponent<VoteCount>().arr[0]++;
    }



    //***********************************************************************************************************************************************************************************
    // Selects the second map(volcano)
    //***********************************************************************************************************************************************************************************
    public void selectMap2()
    {
        if (isServer)
        {
            GameObject.Find("Voting").GetComponent<VoteCount>().arr[1]++;
            Debug.Log("IAMSELECT");
        }
        else
        {
            CmdSelectMap2();
        }
    }

    [Command]
    public void CmdSelectMap2()
    {
        GameObject.Find("Voting").GetComponent<VoteCount>().arr[1]++;
    }


    //***********************************************************************************************************************************************************************************
    // Selects the third map(INdustry)
    //***********************************************************************************************************************************************************************************
    public void selectMap3()
    {
        if (isServer)
        {
            GameObject.Find("Voting").GetComponent<VoteCount>().arr[2]++;
            Debug.Log("IAMSELECT");
        }
        else
        {
            CmdSelectMap3();
        }
    }

    [Command]
    public void CmdSelectMap3()
    {
        GameObject.Find("Voting").GetComponent<VoteCount>().arr[2]++;
    }

    //***********************************************************************************************************************************************************************************
    // Selects the fourth map(Litter)
    //***********************************************************************************************************************************************************************************
    public void selectMap4()
    {
        if (isServer)
        {
            GameObject.Find("Voting").GetComponent<VoteCount>().arr[3]++;
            Debug.Log("IAMSELECT");
        }
        else
        {
            CmdSelectMap3();
        }
    }
    //***********************************************************************************************************************************************************************************
    // This code block of select map 5-8 selects the 8 player versions in order of the first four
    //***********************************************************************************************************************************************************************************
    public void selectMap5()
    {
        if (isServer)
        {
            GameObject.Find("Voting").GetComponent<VoteCount>().arr[4]++;
            Debug.Log("IAMSELECT");
        }
        else
        {
            CmdSelectMap3();
        }
    }
    public void selectMap6()
    {
        if (isServer)
        {
            GameObject.Find("Voting").GetComponent<VoteCount>().arr[5]++;
            Debug.Log("IAMSELECT");
        }
        else
        {
            CmdSelectMap3();
        }
    }
    public void selectMap7()
    {
        if (isServer)
        {
            GameObject.Find("Voting").GetComponent<VoteCount>().arr[6]++;
            Debug.Log("IAMSELECT");
        }
        else
        {
            CmdSelectMap3();
        }
    }
    public void selectMap8()
    {
        if (isServer)
        {
            GameObject.Find("Voting").GetComponent<VoteCount>().arr[7]++;
            Debug.Log("IAMSELECT");
        }
        else
        {
            CmdSelectMap3();
        }
    }
    //***********************************************************************************************************************************************************************************
    // Returns if the current player is locally controlled as a bool
    //***********************************************************************************************************************************************************************************
    public bool checkIsLocal()
    {
        return (isLocalPlayer);
    }
    //***********************************************************************************************************************************************************************************
    // Initiates map selector
    //***********************************************************************************************************************************************************************************
    public void runMapSelector()
    {
        if (isServer)
        {
            GameObject.Find("Network Manager").GetComponent<networkManage>().RMapSelect();
        }
    }

    //***********************************************************************************************************************************************************************************
    // Runs selected map
    //***********************************************************************************************************************************************************************************
    public void runMap1()
    {
        // NetworkManager.ServerChangeScene();
        if (isServer)
        {
            GameObject.Find("Network Manager").GetComponent<networkManage>().RMap1();
        }
    }
    public void runMap2()
    {
        if (isServer)
        {
            GameObject.Find("Network Manager").GetComponent<networkManage>().RMap2();
        }
    }
    public void runMap3()
    {
        if (isServer)
        {
            GameObject.Find("Network Manager").GetComponent<networkManage>().RMap3();
        }
    }
    public void runMap4()
    {
        if (isServer)
        {
            GameObject.Find("Network Manager").GetComponent<networkManage>().RMap4();
        }
    }
    public void runMap5()
    {
        if (isServer)
        {
            GameObject.Find("Network Manager").GetComponent<networkManage>().RMap5();
        }
    }
    public void runMap6()
    {
        if (isServer)
        {
            GameObject.Find("Network Manager").GetComponent<networkManage>().RMap6();
        }
    }
    public void runMap7()
    {
        if (isServer)
        {
            GameObject.Find("Network Manager").GetComponent<networkManage>().RMap7();
        }
    }
    public void runMap8()
    {
        if (isServer)
        {
            GameObject.Find("Network Manager").GetComponent<networkManage>().RMap8();
        }
    }


}

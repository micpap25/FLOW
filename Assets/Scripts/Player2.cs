using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Player2 : MonoBehaviour
{
    //GOALS FOR FIRST BUILD:
    //BASIC MOVEMENT, JUMPING, DASHING, ETC. AND METERS!
    //BUILD OFF FIRST PROJECT'S IDEAS? USE ITS CONCEPTS?
    //AFTER THAT; BLOCKING AND BASIC ATTACKS! MAYBE EVEN CANCELS! FLOW & COMBO!
    //FOLLOW UP WITH COMBINATION ATTACKS AND SPECIALS! SPLIT UP ENDLAG TO DASHLAG AND BLOCKLAG TO ALLOW FOR DASH CANCELS AND ADVANCE GUARDS!
    //AIRDASHING? DOUBLE JUMPS? WHO KNOWS? 
    //TO SAVE FOR A LONG TIME; ASSISTS, SUPERS, THE STORY, GUI PAST THE VITALS, MOTIVATORS, MODES!
    //EVENTUALLY THIS WILL BE A GOOD GAME!


    //use the name for moves!
    public string playerName;
    public int health;
    public float moveSpeed;
    public float dashSpeed;
    public float jumpSpeed;
    public string facing;
    public float startInputRefreshTime;
    public float startDashTime;
    public bool canCrawl;
    public float crawlSpeed;
    public float crouchScale;
    public float hitstun;
    private float endlag;
    private string keyword;
    private string dashdir;
    public List<string> FrontDashInput;
    public List<string> BackDashInput;
    private List<string> QCFInput;
    private List<string> QCBInput;
    private List<string> DQCFInput;
    private List<string> DQCBInput;
    private List<string> DDInput;
    public List<string> FrontDash;
    public List<string> BackDash;
    private List<string> QCF;
    private List<string> QCB;
    private List<string> DQCF;
    private List<string> DQCB;
    private List<string> DD;
    private List<string> actionsTaken;
    public float dashTime;
    public float inputRefreshTime;
    private bool canCancel;
    private bool isCrouching;
    private bool crouchAdjusted;
    private bool canBlockHigh;
    private bool canBlockMid;
    private bool canBlockLow;
    private bool isAirborne;
    public bool comboing;
    private string mostRecentAttackType;
    private bool blockStunning;
    private BoxCollider2D boxcol;
    private Animator anim;
    private SpriteRenderer spriteRenderer;
    public Sprite neutral;
    public Sprite attacking;
    public Sprite stun;
    public Sprite block;
    private GameObject opponent;
    private Rigidbody2D rb;
    private Camera camera;
    private float oldsize;
    private LayerMask enemies;
    public GameObject visualizer;

    // Use this for initialization
    void Start()
    {
        //set hitstun and endlag. hitstun is for damage lag, endlag is for move lag. Eventually split endlag into endlag, dashlag, and blocklag.
        hitstun = 0;
        endlag = 0;
        //lists for inputs. input list is const, other list changes as it goes through, if it's empty the move can be performed. Maybe change to arraylist?
        //after inputRefreshTime change all the lists in inputs back to the defaults.
        inputRefreshTime = 0;
        dashdir = "none";
        FrontDashInput = new List<string> { "front", "front" };
        BackDashInput = new List<string> { "back", "back" };
        QCFInput = new List<string> { "down", "front" };
        QCBInput = new List<string> { "down", "back" };
        DQCFInput = new List<string> { "front", "down", "front" };
        DQCBInput = new List<string> { "back", "down", "back" };
        DDInput = new List<string> { "down", "down" };
        FrontDash = new List<string>(FrontDashInput);
        BackDash = new List<string>(BackDashInput);
        QCF = new List<string>(QCFInput);
        QCB = new List<string>(QCBInput);
        DQCF = new List<string>(DQCFInput);
        DQCB = new List<string>(DQCBInput);
        DD = new List<string>(DDInput);
        keyword = "none";
        //list of actions taken. can't  repeat the same action twice in a flow.
        actionsTaken = new List<string> { };
        //set the committment to dashing. maybe merge into endlag?
        dashTime = -1;
        //used to check if a move can cancel into a move/jump. true when the attack hits. false otherwise. Must be combined with checking the move's type. 
        canCancel = false;
        //used to check if the character is crouching. Height is lowered & can't move unless isCrawl is true, then they move at CrawlSpeed.
        isCrouching = false;
        //used to check if height change for crouching has been done. 
        crouchAdjusted = true;
        //used to check if the character can currently block certain types of attacks
        canBlockHigh = false;
        canBlockMid = false;
        canBlockLow = false;
        //used to check if the character is in the air. 
        isAirborne = false;
        //what the most recent attack was. use with above.
        mostRecentAttackType = "none";
        //aaaaaaaaaaa
        boxcol = GetComponent<BoxCollider2D>();
        oldsize = boxcol.size.y;
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        camera = Camera.main;
        opponent = GameObject.FindGameObjectWithTag("Player1");
        enemies = LayerMask.GetMask("Player1");

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        bool right = Input.GetKey(KeyCode.RightArrow);
        bool left = Input.GetKey(KeyCode.LeftArrow);
        //set important stuff first
        if (!Input.GetKey(KeyCode.DownArrow))
        {
            isCrouching = false;
            crouchAdjusted = false;
        }
        if (health <= 0)
            Destroy(gameObject);
        if (opponent.transform.position.x < transform.position.x)
            facing = "left";
        else
            facing = "right";
        if (!crouchAdjusted)
        {
            if (isCrouching)
            {
                boxcol.offset = new Vector2(0, ((oldsize * crouchScale) - (oldsize)) / 2);
                boxcol.size = new Vector2(boxcol.size.x, oldsize * crouchScale);
            }
            else
            {
                boxcol.offset = new Vector2(0, 0);
                boxcol.size = new Vector2(boxcol.size.x, oldsize);
            }
            crouchAdjusted = true;
        }

        if (gameObject.transform.position.y > 0)
        {
            isAirborne = true;
            BackDash = new List<string>(BackDashInput);
            FrontDash = new List<string>(FrontDashInput);
            GetComponent<Rigidbody2D>().AddForce(-transform.up * jumpSpeed * jumpSpeed);
        }
        else
        {
            isAirborne = false;
            GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
            if (gameObject.transform.position.y < 0)
            {
                float x = gameObject.transform.position.x;
                gameObject.transform.position = new Vector3(x, 0, 0);
            }
        }

        if (dashTime == -1)
        {
            dashdir = "none";
        }


        //check for actions that can only be taken in neutral (walking, dashing, blocking)
        if (hitstun == 0 && endlag == 0)
        {
            spriteRenderer.sprite = neutral;

            if (left && facing.Equals("right") || right && facing.Equals("left"))
            {
                if (isCrouching)
                {
                    canBlockHigh = false;
                    canBlockLow = true;
                }
                else
                {
                    canBlockLow = false;
                    canBlockHigh = true;
                }
                canBlockMid = true;
            }
            else
            {
                canBlockMid = false;
                canBlockLow = false;
                canBlockHigh = false;
            }

            if (!isAirborne && dashTime <= 0)
            {
                if (Input.GetKey(KeyCode.DownArrow))
                {
                    //Include stuff for inputs  here at some point
                    isCrouching = true;
                    crouchAdjusted = false;
                    if (canCrawl)
                    {
                        if (left)
                            Walk(-crawlSpeed);
                        if (right)
                            Walk(crawlSpeed);
                    }
                }
                else if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    if (left)
                    {
                        GetComponent<Rigidbody2D>().AddForce(new Vector2(-moveSpeed * jumpSpeed * 150, 250 * jumpSpeed));
                    }
                    else if (right)
                    {
                        GetComponent<Rigidbody2D>().AddForce(new Vector2(moveSpeed * jumpSpeed * 150, 250 * jumpSpeed));
                    }
                    else
                    {
                        GetComponent<Rigidbody2D>().AddForce(new Vector2(0, jumpSpeed * 250));
                    }
                }

                else if (left)
                {
                    Walk(-moveSpeed);
                }

                else if (right)
                {
                    Walk(moveSpeed);
                }
            }

            if (dashTime > 0)
            {
                if (facing.Equals("left") && dashdir.Equals("front") || facing.Equals("right") && dashdir.Equals("back"))
                {
                    if (Input.GetKeyDown(KeyCode.UpArrow))
                    {
                        GetComponent<Rigidbody2D>().AddForce(new Vector2(-moveSpeed * jumpSpeed * 100 * (dashSpeed / moveSpeed), 250 * jumpSpeed));
                        dashTime = 0;
                    }
                    else
                        Walk(-dashSpeed);
                }

                else
                {
                    if (Input.GetKeyDown(KeyCode.UpArrow))
                    {
                        GetComponent<Rigidbody2D>().AddForce(new Vector2(moveSpeed * jumpSpeed * 100 * (dashSpeed / moveSpeed), 250 * jumpSpeed));
                        dashTime = 0;
                    }
                    else
                        Walk(dashSpeed);
                }

                dashTime--;
            }

            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                if (facing == "left")
                    keyword = "front";
                else
                    keyword = "back";
                if (BackDash[0].Equals(keyword))
                {
                    BackDash.RemoveAt(0);
                    if (BackDash.Count == 0)
                    {
                        dashTime = startDashTime;
                        dashdir = "back";
                        inputRefreshTime = 0;
                        //facing == 
                        BackDash = new List<string>(BackDashInput);
                    }
                    else
                    {
                        inputRefreshTime = startInputRefreshTime;
                    }
                }
                else
                {
                    if (!BackDash.Equals(BackDashInput))
                    {
                        BackDash = new List<string>(BackDashInput);
                    }
                }

                if (FrontDash[0].Equals(keyword))
                {
                    FrontDash.RemoveAt(0);
                    if (FrontDash.Count == 0)
                    {
                        dashTime = startDashTime;
                        dashdir = "front";
                        inputRefreshTime = 0;
                        FrontDash = new List<string>(FrontDashInput);
                    }
                    else
                        inputRefreshTime = startInputRefreshTime;
                }
                else
                {
                    if (!FrontDash.Equals(FrontDashInput))
                    {
                        FrontDash = new List<string>(FrontDashInput);
                    }
                }
            }

            if (Input.GetKeyDown(KeyCode.RightArrow))
            {

                //setting the keyword
                if (facing == "left")
                    keyword = "back";
                else
                    keyword = "front";
                if (BackDash[0].Equals(keyword))
                {
                    BackDash.RemoveAt(0);
                    if (BackDash.Count == 0)
                    {
                        dashTime = startDashTime;
                        inputRefreshTime = 0;
                        dashdir = "back";
                        BackDash = new List<string>(BackDashInput);
                    }
                    else
                        inputRefreshTime = startInputRefreshTime;
                }
                else
                {
                    if (!BackDash.Equals(BackDashInput))
                    {
                        BackDash = new List<string>(BackDashInput);
                    }
                }
                if (FrontDash[0].Equals(keyword))
                {
                    FrontDash.RemoveAt(0);
                    if (FrontDash.Count == 0)
                    {
                        dashTime = startDashTime;
                        inputRefreshTime = 0;
                        dashdir = "front";
                        FrontDash = new List<string>(FrontDashInput);
                    }
                    else
                        inputRefreshTime = startInputRefreshTime;
                }
                else
                {
                    if (!FrontDash.Equals(FrontDashInput))
                    {
                        FrontDash = new List<string>(FrontDashInput);
                    }
                }
            }

            if (isAirborne)
            {
                if (isCrouching)
                {
                    if (Input.GetKeyDown(KeyCode.O))
                        AttackChecker(false, false, true, true, false);
                    if (Input.GetKeyDown(KeyCode.P))
                        AttackChecker(false, true, true, true, false);
                    if (Input.GetKeyDown(KeyCode.L))
                        AttackChecker(true, false, true, true, false);
                    if (Input.GetKeyDown(KeyCode.Semicolon))
                        AttackChecker(true, true, true, true, false);
                }
                else
                {
                    if (Input.GetKeyDown(KeyCode.O))
                        AttackChecker(false, false, false, true, false);
                    if (Input.GetKeyDown(KeyCode.P))
                        AttackChecker(false, true, false, true, false);
                    if (Input.GetKeyDown(KeyCode.L))
                        AttackChecker(true, false, false, true, false);
                    if (Input.GetKeyDown(KeyCode.Semicolon))
                        AttackChecker(true, true, false, true, false);
                }
            }
            else
            {
                if ((left && facing.Equals("left") || right && facing.Equals("right")) && Input.GetKey(KeyCode.S))
                {
                    if (Input.GetKeyDown(KeyCode.O))
                        AttackChecker(false, false, true, false, true);
                    if (Input.GetKeyDown(KeyCode.P))
                        AttackChecker(false, true, true, false, true);
                    if (Input.GetKeyDown(KeyCode.L))
                        AttackChecker(true, false, true, false, true);
                    if (Input.GetKeyDown(KeyCode.Semicolon))
                        AttackChecker(true, true, true, false, true);
                }
                else if (left && facing.Equals("left") || right && facing.Equals("right"))
                {
                    if (Input.GetKeyDown(KeyCode.O))
                        AttackChecker(false, false, false, false, true);
                    if (Input.GetKeyDown(KeyCode.P))
                        AttackChecker(false, true, false, false, true);
                    if (Input.GetKeyDown(KeyCode.L))
                        AttackChecker(true, false, false, false, true);
                    if (Input.GetKeyDown(KeyCode.Semicolon))
                        AttackChecker(true, true, false, false, true);
                }
                else if (isCrouching)
                {
                    if (Input.GetKeyDown(KeyCode.O))
                        AttackChecker(false, false, true, false, true);
                    if (Input.GetKeyDown(KeyCode.P))
                        AttackChecker(false, true, true, false, true);
                    if (Input.GetKeyDown(KeyCode.L))
                        AttackChecker(true, false, true, false, true);
                    if (Input.GetKeyDown(KeyCode.Semicolon))
                        AttackChecker(true, true, true, false, true);
                }
                else
                {
                    if (Input.GetKeyDown(KeyCode.O))
                        AttackChecker(false, false, false, false, false);
                    if (Input.GetKeyDown(KeyCode.P))
                        AttackChecker(false, true, false, false, false);
                    if (Input.GetKeyDown(KeyCode.L))
                        AttackChecker(true, false, false, false, false);
                    if (Input.GetKeyDown(KeyCode.Semicolon))
                        AttackChecker(true, true, false, false, false);
                }
            }
            //TODO: set canblocks here


        }
        //check for actions that can be taken in either neutral or mid-attack(jumping, attacking, inputs for specials, supers)
        else if (hitstun == 0 && canCancel)
        {

            if (Input.GetKeyDown(KeyCode.UpArrow) && !isAirborne && dashTime <= 0)
            {
                if (left)
                {
                    GetComponent<Rigidbody2D>().AddForce(new Vector2(-moveSpeed * jumpSpeed * 150, 250 * jumpSpeed));
                }
                else if (right)
                {
                    GetComponent<Rigidbody2D>().AddForce(new Vector2(moveSpeed * jumpSpeed * 150, 250 * jumpSpeed));
                }
                else
                {
                    GetComponent<Rigidbody2D>().AddForce(new Vector2(0, jumpSpeed * 250));
                }
            }

            if (dashTime == -1)
            {
                dashdir = "none";
            }
            if (dashTime > 0)
            {
                if (facing.Equals("left") && dashdir.Equals("front") || facing.Equals("right") && dashdir.Equals("back"))
                {
                    if (Input.GetKeyDown(KeyCode.UpArrow))
                    {
                        GetComponent<Rigidbody2D>().AddForce(new Vector2(-moveSpeed * jumpSpeed * 100 * (dashSpeed / moveSpeed), 250 * jumpSpeed));
                        dashTime = 0;
                    }
                    else
                        Walk(-dashSpeed);
                }
                else
                {
                    if (Input.GetKeyDown(KeyCode.UpArrow))
                    {
                        GetComponent<Rigidbody2D>().AddForce(new Vector2(moveSpeed * jumpSpeed * 100 * (dashSpeed / moveSpeed), 250 * jumpSpeed));
                        dashTime = 0;
                    }
                    else
                        Walk(dashSpeed);
                }
                dashTime--;
            }


            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                if (facing == "left")
                    keyword = "front";
                else
                    keyword = "back";
                if (BackDash[0].Equals(keyword))
                {
                    BackDash.RemoveAt(0);
                    if (BackDash.Count == 0)
                    {
                        dashTime = startDashTime;
                        dashdir = "back";
                        inputRefreshTime = 0;
                        //facing == 
                        BackDash = new List<string>(BackDashInput);
                    }
                    else
                    {
                        inputRefreshTime = startInputRefreshTime;
                    }
                }
                else
                {
                    if (!BackDash.Equals(BackDashInput))
                    {
                        BackDash = new List<string>(BackDashInput);
                    }
                }

                if (FrontDash[0].Equals(keyword))
                {
                    FrontDash.RemoveAt(0);
                    if (FrontDash.Count == 0)
                    {
                        dashTime = startDashTime;
                        dashdir = "front";
                        inputRefreshTime = 0;
                        FrontDash = new List<string>(FrontDashInput);
                    }
                    else
                        inputRefreshTime = startInputRefreshTime;
                }
                else
                {
                    if (!FrontDash.Equals(FrontDashInput))
                    {
                        FrontDash = new List<string>(FrontDashInput);
                    }
                }
            }

            if (Input.GetKeyDown(KeyCode.RightArrow))
            {

                //setting the keyword
                if (facing == "left")
                    keyword = "back";
                else
                    keyword = "front";
                if (BackDash[0].Equals(keyword))
                {
                    BackDash.RemoveAt(0);
                    if (BackDash.Count == 0)
                    {
                        dashTime = startDashTime;
                        inputRefreshTime = 0;
                        dashdir = "back";
                        BackDash = new List<string>(BackDashInput);
                    }
                    else
                        inputRefreshTime = startInputRefreshTime;
                }
                else
                {
                    if (!BackDash.Equals(BackDashInput))
                    {
                        BackDash = new List<string>(BackDashInput);
                    }
                }
                if (FrontDash[0].Equals(keyword))
                {
                    FrontDash.RemoveAt(0);
                    if (FrontDash.Count == 0)
                    {
                        dashTime = startDashTime;
                        inputRefreshTime = 0;
                        dashdir = "front";
                        FrontDash = new List<string>(FrontDashInput);
                    }
                    else
                        inputRefreshTime = startInputRefreshTime;
                }
                else
                {
                    if (!FrontDash.Equals(FrontDashInput))
                    {
                        FrontDash = new List<string>(FrontDashInput);
                    }
                }
            }


            if (isAirborne)
            {
                if (isCrouching)
                {
                    if (Input.GetKeyDown(KeyCode.O))
                        AttackChecker(false, false, true, true, false);
                    if (Input.GetKeyDown(KeyCode.P))
                        AttackChecker(false, true, true, true, false);
                    if (Input.GetKeyDown(KeyCode.L))
                        AttackChecker(true, false, true, true, false);
                    if (Input.GetKeyDown(KeyCode.Semicolon))
                        AttackChecker(true, true, true, true, false);
                }
                else
                {
                    if (Input.GetKeyDown(KeyCode.O))
                        AttackChecker(false, false, false, true, false);
                    if (Input.GetKeyDown(KeyCode.P))
                        AttackChecker(false, true, false, true, false);
                    if (Input.GetKeyDown(KeyCode.L))
                        AttackChecker(true, false, false, true, false);
                    if (Input.GetKeyDown(KeyCode.Semicolon))
                        AttackChecker(true, true, false, true, false);
                }
            }
            else
            {
                if ((left && facing.Equals("left") || right && facing.Equals("right")) && isCrouching)
                {
                    if (Input.GetKeyDown(KeyCode.O))
                        AttackChecker(false, false, true, false, true);
                    if (Input.GetKeyDown(KeyCode.P))
                        AttackChecker(false, true, true, false, true);
                    if (Input.GetKeyDown(KeyCode.L))
                        AttackChecker(true, false, true, false, true);
                    if (Input.GetKeyDown(KeyCode.Semicolon))
                        AttackChecker(true, true, true, false, true);
                }
                else if (left && facing.Equals("left") || right && facing.Equals("right"))
                {
                    if (Input.GetKeyDown(KeyCode.O))
                        AttackChecker(false, false, true, false, false);
                    if (Input.GetKeyDown(KeyCode.P))
                        AttackChecker(false, true, true, false, false);
                    if (Input.GetKeyDown(KeyCode.L))
                        AttackChecker(true, false, true, false, false);
                    if (Input.GetKeyDown(KeyCode.Semicolon))
                        AttackChecker(true, true, true, false, false);
                }
                else if (isCrouching)
                {
                    if (Input.GetKeyDown(KeyCode.O))
                        AttackChecker(false, false, false, false, true);
                    if (Input.GetKeyDown(KeyCode.P))
                        AttackChecker(false, true, false, false, true);
                    if (Input.GetKeyDown(KeyCode.L))
                        AttackChecker(true, false, false, false, true);
                    if (Input.GetKeyDown(KeyCode.Semicolon))
                        AttackChecker(true, true, false, false, true);
                }
                else
                {
                    if (Input.GetKeyDown(KeyCode.O))
                        AttackChecker(false, false, false, false, false);
                    if (Input.GetKeyDown(KeyCode.P))
                        AttackChecker(false, true, false, false, false);
                    if (Input.GetKeyDown(KeyCode.L))
                        AttackChecker(true, false, false, false, false);
                    if (Input.GetKeyDown(KeyCode.Semicolon))
                        AttackChecker(true, true, false, false, false);
                }
            }
        }
        //check for actions that can only be taken during hitstun(breaking out of hitstun)
        else if (hitstun != 0)
        {
            if (blockStunning)
                spriteRenderer.sprite = block;
            else
                spriteRenderer.sprite = stun;
        }
        //decrease hitstun/endlag.
        if (hitstun > 0)
            hitstun--;
        if (hitstun < 0)
            hitstun = 0;
        if (endlag > 0)
            endlag--;
        if (endlag < 0)
            endlag = 0;
        if (inputRefreshTime == 1)
        {
            FrontDash = new List<string>(FrontDashInput);
            BackDash = new List<string>(BackDashInput);
            QCF = new List<string>(QCFInput);
            QCB = new List<string>(QCBInput);
            DQCF = new List<string>(DQCFInput);
            DQCB = new List<string>(DQCBInput);
            DD = new List<string>(DDInput);
        }
        if (inputRefreshTime > 0)
            inputRefreshTime--;
        keyword = "none";

    }
    void Walk(float speed)
    {
        gameObject.transform.Translate(speed, 0, 0);
    }
    //Attack types info:
    //First is always attack height. High means only stand block works, low means only crouch block works, mid means anything works.
    //Next is if the attack has light, medium, or hard knockback
    //Arm, leg, and weapon are there for cancels.
    //Do the rest later.

    //figure out what the attack is. going to be massive as more characters are added. Should probably simplify but I like self destruction
    void AttackChecker(bool isKick, bool isHeavy, bool isDirectional, bool isAirborne, bool isCrouching)
    {
        if (isAirborne)
        {
            if (isHeavy)
            {
                if (isDirectional)
                {
                    if (isKick)
                    {

                    }
                    else
                    {

                    }
                }
                else
                {
                    if (isKick)
                    {

                    }
                    else
                    {

                    }
                }
            }
            else
            {
                if (isDirectional)
                {
                    if (isKick)
                    {

                    }
                    else
                    {

                    }
                }
                else
                {
                    if (isKick)
                    {

                    }
                    else
                    {

                    }
                }
            }
        }
        else
        {
            if (isHeavy)
            {
                if (isDirectional)
                {
                    if (isKick)
                    {
                        if (isCrouching)
                        {

                        }
                        else
                        {

                        }
                    }
                    else
                    {
                        if (isCrouching)
                        {

                        }
                        else
                        {

                        }
                    }
                }
                else
                {
                    if (isKick)
                    {
                        if (isCrouching)
                        {

                        }
                        else
                        {

                        }
                    }
                    else
                    {
                        if (isCrouching)
                        {

                        }
                        else
                        {
                            StartCoroutine(Attack(11, 4, 30, 1.25f, .5f, 0, 8, 17, new List<string> { "mid", "punch", "med" }, 0, new Vector2(0, 0)));
                        }
                    }
                }
            }
            else
            {
                if (isDirectional)
                {
                    if (isKick)
                    {
                        if (isCrouching)
                        {

                        }
                        else
                        {

                        }
                    }
                    else
                    {
                        if (isCrouching)
                        {
                            StartCoroutine(Attack(5, 3, 19, .75f, .5f, -2, 3, 12, new List<string> { "low", "punch", "light" }, 0, new Vector2(0, 0)));
                        }
                        else
                        {
                            StartCoroutine(Attack(17, 5, 25, 1.25f, .75f, 0, 6, 12, new List<string> { "mid", "punch", "light" }, 0, new Vector2(2, 0)));
                        }
                    }
                }
                else
                {
                    if (isKick)
                    {
                        if (isCrouching)
                        {

                        }
                        else
                        {

                        }
                    }
                    else
                    {
                        if (isCrouching)
                        {
                            StartCoroutine(Attack(6, 2, 19, 1, .5f, -2, 8, 17, new List<string> { "low", "punch", "light" }, 0, new Vector2(0, 0)));
                        }
                        else
                        {
                            StartCoroutine(Attack(5, 2, 18, 1, .5f, 0, 4, 12, new List<string> { "mid", "punch", "light" }, 0, new Vector2(0, 0)));
                        }
                    }
                }
            }
        }

    }
    //attack using info. startup is time until attacking, hitend is when attack ends, ending is when move ends. Ties into endlag.
    //lens are range, damage is obvious, stun is amount of hitstun applied, attackTypes are also important
    IEnumerator Attack(float startup, float hittime, float ending, float xlen, float ylen, float yadjust, int damage, float stun, List<string> attackTypes, float angle, Vector2 playerMovement)
    {
        int facingAdjust = facing.Equals("right") ? 1 : -1;
        Vector3 movement = new Vector3(playerMovement.x / hittime * facingAdjust, playerMovement.y / hittime, 0);
        bool startedAir = isAirborne;
        spriteRenderer.sprite = attacking;
        bool hasHit = false;
        canCancel = false;
        endlag = ending;
        dashTime = -1;
        GameObject visual = Instantiate(visualizer, new Vector3(playerMovement.x + transform.position.x + xlen * 4 * facingAdjust, playerMovement.y + transform.position.y + yadjust, 0), transform.rotation);
        visual.transform.localScale = new Vector3(xlen, ylen, 0);
        visual.SetActive(false);
        while (endlag > 0)
        {
            if (startedAir != isAirborne)
                endlag = 0;
            if (endlag <= ending - startup && endlag > ending - startup - hittime && !hasHit)
            {

                transform.position += movement;
                Collider2D enemyHit = Physics2D.OverlapBox(new Vector2(transform.position.x + xlen * 4 * facingAdjust, transform.position.y + yadjust), new Vector2(xlen * 4, ylen * 4), 0, enemies);
                visual.SetActive(true);
                if (enemyHit != null)
                {

                    canCancel = true;
                    enemyHit.gameObject.GetComponent<Player1>().TakeDamage(damage, stun, angle, attackTypes);

                    hasHit = true;
                }
            }
            else
                visual.SetActive(false);
            yield return null;
        }
        Destroy(visual);
        canCancel = false;
        spriteRenderer.sprite = neutral;

    }
    //the damage is taken. applies damage, stun, and attack Types. Ties into hitstun.
    public void TakeDamage(int damage, float stun, float angle, List<string> attackTypes)
    {
        if (attackTypes[0].Equals("mid") && canBlockMid || attackTypes[0].Equals("low") && canBlockLow || attackTypes[0].Equals("high") && canBlockHigh)
        {
            Block(damage, stun, attackTypes);
        }
        else
        {
            blockStunning = false;
            endlag = 0.0f;
            hitstun = stun;
            health -= damage;

            if (angle != 0)
            {

            }

            else if (isAirborne)
            {

            }

            else
            {
                if (attackTypes[attackTypes.Count - 1].Equals("light"))
                {
                    if (facing.Equals("left"))
                    {
                        transform.position = transform.position + new Vector3(1, 0, 0);
                    }
                    else
                    {
                        transform.position = transform.position - new Vector3(1, 0, 0);
                    }
                }
                else if (attackTypes[attackTypes.Count - 1].Equals("medium"))
                {
                    if (facing.Equals("left"))
                    {
                        transform.position = transform.position + new Vector3(1, 0, 0);
                    }
                    else
                    {
                        transform.position = transform.position - new Vector3(1, 0, 0);
                    }
                }
                else
                {
                    if (facing.Equals("left"))
                    {
                        transform.position = transform.position + new Vector3(1, 0, 0);
                    }
                    else
                    {
                        transform.position = transform.position - new Vector3(1, 0, 0);
                    }
                }
            }
        }
    }
    //the attack is blocked, applies reduced damage, stun, and attack types. 
    void Block(int damage, float stun, List<string> attackTypes)
    {
        blockStunning = true;
        endlag = 0;
        hitstun = stun - 3;
        health -= (damage / 4) <= 0 ? 1 : damage / 4;

        if (attackTypes[attackTypes.Count - 1].Equals("light"))
        {
            if (facing.Equals("left"))
            {
                transform.position = transform.position + new Vector3(1.5f, 0, 0);
            }
            else
            {
                transform.position = transform.position - new Vector3(1.5f, 0, 0);
            }
        }
        else if (attackTypes[attackTypes.Count - 1].Equals("medium"))
        {
            if (facing.Equals("left"))
            {
                transform.position = transform.position + new Vector3(3, 0, 0);
            }
            else
            {
                transform.position = transform.position - new Vector3(3, 0, 0);
            }
        }
        else
        {
            if (facing.Equals("left"))
            {
                transform.position = transform.position + new Vector3(4.5f, 0, 0);
            }
            else
            {
                transform.position = transform.position - new Vector3(4.5f, 0, 0);
            }
        }

    }

    void OnCollisionEnter2D(Collision2D col)
    {
        float yvel = rb.velocity.y;
        if (col.gameObject.Equals(opponent) && transform.position.y > col.gameObject.transform.position.y + col.gameObject.GetComponent<BoxCollider2D>().size.y && yvel < 0)
        {
            if (facing.Equals("left"))
                transform.position = new Vector3(col.gameObject.transform.position.x + 5, transform.position.y, transform.position.z);
            else
                transform.position = new Vector3(col.gameObject.transform.position.x - 5, transform.position.y, transform.position.z);
            rb.velocity = new Vector2(rb.velocity.x, yvel);
        }
    }

}

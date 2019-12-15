﻿using System.Collections;
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

    //TODO: Make an indicator of who is who

    //use the name for moves!
    public string playerName;
    public int health;
    public float moveSpeed;
    public float dashSpeed;
    public float jumpSpeed;
    public float jumpHeight;
    public string facing;
    public bool hit;
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
    public List<List<bool>> attacksTaken;
    public List<string> movementsTaken;
    private float dashTime;
    private float inputRefreshTime;
    private bool canCancel;
    private bool isCrouching;
    private bool canBlockHigh;
    private bool canBlockMid;
    private bool canBlockLow;
    private bool isAirborne;
    private bool combocrouching;
    private string mostRecentAttackType;
    private bool blockStunning;
    private BoxCollider2D boxcol;
    private Animator anim;
    private SpriteRenderer spriteRenderer;
    private Sprite[] spriteList;
    private Sprite neutral;
    private Sprite attacking;
    private Sprite stun;
    private Sprite block;
    private GameObject opponent;
    private Rigidbody2D rb;
    private float oldsize;
    private float oldheight;
    private float crouchsize;
    private float crouchheight;
    private LayerMask enemies;
    public GameObject visualizer;
    private bool left;
    private bool right;

    // Use this for initialization
    void Start()
    {
        //set hitstun and endlag. hitstun is for damage lag, endlag is for move lag. Eventually split endlag into endlag, dashlag, and blocklag.
        hitstun = 0;
        endlag = 0;
        hit = false;
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
        attacksTaken = new List<List<bool>> { };
        movementsTaken = new List<string> { };
        //set the committment to dashing. maybe merge into endlag?
        dashTime = -1;
        //used to check if a move can cancel into a move/jump. true when the attack hits. false otherwise. Must be combined with checking the move's type. 
        canCancel = false;
        //used to check if the character is crouching. Height is lowered & can't move unless isCrawl is true, then they move at CrawlSpeed.
        isCrouching = false;
        //used to check if the character can currently block certain types of attacks
        canBlockHigh = false;
        canBlockMid = false;
        canBlockLow = false;
        //used to check if the character is in the air. 
        isAirborne = false;
        //used to check if the character can change crouching state while comboing
        combocrouching = false;
        //what the most recent attack was. use with above.
        mostRecentAttackType = "none";
        //aaaaaaaaaaa
        boxcol = GetComponent<BoxCollider2D>();
        //the old and new heights for crouching
        oldsize = boxcol.size.y;
        oldheight = transform.localScale.y;
        crouchsize = oldsize * crouchScale;
        crouchheight = oldheight * crouchScale;
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        opponent = GameObject.FindGameObjectWithTag("Player1");
        enemies = LayerMask.GetMask("Player1");
        spriteList = new Sprite[] {
        Resources.Load<Sprite>("Sprites/NeutralTest"),
        Resources.Load<Sprite>("Sprites/NeutralTest2"),
        Resources.Load<Sprite>("Sprites/AttackTest"),
        Resources.Load<Sprite>("Sprites/AttackTest2"),
        Resources.Load<Sprite>("Sprites/HitTest"),
        Resources.Load<Sprite>("Sprites/HitTest2"),
        Resources.Load<Sprite>("Sprites/BlockTest"),
        Resources.Load<Sprite>("Sprites/BlockTest2")};
        neutral = spriteList[1];
        attacking = spriteList[3];
        stun = spriteList[5];
        block = spriteList[7];
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        foreach (List<bool> l in attacksTaken)
        {
            foreach (bool b in l)
            {
                Debug.Log(b + ", ");
            }
        }
        hit = false;
        bool right = Input.GetKey(KeyCode.RightArrow);
        bool left = Input.GetKey(KeyCode.LeftArrow);
        //set important stuff first
        if (opponent.GetComponent<Player1>().hitstun == 0 && endlag == 0)
        {
            attacksTaken = new List<List<bool>> { };
            movementsTaken = new List<string> { };
        }
        if (!Input.GetKey(KeyCode.DownArrow) && endlag == 0 && hitstun == 0)
        {
            isCrouching = false;
        }
        if (health <= 0)
            Destroy(gameObject);
        if (opponent.transform.position.x < transform.position.x)
        {
            facing = "left";
            neutral = spriteList[1];
            attacking = spriteList[3];
            stun = spriteList[5];
            block = spriteList[7];
        }
        else
        {
            facing = "right";
            neutral = spriteList[0];
            attacking = spriteList[2];
            stun = spriteList[4];
            block = spriteList[6];
        }
        if (isCrouching)
        {
            boxcol.offset = new Vector2(0, (crouchsize - oldsize) / 2);
            boxcol.size = new Vector2(boxcol.size.x, crouchsize);
            transform.localScale = new Vector3(transform.localScale.x, crouchheight, transform.localScale.z);
            transform.position = new Vector3(transform.position.x, (crouchheight - oldheight) / 2, transform.position.z);
        }
        else if (!isCrouching && transform.position.y <= 0)
        {
            boxcol.offset = new Vector2(0, 0);
            boxcol.size = new Vector2(boxcol.size.x, oldsize);
            transform.localScale = new Vector3(transform.localScale.x, oldheight, transform.localScale.z);
            transform.position = new Vector3(transform.position.x, 0, transform.position.z);
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
                        GetComponent<Rigidbody2D>().AddForce(new Vector2(-moveSpeed * jumpSpeed * 100, jumpHeight * jumpSpeed));
                    }
                    else if (right)
                    {
                        GetComponent<Rigidbody2D>().AddForce(new Vector2(moveSpeed * jumpSpeed * 100, jumpHeight * jumpSpeed));
                    }
                    else
                    {
                        GetComponent<Rigidbody2D>().AddForce(new Vector2(0, jumpSpeed * jumpHeight));
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
                        GetComponent<Rigidbody2D>().AddForce(new Vector2(-moveSpeed * jumpSpeed * 100 * (dashSpeed / moveSpeed), jumpHeight * jumpSpeed));
                        dashTime = 0;
                    }
                    else
                        Walk(-dashSpeed);
                }

                else
                {
                    if (Input.GetKeyDown(KeyCode.UpArrow))
                    {
                        GetComponent<Rigidbody2D>().AddForce(new Vector2(moveSpeed * jumpSpeed * 100 * (dashSpeed / moveSpeed), jumpHeight * jumpSpeed));
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
                if (Input.GetKey(KeyCode.DownArrow))
                {
                    if (Input.GetKeyDown(KeyCode.O))
                    {
                        AttackChecker(false, false, true, true, false);
                        attacksTaken.Add(new List<bool> { false, false, true, true, false });
                    }
                    if (Input.GetKeyDown(KeyCode.P))
                    {
                        AttackChecker(false, true, true, true, false);
                        attacksTaken.Add(new List<bool> { false, true, true, true, false });
                    }
                    if (Input.GetKeyDown(KeyCode.L))
                    {
                        AttackChecker(true, false, true, true, false);
                        attacksTaken.Add(new List<bool> { true, false, true, true, false });
                    }
                    if (Input.GetKeyDown(KeyCode.Semicolon))
                    {
                        AttackChecker(true, true, true, true, false);
                        attacksTaken.Add(new List<bool> { true, true, true, true, false });
                    }
                }
                else
                {
                    if (Input.GetKeyDown(KeyCode.O))
                    {
                        AttackChecker(false, false, false, true, false);
                        attacksTaken.Add(new List<bool> { false, false, false, true, false });
                    }
                    if (Input.GetKeyDown(KeyCode.P))
                    {
                        AttackChecker(false, true, false, true, false);
                        attacksTaken.Add(new List<bool> { false, true, false, true, false });
                    }
                    if (Input.GetKeyDown(KeyCode.L))
                    {
                        AttackChecker(true, false, false, true, false);
                        attacksTaken.Add(new List<bool> { true, false, false, true, false });
                    }
                    if (Input.GetKeyDown(KeyCode.Semicolon))
                    {
                        AttackChecker(true, true, false, true, false);
                        attacksTaken.Add(new List<bool> { true, true, false, true, false });
                    }
                }
            }
            else
            {
                if ((left && facing.Equals("left") || right && facing.Equals("right")) && isCrouching)
                {
                    if (Input.GetKeyDown(KeyCode.O))
                    {
                        AttackChecker(false, false, true, false, true);
                        attacksTaken.Add(new List<bool> { false, false, true, false, true });
                    }
                    if (Input.GetKeyDown(KeyCode.P))
                    {
                        AttackChecker(false, true, true, false, true);
                        attacksTaken.Add(new List<bool> { false, true, true, false, true });
                    }
                    if (Input.GetKeyDown(KeyCode.L))
                    {
                        AttackChecker(true, false, true, false, true);
                        attacksTaken.Add(new List<bool> { true, false, true, false, true });
                    }
                    if (Input.GetKeyDown(KeyCode.Semicolon))
                    {
                        AttackChecker(true, true, true, false, true);
                        attacksTaken.Add(new List<bool> { true, true, true, false, true });
                    }
                }
                else if (left && facing.Equals("left") || right && facing.Equals("right"))
                {
                    if (Input.GetKeyDown(KeyCode.O))
                    {
                        AttackChecker(false, false, true, false, false);
                        attacksTaken.Add(new List<bool> { false, false, true, false, false });
                    }
                    if (Input.GetKeyDown(KeyCode.P))
                    {
                        AttackChecker(false, true, true, false, false);
                        attacksTaken.Add(new List<bool> { false, true, true, false, false });
                    }
                    if (Input.GetKeyDown(KeyCode.L))
                    {
                        AttackChecker(true, false, true, false, false);
                        attacksTaken.Add(new List<bool> { true, false, true, false, false });
                    }
                    if (Input.GetKeyDown(KeyCode.Semicolon))
                    {
                        AttackChecker(true, true, true, false, false);
                        attacksTaken.Add(new List<bool> { true, true, true, false, false });
                    }
                }
                else if (isCrouching)
                {
                    if (Input.GetKeyDown(KeyCode.O))
                    {
                        AttackChecker(false, false, false, false, true);
                        attacksTaken.Add(new List<bool> { false, false, false, false, true });
                    }
                    if (Input.GetKeyDown(KeyCode.P))
                    {
                        AttackChecker(false, true, false, false, true);
                        attacksTaken.Add(new List<bool> { false, true, false, false, true });
                    }
                    if (Input.GetKeyDown(KeyCode.L))
                    {
                        AttackChecker(true, false, false, false, true);
                        attacksTaken.Add(new List<bool> { true, false, false, false, true });
                    }
                    if (Input.GetKeyDown(KeyCode.Semicolon))
                    {
                        AttackChecker(true, true, false, false, true);
                        attacksTaken.Add(new List<bool> { true, true, false, false, true });
                    }
                }
                else
                {
                    if (Input.GetKeyDown(KeyCode.O))
                    {
                        AttackChecker(false, false, false, false, false);
                        attacksTaken.Add(new List<bool> { false, false, false, false, false });
                    }
                    if (Input.GetKeyDown(KeyCode.P))
                    {
                        AttackChecker(false, true, false, false, false);
                        attacksTaken.Add(new List<bool> { false, true, false, false, false });
                    }
                    if (Input.GetKeyDown(KeyCode.L))
                    {
                        AttackChecker(true, false, false, false, false);
                        attacksTaken.Add(new List<bool> { true, false, false, false, false });
                    }
                    if (Input.GetKeyDown(KeyCode.Semicolon))
                    {
                        AttackChecker(true, true, false, false, false);
                        attacksTaken.Add(new List<bool> { true, true, false, false, false });
                    }
                }
            }
            //TODO: set canblocks here


        }
        //check for actions that can be taken in either neutral or mid-attack(jumping, attacking, inputs for specials, supers)
        else if (hitstun == 0 && canCancel)
        {
            if (Input.GetKey(KeyCode.DownArrow) && combocrouching)
            {
                combocrouching = false;
                isCrouching = true;
            }
            if (!Input.GetKey(KeyCode.DownArrow) && combocrouching)
            {
                combocrouching = false;
                isCrouching = false;
            }

            if (Input.GetKeyDown(KeyCode.UpArrow) && !isAirborne && dashTime <= 0 && !movementsTaken.Contains("jump"))
            {
                if (left)
                {
                    GetComponent<Rigidbody2D>().AddForce(new Vector2(-moveSpeed * jumpSpeed * 100, jumpHeight * jumpSpeed));
                    movementsTaken.Add("jump");
                }
                else if (right)
                {
                    GetComponent<Rigidbody2D>().AddForce(new Vector2(moveSpeed * jumpSpeed * 100, jumpHeight * jumpSpeed));
                    movementsTaken.Add("jump");
                }
                else
                {
                    GetComponent<Rigidbody2D>().AddForce(new Vector2(0, jumpSpeed * jumpHeight));
                    movementsTaken.Add("jump");
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
                    if (Input.GetKeyDown(KeyCode.UpArrow) && !movementsTaken.Contains("jump"))
                    {
                        GetComponent<Rigidbody2D>().AddForce(new Vector2(-moveSpeed * jumpSpeed * 100 * (dashSpeed / moveSpeed), jumpHeight * jumpSpeed));
                        dashTime = 0;
                        movementsTaken.Add("jump");
                    }
                    else
                        Walk(-dashSpeed);
                }
                else
                {
                    if (Input.GetKeyDown(KeyCode.UpArrow) && !movementsTaken.Contains("jump"))
                    {
                        GetComponent<Rigidbody2D>().AddForce(new Vector2(moveSpeed * jumpSpeed * 100 * (dashSpeed / moveSpeed), jumpHeight * jumpSpeed));
                        dashTime = 0;
                        movementsTaken.Add("jump");
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
                    if (BackDash.Count == 0 && !movementsTaken.Contains("dash"))
                    {
                        dashTime = startDashTime;
                        dashdir = "back";
                        movementsTaken.Add("dash");
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
                    if (FrontDash.Count == 0 && !movementsTaken.Contains("dash"))
                    {
                        dashTime = startDashTime;
                        dashdir = "front";
                        movementsTaken.Add("dash");
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
                    if (BackDash.Count == 0 && !movementsTaken.Contains("dash"))
                    {
                        dashTime = startDashTime;
                        inputRefreshTime = 0;
                        movementsTaken.Add("dash");
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
                    if (FrontDash.Count == 0 && !movementsTaken.Contains("dash"))
                    {
                        dashTime = startDashTime;
                        inputRefreshTime = 0;
                        movementsTaken.Add("dash");
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
                if (Input.GetKey(KeyCode.DownArrow))
                {
                    if (Input.GetKeyDown(KeyCode.O) && !attacksTakenComparison(attacksTaken, false, false, true, true, false))
                    {
                        AttackChecker(false, false, true, true, false);
                        attacksTaken.Add(new List<bool> { false, false, true, true, false });
                    }
                    if (Input.GetKeyDown(KeyCode.P) && !attacksTakenComparison(attacksTaken, false, true, true, true, false))
                    {
                        AttackChecker(false, true, true, true, false);
                        attacksTaken.Add(new List<bool> { false, true, true, true, false });
                    }
                    if (Input.GetKeyDown(KeyCode.L) && !attacksTakenComparison(attacksTaken, true, false, true, true, false))
                    {
                        AttackChecker(true, false, true, true, false);
                        attacksTaken.Add(new List<bool> { true, false, true, true, false });
                    }
                    if (Input.GetKeyDown(KeyCode.Semicolon) && !attacksTakenComparison(attacksTaken, true, true, true, true, false))
                    {
                        AttackChecker(true, true, true, true, false);
                        attacksTaken.Add(new List<bool> { true, true, true, true, false });
                    }
                }
                else
                {
                    if (Input.GetKeyDown(KeyCode.O) && !attacksTakenComparison(attacksTaken, false, false, false, true, false))
                    {
                        AttackChecker(false, false, false, true, false);
                        attacksTaken.Add(new List<bool> { false, false, false, true, false });
                    }
                    if (Input.GetKeyDown(KeyCode.P) && !attacksTakenComparison(attacksTaken, false, true, false, true, false))
                    {
                        AttackChecker(false, true, false, true, false);
                        attacksTaken.Add(new List<bool> { false, true, false, true, false });
                    }
                    if (Input.GetKeyDown(KeyCode.L) && !attacksTakenComparison(attacksTaken, true, false, false, true, false))
                    {
                        AttackChecker(true, false, false, true, false);
                        attacksTaken.Add(new List<bool> { true, false, false, true, false });
                    }
                    if (Input.GetKeyDown(KeyCode.Semicolon) && !attacksTakenComparison(attacksTaken, true, true, false, true, false))
                    {
                        AttackChecker(true, true, false, true, false);
                        attacksTaken.Add(new List<bool> { true, true, false, true, false });
                    }
                }
            }
            else
            {
                if ((left && facing.Equals("left") || right && facing.Equals("right")) && isCrouching)
                {
                    if (Input.GetKeyDown(KeyCode.O) && !attacksTakenComparison(attacksTaken, false, false, true, false, true))
                    {
                        AttackChecker(false, false, true, false, true);
                        attacksTaken.Add(new List<bool> { false, false, true, false, true });
                    }
                    if (Input.GetKeyDown(KeyCode.P) && !attacksTakenComparison(attacksTaken, false, true, true, false, true))
                    {
                        AttackChecker(false, true, true, false, true);
                        attacksTaken.Add(new List<bool> { false, true, true, false, true });
                    }
                    if (Input.GetKeyDown(KeyCode.L) && !attacksTakenComparison(attacksTaken, true, false, true, false, true))
                    {
                        AttackChecker(true, false, true, false, true);
                        attacksTaken.Add(new List<bool> { true, false, true, false, true });
                    }
                    if (Input.GetKeyDown(KeyCode.Semicolon) && !attacksTakenComparison(attacksTaken, true, true, true, false, true))
                    {
                        AttackChecker(true, true, true, false, true);
                        attacksTaken.Add(new List<bool> { true, true, true, false, true });
                    }
                }
                else if (left && facing.Equals("left") || right && facing.Equals("right"))
                {
                    if (Input.GetKeyDown(KeyCode.O) && !attacksTakenComparison(attacksTaken, false, false, true, false, false))
                    {
                        AttackChecker(false, false, true, false, false);
                        attacksTaken.Add(new List<bool> { false, false, true, false, false });
                    }
                    if (Input.GetKeyDown(KeyCode.P) && !attacksTakenComparison(attacksTaken, false, true, true, false, false))
                    {
                        AttackChecker(false, true, true, false, false);
                        attacksTaken.Add(new List<bool> { false, true, true, false, false });
                    }
                    if (Input.GetKeyDown(KeyCode.L) && !attacksTakenComparison(attacksTaken, true, false, true, false, false))
                    {
                        AttackChecker(true, false, true, false, false);
                        attacksTaken.Add(new List<bool> { true, false, true, false, false });
                    }
                    if (Input.GetKeyDown(KeyCode.Semicolon) && !attacksTakenComparison(attacksTaken, true, true, true, false, false))
                    {
                        AttackChecker(true, true, true, false, false);
                        attacksTaken.Add(new List<bool> { true, true, true, false, false });
                    }
                }
                else if (isCrouching)
                {
                    if (Input.GetKeyDown(KeyCode.O) && !attacksTakenComparison(attacksTaken, false, false, false, false, true))
                    {
                        AttackChecker(false, false, false, false, true);
                        attacksTaken.Add(new List<bool> { false, false, false, false, true });
                    }
                    if (Input.GetKeyDown(KeyCode.P) && !attacksTakenComparison(attacksTaken, false, true, false, false, true))
                    {
                        AttackChecker(false, true, false, false, true);
                        attacksTaken.Add(new List<bool> { false, true, false, false, true });
                    }
                    if (Input.GetKeyDown(KeyCode.L) && !attacksTakenComparison(attacksTaken, true, false, false, false, true))
                    {
                        AttackChecker(true, false, false, false, true);
                        attacksTaken.Add(new List<bool> { true, false, false, false, true });
                    }
                    if (Input.GetKeyDown(KeyCode.Semicolon) && !attacksTakenComparison(attacksTaken, true, true, false, false, true))
                    {
                        AttackChecker(true, true, false, false, true);
                        attacksTaken.Add(new List<bool> { true, true, false, false, true });
                    }
                }
                else
                {
                    if (Input.GetKeyDown(KeyCode.O) && !attacksTakenComparison(attacksTaken, false, false, false, false, false))
                    {
                        AttackChecker(false, false, false, false, false);
                        attacksTaken.Add(new List<bool> { false, false, false, false, false });
                    }
                    if (Input.GetKeyDown(KeyCode.P) && !attacksTakenComparison(attacksTaken, false, true, false, false, false))
                    {
                        AttackChecker(false, true, false, false, false);
                        attacksTaken.Add(new List<bool> { false, true, false, false, false });
                    }
                    if (Input.GetKeyDown(KeyCode.L) && !attacksTakenComparison(attacksTaken, true, false, false, false, false))
                    {
                        AttackChecker(true, false, false, false, false);
                        attacksTaken.Add(new List<bool> { true, false, false, false, false });
                    }
                    if (Input.GetKeyDown(KeyCode.Semicolon) && !attacksTakenComparison(attacksTaken, true, true, false, false, false))
                    {
                        AttackChecker(true, true, false, false, false);
                        attacksTaken.Add(new List<bool> { true, true, false, false, false });
                    }
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
                        StartCoroutine(Attack(9, 3, 25, 1, .25f, -.5f, 6, 14, new List<string> { "high", "punch", "medium" }, 0, new Vector2(0, 0)));
                    }
                }
                else
                {
                    if (isKick)
                    {

                    }
                    else
                    {
                        StartCoroutine(Attack(9, 3, 25, 1, .25f, -.5f, 6, 14, new List<string> { "high", "punch", "medium" }, 0, new Vector2(0, 0)));
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
                        StartCoroutine(Attack(4, 2, 19, .75f, .5f, -2, 4, 9, new List<string> { "high", "punch", "light" }, 0, new Vector2(0, 0)));
                    }
                }
                else
                {
                    if (isKick)
                    {

                    }
                    else
                    {
                        StartCoroutine(Attack(3, 3, 18, 1, .5f, 0, 3, 9, new List<string> { "high", "punch", "light" }, 0, new Vector2(0, 0)));
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
                            StartCoroutine(Attack(13, 3, 31, 1f, .6f, -1.5f, 8, 17, new List<string> { "mid", "punch", "medium" }, 0, new Vector2(0, 0)));
                        }
                        else
                        {
                            StartCoroutine(Attack(15, 3, 44, .75f, .5f, 0, 15, 22, new List<string> { "mid", "punch", "heavy" }, 0, new Vector2(4, 0)));
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
                            StartCoroutine(Attack(10, 6, 32, .75f, 2f, 1, 10, 17, new List<string> { "mid", "punch", "med" }, 0, new Vector2(0, 0)));
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
                            StartCoroutine(Attack(6, 2, 19, 1, .5f, -2, 4, 17, new List<string> { "low", "punch", "light" }, 0, new Vector2(0, 0)));
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
        GameObject visual = Instantiate(visualizer);
        visual.transform.localScale = new Vector3(xlen, ylen, 0);
        visual.SetActive(false);
        while (endlag > 0)
        {
            if (startedAir != isAirborne)
                endlag = 0;
            if (endlag <= ending - startup && endlag > ending - startup - hittime && !hasHit)
            {
                visual.transform.position = new Vector3(transform.position.x + xlen * 4 * facingAdjust, playerMovement.y + transform.position.y + yadjust, 0);

                transform.position += movement;
                Collider2D enemyHit = Physics2D.OverlapBox(new Vector2(transform.position.x + xlen * 4 * facingAdjust, transform.position.y + yadjust), new Vector2(xlen * 4, ylen * 4), 0, enemies);
                visual.SetActive(true);
                if (enemyHit != null)
                {

                    canCancel = true;
                    combocrouching = true;
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
        hit = true;
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
                if (facing.Equals("left") && rb.velocity.x < 0)
                {
                    rb.velocity = new Vector2(-rb.velocity.x, rb.velocity.y);
                }
                else if (facing.Equals("right") && rb.velocity.x > 0)
                {
                    rb.velocity = new Vector2(-rb.velocity.x, rb.velocity.y);
                }
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
                        transform.position = transform.position + new Vector3(1.5f, 0, 0);
                    }
                    else
                    {
                        transform.position = transform.position - new Vector3(1.5f, 0, 0);
                    }
                }
                else
                {
                    if (facing.Equals("left"))
                    {
                        transform.position = transform.position + new Vector3(2, 0, 0);
                    }
                    else
                    {
                        transform.position = transform.position - new Vector3(2, 0, 0);
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
                transform.position = transform.position + new Vector3(1.5f, 0, 0);
            }
            else
            {
                transform.position = transform.position - new Vector3(1.5f, 0, 0);
            }
        }
        else
        {
            if (facing.Equals("left"))
            {
                transform.position = transform.position + new Vector3(2f, 0, 0);
            }
            else
            {
                transform.position = transform.position - new Vector3(2f, 0, 0);
            }
        }

    }

    bool attacksTakenComparison(List<List<bool>> attacksTaken, bool one, bool two, bool three, bool four, bool five)
    {
        foreach (List<bool> l in attacksTaken)
        {
            if (l[0] == one && l[1] == two && l[2] == three && l[3] == four && l[4] == five)
                return true;
        }
        return false;
    }

    void OnCollisionStay2D(Collision2D col)
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
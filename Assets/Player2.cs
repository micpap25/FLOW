﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine; 
public class Player2 : MonoBehaviour {
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
	private float hitstun;
	private float endlag;
	private string keyword;
	private string dashdir;
	private List<string> FrontDashInput;
	private List<string> BackDashInput;
	private List<string> QCFInput;
	private List<string> QCBInput;
	private List<string> DQCFInput;
	private List<string> DQCBInput;
	private List<string> DDInput;
	private List<string> FrontDash;
	private List<string> BackDash;
	private List<string> QCF;
	private List<string> QCB;
	private List<string> DQCF;
	private List<string> DQCB;
	private List<string> DD;
	private List<string> actionsTaken;
	private float dashTime;
	private float inputRefreshTime;
	private bool canCancel;
	private bool isCrouching;
	private bool crouchAdjusted;
	private bool canBlockHigh;
	private bool canBlockMid;
	private bool canBlockLow;
	private bool isAirborne;
	private string mostRecentAttackType;
	private int comboLength;
	private int flowLength;
	private BoxCollider2D boxcol;
	private Animator anim;
    private Rigidbody2D rb;
    private GameObject opponent;
	private float oldsize;

	// Use this for initialization
	void Start () {
		//set hitstun and endlag. hitstun is for damage lag, endlag is for move lag. Eventually split endlag into endlag, dashlag, and blocklag.
		hitstun = 0;
		endlag = 0;
		//lists for inputs. input list is const, other list changes as it goes through, if it's empty the move can be performed. Maybe change to arraylist?
		//after inputRefreshTime change all the lists in inputs back to the defaults.
		inputRefreshTime = 0;
		dashdir = "none";
		FrontDashInput = new List<string>{ "front", "front" };
		BackDashInput = new List<string>{ "back", "back" };
		QCFInput = new List<string>{ "down", "front" };
		QCBInput = new List<string>{ "down",  "back" };
		DQCFInput = new List<string>{ "front", "down", "front"};
		DQCBInput = new List<string>{ "back", "down", "back"};
		DDInput = new List<string>{ "down", "down" };
		FrontDash = new List<string>(FrontDashInput);
		BackDash = new List<string>(BackDashInput);
		QCF = new List<string> (QCFInput);
		QCB =  new List<string> (QCBInput);
		DQCF = new List<string> (DQCFInput);
		DQCB =  new List<string> (DQCBInput);
		DD =  new List<string> (DDInput);
		keyword = "none";
		//list of actions taken. can't  repeat the same action twice in a flow.
		actionsTaken = new List<string>{ };
		//set the committment to dashing. maybe merge into endlag?
		dashTime = 0;
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
		//combos increase if hitstun is true & an attack lands. scaling & stuff. Can be escaped.
		comboLength = 0;
		//flows increase 
		flowLength = 0;
		//aaaaaaaaaaa
		boxcol = GetComponent<BoxCollider2D>();
		oldsize = boxcol.size.y;
        rb = GetComponent<Rigidbody2D>();
		anim = GetComponent<Animator> ();
		opponent = GameObject.FindGameObjectWithTag ("Player1");

	}
	
	// Update is called once per frame
	void FixedUpdate () {
		//set important stuff first
		if (!Input.GetKey (KeyCode.DownArrow)) {
			isCrouching = false;
			crouchAdjusted = false;
		}
		if (health <= 0)
			Destroy (gameObject);
        if (hitstun == 0 && endlag == 0)
        {
            if (opponent.transform.position.x < transform.position.x)
                facing = "left";
            else
                facing = "right";
        }
		
		if (!crouchAdjusted){
			if (isCrouching) {
				boxcol.offset = new Vector2 (0, ((oldsize * crouchScale) - (oldsize))/2);
				boxcol.size = new Vector2 (boxcol.size.x, oldsize * crouchScale);
			} else {
				boxcol.offset = new Vector2 (0, 0);
				boxcol.size = new Vector2 (boxcol.size.x, oldsize);
			}
			crouchAdjusted = true;
		}
		
		if (gameObject.transform.position.y > 0) {
			isAirborne = true;
			BackDash = new List<string> (BackDashInput);
			FrontDash = new List<string> (FrontDashInput);
			GetComponent<Rigidbody2D> ().AddForce (-transform.up * jumpSpeed * jumpSpeed);
		} else {
			isAirborne = false;
			GetComponent<Rigidbody2D> ().velocity = new Vector2 (0, 0);
			if (gameObject.transform.position.y < 0) {
				float x = gameObject.transform.position.x;
				gameObject.transform.position = new Vector3 (x, 0, 0);
			}
		}
		if (dashTime == 0) {
			dashdir = "none";
		}
		if (dashTime > 0) {
			if (facing.Equals ("left") && dashdir.Equals ("front") || facing.Equals ("right") && dashdir.Equals ("back")) {
				if (Input.GetKeyDown (KeyCode.UpArrow)) {
					GetComponent<Rigidbody2D> ().AddForce (new Vector2 (-moveSpeed * jumpSpeed * 100 * (dashSpeed / moveSpeed), 250 * jumpSpeed));
					dashTime = 0;
				} else
					Walk (-dashSpeed);
			} else {
				if (Input.GetKeyDown (KeyCode.UpArrow)) {
					GetComponent<Rigidbody2D> ().AddForce (new Vector2 (moveSpeed * jumpSpeed * 100 * (dashSpeed / moveSpeed), 250 * jumpSpeed));
					dashTime = 0;
				} else
					Walk (dashSpeed);
			}
			dashTime--;
		}
		//check for actions that can only be taken in neutral (walking, dashing, blocking)
		else if (hitstun == 0 && endlag == 0 && isAirborne == false) {
			if (Input.GetKey (KeyCode.DownArrow)) {
				//Include stuff for inputs  here at some point
				isCrouching = true;
				crouchAdjusted = false;
				if (canCrawl){
					if (Input.GetKey (KeyCode.LeftArrow))
						Walk (-crawlSpeed);
					if (Input.GetKey (KeyCode.RightArrow))
						Walk (crawlSpeed);
				}
			}
			else if (Input.GetKeyDown (KeyCode.UpArrow)) {
				if (Input.GetKey (KeyCode.LeftArrow)) {
					GetComponent<Rigidbody2D> ().AddForce (new Vector2 (-moveSpeed * jumpSpeed * 150, 250 * jumpSpeed));
				} else if (Input.GetKey (KeyCode.RightArrow)) {
					GetComponent<Rigidbody2D> ().AddForce (new Vector2 (moveSpeed * jumpSpeed * 150, 250 * jumpSpeed));
				} else {
					GetComponent<Rigidbody2D> ().AddForce (new Vector2 (0, jumpSpeed * 250));
				}
			} 

			else if (Input.GetKey (KeyCode.LeftArrow)) {
				//setting the keyword
				if (facing == "left")
					keyword = "front";
				else
					keyword = "back";
				if (Input.GetKeyDown (KeyCode.LeftArrow)) {
					if (BackDash [0].Equals (keyword)) {
						BackDash.RemoveAt (0);
						if (BackDash.Count == 0) {
							dashTime = startDashTime;
							dashdir = "back";
							inputRefreshTime = 0;
							//facing == 
							BackDash = new List<string> (BackDashInput);
						} else
							inputRefreshTime = 500;
					} else {
						BackDash = new List<string> (BackDashInput);
						inputRefreshTime = 0;
					}
						
					if (FrontDash [0].Equals (keyword)) {
						FrontDash.RemoveAt (0);
						if (FrontDash.Count == 0) {
							dashTime = startDashTime;
							dashdir = "front";
							inputRefreshTime = 0;
							FrontDash = new List<string> (FrontDashInput);
						}else
							inputRefreshTime = 500;
					}else {
						FrontDash = new List<string> (FrontDashInput);
						inputRefreshTime = 0;
					}
				}
				//do the rest for other inputs later
				else {
					Walk (-moveSpeed);
				}
			}
			else if (Input.GetKey (KeyCode.RightArrow)) {
				//setting the keyword
				if (facing == "left")
					keyword = "back";
				else
					keyword = "front";
				if (Input.GetKeyDown (KeyCode.RightArrow)) {
					if (BackDash [0].Equals (keyword)) {
						BackDash.RemoveAt (0);
						if (BackDash.Count == 0) {
							dashTime = startDashTime;
							inputRefreshTime = 0;
							dashdir = "back";
							BackDash = new List<string> (BackDashInput);
						} else
							inputRefreshTime = 500;
					}else {
						BackDash = new List<string> (BackDashInput);
						inputRefreshTime = 0;
					}
					if (FrontDash [0].Equals (keyword)) {
						FrontDash.RemoveAt (0);
						if (FrontDash.Count == 0) {
							dashTime = startDashTime;
							inputRefreshTime = 0;
							dashdir = "front";
							FrontDash = new List<string> (FrontDashInput);
						} else
							inputRefreshTime = 500;
					}else {
						FrontDash = new List<string> (FrontDashInput);
						inputRefreshTime = 0;
					}
				}

				//do the rest for other inputs later
				else {
					Walk (moveSpeed);
				}
			}

		} 
		//check for actions that can be taken in either neutral or mid-attack(jumping, attacking, inputs for specials, supers)
		else if (hitstun == 0){
		} 
		//check for actions that can only be taken during hitstun(breaking out of hitstun)
		else if (hitstun != 0){
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
		if(inputRefreshTime == 1){
			FrontDash = new List<string>(FrontDashInput);
			BackDash = new List<string>(BackDashInput);
			QCF = new List<string> (QCFInput);
			QCB =  new List<string> (QCBInput);
			DQCF = new List<string> (DQCFInput);
			DQCB =  new List<string> (DQCBInput);
			DD =  new List<string> (DDInput);
		}
		if (inputRefreshTime > 0)
			inputRefreshTime--;
		keyword = "none";

	}
	void Walk(float speed){
		gameObject.transform.Translate (speed, 0, 0);
	}
	//Attack types info:
	//First is always attack height. High means only stand block works, low means only crouch block works, mid means anything works. 
	//Arm, leg, and weapon are there for cancels.
	//Do the rest later.

	//figure out what the attack is. going to be massive as more characters are added. Should probably simplify but I like self destruction
	void AttackChecker(){
	}
	//attack using info. startup is time until attacking, hitend is when attack ends, ending is when move ends. Ties into endlag.
	//lens are range, damage is obvious, stun is amount of hitstun applied, attackTypes are also important
 	void Attack(float startup, float hitend, float ending, int xlen, int ylen, int damage, float stun, List<string> attackTypes){
	}
	//the damage is taken. applies damage, stun, and attack Types. Ties into hitstun.
	public void TakeDamage(int damage, float stun, float angle, List<string> attackTypes){
		endlag = 0.0f;
		hitstun = stun;
		health -= damage;
	}
	//the attack is blocked, applies reduced damage, stun, and attack types. 
	void Block(int damage, float stun, List<string> attackTypes){
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

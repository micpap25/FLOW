using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControls : MonoBehaviour {
	public float bufferspace;
    public float startingHeight;
	private Vector3 playeroneposition;
	private Vector3 playertwoposition;

	// Use this for initialization
	void Start () {
	playeroneposition = GameObject.FindGameObjectWithTag ("Player1").transform.position;
	playertwoposition = GameObject.FindGameObjectWithTag ("Player2").transform.position;

	}
	
	// Update is called once per frame
	void FixedUpdate () {
	Camera.main.transform.position = new Vector3 ((playeroneposition.x + playertwoposition.x) / 2, startingHeight + Mathf.Max(playeroneposition.y-startingHeight, playertwoposition.y-startingHeight ,0), -1);
    Camera.main.orthographicSize = Mathf.Min(Mathf.Max(10, (Mathf.Abs(playeroneposition.x - playertwoposition.x) / 2)), 20);
	playeroneposition = GameObject.FindGameObjectWithTag ("Player1").transform.position;
	playertwoposition = GameObject.FindGameObjectWithTag ("Player2").transform.position;
	}
}

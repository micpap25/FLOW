using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;

public class Countdown : MonoBehaviour
{
    public int RoundTime= 99;
    public Text countdown;//the timer gameobject
	private Player1 p1script;
	private Player2 p2script;
	//Attach your own Font in the Inspector
	public Font timeFont;
	public Font winFont;

	void Start()
    {
		GameObject player1 = GameObject.FindGameObjectWithTag("Player1");
		GameObject player2 = GameObject.FindGameObjectWithTag("Player2");
		p1script = player1.GetComponent<Player1>();
		p2script = player2.GetComponent<Player2>();
		StartCoroutine("TimegoDown");
        Time.timeScale=1;
		countdown.font = timeFont;
    }

    // Update is called once per frame
    void Update()
    {
        if (RoundTime == 0) {
            countdown.font = winFont;
            if (p1script.health > p2script.health)
            {
                countdown.text = ("Player 1 wins!");

            }
            else if (p2script.health > p1script.health)
            {
                countdown.text = ("Player 1 wins!");

            }
            else
            {
                countdown.text = ("Draw");
            }
        }
        else { countdown.text = ("" + RoundTime); }
        
    }

    IEnumerator TimegoDown()
    {
        while(RoundTime>0){
            yield return new WaitForSeconds(1);
            RoundTime--;
        }
		
	}
}

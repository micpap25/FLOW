using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;


public class Healthp1 : MonoBehaviour
{
	// Start is called before the first frame update
	public Image healthbar;
	public int currenthealth;
	public int maxhealth;
    private Player1 p1script;
	void Start()
    {
		GameObject player1 = GameObject.FindGameObjectWithTag("Player1");
		p1script = player1.GetComponent<Player1>();
        currenthealth = p1script.health;
        maxhealth = p1script.health;

    }

    // Update is called once per frame
    void Update()
    {
		currenthealth = p1script.health;

		healthbar.fillAmount = (float)currenthealth / (float)maxhealth;
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;


public class Healthp2 : MonoBehaviour
{
    // Start is called before the first frame update
    public Image healthbar;
    public int currenthealth;
    public int maxhealth=100;
    private Player2 p2script;
    void Start()
    {
        GameObject player2 = GameObject.FindGameObjectWithTag("Player2");
        p2script = player2.GetComponent<Player2>();
        currenthealth = p2script.health;
        maxhealth = p2script.health;
    }

    // Update is called once per frame
    void Update()
    {
        currenthealth = p2script.health;

        healthbar.fillAmount = (float)currenthealth / (float)maxhealth;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombobChanger : MonoBehaviour
{
    // Start is called before the first frame update
    public Text counterp1;//the counter gameobject
    private Player2 p2script;
    //Attach your own Font in the Inspector
    public Font timeFont;
    public Font winFont;
    public int number;
    void Start()
    {
        GameObject player2 = GameObject.FindGameObjectWithTag("Player2");
        p2script = player2.GetComponent<Player2>();

    }

    // Update is called once per frame
    void Update()
    {
        if ((p2script.hitstun>0)&&(p2script.hit)) {
            number++;
            if (number >= 2 && number<5) {
                counterp1.text = (number.ToString());
                    }
            if (number >= 5 && number < 10)
            {
                counterp1.text = (number.ToString()+ "\n"+"Nice!");
            }
            if (number >10)
            {
                counterp1.text = (number.ToString() + "Amazing!");
            }

        }
        if (p2script.hitstun.Equals(0)) {
            number = 0;
        }

    }
}

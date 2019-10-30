using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;

public class Countdown : MonoBehaviour
{
    public int RoundTime= 99;
    public Text countdown;//the timer gameobject
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("TimegoDown");
        Time.timeScale=1;
    }

    // Update is called once per frame
    void Update()
    {
       countdown.text=(""+RoundTime); 
    }
    IEnumerator TimegoDown()
    {
        while(true){
            yield return new WaitForSeconds(1);
            RoundTime--;
        }

    }
}

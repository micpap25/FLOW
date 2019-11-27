
using UnityEngine;
using System.Collections;

public class gameManager : MonoBehaviour
{
    public float barDisplay; //current progress
    public Vector2 pos1 = new Vector2(20, 40);
    public Vector2 size1 = new Vector2(60, 20);
    public Texture2D emptyTex;
    public Texture2D fullTex;
    private Player1 p1script;
    private Player2 p2script;

    void Start()
    {
        GameObject player1 = GameObject.FindGameObjectWithTag("Player1");
        GameObject player2 = GameObject.FindGameObjectWithTag("Player2");
        p1script = player1.GetComponent<Player1>();
        p2script = player1.GetComponent<Player2>();
    }


    void OnGUI()
    {
        //draw the background:
        GUI.BeginGroup(new Rect(pos1.x, pos1.y, size1.x, size1.y));
        GUI.Box(new Rect(0, 0, size1.x, size1.y), emptyTex);

        //draw the filled-in part:
        GUI.BeginGroup(new Rect(0, 0, size1.x * barDisplay, size1.y));
        GUI.Box(new Rect(0, 0, size1.x, size1.y), fullTex);
        GUI.EndGroup();
        GUI.EndGroup();
    }

    void Update()
    {
        //for this example, the bar display is linked to the current time,
        //however you would set this value based on your desired display
        //eg, the loading progress, the player's health, or whatever.
        barDisplay = p1script.health * 0.05f;
        //        barDisplay = MyControlScript.staticHealth;
    }
}
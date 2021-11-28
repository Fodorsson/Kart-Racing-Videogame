using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheckpointScript : MonoBehaviour
{
    //The serial number of the checkoint that this script is attached to
    public int number;

    //Declare the ListGO GameObject, which has the FindGO script attached to it
    public GameObject ListGO;
    public static FindGO FindGO;

    private void Start()
    {
        ListGO = GameObject.Find("ListGO");

        //Get the script as the component of the ListGO GameObject
        FindGO = ListGO.GetComponent<FindGO>();
    }

    private void OnTriggerEnter(Collider other)
    {
        //If the colliding gameobject is a player
        if (other.tag == "P1" || other.tag == "P2")
        {
            other.GetComponent<KartControl>().LatestCPindex = number;

            //How many checkpoints has the player cleared?
            int ClearedCheckpoints = other.GetComponent<KartControl>().CheckpointsCleared;

            //How many checkpoints are there on this track?
            int TotalCheckpoints = FindGO.Track.GetComponent<TrackGenerator>().points.Length / 2 + 1;

            //The number of checkpoints the player has cleared has to be compared to the serial number of this checkpoint, so we repeat the value as if it was reset to 0 with each lap
            int repeated = (int)Mathf.Repeat(ClearedCheckpoints, TotalCheckpoints);

            //If the two numbers match
            if (repeated == number)
            {
                //The player is granted one more cleared checkpoint
                other.GetComponent<KartControl>().CheckpointsCleared++;

                //If it is the player's 3rd time crossing the finish line (not counting the one at the very beginning) 
                if (ClearedCheckpoints == TotalCheckpoints * 3)
                {
                    //Then determine which player it was and display the finished message through its coroutine
                    if (other.tag == "P1")
                    {
                        GameObject P1Canvas = FindGO.P1Canvas;
                        P1Canvas.GetComponent<InterfaceScript>().kartMesh.transform.parent.GetComponent<KartControl>().CutsceneMode = true;
                        StartCoroutine(P1Canvas.GetComponent<InterfaceScript>().Finish());
                    }
                    else if (other.tag == "P2")
                    {
                        GameObject P2Canvas = FindGO.P2Canvas;
                        P2Canvas.GetComponent<InterfaceScript>().kartMesh.transform.parent.GetComponent<KartControl>().CutsceneMode = true;
                        StartCoroutine(P2Canvas.GetComponent<InterfaceScript>().Finish());
                    }

                }
                //If it's the player's second time crossing the finish line, update the lap count text accordingly
                else if (ClearedCheckpoints == TotalCheckpoints * 2)
                {
                    if (other.tag == "P1")
                    {
                        GameObject P1Canvas = FindGO.P1Canvas;
                        Text LapCountText = P1Canvas.transform.GetChild(2).gameObject.GetComponent<Text>();
                        GameObject.Find("P1 Avatar").GetComponent<KartControl>().currentLap = 3;
                        StartCoroutine(InterfaceScript.HighlightChange(LapCountText.gameObject, "Lap 3/3", 0.3f));
                    }
                    else if (other.tag == "P2")
                    {
                        GameObject P2Canvas = FindGO.P2Canvas;
                        Text LapCountText = P2Canvas.transform.GetChild(2).gameObject.GetComponent<Text>();
                        GameObject.Find("P2 Avatar").GetComponent<KartControl>().currentLap = 3;
                        StartCoroutine(InterfaceScript.HighlightChange(LapCountText.gameObject, "Lap 3/3", 0.3f));
                    }
                }
                //If it's the player's first time crossing the finish line, update the lap count text accordingly
                else if (ClearedCheckpoints == TotalCheckpoints * 1)
                {
                    if (other.tag == "P1")
                    {
                        GameObject P1Canvas = FindGO.P1Canvas;
                        Text LapCountText = P1Canvas.transform.GetChild(2).gameObject.GetComponent<Text>();
                        GameObject.Find("P1 Avatar").GetComponent<KartControl>().currentLap = 2;
                        StartCoroutine(InterfaceScript.HighlightChange(LapCountText.gameObject, "Lap 2/3", 0.3f));
                    }
                    else if (other.tag == "P2")
                    {
                        GameObject P2Canvas = FindGO.P2Canvas;
                        Text LapCountText = P2Canvas.transform.GetChild(2).gameObject.GetComponent<Text>();
                        GameObject.Find("P2 Avatar").GetComponent<KartControl>().currentLap = 2;
                        StartCoroutine(InterfaceScript.HighlightChange(LapCountText.gameObject, "Lap 2/3", 0.3f));
                    }
                }

            }
        }


    }


}

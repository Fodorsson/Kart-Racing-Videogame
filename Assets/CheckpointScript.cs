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

    public static float[] P1LapStarts = new float[] { 0f, 0f, 0f, 0f };
    public static float[] P2LapStarts = new float[] { 0f, 0f, 0f, 0f };
    public static float[] P1LapTimes = new float[] { 0f, 0f, 0f };
    public static float[] P2LapTimes = new float[] { 0f, 0f, 0f };

    public static bool P1Won;

    private void Start()
    {
        ListGO = GameObject.Find("ListGO");

        //Get the script as the component of the ListGO GameObject
        FindGO = ListGO.GetComponent<FindGO>();

        P1LapStarts = new float[] { 0f, 0f, 0f, 0f };
        P2LapStarts = new float[] { 0f, 0f, 0f, 0f };
        P1LapTimes = new float[] { 0f, 0f, 0f };
        P2LapTimes = new float[] { 0f, 0f, 0f };
    }

    string ToTimeString(float time)
    {
        int mins = 0;

        while (time - 60f > 0)
        {
            mins++;
            time -= 60f;
        }

        return mins.ToString("00") + ":" + time.ToString("00.000");
    }

    private void Update()
    {
        if (P1LapStarts[0] > 0f)
        {
            float currentTime = Time.time - P1LapStarts[0];
            //Update the lap times text
            FindGO.P1Canvas.transform.GetChild(5).GetComponent<Text>().text = "Time " + ToTimeString(currentTime);
            if (P1LapTimes[0] > 0f)
                FindGO.P1Canvas.transform.GetChild(5).GetComponent<Text>().text += "\nLap 1 " + ToTimeString(P1LapTimes[0]);
            if (P1LapTimes[1] > 0f)
                FindGO.P1Canvas.transform.GetChild(5).GetComponent<Text>().text += "\nLap 2 " + ToTimeString(P1LapTimes[1]);
            if (P1LapTimes[2] > 0f)
                FindGO.P1Canvas.transform.GetChild(5).GetComponent<Text>().text += "\nLap 3 " + ToTimeString(P1LapTimes[2]);
        }

        if (P2LapStarts[0] > 0f)
        {
            float currentTime = Time.time - P2LapStarts[0];
            //Update the lap times text
            FindGO.P2Canvas.transform.GetChild(5).GetComponent<Text>().text = ToTimeString(currentTime);
            if (P2LapTimes[0] > 0f)
                FindGO.P2Canvas.transform.GetChild(5).GetComponent<Text>().text += "\nLap 1 " + ToTimeString(P2LapTimes[0]);
            if (P2LapTimes[1] > 0f)
                FindGO.P2Canvas.transform.GetChild(5).GetComponent<Text>().text += "\nLap 2 " + ToTimeString(P2LapTimes[1]);
            if (P2LapTimes[2] > 0f)
                FindGO.P2Canvas.transform.GetChild(5).GetComponent<Text>().text += "\nLap 3 " + ToTimeString(P2LapTimes[2]);
        }

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
                        P1LapStarts[3] = Time.time;
                        P1LapTimes[2] = P1LapStarts[3] - P1LapStarts[2];
                        P1Won = true;


                    }
                    else if (other.tag == "P2")
                    {
                        P2LapStarts[3] = Time.time;
                        P2LapTimes[2] = P2LapStarts[3] - P2LapStarts[2];
                        P1Won = false;

                    }
                    FindGO.P1Canvas.GetComponent<InterfaceScript>().kartMesh.transform.parent.GetComponent<KartControl>().CutsceneMode = true;
                    FindGO.P2Canvas.GetComponent<InterfaceScript>().kartMesh.transform.parent.GetComponent<KartControl>().CutsceneMode = true;
                    StartCoroutine(FindGO.P1Canvas.GetComponent<InterfaceScript>().Finish());
                    StartCoroutine(FindGO.P2Canvas.GetComponent<InterfaceScript>().Finish());

                }
                //If it's the player's second time crossing the finish line, update the lap count text accordingly
                else if (ClearedCheckpoints == TotalCheckpoints * 2)
                {
                    if (other.tag == "P1")
                    {
                        P1LapStarts[2] = Time.time;
                        P1LapTimes[1] = P1LapStarts[2] - P1LapStarts[1];


                        GameObject P1Canvas = FindGO.P1Canvas;
                        Text LapCountText = P1Canvas.transform.GetChild(2).gameObject.GetComponent<Text>();
                        FindGO.P1Avatar.GetComponent<KartControl>().currentLap = 3;
                        FindGO.PlaySound("sfx/22 lapcomplete", 0.2f);
                        StartCoroutine(InterfaceScript.HighlightChange(LapCountText.gameObject, "3/3", 0.3f));
                    }
                    else if (other.tag == "P2")
                    {
                        P2LapStarts[2] = Time.time;
                        P2LapTimes[1] = P2LapStarts[2] - P2LapStarts[1];


                        GameObject P2Canvas = FindGO.P2Canvas;
                        Text LapCountText = P2Canvas.transform.GetChild(2).gameObject.GetComponent<Text>();
                        FindGO.P2Avatar.GetComponent<KartControl>().currentLap = 3;
                        FindGO.PlaySound("sfx/22 lapcomplete", 0.2f);
                        StartCoroutine(InterfaceScript.HighlightChange(LapCountText.gameObject, "3/3", 0.3f));
                    }
                }
                //If it's the player's first time crossing the finish line, update the lap count text accordingly
                else if (ClearedCheckpoints == TotalCheckpoints * 1)
                {
                    if (other.tag == "P1")
                    {
                        P1LapStarts[1] = Time.time;
                        P1LapTimes[0] = P1LapStarts[1] - P1LapStarts[0];

                        GameObject P1Canvas = FindGO.P1Canvas;
                        Text LapCountText = P1Canvas.transform.GetChild(2).gameObject.GetComponent<Text>();
                        FindGO.P1Avatar.GetComponent<KartControl>().currentLap = 2;
                        FindGO.PlaySound("sfx/22 lapcomplete", 0.2f);
                        StartCoroutine(InterfaceScript.HighlightChange(LapCountText.gameObject, "2/3", 0.3f));
                    }
                    else if (other.tag == "P2")
                    {
                        P2LapStarts[1] = Time.time;
                        P2LapTimes[0] = P2LapStarts[1] - P2LapStarts[0];

                        GameObject P2Canvas = FindGO.P2Canvas;
                        Text LapCountText = P2Canvas.transform.GetChild(2).gameObject.GetComponent<Text>();
                        FindGO.P2Avatar.GetComponent<KartControl>().currentLap = 2;
                        FindGO.PlaySound("sfx/22 lapcomplete", 0.2f);
                        StartCoroutine(InterfaceScript.HighlightChange(LapCountText.gameObject, "2/3", 0.3f));
                    }
                }
                else if (ClearedCheckpoints == 0)
                {
                    if (other.tag == "P1")
                    {
                        P1LapStarts[0] = Time.time;
                    }
                    else if (other.tag == "P2")
                    {
                        P2LapStarts[0] = Time.time;
                    }

                }

            }
        }


    }


}

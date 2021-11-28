using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PositionUpdater : MonoBehaviour
{
    public GameObject P1;
    public GameObject P2;

    Text P1PosText; 
    Text P2PosText;

    int P1CheckpointsCleared;
    int P2CheckpointsCleared;

    public static string firsttext;

    //Declare the ListGO GameObject, which has the FindGO script attached to it
    public GameObject ListGO;
    public static FindGO FindGO;

    private void Awake()
    {
        //Get the script as the component of the ListGO GameObject
        FindGO = ListGO.GetComponent<FindGO>();
    }

    void Start()
    {
        P1PosText = FindGO.P1Canvas.transform.GetChild(3).gameObject.GetComponent<Text>();
        P2PosText = FindGO.P2Canvas.transform.GetChild(3).gameObject.GetComponent<Text>();
    }

    void Update()
    {
        //What is the next checkpoint that the players are going to reach?
        P1CheckpointsCleared = P1.GetComponent<KartControl>().CheckpointsCleared;
        P2CheckpointsCleared = P2.GetComponent<KartControl>().CheckpointsCleared;

        //If the first player has cleared more checkpoints
        if (P1CheckpointsCleared > P2CheckpointsCleared)
        {
            //        P1PosText.text = "1st";
            //        P2PosText.text = "2nd";
            if (firsttext != "P1")
            {
                firsttext = "P1";

                StartCoroutine(InterfaceScript.HighlightChange(P1PosText.gameObject, "1st", 0.3f));
                StartCoroutine(InterfaceScript.HighlightChange(P2PosText.gameObject, "2nd", 0.3f));
            }
            

        }
        //If the second player as cleared more checkpoints
        else if (P2CheckpointsCleared > P1CheckpointsCleared)
        {

            if(firsttext != "P2")
            {
                firsttext = "P2";

                StartCoroutine(InterfaceScript.HighlightChange(P2PosText.gameObject, "1st", 0.3f));
                StartCoroutine(InterfaceScript.HighlightChange(P1PosText.gameObject, "2nd", 0.3f));
            }
        }
        //If they have cleared the same amount of checkpoints, we have to decide based on other factors
        else
        {
            //How many checkpoints are there on this track?
            int TotalCheckpoints = FindGO.Track.GetComponent<TrackGenerator>().CheckpointPositions.Count;

            
            //The index of the next checkpoint
            int closestCPindex = (int)Mathf.Repeat(P1CheckpointsCleared, TotalCheckpoints);

            //What are the coordinates for this checkpoint?
            Vector3 CPpos = FindGO.Track.GetComponent<TrackGenerator>().CheckpointPositions[closestCPindex];

            //Position of each kart
            Vector3 P1pos = P1.transform.position;
            Vector3 P2pos = P2.transform.position;

            //Each player's distance to the next checkpoint
            float P1dist = Vector3.Distance(CPpos, P1pos);
            float P2dist = Vector3.Distance(CPpos, P2pos);

            //If the first player is closer
            if (P1dist < P2dist)
            {

                if (firsttext != "P1")
                {
                    firsttext = "P1";

                    StartCoroutine(InterfaceScript.HighlightChange(P1PosText.gameObject, "1st", 0.3f));
                    StartCoroutine(InterfaceScript.HighlightChange(P2PosText.gameObject, "2nd", 0.3f));
                }
            }
            //If the second player is closer
            else if (P2dist < P1dist)
            {

                if (firsttext != "P2")
                {
                    firsttext = "P2";

                    StartCoroutine(InterfaceScript.HighlightChange(P2PosText.gameObject, "1st", 0.3f));
                    StartCoroutine(InterfaceScript.HighlightChange(P1PosText.gameObject, "2nd", 0.3f));
                }
            }
            //Otherwise, the position counter just remains unchanged

        }

    }
}

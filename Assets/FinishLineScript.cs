using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishLineScript : MonoBehaviour
{

    public GameObject P1Canvas;
    public GameObject P2Canvas;

    //When the finish line's collider detects a collision with something
    private void OnTriggerEnter(Collider other)
    {
        
        if (other.tag == "P1")
        {
            P1Canvas.GetComponent<InterfaceScript>().kartMesh.transform.parent.GetComponent<KartControl>().CutsceneMode = true;
            StartCoroutine(P1Canvas.GetComponent<InterfaceScript>().Finish());
        }
        else if (other.tag == "P2")
        {
            P2Canvas.GetComponent<InterfaceScript>().kartMesh.transform.parent.GetComponent<KartControl>().CutsceneMode = true;
            StartCoroutine(P2Canvas.GetComponent<InterfaceScript>().Finish());
        }

    }

}

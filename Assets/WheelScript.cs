using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelScript : MonoBehaviour
{

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "ground")
        {
            transform.parent.GetComponent<KartControl>().OnGround = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "ground")
        {
            transform.parent.GetComponent<KartControl>().OnGround = true;
        }
    }

}

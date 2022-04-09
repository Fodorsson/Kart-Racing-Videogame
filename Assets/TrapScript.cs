using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapScript : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {

        if (other.tag == "P1" || other.tag == "P2")
        {
            FindGO.PlaySound("sfx/18 garlic squash", 0.3f);

            if (!other.transform.GetComponent<KartControl>().invincible)
                StartCoroutine(Stun(other.gameObject, 1f));
            else
            {
                //This trap got blocked but the victim isn't invincible anymore
                other.transform.GetComponent<KartControl>().invincible = false;

                //Destroy the trap
                Destroy(transform.gameObject);
            }

        }

    }

    private IEnumerator Stun(GameObject player, float duration)
    {
        //Hide the trap
        //We need to wait with the destruction of the trap, because this script is linked to it
        transform.GetComponent<BoxCollider>().enabled = false;
        transform.GetChild(0).GetComponent<Renderer>().enabled = false;
        transform.GetChild(1).GetComponent<Renderer>().enabled = false;

        player.transform.GetComponent<Rigidbody>().velocity = Vector3.zero;
        player.transform.GetComponent<KartControl>().stunned = true;
        yield return new WaitForSeconds(duration);
        player.transform.GetComponent<KartControl>().stunned = false;

        //Destroy the hidden trap
        Destroy(transform.gameObject);
    }

}

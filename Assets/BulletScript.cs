using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "P1" || other.tag == "P2")
        {
            if (!other.transform.GetComponent<KartControl>().invincible)
                StartCoroutine(Stun(other.gameObject, 1f) );
            else
            {
                //This bullet got blocked but the victim isn't invincible anymore
                other.transform.GetComponent<KartControl>().invincible = false;

                //Destroy the trap
                Destroy(transform.gameObject);
            }

        }
        else if (other.tag != "checkpoint" && other.tag != "pickup")
            Destroy(transform.gameObject);

    }

    private IEnumerator Stun(GameObject player, float duration)
    {
        //Hide the bullet
        transform.GetComponent<SphereCollider>().enabled = false;
        transform.GetComponent<Renderer>().material.color = new Color(0f, 0f, 0f, 0f);

        player.transform.GetComponent<Rigidbody>().velocity = Vector3.zero;
        player.transform.GetComponent<KartControl>().stunned = true;
        yield return new WaitForSeconds(duration);
        player.transform.GetComponent<KartControl>().stunned = false;

        //Destroy the hidden bullet
        Destroy(transform.gameObject);
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldScript : MonoBehaviour
{
    public float lifespan;

    private void Start()
    {
        lifespan = 5f;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "P1" || other.tag == "P2")
        {
            if (!other.transform.GetComponent<KartControl>().invincible)
                StartCoroutine(Stun(other.gameObject, 1f));
            else
            {
                //This shield got blocked but the victim isn't invincible anymore
                other.transform.GetComponent<KartControl>().invincible = false;

                //Destroy the shield
                Destroy(transform.gameObject);
            }

        }
        else if (other.tag == "bullet" || other.tag == "trap" || other.tag == "shield")
        {
            //The victim isn't invincible anymore
            transform.parent.transform.GetComponent<KartControl>().invincible = false;

            //Destroy both the hostile projectiles, and this shield
            Destroy(other.transform.gameObject);
            Destroy(transform.gameObject);
            
        }
            

    }

    private void Update()
    {
        lifespan -= Time.deltaTime;

        if (lifespan <= 0f)
        {
            Destroy(transform.gameObject);
            transform.parent.GetComponent<KartControl>().invincible = false;
        }

    }

    private IEnumerator Stun(GameObject player, float duration)
    {
        //Hide the shield
        transform.GetComponent<SphereCollider>().enabled = false;
        transform.GetComponent<Renderer>().material.color = new Color(0f, 0f, 0f, 0f);

        player.transform.GetComponent<Rigidbody>().velocity = Vector3.zero;
        player.transform.GetComponent<KartControl>().stunned = true;
        yield return new WaitForSeconds(duration);
        player.transform.GetComponent<KartControl>().stunned = false;

        //Destroy the hidden shield
        Destroy(transform.gameObject);
    }


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldScript : MonoBehaviour
{
    public float lifespan;

    public GameObject batShieldPrefab;

    private GameObject[] batties1 = new GameObject[16];
    private GameObject[] batties2 = new GameObject[16];
    private float[] randomDist = new float[16];

    public int corCount = 0;


    private void Start()
    {
        //Instantiate the moving bat sprites around the kart once the shield is active
        for (int i = 0; i < batties1.Length; i++)
        {
            //The bats are placed at a random distance from the center to make them feel more organic
            randomDist[i] = Random.Range(4f, 6f);

            //Evenly distribute the bats around the kart
            float angle = 360f / batties1.Length;

            batties1[i] = Instantiate(batShieldPrefab, transform.position, Quaternion.identity);
            batties1[i].transform.SetParent(transform);

            batties1[i].transform.position = transform.position + batties1[i].transform.forward * randomDist[i] + batties1[i].transform.up * -1f;
            batties1[i].transform.rotation = transform.parent.rotation * Quaternion.Euler(new Vector3(0f, angle * i, 0f));

            //Now the second line
            batties2[i] = Instantiate(batShieldPrefab, transform.position, Quaternion.identity);
            batties2[i].transform.SetParent(transform);

            batties2[i].transform.position = transform.position + batties2[i].transform.forward * -randomDist[i] + batties2[i].transform.up * 1f;
            batties2[i].transform.rotation = transform.parent.rotation * Quaternion.Euler(new Vector3(0f, angle * i, 0f));


        }

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
        for (int i = 0; i < batties1.Length; i++)
        {

            //Lower line
            batties1[i].transform.rotation *= Quaternion.Euler(0f, 1f, 0f);
            batties1[i].transform.position = transform.position + batties1[i].transform.forward * randomDist[i] + batties1[i].transform.up * -1f;

            //Upper line
            batties2[i].transform.rotation *= Quaternion.Euler(0f, -1f, 0f);
            batties2[i].transform.position = transform.position + batties2[i].transform.forward * -randomDist[i] + batties2[i].transform.up * 1f;

        }

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

        player.transform.GetComponent<Rigidbody>().velocity = Vector3.zero;
        player.transform.GetComponent<KartControl>().stunned = true;
        yield return new WaitForSeconds(duration);
        player.transform.GetComponent<KartControl>().stunned = false;

        //Destroy the hidden shield
        Destroy(transform.gameObject);
    }

    public IEnumerator ShieldFade(bool needFadein, float duration)
    {
        corCount++;

        AudioClip clipShield = Resources.Load("sfx/14 shield active", typeof(AudioClip)) as AudioClip;
        FindGO.ASshield.clip = clipShield;
        FindGO.ASshield.loop = true;
        FindGO.ASshield.Play();

        Color oldColor = batShieldPrefab.GetComponent<Renderer>().sharedMaterial.color;

        float startTime;
        float endTime;

        //Fade In
        if (needFadein)
        {
            startTime = Time.time;
            endTime = startTime + duration;

            while (Time.time < endTime)
            {
                float timePassed = Time.time - startTime;

                //How much time has passed compared to the whole duration of the animation
                float alpha = Mathf.Lerp(0f, 1f, Mathf.SmoothStep(0.0f, 1.0f, timePassed / duration));

                //Apply the new opacity value to the bat shield prefab's renderer
                batShieldPrefab.GetComponent<Renderer>().sharedMaterial.color = new Color(oldColor.r, oldColor.g, oldColor.b, alpha);

                yield return null;
            }

            //Now we wait before starting the fade out
            //We have to wait 5s (the shield's lifespan) - 2 * duration (one for fade in, one for fade out)
            yield return new WaitForSeconds(5f - 2 * duration);
            corCount--;

        }
        else
        {
            //We wait before starting the fade out
            //We have to wait the shield's remaining lifespan + 5s (the shield's lifespan) - 2 * duration (one for fade in, one for fade out)
            yield return new WaitForSeconds(5f - duration);
            corCount--;
        }

        //Fade Out
        if (corCount == 0)
        {
            corCount = 0;

            startTime = Time.time;
            endTime = startTime + duration;

            while (Time.time < endTime)
            {
                float timePassed = Time.time - startTime;

                //How much time has passed compared to the whole duration of the animation
                float alpha = 1 - Mathf.Lerp(0f, 1f, Mathf.SmoothStep(0.0f, 1.0f, timePassed / duration));

                //Apply the new opacity value to the bat shield prefab's renderer
                batShieldPrefab.GetComponent<Renderer>().sharedMaterial.color = new Color(oldColor.r, oldColor.g, oldColor.b, alpha);

                yield return null;
            }
            FindGO.ASshield.Stop();
            yield return null;

        }



    }


}

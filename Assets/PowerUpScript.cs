using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerUpScript : MonoBehaviour
{
    public int type;

    private float yval;

    void Start()
    {

        yval = 0f;

    }

    public void SetType(int newVal)
    {
        //Type can be 1: attack, 2: defend, 3: trap
        type = newVal;

        //Assign an appropriate material to the pickup, depending on the value of its type variable given to it in another script, where the prefab gets instantiated
        if (newVal == 1)
        {
            //transform.GetChild(0).GetComponent<Renderer>().material = Resources.Load("MatPUattack", typeof(Material)) as Material;
            GetComponent<Renderer>().material = Resources.Load("MatPUattack", typeof(Material)) as Material;
        }
        else if (newVal == 2)
        {
            //transform.GetChild(0).GetComponent<Renderer>().material = Resources.Load("MatPUdefend", typeof(Material)) as Material;
            GetComponent<Renderer>().material = Resources.Load("MatPUdefend", typeof(Material)) as Material;
        }
        else if (newVal == 3)
        {
            //transform.GetChild(0).GetComponent<Renderer>().material = Resources.Load("MatPUtrap", typeof(Material)) as Material;
            GetComponent<Renderer>().material = Resources.Load("MatPUtrap", typeof(Material)) as Material;
        }

    }

    void FixedUpdate()
    {
        yval = transform.rotation.eulerAngles.y + 1.5f;

        transform.rotation = Quaternion.Euler(35f, yval, 0f);
    }

    private void OnTriggerEnter(Collider other)
    {
        //If the colliding gameobject is a player
        if (other.tag == "P1" || other.tag == "P2")
        {
            other.GetComponent<KartControl>().powerupPossessed = type;

            other.GetComponent<KartControl>().playerCanvas.transform.GetChild(4).GetComponent<Image>().color = new Color(1, 1, 1, 1);

            //Update the player's powerup icon
            if (type == 1)
                other.GetComponent<KartControl>().playerCanvas.transform.GetChild(4).GetComponent<Image>().sprite = Resources.Load("icon_attack", typeof(Sprite)) as Sprite;
            else if (type == 2)
                other.GetComponent<KartControl>().playerCanvas.transform.GetChild(4).GetComponent<Image>().sprite = Resources.Load("icon_defend", typeof(Sprite)) as Sprite;
            else if (type == 3)
                other.GetComponent<KartControl>().playerCanvas.transform.GetChild(4).GetComponent<Image>().sprite = Resources.Load("icon_trap", typeof(Sprite)) as Sprite;

            //Make the powerup disappear
            //Destroy(gameObject);
            

            StartCoroutine(RespawnPowerUp(5f));


        }

    }

    private IEnumerator RespawnPowerUp(float waitTime)
    {
        //Disable the collider of the pickup
        transform.GetComponent<CapsuleCollider>().enabled = false;

        //Get the color of the pickup's material
        Color oldColor = transform.GetComponent<Renderer>().material.color;

        float fadeTime = 0.25f;

        float startTime = Time.time;
        float endTime = startTime + fadeTime;

        while (Time.time < endTime)
        {
            float timePassed = Time.time - startTime;

            //How much time has passed compared to the whole duration of the animation
            float alpha = 1 - Mathf.Lerp(0f, 1f, Mathf.SmoothStep(0.0f, 1.0f, timePassed / fadeTime));

            //Apply the new alpha value to the material of the pickup
            transform.GetComponent<Renderer>().material.color = new Color(oldColor.r, oldColor.g, oldColor.b, alpha);

            yield return null;
        }

        //Change the type of the powerup
        SetType(Random.Range(1, 4) );
        //Assign the new color to the oldColor variable
        oldColor = transform.GetComponent<Renderer>().material.color;
        //Make its materal invisible
        transform.GetComponent<Renderer>().material.color = new Color(oldColor.r, oldColor.g, oldColor.b, 0f);

        yield return new WaitForSeconds(waitTime);

        transform.GetComponent<CapsuleCollider>().enabled = true;

        //Now fade in
        startTime = Time.time;
        endTime = startTime + fadeTime;

        while (Time.time < endTime)
        {
            float timePassed = Time.time - startTime;

            //How much time has passed compared to the whole duration of the animation
            float alpha = Mathf.Lerp(0f, 1f, Mathf.SmoothStep(0.0f, 1.0f, timePassed / fadeTime));

            //Apply the new alpha value to the material of the pickup
            transform.GetComponent<Renderer>().material.color = new Color(oldColor.r, oldColor.g, oldColor.b, alpha);

            yield return null;
        }


        yield return null;

    }

}

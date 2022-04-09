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
            GetComponent<Renderer>().material = Resources.Load("MatPUattack", typeof(Material)) as Material;
        }
        else if (newVal == 2)
        {
            GetComponent<Renderer>().material = Resources.Load("MatPUdefend", typeof(Material)) as Material;
        }
        else if (newVal == 3)
        {
            GetComponent<Renderer>().material = Resources.Load("MatPUtrap", typeof(Material)) as Material;
        }

        //Assign the powerup's color to the glow as well
        Color matColor = transform.GetComponent<Renderer>().material.color;

        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).GetComponent<Renderer>().material.color = new Color(matColor.r, matColor.g, matColor.b, 1f);
        }

    }

    void FixedUpdate()
    {
        yval = transform.rotation.eulerAngles.y + 1.5f;

        transform.rotation = Quaternion.Euler(-45f, yval, 0f);
    }

    private void OnTriggerEnter(Collider other)
    {
        //If the colliding gameobject is a player
        if (other.tag == "P1" || other.tag == "P2")
        {
            other.GetComponent<KartControl>().powerupPossessed = type;

            Image iconImg = other.GetComponent<KartControl>().playerCanvas.transform.GetChild(4).GetComponent<Image>();

            //Update the player's powerup icon
            if (type == 1)
            {
                iconImg.sprite = Resources.Load("icon_attack", typeof(Sprite)) as Sprite;
                FindGO.PlaySound("sfx/10 stake acquire", 0.2f);
            }
            else if (type == 2)
            {
                iconImg.sprite = Resources.Load("icon_defend", typeof(Sprite)) as Sprite;
                FindGO.PlaySound("sfx/13 shield acquire", 0.2f);
            }
            else if (type == 3)
            {
                iconImg.sprite = Resources.Load("icon_trap", typeof(Sprite)) as Sprite;
                FindGO.PlaySound("sfx/16 garlic acquire", 0.2f);
            }
                
            other.GetComponent<KartControl>().playerCanvas.transform.GetChild(4).GetComponent<Image>().color = new Color(1, 1, 1, 0.80f);

            StartCoroutine(IconPopUp(iconImg, 0.4f));

            //Make the powerup disappear
            StartCoroutine(RespawnPowerUp(5f));

        }

    }

    private IEnumerator IconPopUp(Image img, float duration)
    {

        float startTime = Time.time;
        float endTime = startTime + duration;

        while (Time.time < endTime)
        {
            float timePassed = Time.time - startTime;

            //How much time has passed compared to the whole duration of the animation
            float scale = Mathf.Lerp(0f, 1f, Mathf.SmoothStep(0.0f, 1.0f, timePassed / duration));

            //Apply the new scale value to the icon's sprite
            img.rectTransform.localScale = new Vector3(scale, scale, scale);

            yield return null;
        }
    }

    private IEnumerator RespawnPowerUp(float waitTime)
    {
        //Disable the collider of the pickup
        transform.GetComponent<MeshCollider>().enabled = false;

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

            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).GetComponent<Renderer>().material.color = new Color(oldColor.r, oldColor.g, oldColor.b, alpha);
            }

            yield return null;
        }

        //Change the type of the powerup
        SetType(Random.Range(1, 4));
        //Assign the new color to the oldColor variable
        oldColor = transform.GetComponent<Renderer>().material.color;
        //Make its materal invisible
        transform.GetComponent<Renderer>().material.color = new Color(oldColor.r, oldColor.g, oldColor.b, 0f);

        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).GetComponent<Renderer>().material.color = new Color(oldColor.r, oldColor.g, oldColor.b, 0f);
        }

        yield return new WaitForSeconds(waitTime);

        transform.GetComponent<MeshCollider>().enabled = true;

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

            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).GetComponent<Renderer>().material.color = new Color(oldColor.r, oldColor.g, oldColor.b, alpha);
            }

            yield return null;
        }


        yield return null;

    }

}

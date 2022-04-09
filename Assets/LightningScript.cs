using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningScript : MonoBehaviour
{
    GameObject LightningPrefab;
    bool striking;

    void Start()
    {
        striking = false;
        LightningPrefab = Resources.Load("lightningPrefab", typeof(GameObject)) as GameObject;
    }

    void Update()
    {
        if (!striking)
            StartCoroutine(LightningStrike());

    }

    private IEnumerator LightningStrike()
    {
        striking = true;
        FindGO.PlaySound("sfx/02 lightning strike", 0.3f);

        GameObject LightningGO = Instantiate(LightningPrefab, Vector3.zero, Quaternion.identity);

        //We will determine where the lightning should spawn, and orient it towards the center of the map accordingly
        float x = Random.Range(0f, 360f);

        Quaternion rotation = Quaternion.Euler(0, x, 0);
        Vector3 negDistance = new Vector3(0.0f, 0.0f, -2000);

        Vector3 position = rotation * negDistance;

        //Elevate along the Y axis
        position.y += 750f;

        //Apply the new transform values
        LightningGO.transform.rotation = rotation;
        LightningGO.transform.position = position;

        yield return new WaitForSeconds(0.6f);
        DestroyImmediate(LightningGO);

        //Wait a random amount of time before allowing the next lightning strike
        float waitTime = Random.Range(5f, 20f);
        yield return new WaitForSeconds(waitTime);

        striking = false;
    }


}

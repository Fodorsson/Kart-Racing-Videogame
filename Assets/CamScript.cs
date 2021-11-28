using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamScript : MonoBehaviour
{
    public GameObject camPlace;
    [SerializeField] private float lerpSpeed = 6.0f;

    public Camera cam;

    float targetElevation;

    void Start()
    {

        cam.transform.position = camPlace.transform.position;
    }

    void Update()
    {
        //If the player is going down a steep slope we need to adjust the camera position so that it doesn't clip through the ground
        float pitch = transform.rotation.eulerAngles.x;

        float currentElevation = cam.transform.position.y - transform.position.y - 7.5f;

        currentElevation = Mathf.Clamp(currentElevation, 0f, 16f);

        if (pitch > 14f && pitch < 30f)
            targetElevation = pitch - 14f;
        else
            targetElevation = 0f;

        targetElevation = Mathf.Clamp(targetElevation, 0f, 16f);

        float correction = Mathf.Lerp(currentElevation, targetElevation, 4f * Time.deltaTime);

        //The camera should only update if there is no cutscene playing
        if (!transform.GetComponent<KartControl>().CutsceneMode)
        {
            //The cam place should only rotate along the Y axis, following the kart's rotation. We don't want the camera to tumble when the kart tumbles
            camPlace.transform.rotation = Quaternion.Euler(new Vector3(0f, transform.rotation.eulerAngles.y, 0f) );

            //Lerp camera position and rotation to follow the car
            cam.transform.rotation = Quaternion.Lerp(cam.transform.rotation, camPlace.transform.rotation, lerpSpeed * Time.deltaTime);
            //Let there be a distance of 25f between the kart and the camera, and elevate the camera by 7.5f
            Vector3 negDistance = new Vector3(0.0f, 0.0f, -25f);
            cam.transform.position = cam.transform.rotation * negDistance + transform.position + new Vector3(0f, 7.5f + correction, 0f);
        }

        
    }
}

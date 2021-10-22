using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamScript : MonoBehaviour
{
    public GameObject camPlace;
    [SerializeField] private float lerpSpeed = 6.0f;

    public Camera cam;

    void Start()
    {

        cam.transform.position = camPlace.transform.position;
    }

    void Update()
    {
        //The camera should only update if there is no cutscene playing
        if (!transform.GetComponent<KartControl>().CutsceneMode)
        {
            //The cam place should only rotate along the Y axis, following the kart's rotation. We don't want the camera to tumble when the kart tumbles
            camPlace.transform.rotation = Quaternion.Euler(new Vector3(0f, transform.rotation.eulerAngles.y, 0f) );

            //Lerp camera position and rotation to follow the car
            cam.transform.rotation = Quaternion.Lerp(cam.transform.rotation, camPlace.transform.rotation, lerpSpeed * Time.deltaTime);
            //Let there be a distance of 25f between the kart and the camera, and elevate the camera by 7.5f
            Vector3 negDistance = new Vector3(0.0f, 0.0f, -25f);
            cam.transform.position = cam.transform.rotation * negDistance + transform.position + new Vector3(0f, 7.5f, 0f);
        }
        
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InterfaceScript : MonoBehaviour
{
    public Text MessageText;
    public Camera cam;
    public GameObject kartMesh;

    void Start()
    {

        //Make the text transparent initially
        MessageText.color = new Color(1, 1, 1, 0);

        //Set the initial font size
        MessageText.fontSize = 60;

        kartMesh.transform.parent.GetComponent<KartControl>().finished = false;

        //Start the cutscene
        StartCoroutine(StartCutscene(2f));

    }

    private IEnumerator StartCutscene(float duration)
    {
        float startTime = Time.time;
        float endTime = startTime + duration;

        //The initial rotation of the camera before we start our coroutine
        Vector3 initCamRot = cam.transform.rotation.eulerAngles;

        float MaxRotDegree = kartMesh.transform.rotation.eulerAngles.y - 180f - initCamRot.y;

        while (Time.time < endTime)
        {
            float timePassed = Time.time - startTime;

            //How much time has passed compared to the whole duration of the animation
            float alpha = Mathf.Lerp(0f, 1f, Mathf.SmoothStep(0.0f, 1.0f, timePassed / duration) );

            //Rotation starts from -180 degrees, down to 0
            Vector3 currentRot = initCamRot + new Vector3(0f, MaxRotDegree * (1 - alpha), 0f);
            cam.transform.rotation = Quaternion.Euler(currentRot);

            //Position is position of the kart mesh, minus 25f along the camera's forward vector
            Vector3 currentPos = kartMesh.transform.position + cam.transform.forward * (-25f) + new Vector3(0f, 5.5f, 0f);
            cam.transform.position = currentPos;

            yield return null;
        }

        //Call the next coroutine
        yield return StartCoroutine(Countdown("3"));

    }

    private IEnumerator FinishCutscene(float duration)
    {
        float startTime = Time.time;
        float endTime = startTime + duration;

        //The initial rotation of the camera before we start our coroutine
        Vector3 initCamRot = cam.transform.rotation.eulerAngles;

        float MaxRotDegree = kartMesh.transform.rotation.eulerAngles.y - 180f - initCamRot.y;

        while (Time.time < endTime)
        {
            float timePassed = Time.time - startTime;

            //How much time has passed compared to the whole duration of the animation
            float alpha = Mathf.Lerp(0f, 1f, Mathf.SmoothStep(0.0f, 1.0f, timePassed / duration));

            //Rotation starts from -180 degrees, down to 1
            Vector3 currentRot = initCamRot + new Vector3(0f, MaxRotDegree * alpha, 0f);
            cam.transform.rotation = Quaternion.Euler(currentRot);

            //Position is position of the kart mesh, minus 25f along the camera's forward vector
            Vector3 currentPos = kartMesh.transform.position + cam.transform.forward * (-25f) + new Vector3(0f, 5.5f, 0f);
            cam.transform.position = currentPos;

            yield return null;
        }

        //If we set this bool, it will trigger the code in CamScript which follows the position of the kart with the cam
        kartMesh.transform.parent.GetComponent<KartControl>().finished = true;

        //Exit
        yield return null;

    }

    private IEnumerator Countdown(string text)
    {
        MessageText.text = text;

        float startTime = Time.time;

        //DurationUnit is 1 sec
        float endTime = startTime + 1f;

        while (Time.time < endTime)
        {
            float timePassed = Time.time - startTime;

            //How much time has passed compared to the whole duration of the animation
            float alpha = Mathf.Lerp(0f, 1f, Mathf.SmoothStep(0.0f, 1.0f, timePassed) );

            //Scaling the text
            MessageText.rectTransform.localScale = new Vector3(alpha, alpha, alpha);

            //Getting the current color value of the text
            Color oldColor = MessageText.color;

            //Changing the alpha component
            Color newColor = new Color(oldColor.r, oldColor.g, oldColor.b, 1 - alpha);

            //Assigning the new value to the text color
            MessageText.color = newColor;

            yield return null;
        }

        if (text == "3")
            yield return StartCoroutine(Countdown("2"));
        else if (text == "2")
            yield return StartCoroutine(Countdown("1"));
        else if (text == "1")
        {
            //The cutscene is over, give control to the players
            kartMesh.transform.parent.GetComponent<KartControl>().CutsceneMode = false;

            //Make the font size bigger
            MessageText.fontSize = 100;

            //Make the font color yellow instead of white
            MessageText.color = Color.yellow;

            yield return StartCoroutine(Countdown("GO!"));
        }
        else
        {
            yield return null;
        }

    }

    public IEnumerator Finish()
    {
        StartCoroutine(FinishCutscene(2f));

        MessageText.text = "Finish";
        MessageText.color = Color.yellow;

        float startTime = Time.time;

        //DurationUnit is 1 sec
        float endTime = startTime + 1f;

        while (Time.time < endTime)
        {
            float timePassed = Time.time - startTime;

            //How much time has passed compared to the whole duration of the animation
            float alpha = Mathf.Lerp(0f, 1f, Mathf.SmoothStep(0.0f, 1.0f, timePassed));

            //Scaling the text
            MessageText.rectTransform.localScale = new Vector3(alpha, alpha, alpha);

            //Getting the current color value of the text
            Color oldColor = MessageText.color;

            //Changing the alpha component
            Color newColor = new Color(oldColor.r, oldColor.g, oldColor.b, 1 - alpha);

            //Assigning the new value to the text color
            MessageText.color = newColor;

            yield return null;
        }

        yield return null;

    }

    void Update()
    {
        if (kartMesh.transform.parent.GetComponent<KartControl>() != null && kartMesh.transform.parent.GetComponent<KartControl>().finished)
        {
            //Position is position of the kart mesh, minus 25f along the camera's forward vector
            Vector3 currentPos = kartMesh.transform.position + cam.transform.forward * (-25f) + new Vector3(0f, 5.5f, 0f);
            cam.transform.position = currentPos;
        }

    }
}

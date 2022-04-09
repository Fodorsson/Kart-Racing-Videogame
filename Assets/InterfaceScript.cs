using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InterfaceScript : MonoBehaviour
{
    public Text MessageText;
    public Camera cam;
    public GameObject kartMesh;

    //Declare the ListGO GameObject, which has the FindGO script attached to it
    public GameObject ListGO;
    public static FindGO FindGO;

    public bool gameBegun = false;

    public GameObject playerCanvas;

    public static bool restartEnabled;

    public AudioSource ASmusic;

    private void Awake()
    {
        //Get the script as the component of the ListGO GameObject
        FindGO = ListGO.GetComponent<FindGO>();
    }

    void Start()
    {
        AudioClip clipThunder = Resources.Load("sfx/01 rain and thunder", typeof(AudioClip)) as AudioClip;
        AudioSource ASthunder = ListGO.transform.GetChild(0).GetComponent<AudioSource>();
        ASthunder.clip = clipThunder;
        ASthunder.loop = true;
        ASthunder.Play();

        AudioClip clipMusic = Resources.Load("Music", typeof(AudioClip)) as AudioClip;
        ASmusic = ListGO.transform.GetChild(5).GetComponent<AudioSource>();
        ASmusic.clip = clipMusic;
        ASmusic.loop = true;
        

        //The players can't restart the game until the race is finished
        restartEnabled = false;

        ColorGrading cg;
        FindGO.PostProcessing.GetComponent<PostProcessVolume>().profile.TryGetSettings(out cg);
        cg.enabled.value = true;
        cg.postExposure.value = Save.brightness - 5;

        //Make the text transparent initially
        MessageText.color = new Color(1, 1, 1, 1);

        //Set the initial font size
        MessageText.fontSize = 60;

        kartMesh.transform.parent.GetComponent<KartControl>().finished = false;

        gameBegun = false;

        //Disable HUD elements
        playerCanvas = transform.gameObject;

        for (int i = 0; i < playerCanvas.transform.childCount; i++)
        {
            playerCanvas.transform.GetChild(i).gameObject.SetActive(false);

        }
        MessageText.gameObject.SetActive(true);

    }

    private IEnumerator StartCutscene(float duration)
    {
        MessageText.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, 80);

        yield return new WaitForSeconds(0.1f);

        float startTime = Time.time;
        float endTime = startTime + duration;

        //The initial rotation of the camera before we start our coroutine
        Vector3 initCamRot = kartMesh.transform.rotation.eulerAngles;

        float MaxRotDegree = kartMesh.transform.rotation.eulerAngles.y - 180f - initCamRot.y;

        while (Time.time < endTime)
        {
            float timePassed = Time.time - startTime;

            //How much time has passed compared to the whole duration of the animation
            float alpha = Mathf.Lerp(0f, 1f, Mathf.SmoothStep(0.0f, 1.0f, timePassed / duration));

            //Rotation starts from -180 degrees, down to 0
            Vector3 currentRot = initCamRot + new Vector3(0f, MaxRotDegree * (1 - alpha), 0f);
            cam.transform.rotation = Quaternion.Euler(currentRot);

            //Position is position of the kart mesh, minus 25f along the camera's forward vector
            Vector3 currentPos = kartMesh.transform.position + cam.transform.forward * (-25f) + new Vector3(0f, 7.5f, 0f);
            cam.transform.position = currentPos;

            yield return null;
        }

        //Enable HUD elements
        for (int i = 0; i < playerCanvas.transform.childCount - 1; i++)
        {
            playerCanvas.transform.GetChild(i).gameObject.SetActive(true);
        }

        FindGO.PlaySound("sfx/20 countdown", 0.2f);
        //Call the next coroutine
        yield return StartCoroutine(Countdown("3"));

    }

    private IEnumerator FinishCutscene(float duration)
    {
        GameObject playerCanvas = transform.gameObject;

        //Disable HUD elements
        for (int i = 0; i < playerCanvas.transform.childCount; i++)
        {
            playerCanvas.transform.GetChild(i).gameObject.SetActive(false);
        }

        MessageText.gameObject.SetActive(true);

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
            Vector3 currentPos = kartMesh.transform.position + cam.transform.forward * (-25f) + new Vector3(0f, 7.5f, 0f);
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

        if (text == "3")
        {
            FindGO.PlaySound("sfx/20 countdown", 0.2f);
            yield return StartCoroutine(Countdown("2"));
        }
            
        else if (text == "2")
        {
            FindGO.PlaySound("sfx/20 countdown", 0.2f);
            yield return StartCoroutine(Countdown("1"));
        }
        else if (text == "1")
        {
            FindGO.PlaySound("sfx/21 go", 0.2f);
            
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
            ASmusic.Play();
            yield return null;
        }

    }

    public IEnumerator Finish()
    {
        FindGO.PlaySound("sfx/24 finish", 0.2f);

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

        //Stats text
        playerCanvas.transform.GetChild(6).gameObject.SetActive(true);

        string winmessage;

        if (CheckpointScript.P1Won && transform.name == "P1Canvas")
            winmessage = "You Win";
        else if (!CheckpointScript.P1Won && transform.name == "P2Canvas")
            winmessage = "You Win";
        else
            winmessage = "You Lose";

        playerCanvas.transform.GetChild(6).GetComponent<Text>().text = winmessage + "\n\n" + playerCanvas.transform.GetChild(5).GetComponent<Text>().text + "\n\nPress the 'Activate' button to restart";

        //The game is finished, the players are now permitted to restart
        restartEnabled = true;

        yield return null;

    }

    public static IEnumerator HighlightChange(GameObject text, string newValue, float duration)
    {
        FindGO.PlaySound("sfx/23 woosh", 0.5f);

        float startTime = Time.time;
        float endTime = startTime + duration;

        //The initial scale of the text
        Vector3 initScale = text.transform.localScale;
        Vector3 scaleAmount = text.transform.localScale / 2f;


        while (Time.time < endTime)
        {
            float timePassed = Time.time - startTime;

            //How much time has passed compared to the whole duration of the animation
            float alpha = Mathf.Lerp(0f, 1f, Mathf.SmoothStep(0.0f, 1.0f, timePassed / duration));

            Vector3 currentScale = initScale + scaleAmount * alpha;
            text.transform.localScale = currentScale;

            yield return null;
        }

        text.GetComponent<Text>().text = newValue;


        while (Time.time < endTime + duration)
        {
            float timePassed = Time.time - startTime;

            //How much time has passed compared to the whole duration of the animation
            float alpha = Mathf.Lerp(0f, 1f, Mathf.SmoothStep(0.0f, 1.0f, (timePassed - duration) / duration));

            Vector3 currentScale = initScale + scaleAmount - scaleAmount * alpha;
            text.transform.localScale = currentScale;

            yield return null;
        }

        //Exit
        yield return null;
    }

    void Update()
    {
        //We don't begin the game until both players have pressed a key on their preferred input device
        //This is to identify said device and determine the axis names to be used
        if (FindKey.P1joyNo != null && FindKey.P2joyNo != null && !gameBegun)
        {
            gameBegun = true;
            StartCoroutine(StartCutscene(2f));

        }

        //If we are still on the button prompt screen
        if (FindKey.P2joyNo == null)
        {
            Vector3 initCamRot = kartMesh.transform.rotation.eulerAngles;
            float MaxRotDegree = kartMesh.transform.rotation.eulerAngles.y - 180f - initCamRot.y;
            Vector3 currentRot = initCamRot + new Vector3(0f, MaxRotDegree * (1 - 0), 0f);

            cam.transform.rotation = Quaternion.Euler(currentRot);

            Vector3 currentPos = kartMesh.transform.position + cam.transform.forward * (-25f) + new Vector3(0f, 7.5f, 0f);
            cam.transform.position = currentPos;
        }

        if (kartMesh.transform.parent.GetComponent<KartControl>() != null && kartMesh.transform.parent.GetComponent<KartControl>().finished)
        {
            //Position is position of the kart mesh, minus 25f along the camera's forward vector
            Vector3 currentPos = kartMesh.transform.position + cam.transform.forward * (-25f) + new Vector3(0f, 7.5f, 0f);
            cam.transform.position = currentPos;
        }

        //Check for wrong way

        //How many checkpoints are there on this track?
        int TotalCheckpoints = FindGO.Track.GetComponent<TrackGenerator>().CheckpointPositions.Count;

        //The index of the next checkpoint
        int latestCPindex = kartMesh.transform.parent.GetComponent<KartControl>().LatestCPindex;

        //The index of the previous checkpoint
        int prevCPindex = (int)Mathf.Repeat(latestCPindex - 1, TotalCheckpoints);

        //What are the coordinates for this checkpoint?
        Vector3 CPposNext = FindGO.Track.GetComponent<TrackGenerator>().CheckpointPositions[latestCPindex];

        //What are the coordinates for the previous checkpoint?
        Vector3 CPposPrev = FindGO.Track.GetComponent<TrackGenerator>().CheckpointPositions[prevCPindex];

        //Direction from previous to next checkpoint
        Vector3 goodDir = Quaternion.LookRotation(CPposNext - CPposPrev, Vector3.up).eulerAngles;

        //Orientation of the kart
        Vector3 ori = kartMesh.transform.parent.transform.rotation.eulerAngles;

        float angle = goodDir.y - ori.y;
        if (angle < 0)
            angle += 360;

        if (angle > 115f && angle < 245f)
        {
            //Set the font size
            MessageText.fontSize = 50;

            //Set the MessageText to the wrong way message
            MessageText.text = "WRONG WAY!";

            //Make the colour visible
            MessageText.color = Color.red;
        }
        //If the game has already begun and there is no reason to display the text, then make it invisible
        else if (FindKey.P1joyNo != null && FindKey.P2joyNo != null)
        {
            //Make the colour invisible
            MessageText.color = new Color(1, 1, 1, 0);
        }

        //Prompt for keypress for each players
        if (transform.name == "P1Canvas" && !gameBegun)
        {
            if (FindKey.P1joyNo == null)
            {
                MessageText.fontSize = 20;
                MessageText.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, -180);
                MessageText.text = "Press the 'Activate' button to start";
            }
            else if (FindKey.P1joyNo != null && FindKey.P2joyNo == null)
            {
                MessageText.text = "Player 1 is ready";
            }

        }
        else if (transform.name == "P2Canvas" && !gameBegun)
        {
            if (FindKey.P1joyNo == null)
            {
                MessageText.fontSize = 20;
                MessageText.text = "";
            }
            else if (FindKey.P1joyNo != null && FindKey.P2joyNo == null)
            {
                MessageText.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, -180);
                MessageText.text = "Press the 'Activate' button to start";
            }

        }

        if (Input.GetKeyDown(Save.p1ActivateKey) || Input.GetKeyDown(Save.p2ActivateKey))
        {
            if (restartEnabled)
            {
                restartEnabled = false;
                SceneManager.LoadScene("Game", LoadSceneMode.Single);
            }
        }


    }
}

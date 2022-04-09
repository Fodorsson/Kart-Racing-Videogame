using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{

    //The index of the selected button
    private int selected = 0;
    private GameObject mainFrame;
    private GameObject optionsFrame;
    private GameObject controlsFrame;
    private GameObject p1Frame;
    private GameObject p2Frame;
    private GameObject videoFrame;
    private GameObject audioFrame;
    private GameObject confirmationFrame;
    private GameObject currentFrame;
    private GameObject parentFrame;
    private int parentIndex;

    private bool keyPromptActive = false;
    private Text targetText;
    private string targetKey;
    private string prevValue;

    private readonly Array keyCodes = Enum.GetValues(typeof(KeyCode));

    private Text helpText;

    public GameObject TitleVid;

    private Array GetKeyCodes()
    {
        return keyCodes;
    }

    void Awake()
    {
        // Make the game run as fast as possible in Windows
        Application.targetFrameRate = 300;

        //Change every text in the scene onto the chosen font
        foreach (Text item in Resources.FindObjectsOfTypeAll(typeof(Text)) as Text[])
        {
            item.font = Resources.Load("Phantom Fingers", typeof(Font)) as Font;
            item.fontSize /= 2;
        }

    }

    void Start()
    {
        mainFrame = transform.GetChild(0).gameObject;
        optionsFrame = transform.GetChild(1).gameObject;
        controlsFrame = transform.GetChild(2).gameObject;
        p1Frame = transform.GetChild(3).gameObject;
        p2Frame = transform.GetChild(4).gameObject;
        videoFrame = transform.GetChild(5).gameObject;
        audioFrame = transform.GetChild(6).gameObject;
        confirmationFrame = transform.GetChild(7).gameObject;

        helpText = transform.GetChild(8).gameObject.GetComponent<Text>();


        //We are in the main menu's frame
        currentFrame = mainFrame;

        mainFrame.SetActive(true);
        optionsFrame.SetActive(false);
        controlsFrame.SetActive(false);
        p1Frame.SetActive(false);
        p2Frame.SetActive(false);
        videoFrame.SetActive(false);
        audioFrame.SetActive(false);
        confirmationFrame.SetActive(false);

        //Make the selected button's text have red colour
        mainFrame.transform.GetChild(selected).GetComponent<Text>().color = Color.red;

        //Load all the data stored in the save file
        Save.LoadFile();

        UpdateSettingsDisplay();

        helpText.text = "[Up/Down Arrow]: Navigate\n[Enter]: Select";

        AudioClip clipThunder = Resources.Load("sfx/01 rain and thunder", typeof(AudioClip)) as AudioClip;
        AudioSource ASthunder = GetComponent<AudioSource>();
        ASthunder.clip = clipThunder;
        ASthunder.loop = true;
        ASthunder.Play();

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !keyPromptActive)
        {
            PlaySound("sfx/10 stake acquire", 0.3f);

            if (currentFrame == optionsFrame)
            {
                helpText.text = "[Up/Down Arrow]: Navigate\n[Enter]: Select";
                ChangeMenuFromTo(optionsFrame, mainFrame);
            }
            else if (currentFrame == controlsFrame)
            {
                helpText.text = "[Up/Down Arrow]: Navigate\n[Enter]: Select\n[Escape]: Back";
                ChangeMenuFromTo(controlsFrame, optionsFrame);
            }
            else if (currentFrame == p1Frame)
            {
                helpText.text = "[Up/Down Arrow]: Navigate\n[Enter]: Select\n[Escape]: Back";
                ChangeMenuFromTo(p1Frame, controlsFrame);
            }
            else if (currentFrame == p2Frame)
            {
                helpText.text = "[Up/Down Arrow]: Navigate\n[Enter]: Select\n[Escape]: Back";
                ChangeMenuFromTo(p2Frame, controlsFrame);
            }
            else if (currentFrame == audioFrame)
            {
                helpText.text = "[Up/Down Arrow]: Navigate\n[Enter]: Select\n[Escape]: Back";
                ChangeMenuFromTo(audioFrame, optionsFrame);
            }
            else if (currentFrame == videoFrame)
            {
                helpText.text = "[Up/Down Arrow]: Navigate\n[Enter]: Select\n[Escape]: Back";
                ChangeMenuFromTo(videoFrame, optionsFrame);
            }
            else if (currentFrame == confirmationFrame)
            {
                if (parentFrame == mainFrame)
                    parentIndex = 2;
                helpText.text = "[Up/Down Arrow]: Navigate\n[Enter]: Select\n[Escape]: Back";
                ChangeMenuFromTo(confirmationFrame, parentFrame, parentIndex);
            }


        }

        if (keyPromptActive && Input.anyKeyDown)
        {
            PlaySound("sfx/17 drop", 0.4f);

            foreach (KeyCode keyCode in GetKeyCodes())
            {
                if (Input.GetKeyDown(keyCode))
                {

                    if (keyCode == KeyCode.Escape)
                    {
                        keyPromptActive = false;
                        targetText.text = prevValue;
                        targetText.color = Color.white;
                        break;
                    }
                    //If the player supposedly has keyboard controls enabled and is trying to assign a joystick button, ignore them
                    else if (currentFrame == p1Frame && Save.p1ControllerIsKeyboard == keyCode.ToString().Contains("Joystick"))
                    {
                    }
                    else if (currentFrame == p2Frame && Save.p2ControllerIsKeyboard == keyCode.ToString().Contains("Joystick"))
                    {
                    }
                    else if (
                        //Forbidden keys
                        keyCode != KeyCode.UpArrow &&
                        keyCode != KeyCode.DownArrow &&
                        keyCode != KeyCode.LeftArrow &&
                        keyCode != KeyCode.RightArrow &&
                        keyCode != KeyCode.W &&
                        keyCode != KeyCode.S &&
                        keyCode != KeyCode.A &&
                        keyCode != KeyCode.D &&
                        keyCode != KeyCode.Return
                        )
                    {
                        keyPromptActive = false;
                        targetText.text = "[" + keyCode.ToString() + "]";
                        targetText.color = Color.white;

                        Debug.Log(Save.p1BrakeKey);

                        //Change the values in the Save class upon key binding
                        switch (targetKey)
                        {
                            case "Save.p1BrakeKey":
                                Save.p1BrakeKey = keyCode;
                                break;
                            case "Save.p1ActivateKey":
                                Save.p1ActivateKey = keyCode;
                                break;
                            case "Save.p1LookBehindKey":
                                Save.p1LookBehindKey = keyCode;
                                break;
                            case "Save.p1PauseKey":
                                Save.p1PauseKey = keyCode;
                                break;
                            case "Save.p2BrakeKey":
                                Save.p2BrakeKey = keyCode;
                                break;
                            case "Save.p2ActivateKey":
                                Save.p2ActivateKey = keyCode;
                                break;
                            case "Save.p2LookBehindKey":
                                Save.p2LookBehindKey = keyCode;
                                break;
                            case "Save.p2PauseKey":
                                Save.p2PauseKey = keyCode;
                                break;
                            default:
                                break;
                        }

                        Save.SaveFile();

                        Debug.Log(Save.p1BrakeKey);

                        break;

                    }

                }

            }

        }

        if ((Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) && !keyPromptActive)
        {
            PlaySound("sfx/20 countdown", 0.3f);

            //The index of the currently selected button
            int prevIndex = selected;

            //The index of the button above the currently selected one
            selected--;

            //Keep it inside the current frame's bounds
            if (currentFrame == mainFrame)
                selected = (int)Mathf.Repeat(selected, 3);
            else if (currentFrame == optionsFrame)
                selected = (int)Mathf.Repeat(selected, 4);
            else if (currentFrame == controlsFrame)
                selected = (int)Mathf.Repeat(selected, 3);
            else if (currentFrame == p1Frame)
                selected = (int)Mathf.Repeat(selected, 7);
            else if (currentFrame == p2Frame)
                selected = (int)Mathf.Repeat(selected, 7);
            else if (currentFrame == videoFrame)
                selected = (int)Mathf.Repeat(selected, 6);
            else if (currentFrame == audioFrame)
                selected = (int)Mathf.Repeat(selected, 5);
            else if (currentFrame == confirmationFrame)
                selected = (int)Mathf.Repeat(selected, 2);

            if (currentFrame == mainFrame)
            {
                helpText.text = "[Up/Down Arrow]: Navigate\n[Enter]: Select";
            }
            else if (currentFrame == optionsFrame)
            {
                helpText.text = "[Up/Down Arrow]: Navigate\n[Enter]: Select\n[Escape]: Back";
            }
            else if (currentFrame == controlsFrame)
            {
                helpText.text = "[Up/Down Arrow]: Navigate\n[Enter]: Select\n[Escape]: Back";
            }
            else if (currentFrame == p1Frame)
            {
                if (selected == 0)
                    helpText.text = "[Up/Down Arrow]: Navigate\n[Left/Right Arrow]: Change Value\n[Escape]: Back";
                else if (selected > 0 && selected < 5)
                    helpText.text = "[Up/Down Arrow]: Navigate\n[Enter]: Assign New Button\n[Escape]: Back";
                else if (selected == 5)
                    helpText.text = "[Up/Down Arrow]: Navigate\n[Enter]: Select\n[Escape]: Back";
            }
            else if (currentFrame == p2Frame)
            {
                if (selected == 0)
                    helpText.text = "[Up/Down Arrow]: Navigate\n[Left/Right Arrow]: Change Value\n[Escape]: Back";
                else if (selected > 0 && selected < 5)
                    helpText.text = "[Up/Down Arrow]: Navigate\n[Enter]: Assign New Button\n[Escape]: Back";
                else if (selected == 5)
                    helpText.text = "[Up/Down Arrow]: Navigate\n[Enter]: Select\n[Escape]: Back";
            }
            else if (currentFrame == videoFrame)
            {
                if (selected > -1 && selected < 4)
                    helpText.text = "[Up/Down Arrow]: Navigate\n[Left/Right Arrow]: Change Value\n[Escape]: Back";
                else if (selected == 4)
                    helpText.text = "[Up/Down Arrow]: Navigate\n[Enter]: Select\n[Escape]: Back";
            }
            else if (currentFrame == audioFrame)
            {
                if (selected > -1 && selected < 3)
                    helpText.text = "[Up/Down Arrow]: Navigate\n[Left/Right Arrow]: Change Value\n[Escape]: Back";
                else if (selected == 3)
                    helpText.text = "[Up/Down Arrow]: Navigate\n[Enter]: Select\n[Escape]: Back";
            }

            //Make the selected button's text have red colour
            currentFrame.transform.GetChild(selected).GetComponent<Text>().color = Color.red;

            //Make the previously selected button's text have the standard white colour
            currentFrame.transform.GetChild(prevIndex).GetComponent<Text>().color = Color.white;

        }

        if ((Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)) && !keyPromptActive)
        {
            PlaySound("sfx/20 countdown", 0.3f);

            //The index of the currently selected button
            int prevIndex = selected;

            //The index of the button below the currently selected one
            selected++;

            //Keep it inside the current frame's bounds
            if (currentFrame == mainFrame)
                selected = (int)Mathf.Repeat(selected, 3);
            else if (currentFrame == optionsFrame)
                selected = (int)Mathf.Repeat(selected, 4);
            else if (currentFrame == controlsFrame)
                selected = (int)Mathf.Repeat(selected, 3);
            else if (currentFrame == p1Frame)
                selected = (int)Mathf.Repeat(selected, 7);
            else if (currentFrame == p2Frame)
                selected = (int)Mathf.Repeat(selected, 7);
            else if (currentFrame == videoFrame)
                selected = (int)Mathf.Repeat(selected, 6);
            else if (currentFrame == audioFrame)
                selected = (int)Mathf.Repeat(selected, 5);
            else if (currentFrame == confirmationFrame)
                selected = (int)Mathf.Repeat(selected, 2);

            if (currentFrame == mainFrame)
            {
                helpText.text = "[Up/Down Arrow]: Navigate\n[Enter]: Select";
            }
            else if (currentFrame == optionsFrame)
            {
                helpText.text = "[Up/Down Arrow]: Navigate\n[Enter]: Select\n[Escape]: Back";
            }
            else if (currentFrame == controlsFrame)
            {
                helpText.text = "[Up/Down Arrow]: Navigate\n[Enter]: Select\n[Escape]: Back";
            }
            else if (currentFrame == p1Frame)
            {
                if (selected == 0)
                    helpText.text = "[Up/Down Arrow]: Navigate\n[Left/Right Arrow]: Change Value\n[Escape]: Back";
                else if (selected > 0 && selected < 5)
                    helpText.text = "[Up/Down Arrow]: Navigate\n[Enter]: Assign New Button\n[Escape]: Back";
                else if (selected == 5)
                    helpText.text = "[Up/Down Arrow]: Navigate\n[Enter]: Select\n[Escape]: Back";
            }
            else if (currentFrame == p2Frame)
            {
                if (selected == 0)
                    helpText.text = "[Up/Down Arrow]: Navigate\n[Left/Right Arrow]: Change Value\n[Escape]: Back";
                else if (selected > 0 && selected < 5)
                    helpText.text = "[Up/Down Arrow]: Navigate\n[Enter]: Assign New Button\n[Escape]: Back";
                else if (selected == 5)
                    helpText.text = "[Up/Down Arrow]: Navigate\n[Enter]: Select\n[Escape]: Back";
            }
            else if (currentFrame == videoFrame)
            {
                if (selected > -1 && selected < 4)
                    helpText.text = "[Up/Down Arrow]: Navigate\n[Left/Right Arrow]: Change Value\n[Escape]: Back";
                else if (selected == 4)
                    helpText.text = "[Up/Down Arrow]: Navigate\n[Enter]: Select\n[Escape]: Back";
            }
            else if (currentFrame == audioFrame)
            {
                if (selected > -1 && selected < 3)
                    helpText.text = "[Up/Down Arrow]: Navigate\n[Left/Right Arrow]: Change Value\n[Escape]: Back";
                else if (selected == 3)
                    helpText.text = "[Up/Down Arrow]: Navigate\n[Enter]: Select\n[Escape]: Back";
            }

            //Make the selected button's text have red colour
            currentFrame.transform.GetChild(selected).GetComponent<Text>().color = Color.red;

            //Make the previously selected button's text have the standard white colour
            currentFrame.transform.GetChild(prevIndex).GetComponent<Text>().color = Color.white;
        }

        if (Input.GetKeyDown(KeyCode.Return) && !keyPromptActive)
        {
            PlaySound("sfx/12 stake collide", 0.2f);

            if (currentFrame == mainFrame)
            {
                //If the play button is selected
                if (selected == 0)
                {
                    SceneManager.LoadScene("Game", LoadSceneMode.Single);
                }
                //If the options button is selected
                else if (selected == 1)
                {
                    helpText.text = "[Up/Down Arrow]: Navigate\n[Enter]: Select\n[Escape]: Back";
                    ChangeMenuFromTo(mainFrame, optionsFrame);
                }
                //If the exit button is selected
                else if (selected == 2)
                {
                    parentFrame = mainFrame;
                    helpText.text = "[Up/Down Arrow]: Navigate\n[Enter]: Select\n[Escape]: Back";
                    ChangeMenuFromTo(mainFrame, confirmationFrame);
                    //Application.Quit();
                }

            }
            else if (currentFrame == optionsFrame)
            {
                //If the controls button is selected
                if (selected == 0)
                {
                    helpText.text = "[Up/Down Arrow]: Navigate\n[Enter]: Select\n[Escape]: Back";
                    ChangeMenuFromTo(optionsFrame, controlsFrame);
                }
                //If the video button is selected
                else if (selected == 1)
                {
                    helpText.text = "[Up/Down Arrow]: Navigate\n[Left/Right Arrow]: Change Value\n[Escape]: Back";
                    ChangeMenuFromTo(optionsFrame, videoFrame);
                }
                //If the audio button is selected
                else if (selected == 2)
                {
                    helpText.text = "[Up/Down Arrow]: Navigate\n[Left/Right Arrow]: Change Value\n[Escape]: Back";
                    ChangeMenuFromTo(optionsFrame, audioFrame);
                }
                //If the back button is selected
                else if (selected == 3)
                {
                    helpText.text = "[Up/Down Arrow]: Navigate\n[Enter]: Select";
                    ChangeMenuFromTo(optionsFrame, mainFrame);
                }

            }
            else if (currentFrame == controlsFrame)
            {
                //If the p1 button is selected
                if (selected == 0)
                {
                    helpText.text = "[Up/Down Arrow]: Navigate\n[Left/Right Arrow]: Change Value\n[Escape]: Back";
                    ChangeMenuFromTo(controlsFrame, p1Frame);
                }
                //If the p2 button is selected
                else if (selected == 1)
                {
                    helpText.text = "[Up/Down Arrow]: Navigate\n[Left/Right Arrow]: Change Value\n[Escape]: Back";
                    ChangeMenuFromTo(controlsFrame, p2Frame);
                }
                //If the back button is selected
                else if (selected == 2)
                {
                    helpText.text = "[Up/Down Arrow]: Navigate\n[Enter]: Select\n[Escape]: Back";
                    ChangeMenuFromTo(controlsFrame, optionsFrame);
                }

            }
            else if (currentFrame == p1Frame)
            {
                //If brake is selected
                if (selected == 1)
                {
                    prevValue = p1Frame.transform.GetChild(8).GetComponent<Text>().text;
                    p1Frame.transform.GetChild(8).GetComponent<Text>().text = "[]";
                    p1Frame.transform.GetChild(8).GetComponent<Text>().color = Color.red;
                    targetText = p1Frame.transform.GetChild(8).GetComponent<Text>();
                    targetKey = "Save.p1BrakeKey";
                    keyPromptActive = true;
                }
                //If activate is selected
                else if (selected == 2)
                {
                    prevValue = p1Frame.transform.GetChild(9).GetComponent<Text>().text;
                    p1Frame.transform.GetChild(9).GetComponent<Text>().text = "[]";
                    p1Frame.transform.GetChild(9).GetComponent<Text>().color = Color.red;
                    targetText = p1Frame.transform.GetChild(9).GetComponent<Text>();
                    targetKey = "Save.p1ActivateKey";
                    keyPromptActive = true;
                }
                //If look behind is selected
                else if (selected == 3)
                {
                    prevValue = p1Frame.transform.GetChild(10).GetComponent<Text>().text;
                    p1Frame.transform.GetChild(10).GetComponent<Text>().text = "[]";
                    p1Frame.transform.GetChild(10).GetComponent<Text>().color = Color.red;
                    targetText = p1Frame.transform.GetChild(10).GetComponent<Text>();
                    targetKey = "Save.p1LookBehindKey";
                    keyPromptActive = true;
                }
                //If pause game is selected
                else if (selected == 4)
                {
                    prevValue = p1Frame.transform.GetChild(11).GetComponent<Text>().text;
                    p1Frame.transform.GetChild(11).GetComponent<Text>().text = "[]";
                    p1Frame.transform.GetChild(11).GetComponent<Text>().color = Color.red;
                    targetText = p1Frame.transform.GetChild(11).GetComponent<Text>();
                    targetKey = "Save.p1PauseKey";
                    keyPromptActive = true;
                }
                //If restore to defaults button is selected
                else if (selected == 5)
                {
                    parentFrame = p1Frame;
                    parentIndex = 5;
                    helpText.text = "[Up/Down Arrow]: Navigate\n[Enter]: Select\n[Escape]: Back";
                    ChangeMenuFromTo(p1Frame, confirmationFrame);
                }
                //If back button is selected
                else if (selected == 6)
                {
                    helpText.text = "[Up/Down Arrow]: Navigate\n[Enter]: Select\n[Escape]: Back";
                    ChangeMenuFromTo(p1Frame, controlsFrame);
                }

            }
            else if (currentFrame == p2Frame)
            {
                //If brake is selected
                if (selected == 1)
                {
                    prevValue = p2Frame.transform.GetChild(8).GetComponent<Text>().text;
                    p2Frame.transform.GetChild(8).GetComponent<Text>().text = "[]";
                    p2Frame.transform.GetChild(8).GetComponent<Text>().color = Color.red;
                    targetText = p2Frame.transform.GetChild(8).GetComponent<Text>();
                    targetKey = "Save.p2BrakeKey";
                    keyPromptActive = true;
                }
                //If activate is selected
                else if (selected == 2)
                {
                    prevValue = p2Frame.transform.GetChild(9).GetComponent<Text>().text;
                    p2Frame.transform.GetChild(9).GetComponent<Text>().text = "[]";
                    p2Frame.transform.GetChild(9).GetComponent<Text>().color = Color.red;
                    targetText = p2Frame.transform.GetChild(9).GetComponent<Text>();
                    targetKey = "Save.p2ActivateKey";
                    keyPromptActive = true;
                }
                //If look behind is selected
                else if (selected == 3)
                {
                    prevValue = p2Frame.transform.GetChild(10).GetComponent<Text>().text;
                    p2Frame.transform.GetChild(10).GetComponent<Text>().text = "[]";
                    p2Frame.transform.GetChild(10).GetComponent<Text>().color = Color.red;
                    targetText = p2Frame.transform.GetChild(10).GetComponent<Text>();
                    targetKey = "Save.p2LookBehindKey";
                    keyPromptActive = true;
                }
                //If pause game is selected
                else if (selected == 4)
                {
                    prevValue = p2Frame.transform.GetChild(11).GetComponent<Text>().text;
                    p2Frame.transform.GetChild(11).GetComponent<Text>().text = "[]";
                    p2Frame.transform.GetChild(11).GetComponent<Text>().color = Color.red;
                    targetText = p2Frame.transform.GetChild(11).GetComponent<Text>();
                    targetKey = "Save.p2PauseKey";
                    keyPromptActive = true;
                }
                //If restore to defaults button is selected
                else if (selected == 5)
                {
                    parentFrame = p2Frame;
                    parentIndex = 5;
                    helpText.text = "[Up/Down Arrow]: Navigate\n[Enter]: Select\n[Escape]: Back";
                    ChangeMenuFromTo(p2Frame, confirmationFrame);
                }
                //If back button is selected
                else if (selected == 6)
                {
                    helpText.text = "[Up/Down Arrow]: Navigate\n[Enter]: Select\n[Escape]: Back";
                    ChangeMenuFromTo(p2Frame, controlsFrame);
                }

            }
            else if (currentFrame == videoFrame)
            {
                //If restore to defaults button is selected
                if (selected == 4)
                {
                    parentFrame = videoFrame;
                    parentIndex = 4;
                    helpText.text = "[Up/Down Arrow]: Navigate\n[Enter]: Select\n[Escape]: Back";
                    ChangeMenuFromTo(videoFrame, confirmationFrame);
                }
                //If back button is selected
                else if (selected == 5)
                {
                    helpText.text = "[Up/Down Arrow]: Navigate\n[Enter]: Select\n[Escape]: Back";
                    ChangeMenuFromTo(videoFrame, optionsFrame);
                }

            }
            else if (currentFrame == audioFrame)
            {
                //If restore to defaults button is selected
                if (selected == 3)
                {
                    parentFrame = audioFrame;
                    parentIndex = 3;
                    helpText.text = "[Up/Down Arrow]: Navigate\n[Enter]: Select\n[Escape]: Back";
                    ChangeMenuFromTo(audioFrame, confirmationFrame);
                }
                //If back button is selected
                else if (selected == 4)
                {
                    helpText.text = "[Up/Down Arrow]: Navigate\n[Enter]: Select\n[Escape]: Back";
                    ChangeMenuFromTo(audioFrame, optionsFrame);
                }

            }
            else if (currentFrame == confirmationFrame)
            {
                //If yes is selected
                if (selected == 0)
                {
                    //Are you sure you want to exit the game?
                    if (parentFrame == mainFrame)
                    {
                        Application.Quit();
                        Debug.Log("Exited game");
                    }
                    //Restore P1 controls
                    else if (parentFrame == p1Frame)
                    {
                        RestoreP1();
                    }
                    //Restore P2 controls
                    else if (parentFrame == p2Frame)
                    {
                        RestoreP2();
                    }
                    //Restore video
                    else if (parentFrame == videoFrame)
                    {
                        RestoreVideo();
                    }
                    //Restore audio
                    else if (parentFrame == audioFrame)
                    {
                        RestoreAudio();

                    }

                }
                //If no is selected, or yes is selected but already executed, then return to the parent frame
                if (parentFrame == mainFrame)
                    parentIndex = 2;
                helpText.text = "[Up/Down Arrow]: Navigate\n[Enter]: Select\n[Escape]: Back";
                ChangeMenuFromTo(confirmationFrame, parentFrame, parentIndex);

            }


        }

        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            PlaySound("sfx/11 stake shoot", 0.3f);

            //Controller type
            if (currentFrame == p1Frame && selected == 0)
            {
                Save.p1ControllerIsKeyboard = !Save.p1ControllerIsKeyboard;

                if (Save.p1ControllerIsKeyboard)
                    ResetP1KeyboardControls();
                else
                    ResetP1JoystickControls();

                UpdateSettingsDisplay();

                Save.SaveFile();
            }
            //Controller type
            else if (currentFrame == p2Frame && selected == 0)
            {
                Save.p2ControllerIsKeyboard = !Save.p2ControllerIsKeyboard;

                if (Save.p2ControllerIsKeyboard)
                    ResetP2KeyboardControls();
                else
                    ResetP2JoystickControls();

                UpdateSettingsDisplay();

                Save.SaveFile();
            }
            //Video menu
            else if (currentFrame == videoFrame)
            {
                //Brightness
                if (selected == 0)
                {
                    Save.brightness--;
                    Save.brightness = Mathf.Clamp(Save.brightness, 0, 10);
                    UpdateSettingsDisplay();
                    Save.SaveFile();
                }
                //Track segments
                else if (selected == 1)
                {
                    Save.trackSegs--;
                    Save.trackSegs = Mathf.Clamp(Save.trackSegs, 3, 30);
                    UpdateSettingsDisplay();
                    Save.SaveFile();
                }
                //Background segments
                else if (selected == 2)
                {
                    Save.bgSegs--;
                    Save.bgSegs = Mathf.Clamp(Save.bgSegs, 2, 8);
                    UpdateSettingsDisplay();
                    Save.SaveFile();
                }
                //Doodads
                else if (selected == 3)
                {
                    Save.doodads = !Save.doodads;

                    UpdateSettingsDisplay();

                    Save.SaveFile();
                }

            }
            //Audio menu
            else if (currentFrame == audioFrame)
            {
                //Music
                if (selected == 0)
                {
                    Save.musicVol -= 1f;
                    Save.musicVol = Mathf.Clamp(Save.musicVol, 0f, 10f);
                    UpdateSettingsDisplay();
                    Save.SaveFile();
                }
                //Ambience
                else if (selected == 1)
                {
                    Save.ambienceVol -= 1f;
                    Save.ambienceVol = Mathf.Clamp(Save.ambienceVol, 0f, 10f);
                    UpdateSettingsDisplay();
                    Save.SaveFile();
                }
                //SFX
                else if (selected == 2)
                {
                    Save.sfxVol -= 1f;
                    Save.sfxVol = Mathf.Clamp(Save.sfxVol, 0f, 10f);
                    UpdateSettingsDisplay();
                    Save.SaveFile();
                }

            }

        }

        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            PlaySound("sfx/11 stake shoot", 0.3f);

            //Controller type
            if (currentFrame == p1Frame && selected == 0)
            {
                Save.p1ControllerIsKeyboard = !Save.p1ControllerIsKeyboard;

                if (Save.p1ControllerIsKeyboard)
                    ResetP1KeyboardControls();
                else
                    ResetP1JoystickControls();

                UpdateSettingsDisplay();

                Save.SaveFile();
            }
            //Controller type
            else if (currentFrame == p2Frame && selected == 0)
            {
                Save.p2ControllerIsKeyboard = !Save.p2ControllerIsKeyboard;

                if (Save.p2ControllerIsKeyboard)
                    ResetP2KeyboardControls();
                else
                    ResetP2JoystickControls();

                UpdateSettingsDisplay();

                Save.SaveFile();
            }
            //Video menu
            else if (currentFrame == videoFrame)
            {
                //Brightness
                if (selected == 0)
                {
                    Save.brightness++;
                    Save.brightness = Mathf.Clamp(Save.brightness, 0, 10);
                    UpdateSettingsDisplay();
                    Save.SaveFile();
                }
                //Track segments
                else if (selected == 1)
                {
                    Save.trackSegs++;
                    Save.trackSegs = Mathf.Clamp(Save.trackSegs, 3, 30);
                    UpdateSettingsDisplay();
                    Save.SaveFile();
                }
                //Background segments
                else if (selected == 2)
                {
                    Save.bgSegs++;
                    Save.bgSegs = Mathf.Clamp(Save.bgSegs, 2, 8);
                    UpdateSettingsDisplay();
                    Save.SaveFile();
                }
                //Doodads
                else if (selected == 3)
                {
                    Save.doodads = !Save.doodads;

                    UpdateSettingsDisplay();

                    Save.SaveFile();
                }

            }
            //Audio menu
            else if (currentFrame == audioFrame)
            {
                //Music
                if (selected == 0)
                {
                    Save.musicVol += 1f;
                    Save.musicVol = Mathf.Clamp(Save.musicVol, 0f, 10f);
                    UpdateSettingsDisplay();
                    Save.SaveFile();
                }
                //Ambience
                else if (selected == 1)
                {
                    Save.ambienceVol += 1f;
                    Save.ambienceVol = Mathf.Clamp(Save.ambienceVol, 0f, 10f);
                    UpdateSettingsDisplay();
                    Save.SaveFile();
                }
                //SFX
                else if (selected == 2)
                {
                    Save.sfxVol += 1f;
                    Save.sfxVol = Mathf.Clamp(Save.sfxVol, 0f, 10f);
                    UpdateSettingsDisplay();
                    Save.SaveFile();
                }

            }

        }


    }

    void ChangeMenuFromTo(GameObject oldMenu, GameObject newMenu)
    {
        if (oldMenu == mainFrame)
            TitleVid.transform.GetChild(0).GetComponent<MeshRenderer>().material.color = new Color(1f, 1f, 1f, 0f);
        else if (newMenu == mainFrame)
            TitleVid.transform.GetChild(0).GetComponent<MeshRenderer>().material.color = new Color(1f, 1f, 1f, 1f);

        oldMenu.transform.GetChild(selected).GetComponent<Text>().color = Color.white;

        currentFrame = newMenu;
        oldMenu.SetActive(false);
        newMenu.SetActive(true);

        selected = 0;
        //Make the selected button's text have red colour
        newMenu.transform.GetChild(selected).GetComponent<Text>().color = Color.red;
    }

    void ChangeMenuFromTo(GameObject oldMenu, GameObject newMenu, int selectedIndex)
    {
        oldMenu.transform.GetChild(selected).GetComponent<Text>().color = Color.white;

        currentFrame = newMenu;
        oldMenu.SetActive(false);
        newMenu.SetActive(true);

        selected = selectedIndex;
        //Make the selected button's text have red colour
        newMenu.transform.GetChild(selected).GetComponent<Text>().color = Color.red;
    }

    void RestoreP1()
    {
        Save.p1ControllerIsKeyboard = true;

        ResetP1KeyboardControls();

        Save.SaveFile();
        UpdateSettingsDisplay();
    }

    void RestoreP2()
    {
        Save.p2ControllerIsKeyboard = true;

        ResetP2KeyboardControls();

        Save.SaveFile();
        UpdateSettingsDisplay();
    }

    void RestoreVideo()
    {
        Save.brightness = 5;
        Save.trackSegs = 30;
        Save.bgSegs = 8;
        Save.doodads = true;

        Save.SaveFile();
        UpdateSettingsDisplay();
    }

    void RestoreAudio()
    {
        Save.musicVol = 5f;
        Save.ambienceVol = 5f;
        Save.sfxVol = 5f;

        Save.SaveFile();
        UpdateSettingsDisplay();
    }

    void UpdateSettingsDisplay()
    {
        if (Save.p1ControllerIsKeyboard)
            p1Frame.transform.GetChild(7).GetComponent<Text>().text = "< Keyboard >";
        else
            p1Frame.transform.GetChild(7).GetComponent<Text>().text = "< Joystick >";

        p1Frame.transform.GetChild(8).GetComponent<Text>().text = "[" + Save.p1BrakeKey + "]";
        p1Frame.transform.GetChild(9).GetComponent<Text>().text = "[" + Save.p1ActivateKey + "]";
        p1Frame.transform.GetChild(10).GetComponent<Text>().text = "[" + Save.p1LookBehindKey + "]";
        p1Frame.transform.GetChild(11).GetComponent<Text>().text = "[" + Save.p1PauseKey + "]";

        if (Save.p2ControllerIsKeyboard)
            p2Frame.transform.GetChild(7).GetComponent<Text>().text = "< Keyboard >";
        else
            p2Frame.transform.GetChild(7).GetComponent<Text>().text = "< Joystick >";

        p2Frame.transform.GetChild(8).GetComponent<Text>().text = "[" + Save.p2BrakeKey + "]";
        p2Frame.transform.GetChild(9).GetComponent<Text>().text = "[" + Save.p2ActivateKey + "]";
        p2Frame.transform.GetChild(10).GetComponent<Text>().text = "[" + Save.p2LookBehindKey + "]";
        p2Frame.transform.GetChild(11).GetComponent<Text>().text = "[" + Save.p2PauseKey + "]";

        videoFrame.transform.GetChild(6).GetComponent<Text>().text = "- " + Save.brightness + " +";
        videoFrame.transform.GetChild(7).GetComponent<Text>().text = "- " + Save.trackSegs + " +";
        videoFrame.transform.GetChild(8).GetComponent<Text>().text = "- " + Save.bgSegs + " +";

        if (Save.doodads)
            videoFrame.transform.GetChild(9).GetComponent<Text>().text = "< On >";
        else
            videoFrame.transform.GetChild(9).GetComponent<Text>().text = "< Off >";

        audioFrame.transform.GetChild(5).GetComponent<Text>().text = "- " + Save.musicVol + " +";
        audioFrame.transform.GetChild(6).GetComponent<Text>().text = "- " + Save.ambienceVol + " +";
        audioFrame.transform.GetChild(7).GetComponent<Text>().text = "- " + Save.sfxVol + " +";

    }

    void ResetP1KeyboardControls()
    {
        Save.p1BrakeKey = KeyCode.Space;
        Save.p1ActivateKey = KeyCode.E;
        Save.p1LookBehindKey = KeyCode.Q;
        Save.p1PauseKey = KeyCode.Escape;
    }

    void ResetP1JoystickControls()
    {
        Save.p1BrakeKey = KeyCode.JoystickButton0;
        Save.p1ActivateKey = KeyCode.JoystickButton5;
        Save.p1LookBehindKey = KeyCode.JoystickButton4;
        Save.p1PauseKey = KeyCode.JoystickButton7;
    }

    void ResetP2KeyboardControls()
    {
        Save.p2BrakeKey = KeyCode.Keypad2;
        Save.p2ActivateKey = KeyCode.Keypad1;
        Save.p2LookBehindKey = KeyCode.Keypad3;
        Save.p2PauseKey = KeyCode.Escape;
    }

    void ResetP2JoystickControls()
    {
        Save.p2BrakeKey = KeyCode.JoystickButton0;
        Save.p2ActivateKey = KeyCode.JoystickButton5;
        Save.p2LookBehindKey = KeyCode.JoystickButton4;
        Save.p2PauseKey = KeyCode.JoystickButton7;
    }

    void PlaySound(string title, float volume)
    {
        AudioClip clip = Resources.Load(title, typeof(AudioClip)) as AudioClip;
        GetComponent<AudioSource>().PlayOneShot(clip, volume * Save.sfxVol / 10f);

    }


}

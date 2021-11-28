using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{

    //The index of the selected button
    private int selected = 0;

    void Awake()
    {
        // Make the game run as fast as possible in Windows
        Application.targetFrameRate = 300;
    }

    void Start()
    {
        //Make the selected button's text have red colour
        transform.GetChild(selected).GetComponent<Text>().color = Color.red;
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            //The index of the currently selected button
            int prevIndex = selected;

            //The index of the button above the currently selected one
            selected--;

            //Keep it inside 0-2
            selected = (int)Mathf.Repeat(selected, 3);

            //Make the selected button's text have red colour
            transform.GetChild(selected).GetComponent<Text>().color = Color.red;

            //Make the previously selected button's text have the standard white colour
            transform.GetChild(prevIndex).GetComponent<Text>().color = Color.white;
        }

        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            //The index of the currently selected button
            int prevIndex = selected;

            //The index of the button below the currently selected one
            selected++;

            //Keep it inside 0-2
            selected = (int)Mathf.Repeat(selected, 3);

            //Make the selected button's text have red colour
            transform.GetChild(selected).GetComponent<Text>().color = Color.red;

            //Make the previously selected button's text have the standard white colour
            transform.GetChild(prevIndex).GetComponent<Text>().color = Color.white;
        }

        if (Input.GetKeyDown(KeyCode.Return) )
        {
            //If the play button is selected
            if (selected == 0)
            {
                SceneManager.LoadScene("Game", LoadSceneMode.Single);
            }
            //If the options button is selected
            else if (selected == 1)
            { 
            }
            //If the exit button is selected
            else if (selected == 2)
            {
                Application.Quit();
            }

        }


    }
}

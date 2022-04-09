using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainCanvasScript : MonoBehaviour
{
    public static bool gamePaused;

    //The index of the selected button
    private int selected = 0;

    void Awake()
    {
        //Change every text in the scene onto the chosen font
        foreach (Text item in Resources.FindObjectsOfTypeAll(typeof(Text)) as Text[])
        {
            item.font = Resources.Load("Phantom Fingers", typeof(Font)) as Font;
            item.fontSize /= 2;
        }
    }

    void Start()
    {
        gamePaused = false;

        //We initially set all texts and the backdrop to not active
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);

            //If the child is a text
            if (transform.GetChild(i).gameObject.GetComponent<Text>() != null)
                //set its initial colour to white
                transform.GetChild(i).gameObject.GetComponent<Text>().color = Color.white;
        }

        //Make the selected button's text have red colour
        //+1 because the first child is the backdrop
        transform.GetChild(selected + 1).GetComponent<Text>().color = Color.red;

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!gamePaused)
            {
                gamePaused = true;
                Time.timeScale = 0f;

                for (int i = 0; i < transform.childCount; i++)
                {
                    transform.GetChild(i).gameObject.SetActive(true);
                }

            }
            else
            {
                ResumeGame();
            }

        }

        if (gamePaused)
        {

            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
            {
                //The index of the currently selected button
                int prevIndex = selected;

                //The index of the button above the currently selected one
                selected--;

                //Keep it inside 1-3
                selected = (int)Mathf.Repeat(selected, 2);

                //Make the selected button's text have red colour
                transform.GetChild(selected + 1).GetComponent<Text>().color = Color.red;

                //Make the previously selected button's text have the standard white colour
                transform.GetChild(prevIndex + 1).GetComponent<Text>().color = Color.white;
            }

            if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
            {
                //The index of the currently selected button
                int prevIndex = selected;

                //The index of the button below the currently selected one
                selected++;

                //Keep it inside 0-2
                selected = (int)Mathf.Repeat(selected, 2);

                //Make the selected button's text have red colour
                transform.GetChild(selected + 1).GetComponent<Text>().color = Color.red;

                //Make the previously selected button's text have the standard white colour
                transform.GetChild(prevIndex + 1).GetComponent<Text>().color = Color.white;
            }

            if (Input.GetKeyDown(KeyCode.Return))
            {
                //If the resume button is selected
                if (selected == 0)
                {
                    ResumeGame();
                }
                //If the quit button is selected
                else if (selected == 1)
                {
                    ResumeGame();
                    SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
                }

            }

        }

    }

    void ResumeGame()
    {
        //set selected back to 0 so that next time the player pauses, the resume button will be selected
        selected = 0;

        for (int i = 1; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.GetComponent<Text>().color = Color.white;
        }

        transform.GetChild(selected + 1).GetComponent<Text>().color = Color.red;


        gamePaused = false;
        Time.timeScale = 1f;

        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }

        gamePaused = false;

    }

}

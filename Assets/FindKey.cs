using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindKey : MonoBehaviour
{
    private readonly Array keyCodes = Enum.GetValues(typeof(KeyCode));

    //Declare the ListGO GameObject, which has the FindGO script attached to it
    public GameObject ListGO;
    public static FindGO FindGO;

    public static string P1joyNo;
    public static string P2joyNo;

    private Array GetKeyCodes()
    {
        return keyCodes;
    }

    private void Awake()
    {
        //Get the script as the component of the ListGO GameObject
        FindGO = ListGO.GetComponent<FindGO>();
    }

    private void Start()
    {
        P1joyNo = P2joyNo = null;

    }

    void Update()
    {

        string output = null;
        string joyNo = null;

        if (Input.anyKeyDown)
        {
            foreach (KeyCode keyCode in GetKeyCodes())
            {
                if (Input.GetKeyDown(keyCode))
                {
                    output = keyCode.ToString();
                    //If the pressed button is from the Joystick, then
                    //First it will grab JoystickButtonX
                    //Then overwrite it into JoystickXButtonX
                }
            }

            //If it is a Joystick button
            if (output.Contains("Joystick"))
            {
                //If it is a double digit number
                joyNo = output.Substring(8, 2);
                //If it is a single digit number
                if (joyNo.EndsWith("B"))
                    joyNo = joyNo.Substring(0, 1);

                //The joysick number needs to be stored because it will determine which axis we need to use for which player
                //We can't take input from all joysticks for one player, because both players might use a joystick

                string butNo = output.Substring(output.Length - 2, 2);
                if (butNo.StartsWith("n"))
                    butNo = butNo.Substring(1, 1);

                //The name of the button without the Joystick's number
                output = "JoystickButton" + butNo;

            }

            //If it's P1's turn to press their button
            if (P1joyNo == null)
            {
                //If P1's activate button is pressed
                if (output == Save.p1ActivateKey.ToString())
                {

                    if (joyNo != null)
                        P1joyNo = joyNo;
                    else
                        P1joyNo = "keyboard";

                    //If an activate button has already been pressed, then
                    //We should make sure the activate button which is being pressed for the second time, is not from a joystick with the same number as the first one
                }

            }
            //If it's P2's turn to press their button
            if (P1joyNo != null && P2joyNo == null)
            {
                //If P2's activate button is pressed and it is not on the same joystick as P1's activate button
                if (output == Save.p2ActivateKey.ToString() && joyNo != P1joyNo)
                {

                    if (joyNo != null)
                        P2joyNo = joyNo;
                    else
                        P2joyNo = "keyboard";

                    AssignAxes();
                }

            }

        }

    }

    void AssignAxes()
    {

        if (P1joyNo != "keyboard")
        {
            FindGO.P1Avatar.GetComponent<KartControl>().VerticalAxisName = "Joystick" + P1joyNo.ToString() + "Vertical";
            FindGO.P1Avatar.GetComponent<KartControl>().HorizontalAxisName = "Joystick" + P1joyNo.ToString() + "Horizontal";
        }
        else
        {
            FindGO.P1Avatar.GetComponent<KartControl>().VerticalAxisName = "P1Vertical";
            FindGO.P1Avatar.GetComponent<KartControl>().HorizontalAxisName = "P1Horizontal";
        }

        if (P2joyNo != "keyboard")
        {
            FindGO.P2Avatar.GetComponent<KartControl>().VerticalAxisName = "Joystick" + P2joyNo.ToString() + "Vertical";
            FindGO.P2Avatar.GetComponent<KartControl>().HorizontalAxisName = "Joystick" + P2joyNo.ToString() + "Horizontal";
        }
        else
        {
            FindGO.P2Avatar.GetComponent<KartControl>().VerticalAxisName = "P2Vertical";
            FindGO.P2Avatar.GetComponent<KartControl>().HorizontalAxisName = "P2Horizontal";
        }

    }

}

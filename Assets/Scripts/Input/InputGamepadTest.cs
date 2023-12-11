using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InputGamepadTest : MonoBehaviour
{

    public TextMeshPro joysticks;
    public TextMeshPro[] inputText;
    public TextMeshPro[] buttonText;

    public int numSticks;

    void Start()
    {
        int i = 0;

        string sticks = "Joysticks\n";

        foreach (string joyName in Input.GetJoystickNames())
        {
            sticks += i.ToString() + ":" + joyName + "\n";
            i++;
        }

        joysticks.text = sticks;

        numSticks = i;
    }

    /*
     * Read all axis of joystick inputs and display them for configuration purposes
     * Requires the following input managers
     *      Joy[N] Axis 1-9
     *      Joy[N] Button 0-20
     */
    void Update()
    {
        for (int i = 1; i <= numSticks; i++)
        {
            string inputs = "Sticks Joystick " + i + "\n";

            string stick = "Joy " + i + " Axis ";

            for (int a = 1; a <= 28; a++)
            {
                Debug.Log(stick + a);
                inputs += "Axis " + a + ":" + Input.GetAxis(stick + a).ToString("0.00") + "\n";
            }

            inputText[i - 1].text = inputs;

            string buttonsInput = "Buttons Joystick " + i + "\n";

            string button = "Joy " + i + " Button ";

            for (int b = 0; b <= 19; b++)
            {
                buttonsInput += "Btn " + b + ":" + Input.GetButton(button + b) + "\n";
            }

            buttonText[i - 1].text = buttonsInput;
        }
    }
}

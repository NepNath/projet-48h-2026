using UnityEngine;
using UnityEngine.UI;

public class cable_game : MonoBehaviour
{
    bool[] buttonsClicked = new bool[8];
    Button[] buttons = new Button[8];
    bool[] buttonsDone = new bool[4];
    GameObject victoryText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        getButtonsAndCanvas();
        checkClickedButtons();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void getButtonsAndCanvas()
    {
        buttons[0] = GameObject.Find("ButtonGrid/RedButton1").GetComponent<Button>();
        buttons[1] = GameObject.Find("ButtonGrid/RedButton2").GetComponent<Button>();
        buttons[2] = GameObject.Find("ButtonGrid/BlueButton1").GetComponent<Button>();
        buttons[3] = GameObject.Find("ButtonGrid/BlueButton2").GetComponent<Button>();
        buttons[4] = GameObject.Find("ButtonGrid/GreenButton1").GetComponent<Button>();
        buttons[5] = GameObject.Find("ButtonGrid/GreenButton2").GetComponent<Button>();
        buttons[6] = GameObject.Find("ButtonGrid/YellowButton1").GetComponent<Button>();
        buttons[7] = GameObject.Find("ButtonGrid/YellowButton2").GetComponent<Button>();
        victoryText = GameObject.Find("StateGrid/VictoryText");
        victoryText.SetActive(false);
    }

    public void checkClickedButtons()
    {
        for (int i = 0; i <= buttons.Length - 1; i++)
        {
            int index = i;
            buttons[index].onClick.AddListener(() => onButtonClicked(index));
        }
    }

    void onButtonClicked(int index)
    {
        int pairIndex = index / 2;
        int firstButton = pairIndex * 2;
        int secondButton = pairIndex * 2 + 1;
        Debug.Log("Bouton cliqué : " + index);
        for (int i = 0; i < buttonsClicked.Length; i++)
        {
            if (i != firstButton && i != secondButton)
            {
                buttonsClicked[i] = false;
            }
        }
        buttonsClicked[index] = true;
        checkButtons();
    }

    void checkButtons()
    {   
        for (int i = 0; i < buttonsClicked.Length; i += 2)
        {
            int index = i;
            if (buttonsClicked[index] && buttonsClicked[index + 1] && !buttonsDone[index / 2])
            {
                buttonsDone[index/2] = true;
                Debug.Log("Odd" + index/2 + "Done!");
                buttons[index].gameObject.SetActive(false);
                buttons[index + 1].gameObject.SetActive(false);
                buttons[index].interactable = false;
                buttons[index + 1].interactable = false;
                checkVictory();
            }
        }
    }

    void checkVictory()
    {
        if (buttonsDone[0] == true && buttonsDone[1] == true && buttonsDone[2] == true && buttonsDone[3] == true)
        {
            Debug.Log("Victory!");
            victoryText.SetActive(true);
        }
    }
}
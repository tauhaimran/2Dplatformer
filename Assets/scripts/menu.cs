using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class menu : MonoBehaviour
{
    [Header(">> LEVEL-BTN SETTINGS")]
    public Button[] levelButtons;           // Assign all level buttons
    public TMP_Text[] levelLockTexts;       // Text shown when locked
    public int unlockedLevels = 1;          // Default: Only Level 1 unlocked

    [Header(">> PLAYER CHOICES")]
    public int chosenCharacter = 0;         // Default: First character
    public TMP_Text[] choiceStatusTexts;     // Text to show chosen character name
    public int[] choiceStatus = { 1, -1, -1 }; // 1 = chosen, 0 = not chosen, -1 = locked
    public Button[] CharacterSelectButtons;
    public int currentcharacterIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        SetupLevels();
        SetupPlayers();
    }

    // Update is called once per frame
    void Update()
    {

    }

    //function to setup levels
    public void SetupLevels()
    {
        for (int i = 0; i < levelLockTexts.Length; i++)
        {
            // put the minus 1 because unlockedLevels is 1 based (1,2,3..) and array is 0 based (0,1,2..)
            if (i < unlockedLevels - 1) // checks for locked level text 2-4 , basically 0-3 in array
            {
                levelButtons[i + 1].interactable = true;
                levelLockTexts[i].gameObject.SetActive(false);
            }
            else
            {
                levelButtons[i + 1].interactable = false;
                levelLockTexts[i].gameObject.SetActive(true);
            }
        }


    }

    //function to setup player choices
    public void SetupPlayers()
    {
        for (int i = 0; i < choiceStatusTexts.Length; i++)
        {
            if (choiceStatus[i] == 1) // chosen
            {
                choiceStatusTexts[i].text = "Chosen";
            }
            else if (choiceStatus[i] == 0) // not chosen
            {
                choiceStatusTexts[i].text = "Available";
            }
            else if (choiceStatus[i] == -1) // locked
            {
                choiceStatusTexts[i].text = "unlock at Level " + (i + 2); // assuming first character is always unlocked
            }
        }
    }

    public void OnCharacterSelect(int index)
{
    // Ignore clicks on locked characters
    if (choiceStatus[index] == -1)
        return;

    // Mark all as available (0)
    for (int i = 0; i < choiceStatus.Length; i++)
    {
        if (choiceStatus[i] != -1) // don’t change locked ones
            choiceStatus[i] = 0;
    }

    // Mark the selected one as chosen
    choiceStatus[index] = 1;
    chosenCharacter = index;
    currentcharacterIndex = index;

    // Update UI text and colors
    for (int i = 0; i < choiceStatusTexts.Length; i++)
    {
        if (choiceStatus[i] == 1)
        {
            choiceStatusTexts[i].text = "Chosen";
            choiceStatusTexts[i].color = Color.red;
        }
        else if (choiceStatus[i] == 0)
        {
            choiceStatusTexts[i].text = "Available";
            choiceStatusTexts[i].color = Color.white;
        }
        else if (choiceStatus[i] == -1)
        {
            choiceStatusTexts[i].text = "Unlock at Level " + (i + 2);
            choiceStatusTexts[i].color = Color.white;
        }
    }

    Debug.Log("Character " + (index + 1) + " selected!");
}


}

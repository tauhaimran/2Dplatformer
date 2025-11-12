using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerManager : MonoBehaviour
{
    public static playerManager Instance;
    public SpriteRenderer spriteRenderer; // for color tinting

    [Header("Player Data")]
    public int coins = 0;
    public int selectedCharacter = 0; // same index as menu.chosenCharacter
    public int unlockedLevels = 1;

    private void Awake()
    {
        // --- Make this persistent between scenes ---
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
            LoadData();
        }
        else
        {
            Destroy(gameObject); // avoid duplicates
        }

        string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        
        if (currentScene != "MainMenu")
        {
            
                spriteRenderer = GetComponent<SpriteRenderer>();
            int selectedCharacter = PlayerPrefs.GetInt("SelectedCharacter");
            // Apply tint based on selection
            if (selectedCharacter == 0)
            {
                spriteRenderer.color = Color.white; // no tint
            }
            else if (selectedCharacter == 1)
            {
                spriteRenderer.color = Color.green; // green tint
            }
            else if (selectedCharacter == 2)
            {
                spriteRenderer.color = Color.red; // red tint
            }
            else
            {
                spriteRenderer.color = Color.white; // default fallback
            }

        }
        
    }

    // --- Called by main menu or gameplay when coins or character change ---
    public void SetSelectedCharacter(int index)
    {
        selectedCharacter = index;
        SaveData();
    }

    public void AddCoins(int amount)
    {
        coins += amount;
        SaveData();
    }

    public void SpendCoins(int amount)
    {
        coins = Mathf.Max(0, coins - amount);
        SaveData();
    }

    public void UnlockNextLevel()
    {
        unlockedLevels++;
        SaveData();
    }

    // --- Save and Load using PlayerPrefs ---
    public void SaveData()
    {
        PlayerPrefs.SetInt("Coins", coins);
        PlayerPrefs.SetInt("SelectedCharacter", selectedCharacter);
        PlayerPrefs.SetInt("UnlockedLevels", unlockedLevels);
        PlayerPrefs.Save();
    }

    public void LoadData()
    {
        coins = PlayerPrefs.GetInt("Coins", 0);
        selectedCharacter = PlayerPrefs.GetInt("SelectedCharacter", 0);
        unlockedLevels = PlayerPrefs.GetInt("UnlockedLevels", 1);
    }

    // optional reset method
    public void ResetData()
    {
        coins = 0;
        selectedCharacter = 0;
        unlockedLevels = 1;
        PlayerPrefs.DeleteAll();
    }

    public void OnApplicationQuit()
    {
        SaveData();
    }

    public void LoadScene(string sceneName)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }
}

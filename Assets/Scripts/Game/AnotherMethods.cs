using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AnotherMethods : MonoBehaviour
{
    [SerializeField] private AudioSource[] sounds;
    [SerializeField] private GameData gameData;
    [SerializeField] private Slider deltaTimeSlider;
    [SerializeField] private GameObject pauseMenu;

    private void Awake()
    {
        //for fill data
        pauseMenu.SetActive(true);
        for (int i = 0; i < gameData.buttons.Length; i++)
        {
            GameData.NewButton button = gameData.buttons[i];
            button.buttonText = GameObject.Find(button.shortName).GetComponentsInChildren<TextMeshProUGUI>()[0];
            button.buttonText.text = button.keyCode.ToString();
        }
        pauseMenu.SetActive(false);
        for (int i = 0; i < sounds.Length; i++)
        {
            if (i == 0)
                sounds[i].volume = gameData.soundVolume / 2;
            else
                sounds[i].volume = gameData.soundVolume;
        }
    }
    private void Update()
    {
        Time.timeScale = deltaTimeSlider.value;
        if (Input.GetKeyDown(gameData.GetButton("Pause").keyCode) || Input.GetKeyDown(KeyCode.Escape))
        {
            ChangeActivePauseMenu();
        }
    }
    public void ChangeActivePauseMenu()
    {
        pauseMenu.SetActive(!pauseMenu.activeInHierarchy);
    }
    public void Exit()
    {
        SceneManager.LoadScene("LoadScene");
    }
    public static void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AnotherMethods : MonoBehaviour
{
    [SerializeField] private GameData gameData;
    [SerializeField] private Slider deltaTimeSlider;
    [SerializeField] private GameObject pauseMenu;

    private void Awake()
    {
        //for data
        pauseMenu.SetActive(true);        
        for (int i = 0; i < gameData.buttons.Length; i++)
        {
            GameData.NewButton button = gameData.buttons[i];
            button.buttonText = GameObject.Find(button.shortName).GetComponentsInChildren<TextMeshProUGUI>()[0];
            button.buttonText.text = button.keyCode.ToString();
        }
        pauseMenu.SetActive(false);
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

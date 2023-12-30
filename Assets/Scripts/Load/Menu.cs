using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public Slider loading;
    public GameObject MenuObj;

    public bool optionsActive = false;
    public GameObject optionsObj;

    public void PlayButton()
    {
        MenuObj.SetActive(false);
        loading.gameObject.SetActive(true);
        StartCoroutine(AsyncLoad());
    }
    private IEnumerator AsyncLoad()
    {
        AsyncOperation load = SceneManager.LoadSceneAsync("Game");
        while (!load.isDone)
        {
            loading.value = load.progress;
            if (load.progress >= 0.9f)
            {
                loading.value = 1;
            }
            yield return null;
        }
    }
    public void OptionsButton()
    {
        MenuObj.SetActive(optionsActive);
        optionsActive = !optionsActive;
        optionsObj.SetActive(optionsActive);
    }
}

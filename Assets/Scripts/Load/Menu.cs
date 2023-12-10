using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public Slider loading;
    public void PlayButton()
    {
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
}

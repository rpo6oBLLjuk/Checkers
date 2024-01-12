using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    [SerializeField] private Slider loading;
    [SerializeField] private GameObject MenuObj;
    [SerializeField] private TextMeshProUGUI volumeText;

    [SerializeField] private GameObject optionsObj;

    [SerializeField] private AudioSource[] sounds;
    [SerializeField] private Notice notice;

    [Header("Data")]
    [SerializeField] private GameData gameData;
    [SerializeField] private NoticeData noticeData;

    private Coroutine coroutine;

    private void Awake()
    {
        optionsObj.SetActive(true);
        for (int i = 0; i < gameData.options.Length; i++)
        {
            GameData.Option option = gameData.options[i];
            option.rTransform = GameObject.Find(option.shortName).GetComponent<RectTransform>();
            Vector3 scale = option.rTransform.localScale;
            if (option.value)
                scale = new Vector3(-Mathf.Abs(scale.x), scale.y, scale.z);
            else
                scale = new Vector3(Mathf.Abs(scale.x), scale.y, scale.z);
            option.rTransform.localScale = scale;
        }
        Slider _volumeSlider = GameObject.Find("VolumeSlider").GetComponent<Slider>();
        _volumeSlider.value = (gameData.soundVolume) * 100;
        ChangeSliderValue(gameData.soundVolume * 100);
        foreach (AudioSource audioSourse in sounds)
            audioSourse.volume = 0;
        optionsObj.SetActive(false);
        CrutchForMute();
    }

    private void Start()
    {
        NoticeData.NoticeType noticeType = noticeData.GetNotice("Options");
        notice.CreateNotice(noticeType);
    }
    public void PlayButton()
    {
        PlayButtonSound();
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
    public void ChangeSliderValue(float value)
    {
        volumeText.text = "Volume: " + value;
        float volume = value / 100;
        for (int i = 0; i < sounds.Length; i++)
        {
            if (i != 0)
                sounds[i].volume = volume;
            else
                sounds[i].volume = volume / 2;
        }
        gameData.soundVolume = volume;
    }
    public void OptionsButton()
    {
        PlayButtonSound();
        MenuObj.SetActive(!MenuObj.activeInHierarchy);
        optionsObj.SetActive(!optionsObj.activeInHierarchy);
    }

    public void ButtonMarker(string name)
    {
        PlayButtonSound();
        GameData.Option option = gameData.GetOption(name);
        option.value = !option.value;
        Vector3 scale = option.rTransform.localScale;
        scale = new Vector3(-scale.x, scale.y, scale.z);
        option.rTransform.localScale = scale;
    }
    public void OpenLink(string link)
    {
        Application.OpenURL(link);
    }

    //Music Scripts
    public void CrutchForMute()
    {
        bool mark = gameData.GetOption("Music").value;
        if (mark)
        {
            if (coroutine != null)
                StopCoroutine(coroutine);
            coroutine = StartCoroutine(MutingSouns(mark, -1));
        }

        else
        {
            if (coroutine != null)
                StopCoroutine(coroutine);
            coroutine = StartCoroutine(MutingSouns(mark, 1));
        }
    }
    private void PlayButtonSound()
    {
        sounds[1].PlayOneShot(sounds[1].clip);
    }
    private IEnumerator MutingSouns(bool mark, int direction)
    {
        float count = 0;
        if (!mark)
        {
            if (!sounds[0].isPlaying)
                sounds[0].Play();
            for (int i = 0; i < sounds.Length; i++)
            {
                sounds[i].mute = mark;
            }
        }
        while (count < gameData.soundVolume)
        {
            for (int i = 0; i < sounds.Length; i++)
            {
                if (i == 0)
                    sounds[i].volume += Time.unscaledDeltaTime * direction / 2;
                else
                    sounds[i].volume += Time.unscaledDeltaTime * direction;
            }
            count += Time.unscaledDeltaTime;
            yield return new WaitForEndOfFrame();
        }
        if (mark)
        {
            foreach (AudioSource audioSource in sounds)
            {
                audioSource.mute = mark;
                audioSource.Pause();
            }
        }
    }
}

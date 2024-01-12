using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    [SerializeField] private Slider loading;
    [SerializeField] private GameObject MenuObj;
    [SerializeField] private TextMeshProUGUI volumeText;

    [SerializeField] private GameObject optionsObj;

    [SerializeField] private AudioSource backgroundAudioPrefab, backgroundAudio;
    [SerializeField] private Notice notice;

    [Header("Data")]
    [SerializeField] private GameData gameData;
    [SerializeField] private NoticeData noticeData;

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
        backgroundAudio.volume = volume;
        backgroundAudioPrefab.volume = volume;
    }
    public void OptionsButton()
    {
        MenuObj.SetActive(!MenuObj.activeInHierarchy);
        optionsObj.SetActive(!optionsObj.activeInHierarchy);
    }

    public void ButtonMarker(string name)
    {
        GameData.Option option = gameData.GetOption(name);
        option.value = !option.value;
        Vector3 scale = option.rTransform.localScale;
        scale = new Vector3(-scale.x, scale.y, scale.z);
        option.rTransform.localScale = scale;
    }
    public void CrutchForMute()
    {
        bool mark = gameData.GetOption("Music").value;
        backgroundAudio.mute = mark;
        backgroundAudioPrefab.mute = mark;
    }
}

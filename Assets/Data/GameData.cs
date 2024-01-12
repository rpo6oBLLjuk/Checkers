using System;
using TMPro;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "NewData", menuName = "New Data", order = 51)]
public class GameData : ScriptableObject
{
    [Serializable]
    public class Option
    {
        public string shortName;
        public bool value;
        public RectTransform rTransform;
    }
    public Option[] options;

    [Serializable]
    public class NewButton
    {
        public string shortName;
        public KeyCode keyCode;
        public TextMeshProUGUI buttonText;
    }

    public NewButton[] buttons;

    [Header("Colors")]
    public Color deskColor;
    public Color whiteFigureColor;
    public Color blackFigureColor;

    [Header("Camera Options")]
    public int cameraSpeed;
    public int cameraSliderValue;

    [Space]
    [Range(0, 1)] public float soundVolume;
    public void ChangeKeys(string name, KeyCode key)
    {
        Debug.Log("Вхождение совершено");
        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i].shortName == name)
            {
                buttons[i].keyCode = key;
                buttons[i].buttonText.text = key.ToString();
            }
        }
    }

    //Get class Object
    public NewButton GetButton(string name)
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i].shortName == name)
                return buttons[i];
        }
        return null;
    }

    public Option GetOption(string name)
    {
        for (int i = 0; i < options.Length; i++)
        {
            if (options[i].shortName == name)
                return options[i];
        }
        return null;
    }
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class ColorPicker : MonoBehaviour
{
    [SerializeField] private float wheelScale;
    [SerializeField] private Transform cross;
    [SerializeField] private GameObject wheelObj;

    [SerializeField] private TextMeshProUGUI alphaText;
    [SerializeField] private Slider alphaSlider;
    [SerializeField] private GameObject[] Objects;
    [SerializeField] private Image testMaterial;
    [SerializeField] private GameData gameData;


    private Color color_Picked;
    private Vector3 im;
    private Image wheelImg;
    private Texture2D texture;
    private GameObject pickedObj;

    private int pickedIndex;

    private bool onClick;
    private void Start()
    {
        pickedObj = Objects[0];
        LoadDefault();
        SaveDafult();
        color_Picked = Color.white;
        wheelImg = wheelObj.GetComponent<Image>();
        im = wheelObj.transform.position;
        texture = wheelImg.sprite.texture;
        onClick = false;
    }
    private void Update()
    {
        alphaText.text = "alpha: " + Mathf.RoundToInt(alphaSlider.value * 100) + "%";
        Vector3 mousePos = Input.mousePosition;
        if ((im - mousePos).magnitude <= wheelScale)
        {
            if (Input.GetMouseButtonDown(0))
            {
                onClick = true;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                onClick = false;
            }

            if (Input.GetMouseButton(0) & onClick)
            {

                cross.position = mousePos;

                int x = Mathf.RoundToInt(((mousePos.x - im.x) / wheelScale + 1) / 2 * texture.width);
                int y = Mathf.RoundToInt(((mousePos.y - im.y) / wheelScale + 1) / 2 * texture.height);

                color_Picked = texture.GetPixel(x, y);
                //Debug.Log(color_Picked + " & Vector2: " + x + ", " + y);
            }
        }

        wheelImg.color = new Color(wheelImg.color.r, wheelImg.color.g, wheelImg.color.b, alphaSlider.value);
        color_Picked.a = alphaSlider.value;//new Color(color_Picked.r, color_Picked.g, color_Picked.b, alpha.value);
        testMaterial.color = color_Picked;
    }
    public void ChangePickedObj(int index)
    {
        pickedIndex = index;
        pickedObj = Objects[index];
    }
    public void Defualt()
    {
        Color colorInside = Color.white;
        switch (pickedIndex)
        {
            case 0:
                colorInside = gameData.deskColor;
                break;
            case 1:
                colorInside = gameData.blackFigureColor;
                break;
            case 2:
                colorInside = gameData.whiteFigureColor;
                break;
        }
        for (int i = 0; i < pickedObj.GetComponent<MeshRenderer>().materials.Length; i++) //0,3,4
        {
            if (pickedIndex != 0 && (i != 1 || i != 2))
                pickedObj.GetComponent<MeshRenderer>().materials[i].color = colorInside;
        }
    }
    public void SaveColor()
    {
        if (pickedIndex != 0)
            pickedObj.GetComponent<MeshRenderer>().sharedMaterial.color = color_Picked;
        else
            for (int i = 0; i < pickedObj.GetComponent<MeshRenderer>().materials.Length; i++) //0,3,4
                if (i != 1 && i != 2)
                    pickedObj.GetComponent<MeshRenderer>().materials[i].color = color_Picked;
        SaveDafult();
    }
    public void SaveDafult()
    {
        gameData.deskColor = Objects[0].GetComponent<MeshRenderer>().material.GetColor("_MainColor");
        gameData.blackFigureColor = Objects[1].GetComponent<MeshRenderer>().sharedMaterial.GetColor("_MainColor");
        gameData.whiteFigureColor = Objects[2].GetComponent<MeshRenderer>().sharedMaterial.GetColor("_MainColor");
    }
    public void LoadDefault()
    {
        for (int i = 0; i < Objects.Length; i++)
        {
            switch (i)
            {
                case 0:
                    for (int index = 0; index < Objects[0].GetComponent<MeshRenderer>().materials.Length; index++)
                    {
                        if (index != 1 && index != 2)
                        {
                            Objects[0].GetComponent<MeshRenderer>().materials[index].color = gameData.deskColor;
                        }
                    }
                    break;
                case 1:
                    Objects[1].GetComponent<MeshRenderer>().sharedMaterial.color = gameData.blackFigureColor;
                    break;
                case 2:
                    Objects[2].GetComponent<MeshRenderer>().sharedMaterial.color = gameData.blackFigureColor;
                    break;
            }
        }
    }
}   //load material in start for obj (desk, figures);
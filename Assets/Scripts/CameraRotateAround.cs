using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CameraRotateAround : MonoBehaviour
{
    //Rotate Around Desk
    public int speed;
    public bool AutoRotate;
    public bool Rotate;
    public Image buttonImage;
    public Slider slider;
    public TextMeshProUGUI sliderValue;
    public TextMeshProUGUI fps;
    //Rotate of Input Mouse
    public Transform target;
    public Vector3 offset;
    public float sensitivity; // чувствительность мыши
    public float upLimit = 90, downLimit = 7; // ограничения вращения по Y
    public float zoomSpeed; // чувствительность колеса мыши
    public float zoomMax; // макс. увеличение
    public float zoomMin; // мин. увеличение
    public float save;  //поворот камеры относительно хода игрока
    public float rotateAngle;
    public float X, Y;
    private bool stop = true;

    void Start()
    {
        Y = 45;
        transform.localEulerAngles = new Vector3(Y, 0, 0);
        offset.z = -10;
    }
    void Update()
    {
        fps.text = "FPS: " + Mathf.Round(1 / Time.deltaTime).ToString();
        if (Input.GetAxis("Mouse ScrollWheel") > 0) offset.z += zoomSpeed;
        else if (Input.GetAxis("Mouse ScrollWheel") < 0) offset.z -= zoomSpeed;
        offset.z = Mathf.Clamp(offset.z, -Mathf.Abs(zoomMax), -Mathf.Abs(zoomMin));
        if (slider.value != slider.maxValue)
        {
            sliderValue.text = "Rotate Speed: " + slider.value.ToString();
            speed = (int)slider.value;
            if (!stop)
            {
                if (Rotate)
                    save += Time.deltaTime * speed / 5;
                else
                    save -= Time.deltaTime * speed / 5;
                if (save > 180)
                {
                    save = 180;
                    stop = true;
                }
                else if (save < 0)
                {
                    save = 0;
                    stop = true;
                }
                if (Input.GetMouseButton(1))
                    stop = true;
                rotateAngle = save;
            }
        }
        else
        {
            sliderValue.text = "Instantly";
            if (Rotate)
            {
                save = 180;
            }
            else
                save = 0;
            rotateAngle = save;
        }
        //Mouse Rotate
        if (Input.GetMouseButton(1))
        {
            rotateAngle = 0;
            UpdateRotation();
        }
        if (Input.GetKey(KeyCode.R))
        {
            if (!Rotate)
            {
                save = 0;
                X = 0;
            }
            else
            {
                save = 180;
                X = 0;
            }
            rotateAngle = save;
            Y = 45;
            offset.z = -10;
        }
        transform.localEulerAngles = new Vector3(Y, X + rotateAngle, 0);
        transform.position = transform.localRotation * offset + target.position;
    }
    private void UpdateRotation()
    {
        X = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * sensitivity;
        Y -= Input.GetAxis("Mouse Y") * sensitivity;
        Y = Mathf.Clamp(Y, downLimit, upLimit);
    }
    public void Rotation(bool auto)
    {
        if (rotateAngle == 0)
        {
            save = 0;
        }
        if (auto)
        {
            if (AutoRotate)
            {
                Rotate = !Rotate;
                stop = false;
            }
        }
        else
        {
            Rotate = !Rotate;
            stop = false;
        }
    }
    public void AutoRotateControl()
    {
        AutoRotate = !AutoRotate;
        if (AutoRotate)
            buttonImage.color = new Color(0.1605731f, 0.8773585f, 0.1605731f, 0.1960784f);
        else
            buttonImage.color = new Color(0.8784314f, 0.259323f, 0.1607843f, 0.1960784f);
    }
}
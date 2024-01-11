using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GlobalManager : MonoBehaviour
{
    public int PlayerColor;
    public CameraRotateAround camScript;

    [Header("Time to Spawn")]
    [SerializeField] private TextMeshProUGUI NowColor;
    [SerializeField] private TextMeshProUGUI whiteText;
    [SerializeField] private TextMeshProUGUI blackText;
    [SerializeField] private TextMeshProUGUI Winner;

    [Header("Borders")]
    public static int leftX;
    public static int rightX;
    public static int upY;
    public static int downY;

    [Header("Start Points")]
    public Transform[] WhitePoints;
    public Transform[] BlackPoints;

    [Header("Figure Objects")]
    public GameObject[] whiteFigure;
    public GameObject[] blackFigure;

    [Header("Support Objects & Components")]
    public GameObject CameraMain;
    public GameObject RayObj;

    [SerializeField] private GameObject vignette;

    public GameObject[,] Board = new GameObject[13, 13];

    [Header("Figure Count")]
    [SerializeField] private int whiteCount;
    [SerializeField] private int blackCount;

    public static UnityEvent<GameObject[]> PossibleTrue = new();
    public static UnityEvent BlockFalse = new();
    public static UnityEvent<int> CheckPossible = new();
    public static UnityEvent OffLight = new();
    public static GlobalManager This;

    [Header("Time to Spawn")]
    [SerializeField] private int movementSpeed;
    [SerializeField] private float speedVignette;
    private float startTime;    //[0,speedSpaw]

    [Header("Vignette Clamp")]
    [SerializeField] private float minScale;
    [SerializeField] private float maxScale;

    [Header("Data")]
    public GameData gameData;

    private bool isBinding;
    private string nameForBind;

    private void Start()
    {
        //QualitySettings.vSyncCount = 0;
        //Application.targetFrameRate = targetFrameRate;

        leftX = 0;
        downY = 0;
        rightX = 7;
        upY = 7;
        camScript = CameraMain.GetComponent<CameraRotateAround>();
        This = gameObject.GetComponent<GlobalManager>();
        PlayerColor = 1;
        whiteText.text = "White: " + (whiteCount = whiteFigure.Length).ToString();
        blackText.text = "Black: " + (blackCount = whiteFigure.Length).ToString();
        StartCoroutine(StartGame());
    }

    public void ChangeColor()
    {
        camScript.Rotation(true);
        PlayerColor *= -1;
        if (PlayerColor == 1)
            NowColor.text = "White";
        else
            NowColor.text = "Black";
        CheckPossible.Invoke(PlayerColor);
    }
    public void EatFigure(int color)
    {
        if (color == 1) //съедение белой фигуры
        {
            whiteText.text = "White: " + (whiteCount -= 1).ToString();
            if (whiteCount == 0)
            {
                StartCoroutine(Win(-1));
            }
        }
        else            //съедение чёрной фигуры
        {
            blackText.text = "Black: " + (blackCount -= 1).ToString();

            if (blackCount == 0)
            {
                StartCoroutine(Win(1));
            }
        }
    }
    private void OnDrawGizmos()
    {
        for (int x = 3; x < 11; x++)
            for (int y = 3; y < 11; y++)
                if (Board[x, y] != null)
                {
                    if (Board[x, y].GetComponent<Figure>().figureColor == 1)
                        Gizmos.color = Color.green;
                    else
                        Gizmos.color = Color.red;
                    Gizmos.DrawWireSphere(new Vector3(x - 3, 0, y - 3), 0.5f);
                }
    }
    public GameObject TakeObject(Vector3 pos)
    {
        return Board[Mathf.RoundToInt(pos.x) + 3, Mathf.RoundToInt(pos.z) + 3];
    }
    private IEnumerator Win(int color)
    {
        Winner.enabled = true;
        if (color == 1)
        {
            Winner.text = "White is Win";
        }
        else if (color == -1)
        {
            Winner.text = "Black is Win";
        }
        else
        {
            Winner.text = "Draw";
        }
        yield return new WaitForSeconds(3);
        AnotherMethods.Restart();
    }

    public IEnumerator StartGame()
    {
        bool TF = true;
        float save = Time.deltaTime * speedVignette / 20;
        vignette.transform.localScale = new Vector3(maxScale, maxScale, maxScale);
        while (TF)
        {
            vignette.transform.localScale += new Vector3(save, save, save);
            if (vignette.transform.localScale.x >= minScale)
                TF = false;
            yield return null;
        }
        while (1 > startTime)
        {
            int number = 0;
            foreach (GameObject go in whiteFigure)
            {
                go.transform.position = Vector3.Lerp(go.transform.position, WhitePoints[number].position, startTime);
                number += 1;
            }
            number = 0;
            foreach (GameObject go in blackFigure)
            {
                go.transform.position = Vector3.Lerp(go.transform.position, BlackPoints[number].position, startTime);
                number += 1;
            }
            startTime += Time.deltaTime * movementSpeed;
            yield return null;
        }
        int i = 0;
        foreach (GameObject go in whiteFigure)
        {
            go.transform.position = WhitePoints[i].position;
            i++;
            var component = go.GetComponent<Figure>(); component.enabled = true;
            component.StopPos = component.StartPos = new Vector3Int(Mathf.RoundToInt(go.transform.position.x), 0, Mathf.RoundToInt(go.transform.position.z));
            Board[component.StopPos.x + 3, component.StopPos.z + 3] = go;
        }
        i = 0;
        foreach (GameObject go in blackFigure)
        {
            go.transform.position = BlackPoints[i].position;
            i++;
            var component = go.GetComponent<Figure>(); component.enabled = true;
            component.StopPos = component.StartPos = new Vector3Int(Mathf.RoundToInt(go.transform.position.x), 0, Mathf.RoundToInt(go.transform.position.z));
            Board[component.StopPos.x + 3, component.StopPos.z + 3] = go;
        }
    }

    public void BindButton(string name)
    {
        GameData.NewButton button = gameData.GetButton(name);
        button.buttonText.text = "Input";
        nameForBind = button.shortName;
        isBinding = true;
    }
    private void OnGUI()
    {
        if (isBinding)
        {
            Event eventCopy = Event.current;
            if (eventCopy.isKey)
            {
                isBinding = false;
                Debug.Log(nameForBind);
                gameData.ChangeKeys(nameForBind, eventCopy.keyCode);
            }
        }

    }
}
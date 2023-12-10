using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GlobalManager : MonoBehaviour
{
    public int PlayerColor;
    public CameraRotateAround camScript;

    public TextMeshProUGUI NowColor, whiteText, blackText, Winner;
    public static int leftX, rightX, upY, downY;
    public Transform[] WhitePoints, BlackPoints;
    public GameObject[] whiteFigure, blackFigure;

    public GameObject CameraMain, RayObj;

    public GameObject[,] Board = new GameObject[13, 13];

    public int whiteCount, blackCount;

    public static UnityEvent<GameObject[]> PossibleTrue = new();
    public static UnityEvent BlockFalse = new();
    public static UnityEvent<int> CheckPossible = new();
    public static UnityEvent OffLight = new();
    public static GlobalManager This;

    [Header("Time to Spawn")]
    public int msTime;
    private int msTimeOut;

    public int targetFrameRate = 60;

    public GameObject pauseMenu;

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
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            ChangeActivePauseMenu();
        }
    }
    public void FixedUpdate()
    {
        int number = 0;
        if (msTime > msTimeOut)
        {
            foreach (GameObject go in whiteFigure)
            {
                go.transform.position = Vector3.Lerp(go.transform.position, WhitePoints[number].position, 0.02f * msTime / 5);
                number += 1;
            }
            number = 0;
            foreach (GameObject go in blackFigure)
            {
                go.transform.position = Vector3.Lerp(go.transform.position, BlackPoints[number].position, 0.02f * msTime / 5);
                number += 1;
            }
            msTimeOut += 1;
        }
        else if (msTimeOut == msTime)
        {
            msTimeOut += 1;
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
        GlobalManager.This.Restart();
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void ChangeActivePauseMenu()
    {
        pauseMenu.SetActive(!pauseMenu.activeInHierarchy);
    }
}
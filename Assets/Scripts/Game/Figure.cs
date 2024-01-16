using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Figure : MonoBehaviour
{
    [Range(1, 20)] public float speed;

    public Vector3Int StartPos, StopPos;
    public GameObject clickLight, detectLight, eaterLight, crownLight;
    public bool isCrown, block, onlyEat;
    public int figureColor;
    public Transform cam;

    private bool IsClick;

    private void Start()
    {
        GlobalManager.PossibleTrue.AddListener(IsPossibleTrue);
        GlobalManager.BlockFalse.AddListener(BlockFalse);
        GlobalManager.CheckPossible.AddListener(CheckPossible);
        GlobalManager.OffLight.AddListener(OffLightListener);
        cam = Camera.main.transform;
    }
    private void Update()
    {
        transform.rotation = Quaternion.Euler(transform.eulerAngles.x, cam.eulerAngles.y + 180, transform.eulerAngles.z);
        //if (Input.GetKeyDown(KeyCode.C))
        //{
        //    isCrown = !isCrown;
        //    crownLight.SetActive(!crownLight.activeInHierarchy);
        //}
        if (IsClick)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] hits = Physics.RaycastAll(ray, 100);
            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.gameObject != this.gameObject)
                {
                    transform.position = Vector3.Lerp(transform.position, new Vector3(hit.point.x, 0.5f, hit.point.z), Time.deltaTime * speed);

                    if (CheckPosition(new Vector3Int(Mathf.RoundToInt(transform.position.x), 0, Mathf.RoundToInt(transform.position.z)), true))
                    {
                        Transform rayTransofrm = GlobalManager.This.RayObj.transform;
                        float y = rayTransofrm.position.y;
                        rayTransofrm.position = MathF_Position(transform.position);
                        rayTransofrm.position += new Vector3(0, y, 0);
                    }
                }
            }
        }
    }
    private void OnMouseDown()
    {
        if (!block)
        {
            if (figureColor == GlobalManager.This.PlayerColor && !EventSystem.current.IsPointerOverGameObject())
            {
                clickLight.SetActive(true);
                GlobalManager.This.RayObj.SetActive(true);
                StartPos = new Vector3Int(Mathf.RoundToInt(transform.position.x), 0, Mathf.RoundToInt(transform.position.z));
                IsClick = true;
            }
        }
    }
    private void OnMouseUp()
    {
        GlobalManager.This.RayObj.SetActive(false);
        clickLight.SetActive(false);
        StopPos = MathF_Position(transform.position);
        StopPos.y = 0;
        if (IsClick)
        {
            if (CheckPosition(StopPos, true))
            {
                bool block = false;
                bool IsEat = false;
                GameObject eated = null;
                int length = Mathf.RoundToInt(Mathf.Abs(StopPos.z - StartPos.z));
                if (length == 1 && onlyEat)
                    block = true;
                else if (((length != 0 && length <= 2) || (length > 2 && isCrown)) && !GlobalManager.This.TakeObject(StopPos))
                {
                    if ((StopPos.z - StartPos.z == figureColor * length & Mathf.Abs(StopPos.x - StartPos.x) == length) || (Mathf.Abs(StopPos.z - StartPos.z) == Mathf.Abs(StopPos.x - StartPos.x) && isCrown))
                    {
                        if (length == 1)
                        {
                            if (GlobalManager.This.TakeObject(StopPos))
                            {
                                block = true;
                            }
                        }
                        else if (length == 2 || isCrown)
                        {
                            int x = (StopPos.x - StartPos.x) / length;
                            int index = figureColor;
                            if (isCrown)
                                index = (StopPos.z - StartPos.z) / length;
                            for (int i = 1; i < length; i++)
                            {
                                if (!IsEat || (isCrown && i != length))
                                {
                                    GameObject save = GlobalManager.This.TakeObject(StartPos + new Vector3(i * x, 0, i * index));
                                    if (save)
                                    {
                                        if (save.tag != this.tag && !IsEat)
                                        {
                                            if (!GlobalManager.This.TakeObject(StartPos + new Vector3((i + 1) * x, 0, (i + 1) * index)))
                                            {
                                                IsEat = true;
                                                eated = save;
                                            }
                                            else
                                            {
                                                block = true;
                                                break;
                                            }
                                        }
                                        else
                                        {
                                            block = true;
                                            break;
                                        }
                                    }
                                }
                                else if (!isCrown)
                                {
                                    block = true;
                                    break;
                                }
                            }
                            if (!IsEat && !isCrown)
                                block = true;
                            else if (isCrown && onlyEat && !IsEat)
                            {
                                block = true;
                            }
                        }
                    }
                    else
                        block = true;
                }
                else
                {
                    block = true;
                }
                if (!block)
                {
                    if (GlobalManager.This.gameData.GetOption("ContinueCrownEat").value)
                        if (this.CompareTag("Black") && StopPos.z == GlobalManager.downY && !isCrown)
                            Crowning();
                        else if (this.CompareTag("White") && StopPos.z == GlobalManager.upY && !isCrown)
                            Crowning();
                    transform.position = StopPos;
                    GlobalManager.This.Board[StartPos.x + 3, StartPos.z + 3] = null;
                    GlobalManager.This.Board[StopPos.x + 3, StopPos.z + 3] = gameObject;
                    if (length == 2 || isCrown)
                    {
                        if (IsEat)
                        {
                            var eatedComponent = eated.GetComponent<Figure>();
                            Vector3Int _position = eatedComponent.StopPos;
                            GlobalManager.This.Board[eatedComponent.StopPos.x + 3, eatedComponent.StopPos.z + 3] = null;
                            DestroyImmediate(eated);
                            GlobalManager.This.EatFigure(figureColor * -1, _position);
                            if (!PossibleToEat(StopPos, false))
                            {
                                GlobalManager.This.ChangeColor();
                            }
                            else
                            {
                                GlobalManager.CheckPossible.Invoke(figureColor);
                            }
                        }
                        else if (isCrown)
                        {
                            GlobalManager.This.ChangeColor();
                        }
                        else
                        {
                            transform.position = StartPos;
                        }
                    }
                    else
                    {
                        GlobalManager.This.ChangeColor();
                    }
                    if (!GlobalManager.This.gameData.GetOption("ContinueCrownEat").value)
                        if (this.CompareTag("Black") && StopPos.z == GlobalManager.downY && !isCrown)
                            Crowning();
                        else if (this.CompareTag("White") && StopPos.z == GlobalManager.upY && !isCrown)
                            Crowning();
                }
                else
                {
                    transform.position = StartPos;
                }
            }
            else
                transform.position = StartPos;
        }
        IsClick = false;
    }
    private bool PossibleToEat(Vector3Int NowPos, bool autoCheck)
    {
        int length = 2;
        bool possible = false;
        List<GameObject> detected = new();
        int yIndex = figureColor;
        if (isCrown)
        {
            length = Mathf.Min(GlobalManager.rightX - NowPos.x, GlobalManager.upY - NowPos.z);
            yIndex = 1;
        }
        for (int i = 1; i < length; i++)
        {
            GameObject possibleObject = GlobalManager.This.TakeObject(NowPos + new Vector3(i, 0, yIndex * i));
            if (possibleObject && CheckPosition(possibleObject.GetComponent<Figure>().StopPos, false))
            {
                if (!possibleObject.CompareTag(this.tag))
                    if (!GlobalManager.This.TakeObject(NowPos + new Vector3((i + 1), 0, yIndex * (i + 1))))
                    {

                        possible = true;
                        detected.Add(possibleObject);
                        break;
                    }
                    else
                        break;
                else
                    break;
            }
        }

        if (isCrown)
        {
            length = Mathf.Min(NowPos.x, GlobalManager.upY - NowPos.z);
            yIndex = 1;
        }
        for (int i = 1; i < length; i++)
        {
            GameObject possibleObject = GlobalManager.This.TakeObject(NowPos + new Vector3(-i, 0, yIndex * i));
            if (possibleObject && CheckPosition(possibleObject.GetComponent<Figure>().StopPos, false))
            {
                if (!possibleObject.CompareTag(this.tag))
                {
                    if (!GlobalManager.This.TakeObject(NowPos + new Vector3(-(i + 1), 0, yIndex * (i + 1))))
                    {
                        possible = true;
                        detected.Add(possibleObject);
                        break;
                    }
                }
                else
                    break;
            }
        }
        if (isCrown)
        {
            length = Mathf.Min(GlobalManager.rightX - NowPos.x, NowPos.z);
            yIndex = -1;
            for (int i = 1; i < length; i++)
            {
                GameObject possibleObject = GlobalManager.This.TakeObject(NowPos + new Vector3(i, 0, yIndex * i));
                if (possibleObject && CheckPosition(possibleObject.GetComponent<Figure>().StopPos, false))
                {
                    if (!possibleObject.CompareTag(this.tag))
                        if (!GlobalManager.This.TakeObject(NowPos + new Vector3((i + 1), 0, yIndex * (i + 1))))
                        {
                            possible = true;
                            detected.Add(possibleObject);
                            break;
                        }
                        else
                            break;
                    else
                        break;
                }
            }
            length = Mathf.Min(NowPos.x, NowPos.z);
            yIndex = -1;
            for (int i = 1; i < length; i++)
            {
                GameObject possibleObject = GlobalManager.This.TakeObject(NowPos + new Vector3(-i, 0, yIndex * i));
                if (possibleObject && CheckPosition(possibleObject.GetComponent<Figure>().StopPos, false))
                {
                    if (!possibleObject.CompareTag(this.tag))
                    {
                        if (!GlobalManager.This.TakeObject(NowPos + new Vector3(-(i + 1), 0, yIndex * (i + 1))))
                        {
                            possible = true;
                            detected.Add(possibleObject);
                            break;
                        }
                        else
                            break;
                    }
                    else
                        break;
                }
            }
        }

        if (possible)
        {
            GlobalManager.PossibleTrue.Invoke(detected.ToArray());
            eaterLight.SetActive(true);
            onlyEat = true;
            block = false;
            return true;
        }
        else if (!autoCheck)
        {
            eaterLight.SetActive(false);
            onlyEat = false;
            GlobalManager.BlockFalse.Invoke();
            GlobalManager.OffLight.Invoke();
        }
        else
        {
            onlyEat = false;
            eaterLight.SetActive(false);
        }
        return false;
    }
    private void Crowning()
    {
        isCrown = !isCrown;
        crownLight.SetActive(true);
    }

    //Supportive functions of the position
    private bool CheckPosition(Vector3Int NowPos, bool equal)   //проверка нахождения внутри поля, включая/исключая края (equal)
    {
        if (!equal)
            if (NowPos.x > GlobalManager.leftX && NowPos.x < GlobalManager.rightX && NowPos.z < GlobalManager.upY && NowPos.z > GlobalManager.downY)
            {
                return true;
            }
            else
                return false;
        else
        {
            if (NowPos.x >= GlobalManager.leftX && NowPos.x <= GlobalManager.rightX && NowPos.z <= GlobalManager.upY && NowPos.z >= GlobalManager.downY)
            {
                return true;
            }
            else
                return false;
        }
    }

    private Vector3Int MathF_Position(Vector3 pos)    //вычисление позиции исключительно чёрных клеток
    {
        Vector3Int res = Vector3Int.zero;
        Vector3 defPosition = pos;
        float x = defPosition.x;
        float z = defPosition.z;
        int xi = Mathf.RoundToInt(x);
        int zi = Mathf.RoundToInt(z);
        if ((xi + zi) % 2 == 1)
        {
            float xLength = x - xi;
            float zLength = z - zi;
            //Debug.Log("x: " + x + ", z: " + z + ", xi: " + xi + ", zi: " + zi + "\n" + "xLength: " + xLength + "zLength: " + zLength);
            //Debug.Log("xLength: " + xLength + "zLength: " + zLength);
            if (Mathf.Abs(xLength) > Mathf.Abs(zLength))
            {
                if ((xi == GlobalManager.leftX && xLength < 0) || (xi == GlobalManager.rightX && xLength > 0))
                {
                    res.x = xi;
                    res.z = zi + Math_Help(zLength);
                }
                else
                {
                    res.z = zi;
                    res.x = xi + Math_Help(xLength);
                }

            }
            else
            {
                if ((zi == GlobalManager.downY && zLength < 0) || (zi == GlobalManager.upY && zLength > 0))
                {
                    res.z = zi;
                    res.x = xi + Math_Help(xLength);
                }
                else

                {
                    res.x = xi;
                    res.z = zi + Math_Help(zLength);
                }

            }
            res.y = Mathf.RoundToInt(defPosition.y);
        }
        else
        {
            res = new Vector3Int(xi, Mathf.RoundToInt(defPosition.y), zi);
        }

        //support function
        int Math_Help(float length)
        {
            if (length > 0)
                return 1;
            else
                return -1;
        }
        return res;
    }

    //Four Listener
    private void IsPossibleTrue(GameObject[] figure)
    {
        if (!onlyEat)
            block = true;
        foreach (GameObject go in figure)
        {
            if (go == gameObject)
            {
                detectLight.SetActive(true);
            }
        }
    }
    private void BlockFalse()
    {
        block = false;
    }
    private void CheckPossible(int color)
    {
        if (color == figureColor)
            PossibleToEat(StopPos, true);
    }
    private void OffLightListener()
    {
        detectLight.SetActive(false);
        eaterLight.SetActive(false);
    }
}
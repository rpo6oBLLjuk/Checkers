using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Figure : MonoBehaviour
{
    [Range(1, 20)] public float speed;

    public Vector3Int StartPos, StopPos;
    public Light clickLight, detectLight, eaterLight;
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
        if (Input.GetKeyDown(KeyCode.C))
        {
            isCrown = !isCrown;
            transform.rotation = Quaternion.Euler(transform.eulerAngles.x - 180, transform.eulerAngles.y, transform.eulerAngles.z);
            foreach (Transform go in transform.GetComponentsInChildren<Transform>())
            {
                if (go != gameObject.transform)
                    go.localPosition = new Vector3(go.localPosition.x, go.localPosition.y, -go.localPosition.z);
            }
        }
        if (IsClick)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] hits = Physics.RaycastAll(ray, 100);
            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.gameObject != this.gameObject)
                {
                    Vector3 NowPos = new(Mathf.RoundToInt(transform.position.x), -0.075f, Mathf.RoundToInt(transform.position.z));
                    transform.position = Vector3.Lerp(transform.position, new Vector3(hit.point.x, 0.5f, hit.point.z), Time.deltaTime * speed);
                    if (NowPos.x >= GlobalManager.leftX && NowPos.x <= GlobalManager.rightX && NowPos.z >= GlobalManager.downY && NowPos.z <= GlobalManager.upY)
                        GlobalManager.This.RayObj.transform.position = NowPos;
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
                clickLight.enabled = true;
                GlobalManager.This.RayObj.SetActive(true);
                StartPos = new Vector3Int(Mathf.RoundToInt(transform.position.x), 0, Mathf.RoundToInt(transform.position.z));
                IsClick = true;
            }
        }
    }
    private void OnMouseUp()
    {
        GlobalManager.This.RayObj.SetActive(false);
        clickLight.enabled = false;
        StopPos = new Vector3Int(Mathf.RoundToInt(transform.position.x), 0, Mathf.RoundToInt(transform.position.z));
        if (IsClick)
        {
            if (GlobalManager.leftX <= StopPos.x & GlobalManager.rightX >= StopPos.x & GlobalManager.upY >= StopPos.z & GlobalManager.downY <= StopPos.z)
            {
                bool block = false;
                bool IsEat = false;
                GameObject eated = null;
                StopPos = new Vector3Int(Mathf.RoundToInt(transform.position.x), 0, Mathf.RoundToInt(transform.position.z));
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
                                if (!IsEat)
                                {
                                    GameObject save = GlobalManager.This.TakeObject(StartPos + new Vector3(i * x, 0, i * index));
                                    if (save)
                                    {
                                        if (save.GetComponent<Figure>().figureColor != figureColor)
                                        {
                                            if (!GlobalManager.This.TakeObject(StartPos + new Vector3((i + 1) * x, 0, (i + 1) * index)))
                                            {
                                                IsEat = true;
                                                eated = save;
                                            }
                                            else
                                                block = true;
                                        }
                                        else
                                            block = true;
                                    }
                                }
                                else
                                {
                                    block = true;
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
                    transform.position = StartPos;
                    block = true;
                }
                if (!block)
                {
                    transform.position = StopPos;
                    GlobalManager.This.Board[StartPos.x + 3, StartPos.z + 3] = null;
                    GlobalManager.This.Board[StopPos.x + 3, StopPos.z + 3] = gameObject;
                    if (length == 2 || isCrown)
                    {
                        if (IsEat)
                        {
                            var eatedComponent = eated.GetComponent<Figure>();
                            GlobalManager.This.Board[eatedComponent.StopPos.x + 3, eatedComponent.StopPos.z + 3] = null;
                            DestroyImmediate(eated);
                            GlobalManager.This.EatFigure(figureColor * -1);
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
                    if (figureColor == -1 && StopPos.z == GlobalManager.downY && !isCrown)
                        Crowning();
                    else if (figureColor == 1 && StopPos.z == GlobalManager.upY && !isCrown)
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
        bool checkCount = true; //right & up
        if (isCrown)
        {
            length = Mathf.Min(GlobalManager.rightX - NowPos.x, GlobalManager.upY - NowPos.z);
            yIndex = 1;
        }
        for (int i = 1; i < length; i++)
        {
            GameObject possibleObject = GlobalManager.This.TakeObject(NowPos + new Vector3(i, 0, yIndex * i));
            if (possibleObject && CheckPosition(possibleObject.GetComponent<Figure>().StopPos))
            {
                if (possibleObject.GetComponent<Figure>().figureColor != figureColor)
                    if (!GlobalManager.This.TakeObject(NowPos + new Vector3((i + 1), 0, yIndex * (i + 1))))
                    {
                        if (checkCount)
                        {
                            possible = true;
                            detected.Add(possibleObject);
                            checkCount = false;
                        }
                        else
                            break;
                    }
                    else
                        break;
                else
                {
                    checkCount = false;
                    break;
                }
            }
        }

        checkCount = true;      //left & up
        if (isCrown)
        {
            length = Mathf.Min(NowPos.x, GlobalManager.upY - NowPos.z);
            yIndex = 1;
        }
        for (int i = 1; i < length; i++)
        {
            GameObject possibleObject = GlobalManager.This.TakeObject(NowPos + new Vector3(-i, 0, yIndex * i));
            if (possibleObject && CheckPosition(possibleObject.GetComponent<Figure>().StopPos))
            {
                if (possibleObject.GetComponent<Figure>().figureColor != figureColor)
                {
                    if (!GlobalManager.This.TakeObject(NowPos + new Vector3(-(i + 1), 0, yIndex * (i + 1))))
                    {
                        if (checkCount)
                        {
                            possible = true;
                            detected.Add(possibleObject);
                            checkCount = false;
                        }
                        else
                            break;
                    }
                    else
                        break;
                }
                else
                {
                    checkCount = false;
                    break;
                }
            }
        }
        if (isCrown)
        {
            checkCount = true;      //right & down
            length = Mathf.Min(GlobalManager.rightX - NowPos.x, NowPos.z);
            yIndex = -1;
            for (int i = 1; i < length; i++)
            {
                GameObject possibleObject = GlobalManager.This.TakeObject(NowPos + new Vector3(i, 0, yIndex * i));
                if (possibleObject && CheckPosition(possibleObject.GetComponent<Figure>().StopPos))
                {
                    if (possibleObject.GetComponent<Figure>().figureColor != figureColor)
                        if (!GlobalManager.This.TakeObject(NowPos + new Vector3((i + 1), 0, yIndex * (i + 1))))
                        {
                            if (checkCount)
                            {
                                possible = true;
                                detected.Add(possibleObject);
                                checkCount = false;
                            }
                            else
                                break;
                        }
                        else
                            break;
                    else
                    {
                        checkCount = false;
                        break;
                    }
                }
            }

            checkCount = true;      //left & down
            length = Mathf.Min(NowPos.x, NowPos.z);
            yIndex = -1;
            for (int i = 1; i < length; i++)
            {
                GameObject possibleObject = GlobalManager.This.TakeObject(NowPos + new Vector3(-i, 0, yIndex * i));
                if (possibleObject && CheckPosition(possibleObject.GetComponent<Figure>().StopPos))
                {
                    if (possibleObject.GetComponent<Figure>().figureColor != figureColor)
                    {
                        if (!GlobalManager.This.TakeObject(NowPos + new Vector3(-(i + 1), 0, yIndex * (i + 1))))
                        {
                            if (checkCount)
                            {
                                possible = true;
                                detected.Add(possibleObject);
                                checkCount = false;
                            }
                            else
                                break;
                        }
                        else
                            break;
                    }
                    else
                    {
                        checkCount = false;
                        break;
                    }
                }
            }
        }

        if (possible)
        {
            GlobalManager.PossibleTrue.Invoke(detected.ToArray());
            eaterLight.enabled = true;
            onlyEat = true;
            block = false;
            return true;
        }
        else if (!autoCheck)
        {
            eaterLight.enabled = false;
            onlyEat = false;
            GlobalManager.BlockFalse.Invoke();
            GlobalManager.OffLight.Invoke();
        }
        else
        {
            onlyEat = false;
            eaterLight.enabled = false;
        }
        return false;
    }
    private void Crowning()
    {
        isCrown = !isCrown;
        transform.rotation = Quaternion.Euler(transform.eulerAngles.x - 180, transform.eulerAngles.y, transform.eulerAngles.z);
        foreach (Transform go in transform.GetComponentsInChildren<Transform>())
        {
            if (go != gameObject.transform)
                go.localPosition = new Vector3(go.localPosition.x, go.localPosition.y, -go.localPosition.z);
        }
    }
    private bool CheckPosition(Vector3Int NowPos)
    {
        if (NowPos.x > GlobalManager.leftX && NowPos.x < GlobalManager.rightX && NowPos.z < GlobalManager.upY && NowPos.z > GlobalManager.downY)
        {
            return true;
        }
        else
            return false;
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
                detectLight.enabled = true;
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
        detectLight.enabled = false;
        eaterLight.enabled = false;
    }
}
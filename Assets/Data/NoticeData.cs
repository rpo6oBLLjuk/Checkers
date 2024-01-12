using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NoticeData", menuName = "Notices", order = 52)]
public class NoticeData : ScriptableObject
{
    [Serializable]
    public class NoticeType
    {
        public string name;
        public int activeTime;
        public string text;
    }
    [SerializeField] private NoticeType[] Notices;

    public NoticeType GetNotice(string name)
    {
        for (int i = 0; i < Notices.Length; i++)
        {
            if (Notices[i].name == name)
            {
                return Notices[i];
            }
        }
        Debug.LogError("Non notice");
        return null;
    }

}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Notice : MonoBehaviour
{
    public static Notice This;
    public GameObject NoticeObj;
    public TextMeshProUGUI tmproText;
    public Animator animator;

    private void Start()
    {
        This = this;
    }

    public void CreateNotice(NoticeData.NoticeType notice)
    {
        StartCoroutine(CreateNoticeNumerator(notice.activeTime, notice.text));
    }

    private IEnumerator CreateNoticeNumerator(int time, string text)
    {
        NoticeObj.SetActive(true);
        tmproText.text = text;
        yield return new WaitForSeconds(time);
        StartCoroutine(DeleteNotice());
    }

    private IEnumerator DeleteNotice()
    {
        animator.SetTrigger("Death");
        yield return new WaitForSeconds(animator.runtimeAnimatorController.animationClips[1].length);
        NoticeObj.SetActive(false);
    }
}

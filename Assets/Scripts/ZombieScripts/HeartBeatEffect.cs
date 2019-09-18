using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartBeatEffect : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(HeartBeat());
    }

    public IEnumerator HeartBeat()
    {
        while(true)
        {
            yield return new WaitWhile(() => LeanTween.isTweening());
            LeanTween.scale(gameObject.GetComponent<RectTransform>(), Vector2.one * 1.2f, 1f);
            yield return new WaitWhile(() => LeanTween.isTweening());
            LeanTween.scale(gameObject.GetComponent<RectTransform>(), Vector2.one * .8f, .5f);
        }
    }
   
}

using System.Collections;
using UnityEngine;

public class UIAnimationManager : Singleton<UIAnimationManager>
{
    public void PulsingAnimation(GameObject uiObject, Vector3 maxScale, Vector3 minScale, float time)
    {
        StartCoroutine(ScaleOverTime(uiObject, time, maxScale, minScale));
    }

    IEnumerator ScaleOverTime(GameObject uiObject, float time, Vector3 maxScale, Vector3 minScale)
    {
        if(uiObject == null) yield break;

        float currentTime = 0.0f;

        while (currentTime < time)
        {
            if (uiObject == null) yield break;
            currentTime += Time.deltaTime;
            uiObject.transform.localScale = Vector3.Lerp(maxScale, minScale, currentTime / time);
            yield return null;
        }

       
        yield return new WaitForSeconds(0.1f);
        StartCoroutine(ScaleOverTime(uiObject, time, minScale, maxScale));
    }
}

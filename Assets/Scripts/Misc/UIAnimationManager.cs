using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class UIAnimationManager : Singleton<UIAnimationManager>
{
    [SerializeField] GameObject happyEmoji;
    [SerializeField] GameObject angryEmoji;
    Vector3 happyEmojiStartPosition;
    Vector3 angryEmojiStartPosition;

    bool stopPulsingAnim = false;   

    private void Start()
    {
        happyEmojiStartPosition = happyEmoji.transform.position;
        angryEmojiStartPosition = angryEmoji.transform.position;
    }

    public Coroutine PulsingAnimation(GameObject uiObject, Vector3 maxScale, Vector3 minScale, float time, bool useForAngryEmoji = false)
    {
        return StartCoroutine(ScaleOverTime(uiObject, time, maxScale, minScale, useForAngryEmoji));
    }

    IEnumerator ScaleOverTime(GameObject uiObject, float time, Vector3 maxScale, Vector3 minScale, bool useForAngryEmoji)
    {
        if(uiObject == null) yield break;
        if (useForAngryEmoji && stopPulsingAnim) yield break;

        float currentTime = 0.0f;

        while (currentTime < time)
        {
            if (uiObject == null) yield break;
            if (useForAngryEmoji && stopPulsingAnim)
            {
                stopPulsingAnim = false;
                yield break;
            }
            currentTime += Time.deltaTime;
            uiObject.transform.localScale = Vector3.Lerp(maxScale, minScale, currentTime / time);
            yield return null;
        }

       
        yield return new WaitForSeconds(0.1f);
        StartCoroutine(ScaleOverTime(uiObject, time, minScale, maxScale, useForAngryEmoji));
    }

    Coroutine happyEmojiCoroutine;
    public void PlayHappyEmoji()
    {
        if(happyEmojiCoroutine != null) StopCoroutine(happyEmojiCoroutine);
        happyEmojiCoroutine = StartCoroutine(MoveAndFade(happyEmoji, happyEmojiStartPosition, Vector3Int.up, 1f, 2));
    }

    Coroutine angryEmojiCoroutine;
    public void PlayAngryEmoji()
    {
        angryEmoji.SetActive(true);
        if (angryEmojiCoroutine != null) StopCoroutine(angryEmojiCoroutine);
        angryEmoji.transform.localScale = Vector3.one;
        angryEmojiCoroutine = PulsingAnimation(angryEmoji, new Vector3(1.3f, 1.3f, 1.3f), new Vector3(1f, 1f, 1f), 0.5f, true);
    }

    public void StopAngryEmoji()
    {
        if (angryEmojiCoroutine != null) stopPulsingAnim = true;
        angryEmoji.SetActive(false);
    }

    public IEnumerator MoveAndFade(GameObject obj, Vector3 startPosition, Vector3 direction, float time, float speed)
    {
        obj.SetActive(true);
        SpriteRenderer[] spriteRenderer = obj.GetComponentsInChildren<SpriteRenderer>();
        if (spriteRenderer.Length <= 0)
        {
            Debug.LogError("GameObject không có SpriteRenderer.");
            yield break;
        }

        obj.transform.position = startPosition;

        float elapsed = 0f;

        while (elapsed < time)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(elapsed / time);

            obj.transform.Translate(direction * speed * Time.deltaTime);

            foreach(var renderer in spriteRenderer)
            {
                Color color = renderer.color;
                color.a = Mathf.Lerp(1f, 0f, t);
                renderer.color = color;
            }
            

            yield return null; 
        }

        obj.transform.position = startPosition;
        obj.SetActive(false);
    }
    public void MovingToTarget(GameObject gameObject, Vector3 targetPosition, float speed)
    {
        StartCoroutine(MoveToTargetCoroutine(gameObject, targetPosition, 3f));
    }

    private IEnumerator MoveToTargetCoroutine(GameObject gameObject, Vector3 targetPosition, float speed)
    {
        gameObject.SetActive(true);
        Vector3 firstPosition = gameObject.transform.position;
        while (Vector3.Distance(gameObject.transform.position, targetPosition) > 0.01f)
        {
            gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, targetPosition, speed * Time.deltaTime);
            yield return null; 
        }

        gameObject.transform.position = firstPosition;
        gameObject.SetActive(false);
    }
}

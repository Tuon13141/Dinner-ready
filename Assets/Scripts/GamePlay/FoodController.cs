using System;
using System.Collections;
using UnityEngine;

public class FoodController : MonoBehaviour
{
    FoodSpot FoodSpot;
    [SerializeField] float moveTime = 0.5f;
    [SerializeField] FoodStage foodStage = FoodStage.None;

    public void SetFoodSpot(FoodSpot foodSpot)
    {
        this.FoodSpot = foodSpot;
        transform.parent = foodSpot.transform;
        StartCoroutine(LerpPosition(foodSpot.transform.position, moveTime));
        ChangeState(FoodStage.OnWaitingToBill);
    }

    private IEnumerator LerpPosition(Vector3 target, float duration)
    {
        Vector3 startPosition = transform.position;
        float timeElapsed = 0;

        while (timeElapsed < duration)
        {
            transform.position = Vector3.Lerp(startPosition, target, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            yield return null; 
        }

        transform.position = target;
    }

    public void ChangeState(FoodStage newState)
    {
        if (newState == foodStage) return;
        ExitCurrentState();
        foodStage = newState;
        EnterNewState();
    }

    private void EnterNewState()
    {
        switch (foodStage)
        {
            case FoodStage.None:

                break;
            case FoodStage.OnWaitingToBill:

                break;
            case FoodStage.OnBilled:
  
            default:
                break;
        }
    }

    private void ExitCurrentState()
    {
        switch (foodStage)
        {
            case FoodStage.None:

                break;
            case FoodStage.OnWaitingToBill:

                break;
            case FoodStage.OnBilled:

            default:
                break;
        }
    }

}

[Serializable]
public enum FoodStage
{
    None, OnWaitingToBill, OnBilled
}

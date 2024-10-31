using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassengerController : MonoBehaviour
{
    [SerializeField] DayManager dayManager;
    [SerializeField] Animator m_animator;
    [SerializeField] GameObject currentPassengerObject;
    [SerializeField] PassengerObjectKey currentPassengerKey;
    [SerializeField] PassengerStage passengerStage = PassengerStage.None;

    [SerializeField] Transform spawnPoint;
    [SerializeField] Transform waitPoint;
    [SerializeField] Transform despawnPoint;

    Coroutine coroutine;
    public void SetPassengerPrefab(GameObject passengerPref, int gender, int id)
    {
        Reset();
        currentPassengerKey = new PassengerObjectKey(gender, id);
        currentPassengerObject = dayManager.FoodObjectPool.GetPassenger(gender, id, passengerPref, this.transform);

        m_animator = currentPassengerObject.GetComponent<Animator>();
        ChangeState(PassengerStage.OnWalkingIn);
    }

    public void ChangeState(PassengerStage newState)
    {
        if (newState == passengerStage) return;

        passengerStage = newState;
        EnterNewState();
    }

    private void EnterNewState()
    {
        switch (passengerStage)
        {
            case PassengerStage.None:

                break;
            case PassengerStage.OnWalkingOut:
                OnWalkingOut();
                break;
            case PassengerStage.OnWaiting:
                OnWaiting();
                break;
            case PassengerStage.OnWalkingIn:
                OnWalkingIn();
                break;
            default:

                break;
        }
    }

    void OnWaiting()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }        
        
        m_animator.SetTrigger("Idle");

        coroutine = StartCoroutine(MoveAndRotate(waitPoint.position, 0f, 90, 0.25f));
    }

    void OnWalkingIn()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }

     
        transform.position = spawnPoint.position;
        m_animator.SetTrigger("Walk");
        coroutine = StartCoroutine(MoveAndRotate(waitPoint.position, 1f, 90, 0.25f));
    }

    void OnWalkingOut()
    {
        UIAnimationManager.Instance.PlayHappyEmoji();
        if(coroutine != null)
        {
            StopCoroutine(coroutine);
        }
       
        m_animator.SetTrigger("Walk");
        coroutine = StartCoroutine(MoveAndRotate(despawnPoint.position, 1.5f, -90, 0.1f));
    }

    private IEnumerator MoveAndRotate(Vector3 destination, float moveTime, float targetRotationY, float rotationTime)
    {
        Vector3 startPosition = transform.position; 
        Quaternion startRotation = transform.rotation; 
        Quaternion endRotation = Quaternion.Euler(0, targetRotationY, 0) * startRotation; 

        float moveElapsedTime = 0;
        float rotationElapsedTime = 0;

        while (moveElapsedTime < moveTime || rotationElapsedTime < rotationTime)
        {
            if (moveElapsedTime < moveTime)
            {
                transform.position = Vector3.Lerp(startPosition, destination, moveElapsedTime / moveTime);
                moveElapsedTime += Time.deltaTime;
            }

            if (rotationElapsedTime < rotationTime)
            {
                transform.rotation = Quaternion.Lerp(startRotation, endRotation, rotationElapsedTime / rotationTime);
                rotationElapsedTime += Time.deltaTime;
            }

            yield return null;
        }

        transform.position = destination;
        transform.rotation = endRotation;

        switch (passengerStage)
        {
            case PassengerStage.None:
                break;
            case PassengerStage.OnWalkingOut:
                dayManager.WaveFinished = true;
                break;
            case PassengerStage.OnWaiting:
                break;
            case PassengerStage.OnWalkingIn:
                ChangeState(PassengerStage.OnWaiting);
                break;
            default:
                break;
        }
    }


    private void Reset()
    {
        transform.rotation = Quaternion.Euler(0, 0, 0);
        coroutine = null;
        m_animator = null;
        dayManager.FoodObjectPool.RemovePassenger(currentPassengerObject, currentPassengerKey.Gender, currentPassengerKey.Id);
        passengerStage = PassengerStage.None;
    }
}

[Serializable]
public enum PassengerStage
{
    None, OnWalkingIn, OnWaiting, OnWalkingOut
}

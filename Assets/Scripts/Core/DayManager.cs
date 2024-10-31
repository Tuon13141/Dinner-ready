using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayManager : Singleton<DayManager>, IOnStart
{
    [SerializeField] DayConfig dayConfig;
    [SerializeField] FoodConfig foodConfig;
    [SerializeField] FoodObjectPool foodObjectPool;
    public FoodObjectPool FoodObjectPool => foodObjectPool;
    [SerializeField] PassengerManager passengerManager;
    [SerializeField] List<FoodSpot> foodSpots;
    [SerializeField] Transform spawnFoodPoint;
    [SerializeField] Transform despawnFoodPoint;

    [SerializeField] bool useDayInUserData = false;
    public bool UseDayInUserData => useDayInUserData;

    [SerializeField] int dayIndex = 0;
    public int DayIndex => dayIndex;

    public int MaxDay { get; set; }

    public Dictionary<int, Food> FoodDict = new Dictionary<int, Food>();
    public Dictionary<int, Day> DayDict = new Dictionary<int, Day>();
    List<FoodController> currentFoodControllers = new List<FoodController>();

    public bool WaveFinished { get; set; } = true;
    bool isFinishAllWaves = false;
    public bool IsLastWave { get; set; } = false;   
    float waveCoin = 0;
    public float DayCoin { get; private set; } = 0;
    public float TotalDayCoin { get; private set; } = 0;
    public float TotalDayVisitor { get; private set; } = 0;

    Coroutine spawnFoodCoroutine;
    public void OnStart()
    {
        FoodDict = foodConfig.CreateFoodDictionary();
        DayDict = dayConfig.CreateDayDictionary();
        MaxDay = DayDict.Values.Count;

        if (useDayInUserData)
        {
            dayIndex = GameManager.Instance.UserData.day;
        }
    }

    public void LoadDay(bool isRetry = false)
    {
        Reset(isRetry);
        passengerManager.CurrentPassengerController.Reset();
        passengerManager.NextPassengerController.Reset(isRetry);
        GameUI.Instance.Get<UIInGame>().Show();

        Day day = DayDict[dayIndex];

        foreach (Passenger passenger in day.PassengerList)
        {
            foreach (FoodOrder foodOrder in passenger.FoodOrderList)
            {
                int id = foodOrder.FoodId;
                int quantity = foodOrder.Quantity;

                Food food = FoodDict[id];
                TotalDayCoin += food.Price * quantity;
            }
            TotalDayVisitor++;
        }

        GameUI.Instance.Get<UIInGame>().SetProgress(DayCoin, TotalDayCoin);
        GameUI.Instance.Get<UIInGame>().SetDayText(dayIndex);
        spawnFoodCoroutine = StartCoroutine(SpawnFoodToTable(day));
        passengerManager.GetRandomPassengerObject();
        
    }

    IEnumerator SpawnFoodToTable(Day day)
    {
        for(int i = 0; i < day.PassengerList.Count; i++)
        {
            Passenger passenger = day.PassengerList[i];
           
            yield return new WaitUntil(() => WaveFinished);
            passengerManager.CurrentPassengerController.nextPassenger = true;
            if (i == day.PassengerList.Count - 1)
            {
                IsLastWave = true;
            }
            yield return new WaitForSeconds(.25f);
            
            foreach (FoodOrder foodOrder in passenger.FoodOrderList)
            {
                int id = foodOrder.FoodId;
                int quantity = foodOrder.Quantity;

                Food food = FoodDict[id];
                int index = quantity - 1;
             
              
                //Instantiate(food.foodStacks[index], spawnFoodPoint);

                FoodSpot foodSpot = GetFoodSpot();
                if (foodSpot == null)
                {
                    Debug.LogError("Can't have that many food at once !!!");
                    yield break;
                }
                else
                {
                    if (!GameManager.Instance.CheckUnlockedFood(id))
                    {
                        GameUI.Instance.Get<UIUnlockFood>().Show();
                        GameUI.Instance.Get<UIUnlockFood>().AddToShowList(food);
                    }

                    waveCoin += food.Price * quantity;
                   
                    FoodController foodController = foodObjectPool.GetFood(id, quantity, food.foodStacks[index].gameObject, spawnFoodPoint);
                    foodController.SetFoodSpot(foodSpot, despawnFoodPoint, id, quantity);
                    currentFoodControllers.Add(foodController);
                    foodSpot.IsHadFood = true;
                }
            }

            WaveFinished = false;
        }

        isFinishAllWaves = true;

    }

    FoodSpot GetFoodSpot()
    {
        foreach(FoodSpot foodSpot in foodSpots)
        {
            if (!foodSpot.IsHadFood)
            {
                return foodSpot;
            }
        }

        return null;
    }

    public void CheckAnswer(float answer)
    {
        if(answer != waveCoin)
        {
            UIAnimationManager.Instance.PlayAngryEmoji();
            StartCoroutine(EndGameResult(false));
            return;
        }
        foreach (FoodController controller in currentFoodControllers)
        {
            controller.ChangeState(FoodStage.OnBilled);
        }
        

        DayCoin += waveCoin;
        GameUI.Instance.Get<UIInGame>().SetProgress(DayCoin, TotalDayCoin);
        waveCoin = 0;

        passengerManager.CurrentPassengerController.ChangeState(PassengerStage.OnWalkingOut);
        //WaveFinished = true;
        currentFoodControllers.Clear();

        if (isFinishAllWaves)
        {
            StartCoroutine(EndGameResult(true));
            return;
        }
    }

    void Reset(bool needToRemoveAllFood)
    {
        UIAnimationManager.Instance.StopAngryEmoji();
        if (needToRemoveAllFood)
        {
            foodObjectPool.RemoveAllFood();
        }

        if(spawnFoodCoroutine != null) StopCoroutine(spawnFoodCoroutine);
        waveCoin = 0;
        TotalDayCoin = 0;
        DayCoin = 0;
        WaveFinished = true;
        isFinishAllWaves = false;
        currentFoodControllers.Clear();
        IsLastWave = false;
        dayIndex = GameManager.Instance.UserData.day;

        foreach(FoodSpot foodSpot in foodSpots)
        {
            foodSpot.IsHadFood = false;
        }
    }

    IEnumerator EndGameResult(bool isWon)
    {

        if (isWon)
        {
            yield return new WaitForSeconds(0.75f);
            GameManager.Instance.ChangeState(GameStates.Win);
        }
        else
        {
            yield return new WaitForSeconds(0.25f);
            GameManager.Instance.ChangeState(GameStates.Lose);
        }
       
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayManager : Singleton<DayManager>, IOnStart
{
    [SerializeField] DayConfig dayConfig;
    [SerializeField] FoodConfig foodConfig;
    [SerializeField] FoodObjectPool foodObjectPool;
    public FoodObjectPool FoodObjectPool => foodObjectPool;
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
    bool isLastWave = false;

    float waveCoin = 0;
    public float DayCoin { get; private set; } = 0;
    public float TotalDayCoin { get; private set; } = 0;

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

    public void LoadDay(bool needToRemoveAllFood = false)
    {
        Reset(needToRemoveAllFood);
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
        }

        GameUI.Instance.Get<UIInGame>().SetProgress(DayCoin, TotalDayCoin);
        GameUI.Instance.Get<UIInGame>().SetDayText(dayIndex);
        spawnFoodCoroutine = StartCoroutine(SpawnFoodToTable(day));
    }

    IEnumerator SpawnFoodToTable(Day day)
    {
        foreach(Passenger passenger in day.PassengerList)
        {
            yield return new WaitUntil(() => WaveFinished);

            yield return new WaitForSeconds(.5f);

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

        isLastWave = true;

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
            StartCoroutine(EndGameResult(false));
            return;
        }
        foreach (FoodController controller in currentFoodControllers)
        {
            controller.ChangeState(FoodStage.OnBilled);
        }
        if (isLastWave)
        {
            StartCoroutine(EndGameResult(true));
            return;
        }

        DayCoin += waveCoin;
        GameUI.Instance.Get<UIInGame>().SetProgress(DayCoin, TotalDayCoin);
        waveCoin = 0;
        WaveFinished = true;
        currentFoodControllers.Clear();
    }

    void Reset(bool needToRemoveAllFood)
    {
        if (needToRemoveAllFood)
        {
            foodObjectPool.RemoveAllFood();
        }

        if(spawnFoodCoroutine != null) StopCoroutine(spawnFoodCoroutine);
        waveCoin = 0;
        TotalDayCoin = 0;
        DayCoin = 0;
        WaveFinished = true;
        isLastWave = false;
        currentFoodControllers.Clear();

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

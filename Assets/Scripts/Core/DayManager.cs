using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayManager : Singleton<DayManager>, IOnStart
{
    [SerializeField] DayConfig dayConfig;
    [SerializeField] FoodConfig foodConfig;
    [SerializeField] List<FoodSpot> foodSpots;
    [SerializeField] Transform spawnFoodPoint;
    [SerializeField] Transform despawnFoodPoint;

    [SerializeField] bool useDayInUserData = false;
    public bool UseDayInUserData => useDayInUserData;

    [SerializeField] int dayIndex = 0;
    public int DayIndex => dayIndex;

    public Dictionary<int, Food> FoodDict = new Dictionary<int, Food>();
    public Dictionary<int, Day> DayDict = new Dictionary<int, Day>();
    List<FoodController> currentFoodControllers = new List<FoodController>();

    public bool WaveFinished { get; set; } = true;
    bool isLastWave = false;

    float waveCoin = 0;
    float dayCoin = 0;
    public void OnStart()
    {
        FoodDict = foodConfig.CreateFoodDictionary();
        DayDict = dayConfig.CreateDayDictionary();

        if (useDayInUserData)
        {
            dayIndex = GameManager.Instance.UserData.day;
        }
    }

    public void LoadDay()
    {
        GameUI.Instance.Get<UIInGame>().Show();

        Day day = DayDict[dayIndex];

        StartCoroutine(SpawnFoodToTable(day));
    }

    IEnumerator SpawnFoodToTable(Day day)
    {
        foreach(Passenger passenger in day.PassengerList)
        {
            yield return new WaitUntil(() => WaveFinished);

            yield return new WaitForSeconds(.5f);

            foreach (FoodOrder foodOrder in passenger.FoodOrderList)
            {
                Food food = FoodDict[foodOrder.FoodId];
                int index = foodOrder.Quantity - 1;
                waveCoin += food.Price * foodOrder.Quantity;
                dayCoin += food.Price * foodOrder.Quantity;
                //Debug.Log(food.Name);
                //Debug.Log(index);

                FoodController foodController = Instantiate(food.foodStacks[index], spawnFoodPoint);

                FoodSpot foodSpot = GetFoodSpot();
                if (foodSpot == null)
                {
                    Debug.LogError("Can't have that many food at once !!!");
                }
                else
                {
                    foodController.SetFoodSpot(foodSpot, despawnFoodPoint, food.Id);
                    foodSpot.IsHadFood = true;
                }

                currentFoodControllers.Add(foodController);
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
            GameManager.Instance.ChangeState(GameStates.Lose);
            Reset();
            return;
        }
        foreach (FoodController controller in currentFoodControllers)
        {
            controller.ChangeState(FoodStage.OnBilled);
        }
        if (isLastWave)
        {
            GameManager.Instance.ChangeState(GameStates.Win);
            Reset();
            return;
        }

        
        waveCoin = 0;
        WaveFinished = true;
        
    }

    void Reset()
    {
        waveCoin = 0;
        dayCoin = 0;
        WaveFinished = true;
        isLastWave = false;
        currentFoodControllers.Clear();
    }
}

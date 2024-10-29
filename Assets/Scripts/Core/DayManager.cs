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

    [SerializeField] int dayIndex = 0;
    public int DayIndex => dayIndex;

    public Dictionary<int, Food> FoodDict = new Dictionary<int, Food>();
    public Dictionary<int, Day> DayDict = new Dictionary<int, Day>();

    public bool WaveFinished { get; set; } = true;
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
                    foodController.SetFoodSpot(foodSpot);
                    foodSpot.IsHadFood = true;
                }
            }

            WaveFinished = false;
        }



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
}

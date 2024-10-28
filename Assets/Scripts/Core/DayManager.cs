using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayManager : Singleton<DayManager>, IOnStart
{
    [SerializeField] DayConfig dayConfig;
    [SerializeField] FoodConfig foodConfig;
    [SerializeField] List<FoodSpot> foodSpots;

    [SerializeField] bool useDayInUserData = false;

    [SerializeField] int dayIndex = 0;
    public int DayIndex => dayIndex;

    public Dictionary<string, Food> FoodDict = new Dictionary<string, Food>();
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

            yield return new WaitForSeconds(1);

            
        }



    }
}

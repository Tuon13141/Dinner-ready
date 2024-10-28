using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class FoodConfig : ScriptableObject
{
    public List<Food> FoodList = new List<Food>();

    public Dictionary<string, Food> CreateFoodDictionary()
    {
        Dictionary<string, Food> FoodDict = new Dictionary<string, Food>();
        foreach (Food food in FoodList)
        {        
            FoodDict.Add(food.Id, food);          
        }

        return FoodDict;
    }
}

[Serializable]
public class Food
{
    public string Id;
    public string Name;
    public Sprite Icon;
    public int Price;
    public int DayUnlock;
    public List<GameObject> foodStacks;
}

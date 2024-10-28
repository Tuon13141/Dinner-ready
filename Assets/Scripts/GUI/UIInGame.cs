using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class UIInGame : UIElement
{
    public override bool ManualHide => true;

    public override bool DestroyOnHide => false;

    public override bool UseBehindPanel => false;

    [SerializeField] FoodIcon foodIconPref;
    [SerializeField] Transform foodIconParent;

    List<Food> availableFoods = new List<Food>();
    [SerializeField] List<FoodIcon> availableFoodIcons = new List<FoodIcon>();
    public override void Show()
    {
        base.Show();
        
        availableFoodIcons.Clear();
        List<Food> foods = DayManager.Instance.FoodDict.Values.ToList();
        int day = DayManager.Instance.DayIndex;
        foreach (Food food in foods)
        {
            if (food.DayUnlock <= day)
            {
                availableFoods.Add(food);
                FoodIcon foodIcon = Instantiate(foodIconPref, foodIconParent);
                foodIcon.SetUp(food);
                availableFoodIcons.Add(foodIcon);
            }
        }
    }

}

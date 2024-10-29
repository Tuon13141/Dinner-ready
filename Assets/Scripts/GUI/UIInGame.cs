using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class UIInGame : UIElement
{
    public override bool ManualHide => true;

    public override bool DestroyOnHide => false;

    public override bool UseBehindPanel => false;

    [SerializeField] FoodIcon foodIconPref;
    [SerializeField] Transform foodIconParent;

    List<Food> availableFoods = new List<Food>();
    [SerializeField] List<FoodIcon> availableFoodIcons = new List<FoodIcon>();

    [SerializeField] TextMeshProUGUI valueText;
    [SerializeField] List<NumberButton> numberButtons = new List<NumberButton>();

    [SerializeField] Button plusButton;
    [SerializeField] Button minusButton;
    [SerializeField] Button multiplyButton;
    [SerializeField] Button divideButton;
    [SerializeField] Button equalButton;

    [SerializeField] Button decimalButton;

    [SerializeField] string number_1 = "";
    [SerializeField] string number_2 = "";

    [SerializeField] CaculatorStage caculatorStage = CaculatorStage.FirstEnter;
    [SerializeField] MathematicalType mathematicalType = MathematicalType.Plus;
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

        foreach(NumberButton numberButton in numberButtons)
        {
            numberButton.UIInGame = this;
        }
    }

    public void DecimalButton()
    {     
        if (caculatorStage == CaculatorStage.FirstEnter)
        {
            if (mathematicalType == MathematicalType.None)
            {
                number_1 = "";
                mathematicalType = MathematicalType.Plus;
            }
            if (number_1.Length < 11 && number_1.Length > 1 && !number_1.Contains("."))
            {
                number_1 += ".";
           
            }
            else
            {
                Debug.LogError("Can't enter it here !!!");
            }
            DisplayValueText(number_1);
        }
        else
        {
            if (number_2.Length < 11 && number_2.Length > 1 && !number_2.Contains("."))
            {
                number_2 += ".";
                
            }
            else
            {
                Debug.LogError("Can't enter it here !!!");
            }
            DisplayValueText(number_2);
        }
    }
    public void OnClickNumberButton(int value)
    {
        if (caculatorStage == CaculatorStage.FirstEnter)
        {
            if (mathematicalType == MathematicalType.None)
            {
                number_1 = "";
                mathematicalType = MathematicalType.Plus;
            }
            if (number_1.Length < 12)
            {
                number_1 += value.ToString();
                
            }
            else
            {
                Debug.LogError("Can't enter too many number !!!");
            }
            DisplayValueText(number_1);
        }
        else
        {
            if (number_2.Length < 12)
            {
                number_2 += value.ToString();
               
            }
            else
            {
                Debug.LogError("Can't enter too many number !!!");
            }
            DisplayValueText(number_2);
        }
    }

    public void EqualButton()
    {
        switch (mathematicalType)
        {
            case MathematicalType.Plus:
                PlusButton();
                break;
            case MathematicalType.Minus:
                MinusButton();
                break;
            case MathematicalType.Mutiply:
                MutiplyButton();
                break;
            case MathematicalType.Divide:
                DivideButton();
                break;
        }
        DisplayValueText(number_1);
  
        number_2 = "";
        caculatorStage = CaculatorStage.FirstEnter;
        mathematicalType = MathematicalType.None;
        
    }
    public void PlusButton()
    {
        mathematicalType = MathematicalType.Plus;
        if(caculatorStage == CaculatorStage.FirstEnter)
        {
            caculatorStage = CaculatorStage.SecondEnter;
            return;
        }

        float result_1 = ConvertStringToFloat(number_1);
        float result_2 = ConvertStringToFloat(number_2);
        
        number_1 = (result_1 + result_2).ToString();
        number_2 = "";
        DisplayValueText(number_2);
    }

    public void MinusButton()
    {
        mathematicalType = MathematicalType.Minus;
        if (caculatorStage == CaculatorStage.FirstEnter)
        {
            caculatorStage = CaculatorStage.SecondEnter;
            return;
        }

        float result_1 = ConvertStringToFloat(number_1);
        float result_2 = ConvertStringToFloat(number_2);

        number_1 = (result_1 - result_2).ToString();
        number_2 = "";
        DisplayValueText(number_2);
    }

    public void MutiplyButton()
    {
        mathematicalType = MathematicalType.Mutiply;
        if (caculatorStage == CaculatorStage.FirstEnter)
        {
            caculatorStage = CaculatorStage.SecondEnter;
            return;
        }

        float result_1 = ConvertStringToFloat(number_1);
        float result_2 = ConvertStringToFloat(number_2);

        number_1 = (result_1 * result_2).ToString();
        number_2 = "";
        DisplayValueText(number_2);
    }

    public void DivideButton()
    {
        mathematicalType = MathematicalType.Divide;
        if (caculatorStage == CaculatorStage.FirstEnter)
        {
            caculatorStage = CaculatorStage.SecondEnter;
            return;
        }

        float result_1 = ConvertStringToFloat(number_1);
        float result_2 = ConvertStringToFloat(number_2);

        number_1 = (result_1 / result_2).ToString();
        number_2 = "";
        DisplayValueText(number_2);
    }
    void DisplayValueText(string str)
    {
        valueText.text = str;
    }

    float ConvertStringToFloat(string str)
    {
        float result_1 = 0;

        if (float.TryParse(str, out result_1))
        {
            //Debug.Log(result);
        }
        else
        {
            result_1 = 0f;
        }

        return result_1;
    }

    private void Start()
    {
        equalButton.onClick.AddListener(EqualButton);
        decimalButton.onClick.AddListener(DecimalButton);
        plusButton.onClick.AddListener(PlusButton);
        minusButton.onClick.AddListener(MinusButton);
        multiplyButton.onClick.AddListener(MutiplyButton);
        divideButton.onClick.AddListener(DivideButton);
    }
}

[Serializable]
public enum CaculatorStage
{
    FirstEnter, SecondEnter, ThirdEnter
}

public enum MathematicalType
{
    None, Plus, Minus, Mutiply, Divide
}
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FoodIcon : MonoBehaviour
{
    [SerializeField] Image foodIcon;
    [SerializeField] TextMeshProUGUI priceText;

    public void SetUp(Food food)
    {
        priceText.text = food.Price.ToString();
        foodIcon.sprite = food.Icon;
    }
}

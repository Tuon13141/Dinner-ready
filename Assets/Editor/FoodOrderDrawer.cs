using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

[CustomPropertyDrawer(typeof(FoodOrder))]
public class FoodOrderDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        FoodConfig foodConfig = Resources.Load<FoodConfig>("FoodConfig");
        if (foodConfig == null || foodConfig.FoodList == null)
        {
            EditorGUI.LabelField(position, "FoodConfig or FoodList not found!");
            return;
        }

        List<string> foodNames = new List<string>();
        List<int> foodIds = new List<int>();
        Dictionary<int, int> foodStacksCounts = new Dictionary<int, int>();

        foreach (var food in foodConfig.FoodList)
        {
            foodNames.Add(food.Name);
            int foodId = int.Parse(food.Id);
            foodIds.Add(foodId);
            foodStacksCounts[foodId] = food.foodStacks.Count;
        }

        EditorGUI.BeginProperty(position, label, property);

        SerializedProperty quantityProp = property.FindPropertyRelative("Quantity");
        SerializedProperty foodIdProp = property.FindPropertyRelative("FoodId");

        float labelWidth = 70f;
        float fieldWidth = position.width - labelWidth - 10f;
        float padding = 5f;

        Rect quantityLabelRect = new Rect(position.x, position.y, labelWidth, EditorGUIUtility.singleLineHeight);
        Rect quantityFieldRect = new Rect(position.x + labelWidth, position.y, fieldWidth, EditorGUIUtility.singleLineHeight);

        Rect foodLabelRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight + padding, labelWidth, EditorGUIUtility.singleLineHeight);
        Rect foodFieldRect = new Rect(position.x + labelWidth, position.y + EditorGUIUtility.singleLineHeight + padding, fieldWidth, EditorGUIUtility.singleLineHeight);

        EditorGUI.LabelField(foodLabelRect, "Food");
        int selectedIndex = foodIds.IndexOf(foodIdProp.intValue);
        if (selectedIndex == -1) selectedIndex = 0;
        selectedIndex = EditorGUI.Popup(foodFieldRect, selectedIndex, foodNames.ToArray());
        foodIdProp.intValue = foodIds[selectedIndex];

        int maxQuantity = foodStacksCounts.ContainsKey(foodIdProp.intValue) ? foodStacksCounts[foodIdProp.intValue] : 1;
        EditorGUI.LabelField(quantityLabelRect, "Quantity");
        quantityProp.intValue = EditorGUI.IntSlider(quantityFieldRect, quantityProp.intValue, 1, maxQuantity);

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight * 2 + 5;
    }
}

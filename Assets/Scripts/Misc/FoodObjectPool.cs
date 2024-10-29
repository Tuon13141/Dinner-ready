using System.Collections.Generic;
using UnityEngine;

public class FoodObjectPool : MonoBehaviour
{
    public Dictionary<int, List<FoodController>> ActiveFoods = new Dictionary<int, List<FoodController>>();
    public Dictionary<int, List<FoodController>> InativeFoods = new Dictionary<int, List<FoodController>>();

    public void GetFood(int id, GameObject pref)
    {

    }
}

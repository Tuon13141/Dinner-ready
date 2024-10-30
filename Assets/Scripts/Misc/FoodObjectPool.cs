using System;
using System.Collections.Generic;
using UnityEngine;

public class FoodObjectPool : MonoBehaviour
{
    public Dictionary<FoodControllerKey, List<FoodController>> ActiveFoods = new Dictionary<FoodControllerKey, List<FoodController>>();
    public Dictionary<FoodControllerKey, List<FoodController>> InativeFoods = new Dictionary<FoodControllerKey, List<FoodController>>();

    public FoodController GetFood(int id, int quantity, GameObject pref, Transform parent)
    {
        FoodControllerKey key = new FoodControllerKey(id, quantity);
        if (!InativeFoods.ContainsKey(key))
        {
            //Debug.Log(1);
            FoodController foodController = Instantiate(pref, parent).GetComponent<FoodController>();
            if (!ActiveFoods.ContainsKey(key))
            {
                List<FoodController> list = new List<FoodController>();
                list.Add(foodController);
                ActiveFoods.Add(key, list);
            }
            else
            {
                ActiveFoods[key].Add(foodController);
            }
            return foodController;
        }
        else
        {
            //Debug.Log(2);
            FoodController foodController = InativeFoods[key][0];
            InativeFoods[key].Remove(foodController);

            foodController.gameObject.SetActive(true);
            if (!ActiveFoods.ContainsKey(key))
            {
                List<FoodController> list = new List<FoodController>();
                list.Add(foodController);
                ActiveFoods.Add(key, list);
            }
            else
            {
                ActiveFoods[key].Add(foodController);
            }
            foodController.gameObject.transform.parent = parent;
            foodController.transform.position = parent.transform.position;

            return foodController;
        }
    }

    public void RemoveFood(FoodController foodController)
    {
        FoodControllerKey key = foodController.Key;
        if (!ActiveFoods.ContainsKey(key))
        {
            Debug.Log("Key don't exits !!!");
        }
        else
        {
            if (!ActiveFoods[key].Contains(foodController))
            {
                Debug.Log("Key don't contain it !!!");
                return;
            }

            ActiveFoods[key].Remove(foodController);
            if (!InativeFoods.ContainsKey(key))
            {
                List<FoodController> list = new List<FoodController>();
                list.Add(foodController);
                InativeFoods.Add(key, list);
            }
            else
            {
                InativeFoods[key].Add(foodController);
              
            }
            foodController.Reset();
            foodController.gameObject.SetActive(false);
        }
    }

    public void RemoveAllFood()
    {
        //Debug.Log("Remove All Food !!!");
        foreach (List<FoodController> value in ActiveFoods.Values)
        {
            for (int i = 0; i < value.Count; i++)
            {
                //Debug.Log(value[i].gameObject.name);
                RemoveFood(value[i]);
            }
        }
    }
}

[Serializable]
public class FoodControllerKey
{
    int Id;
    int Quantity;

    public FoodControllerKey(int id, int quantity)
    {
        Id = id;
        Quantity = quantity;
    }

    public bool Compare(int id, int quantity)
    {
        if (Id == id && Quantity == quantity) return true;
        return false;
    }

    public bool Compare(FoodControllerKey key)
    {
        int id = key.Id;
        int quantity = key.Quantity;
        return Compare(id, quantity);
    }

    public override bool Equals(object obj)
    {
        if (obj is FoodControllerKey other)
        {
            return Id == other.Id && Quantity == other.Quantity;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode() ^ Quantity.GetHashCode();
    }
}

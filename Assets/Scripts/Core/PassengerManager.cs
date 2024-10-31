using System.Collections.Generic;
using TCP_Fatness_Level;
using UnityEngine;

public class PassengerManager : MonoBehaviour
{
    [SerializeField] List<GameObject> malePassengers = new List<GameObject>();
    [SerializeField] List<GameObject> femalePassengers = new List<GameObject>();

    [SerializeField] PassengerController passengerController;
    public PassengerController PassengerController => passengerController;

    [SerializeField] DayManager dayManager;
    public DayManager DayManager => dayManager;

    public void GetRandomPassengerObject()
    {
        int gender = Random.Range(0, 2);
        if(gender == 0)
        {
            //nam
            int choice = Random.Range(0, malePassengers.Count);
            GameObject passengerObj = malePassengers[choice];
            //FatnessLevel fatnessLevel = passengerObj.GetComponent<FatnessLevel>();
            //HairSelector.HairSelector hairSelector = passengerObj.GetComponent<HairSelector.HairSelector>();
            //HeadSelector.HeadSelector headSelector = passengerObj.GetComponent<HeadSelector.HeadSelector>();
            //SkinSelector.SkinSelector skinSelector = passengerObj.GetComponent<SkinSelector.SkinSelector>();
            //GlassesSelector.GlassesSelector glassesSelector = passengerObj.GetComponent <GlassesSelector.GlassesSelector>();

            passengerController.SetPassengerPrefab(passengerObj, gender, choice);
        }
        else
        {
            //nu
            int choice = Random.Range(0, femalePassengers.Count);
            GameObject passengerObj = femalePassengers[choice];

            passengerController.SetPassengerPrefab(passengerObj, gender, choice);
        }
    }
}

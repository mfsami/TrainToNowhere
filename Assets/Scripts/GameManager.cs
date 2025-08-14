using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{

    public Transform trainParent; // Parent object that holds all carriages
    public int currentCarIndex;     // Which carriage we are in
    private List<Transform> carriages = new List<Transform>();
    private List<Transform> targets = new List<Transform>();

    public int playerStamina = 3;

    
    public Transform player;


    private void Start()
    {
        // Build list of carriages from trainParent children in order
        carriages.Clear();
        targets.Clear();

        // Loop through parent and count train cars
        for (int i = 0; i < trainParent.childCount; i++)
        {
            // Add each car to a list
            var carriage = trainParent.GetChild(i);
            carriages.Add(carriage);

            // Locate target for player to spawn at and add to list
            var t = carriage.Find("CarTarget");

            if (t == null)
            {
                Debug.LogError($"CarTarget missing under {carriage.name}");
            }
                
            targets.Add(t);
        }

        Debug.Log($"Found {carriages.Count} carriages.");
        LogCurrentCarriage();

    }


    public void MoveForward()
    {
        if (currentCarIndex < carriages.Count - 1)
        {
            // Move forward and reduce stamina
            currentCarIndex++;

            // If stamina exhausted (false), stop and do nothing 
            if (!TrySpendStamina()) return;

            var target = targets[currentCarIndex];
            if (target == null) return;

            player.SetPositionAndRotation(target.position, target.rotation);
            Debug.Log("Moved to carriage: " + currentCarIndex);
            LogCurrentCarriage();
        }
        else
        {
            Debug.Log("Reached the last carriage!");
        }
    }

    public void MoveBack()
    {
        // If not the first car
        if (currentCarIndex > 0)
        {
            if (!TrySpendStamina()) return;

            // Move back
            currentCarIndex--;
           

            // Check position of car behind current car
            var target = targets[currentCarIndex];
            if (target == null) return;

            player.SetPositionAndRotation(target.position, target.rotation);
            Debug.Log("Moved to carriage: " + currentCarIndex);
            LogCurrentCarriage();
        }

        else
        {
            Debug.Log("No cars behind you");
        }
    }


    bool TrySpendStamina()
    {
        if (playerStamina <= 0)
        {
            ExhaustedStamina();
            return false;
        }
        playerStamina -= 1;
        return true;
    }

    void ExhaustedStamina()
    {

        Debug.Log("You are too exhausted to move anymore..");
    }

    void LogCurrentCarriage()
    {
        Debug.Log($"Now in carriage index: {currentCarIndex}, name: {carriages[currentCarIndex].name}");
    }
}



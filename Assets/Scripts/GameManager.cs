using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{

    public Transform trainParent; // Parent object that holds all carriages
    public int currentCarIndex;     // Which carriage we are in
    private List<Transform> carriages = new List<Transform>();


    private void Start()
    {
        // Build list of carriages from trainParent children in order
        carriages.Clear();
        for (int i = 0; i < trainParent.childCount; i++)
        {
            carriages.Add(trainParent.GetChild(i));
        }

        Debug.Log($"Found {carriages.Count} carriages.");
        LogCurrentCarriage();

    }


    public void MoveForward()
    {
        if (currentCarIndex < carriages.Count - 1)
        {
            // Move forward
            currentCarIndex++;
            LogCurrentCarriage();
        }
        else
        {
            Debug.Log("Reached the last carriage!");
        }
    }

    void LogCurrentCarriage()
    {
        Debug.Log($"Now in carriage index: {currentCarIndex}, name: {carriages[currentCarIndex].name}");
    }
}



using UnityEngine;

public class Door : MonoBehaviour
{
   
    private GameManager gameManager;

    private void Start()
    {

        gameManager = Object.FindFirstObjectByType<GameManager>();

    }

    public void OnMouseDown()
    {
        gameManager.MoveForward();
    }
}

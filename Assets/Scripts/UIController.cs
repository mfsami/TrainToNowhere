using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    // ------- Buttons -------
    public Button backButton;

    // ------- Text -------
    public TextMeshProUGUI currentCar;

    // ------- References -------
    public GameManager gameManager;

    void Start()
    {
        backButton.onClick.AddListener(OnBackClicked);
    }

    private void Update()
    {
        currentCar.text = $"Current Car: {gameManager.currentCarIndex}";
    }

    void OnBackClicked()
    {
        gameManager.MoveBack();
        
    }
}

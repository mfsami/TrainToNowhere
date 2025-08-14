using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public Button backButton;
    public GameManager gameManager;
    void Start()
    {
        backButton.onClick.AddListener(OnBackClicked);
    }

    void OnBackClicked()
    {
        gameManager.MoveBack();
    }
}

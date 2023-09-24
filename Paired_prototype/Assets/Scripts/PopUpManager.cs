using UnityEngine;
using UnityEngine.UI;

public class PopUpManager : MonoBehaviour
{
    public GameObject popUpPanel; // Reference to the pop-up panel GameObject
    public Button restartButton; // Reference to the restart button
    public Button exitButton; // Reference to the exit button

    private void Start()
    {
        // Hide the pop-up panel initially
        popUpPanel.SetActive(false);

        // Add click event listeners to the buttons
        restartButton.onClick.AddListener(RestartGame);
        exitButton.onClick.AddListener(ExitGame);
    }

    public void ShowPopUp()
    {
        // Show the pop-up panel
        popUpPanel.SetActive(true);
    }

    public void ClosePopUp()
    {
        // Hide the pop-up panel
        popUpPanel.SetActive(false);
    }

    public void RestartGame()
    {
        // Restart the game by reloading the current scene
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }

    public void ExitGame()
    {
        // Exit the game (works in standalone builds)
        Application.Quit();
    }
}

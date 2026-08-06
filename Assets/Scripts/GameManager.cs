using UnityEngine;
using UnityEngine.SceneManagement;
public enum GameState {Play,Build  };
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [Header("—сылки на объекты")]
    [SerializeField] private GameObject playerObject;
    [SerializeField] private GameObject buildCanvasPanel;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject playerCamera;
    [SerializeField] private GameObject builderCamera;
    private GameState currentState = GameState.Play;
    void Awake() { Instance = this; }
    void Start()
    {
        ApplyGameState();
    }
    public void ToggleMode()
    {
        pausePanel.SetActive(false);
        if (currentState == GameState.Build)
        {
            LevelManager.Instance.SaveLevel();

            currentState = GameState.Play;

            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        else if (currentState == GameState.Play)
        {
            currentState = GameState.Build;
            ApplyGameState();
        }
    }
    void ApplyGameState()
    {
        pausePanel.SetActive(false);
        if (currentState == GameState.Build)
        {
            playerCamera.SetActive(false);
            builderCamera.SetActive(true);
            buildCanvasPanel.SetActive(true); 
            Cursor.lockState = CursorLockMode.None;
            if (playerObject != null)
                playerObject.SetActive(false);

        }
        else if (currentState== GameState.Play)
        {
            playerCamera.SetActive(true);
            builderCamera.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            buildCanvasPanel.SetActive(false); 

            if (playerObject != null)
                playerObject.SetActive(true);  

            LevelManager.Instance.LoadLevel();
            Time.timeScale = 1;

        }
    }
}

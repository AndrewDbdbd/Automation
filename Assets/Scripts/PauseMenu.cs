using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI;

    private InputAction pauseAction;
    private void Start()
    {
        pauseAction = InputSystem.actions.FindAction("PauseMenuToggle");

        pauseAction.performed += pauseAction_performed;

        if (pauseAction == null)
            Debug.LogError("Pause action not found!");
    }

    private void OnEnable()
    {
        if (pauseAction == null) return;

        pauseAction.Enable();
        pauseAction.performed += pauseAction_performed;
    }
    private void OnDisable()
    {
        if (pauseAction == null) return;
        pauseAction.performed -= pauseAction_performed;
    }
    private void pauseAction_performed(InputAction.CallbackContext context)
    {
        if (pauseMenuUI.activeSelf) 
        {
            Resume();
        }
        else
        {
            Pause();

        }
    }
    public void Resume()
    {
        Cursor.lockState = CursorLockMode.Locked;
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
    }
    public void Pause()
    {
        Cursor.lockState = CursorLockMode.None;
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
    }

}
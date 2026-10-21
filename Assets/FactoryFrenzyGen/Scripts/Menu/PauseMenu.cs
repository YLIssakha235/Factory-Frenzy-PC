using StarterAssets;
using UnityEngine;
using UnityEngine.SceneManagement;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class PauseMenu : MonoBehaviour
{
    public GameObject pausePanel;
    public SettingsPanel settings;

    bool paused = false;
    ThirdPersonController controller;
    bool controllerWasOn;

    void Start()
    {
        if (pausePanel != null) pausePanel.SetActive(false);
        if (settings != null) settings.gameObject.SetActive(false);
    }

    void Update()
    {
        if (!EscapePressed()) return;

        if (settings != null && settings.gameObject.activeSelf) settings.Close(); 
        else if (paused) Resume();
        else Pause();
    }

    bool EscapePressed()
    {
#if ENABLE_INPUT_SYSTEM
        return Keyboard.current != null && (Keyboard.current.escapeKey.wasPressedThisFrame || Keyboard.current.pKey.wasPressedThisFrame);
#else
        return Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P);
#endif
    }

    public void Pause()
    {

        RaceManager race = FindObjectOfType<RaceManager>();
        if (race != null && race.endPanel != null && race.endPanel.activeSelf) return;

        paused = true;
        Time.timeScale = 0;             
        pausePanel.SetActive(true);

        
        CharacterController body = FindObjectOfType<CharacterController>();
        controller = body != null ? body.GetComponent<ThirdPersonController>() : null;
        controllerWasOn = controller != null && controller.enabled; 
        if (controller != null) controller.enabled = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

   
    public void Resume()
    {
        paused = false;
        Time.timeScale = 1;
        pausePanel.SetActive(false);
        if (settings != null) settings.gameObject.SetActive(false);
        if (controller != null && controllerWasOn) controller.enabled = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void OpenSettings()
    {
        pausePanel.SetActive(false);
        settings.returnTo = pausePanel;
        settings.Open();
    }

    public void Restart()
    {
        Time.timeScale = 1;              
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Menu");
    }

    public void QuitGame()
    {
        Time.timeScale = 1;
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;   // in Unity: stops Play mode
#else
        Application.Quit();                                // in the built game: closes it
#endif
    }
}

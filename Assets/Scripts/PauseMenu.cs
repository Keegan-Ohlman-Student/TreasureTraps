using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu; // Pause Menu UI Panel
    private Button controlsButton;
    [SerializeField] private GameManager gm;
    private string previousScene; // Store previous scene name before opening Controls Menu

    private void Start()
    {
        if (gm == null)
        {
            gm = FindObjectOfType<GameManager>();
        }

        // Find the Controls Button inside the Pause Menu
        controlsButton = pauseMenu.transform.Find("ControlsButton")?.GetComponent<Button>();

        if (controlsButton != null)
        {
            controlsButton.onClick.RemoveAllListeners();
            controlsButton.onClick.AddListener(OpenControlsMenu);
        }
    }

    public void Pause()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0;

        controlsButton = pauseMenu.transform.Find("ControlsButton")?.GetComponent<Button>();

        if (controlsButton != null)
        {
            controlsButton.onClick.RemoveAllListeners();
            controlsButton.onClick.AddListener(gm.OnControlsButtonPressed);
        }
    }

    public void Resume()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1;

        if (controlsButton != null)
        {
            controlsButton.onClick.RemoveAllListeners();
            controlsButton.onClick.AddListener(gm.OnControlsButtonPressed);
        }
    }

    public void Home(string sceneName)
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(sceneName);
    }

    private void OpenControlsMenu()
    { 
        if (gm == null)
        {
            gm = FindObjectOfType<GameManager>();
        }

        if (gm != null)
        {
            Time.timeScale = 1;
            gm.LoadScene("ControlsMenu");
        }
    }

    public void ReassignControlsButton()
    {
        controlsButton = pauseMenu.transform.Find("ControlsButton")?.GetComponent<Button>();

        if (controlsButton != null)
        {
            controlsButton.onClick.RemoveAllListeners();
            controlsButton.onClick.AddListener(gm.OnControlsButtonPressed);
        }
    }
}

using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private List<string> sceneHistory = new List<string>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        string currentScene = SceneManager.GetActiveScene().name;

        if (sceneHistory.Count == 0)
        {
            sceneHistory.Add(currentScene);
        }
    }

    public void LoadScene(string sceneName)
    {
        string currentScene = SceneManager.GetActiveScene().name;

        if (sceneHistory.Count > 0 && sceneHistory[sceneHistory.Count - 1] == sceneName)
        {
            Debug.Log("Scene already in history, not adding again: " + sceneName);
        }

        else
        {
            sceneHistory.Add(currentScene);
        }

        SceneManager.LoadScene(sceneName);
    }

    public bool PreviousScene()
    {

        if (sceneHistory.Count > 0)
        {
            string previousScene = sceneHistory[sceneHistory.Count - 1];
            sceneHistory.RemoveAt(sceneHistory.Count - 1);

            SceneManager.LoadScene(previousScene);
            return true;
        }

        return false;
    }

    public void OnBackButtonPressed()
    {
        if (!PreviousScene())
        {
            LoadScene("TitleMenu");
        }
    }

    public void OnMenuButtonPressed()
    {
        LoadScene("TitleMenu");
    }

    public void OnStartButtonPressed()
    {
        LoadScene("LevelOne");
    }

    public void OnControlsButtonPressed()
    {
        string currentScene = SceneManager.GetActiveScene().name;

        if (sceneHistory.Count == 0 || sceneHistory[sceneHistory.Count - 1] != currentScene)
        {
            sceneHistory.Add(currentScene);
        }

        LoadScene("ControlsMenu");
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Scene Loaded: " + scene.name);

        AssignButton("BackButton", OnBackButtonPressed);
        AssignButton("MenuButton", OnMenuButtonPressed);
        AssignButton("StartButton", OnStartButtonPressed);
        AssignButton("ControlsButton", OnControlsButtonPressed);

        if (scene.name == "LevelOne")
        {
            StartCoroutine(FindPauseMenuAndReassign());
        }
    }

    private void AssignButton(string tag, UnityEngine.Events.UnityAction action)
    {
        Button button = GameObject.FindWithTag(tag)?.GetComponent<Button>();
        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(action);
        }
        
    }

    private IEnumerator FindPauseMenuAndReassign()
    {
        GameObject pauseMenuObj = null;
        PauseMenu pauseMenuScript = null;

        // ✅ Keep searching for the Pause Menu until it is found
        for (int i = 0; i < 20; i++) // Retry for 2 seconds
        {
            pauseMenuObj = GameObject.Find("Pause Menu");
            if (pauseMenuObj != null)
            {
                pauseMenuScript = pauseMenuObj.GetComponent<PauseMenu>();
                if (pauseMenuScript != null)
                {
                    pauseMenuScript.ReassignControlsButton();
                    yield break;
                }
            }
            yield return new WaitForSeconds(0.1f);
        }
    }
}
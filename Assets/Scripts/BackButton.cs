using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackButton : MonoBehaviour
{
    int sceneIndex;
    int sceneToOpen;

    // Start is called before the first frame update
    void Start()
    {
        sceneIndex = SceneManager.GetActiveScene().buildIndex;

        if(!PlayerPrefs.HasKey("previousScene" + sceneIndex))
        {
            PlayerPrefs.SetInt("previousScene" + sceneIndex, sceneIndex);
        }

        sceneToOpen = PlayerPrefs.GetInt("previousScene");
    }

    public void OnButtonClick()
    {
        SceneManager.LoadScene(sceneToOpen);
    }
}

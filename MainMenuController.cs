using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{

    private int? selectedClassID = null;
    [SerializeField] private string sceneName;


    public void SelectClass(int classID)
    {
        selectedClassID = classID;
    }

    public void StartGame()
    {
        if (selectedClassID != null)
        {
            //run game
            PlayerPrefs.SetInt("SelectedClass", selectedClassID.Value);
            SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        }
        else
        {
            //do message
        }
    }
}

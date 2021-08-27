using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{

    private int? selectedClassID = null;
    [SerializeField] private string sceneName;

    public static MainMenuController current;

    [HideInInspector] public Color mainColor, secondaryColor;
    [HideInInspector] public Sprite eyes, ears, nose, mouth;

    void Awake()
    {
        current = this;
    }

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
            Debug.LogError("No player class selected!");
        }
    }
}

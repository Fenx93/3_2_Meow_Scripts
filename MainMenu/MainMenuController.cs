using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameModes
{
    singlePlayer, towers, multiPlayer
}

public class MainMenuController : MonoBehaviour
{
    private string selectedClassID = null;
    [SerializeField] private string sceneName, trainingScenename;
    [SerializeField] private TextMeshProUGUI selectedClassName;

    public ScriptableCharacterClass[] classes;

    [SerializeField] private AudioClip mainMenuTheme;

    public bool unlockClasses = false;

    public static MainMenuController current;

    [HideInInspector] public int mainColorId = 0, secondaryColorId = 0, hatId = 0, earsId = 0, clothesId = 0, mouthId = 0;

    [HideInInspector] public GameModes selectedGameMode;

    void Awake()
    {
        current = this;
    }

    void Start()
    {
        AudioController.current.PlayMusic(mainMenuTheme);
        BattlePreparationScreenController.current.UpdateClassButtons(classes);
        PlayerStatsTracker.SetData(1, 0, 75, 0);
        PlayerStatsTracker.UpdateUI();
    }

    public void SelectClass(int classInteger)
    {
        ScriptableCharacterClass selectedClass = classes[classInteger];
        selectedClassID = selectedClass.ClassName;
        selectedClassName.text = selectedClass.ClassName;
        CharacterCustomizer.current.avatars[0].SetWeapon(selectedClass.WeaponSprite);

        bool locked = (PlayerStatsTracker.CurrentLvl < selectedClass.UnlocksAtLevel && !unlockClasses);
        if (locked)
        {
            MainMenuUI.current.SetStartGameButton(locked, "Character class locked!", selectedClass.UnlocksAtLevel);
        }
        else
        {
            MainMenuUI.current.SetStartGameButton(locked, "To Battle!");
        }
    }

    //public void CycleThroughClasses(int direction)
    //{
    //    if (direction == 1)
    //    {
    //        selectedClassID = (selectedClassID == classes.Length-1) ?
    //            0
    //            : selectedClassID+1;

    //    }
    //    else if (direction == -1)
    //    {
    //        selectedClassID = (selectedClassID == 0) ?
    //            classes.Length-1
    //            : selectedClassID-1;
    //    }
    //    SelectClass(selectedClassID.Value);
    //}

    public void StartTraining()
    {
        SceneManager.LoadScene(trainingScenename, LoadSceneMode.Single);
    }

    public void StartGame()
    {
        if (selectedClassID != null)
        {
            //run game
            PlayerPrefs.SetString("SelectedClass", selectedClassID);

            SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        }
        else
        {
            //do message
            Debug.LogError("No player class selected!");
        }
    }

    public void OpenCharacterSelectionScreen()
    {

    }
}

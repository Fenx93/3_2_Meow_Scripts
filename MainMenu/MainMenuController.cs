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
        BattlePreparationScreenController.current.SetupClassDescriptionItems();
        BattlePreparationScreenController.current.UpdateClassButtons(classes);
        PlayerStatsTracker.SetData(1, 0, 75, 0);
        PlayerStatsTracker.UpdateUI();
    }

    public void SelectClass(int classInteger)
    {
        ScriptableCharacterClass selectedClass = classes[classInteger];
        selectedClassID = selectedClass.CharClass.ToString();
        selectedClassName.text = selectedClass.ClassName;
        CharacterCustomizer.current.avatars[0].SetWeapon(selectedClass.WeaponSprite);

        bool locked = (PlayerStatsTracker.CurrentLvl < selectedClass.UnlocksAtLevel && !unlockClasses);
        if (locked)
        {
            MainMenuUI.current.SetStartGameButton(locked, LocalisationSystem.GetLocalisedValue("character_class_locked"), selectedClass.UnlocksAtLevel);
        }
        else
        {
            MainMenuUI.current.SetStartGameButton(locked, LocalisationSystem.GetLocalisedValue("to_battle"));
        }
    }

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

using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameModes
{
    training, singlePlayer, towers, multiPlayer
}

public class MainMenuController : MonoBehaviour
{
    private int? selectedClassID = null;
    [SerializeField] private string sceneName;
    [SerializeField] private TextMeshProUGUI selectedClassName;
    [SerializeField] private string[] classNames;
    [SerializeField] private Sprite[] classWeaponSprites;

    public static MainMenuController current;

    /*[HideInInspector] public Color mainColor, secondaryColor;
    [HideInInspector] public Sprite eyes, ears, nose, mouth;*/

    [HideInInspector] public int mainColorId = 0, secondaryColorId = 0, eyesId = 0, earsId = 0, noseId = 0, mouthId = 0;

    [HideInInspector] public GameModes selectedGameMode;

    void Awake()
    {
        current = this;
    }

    void Start()
    {
        PlayerStatsTracker.SetData(1, 0, 75, 0);
        PlayerStatsTracker.UpdateUI();
        SelectClass(0);
    }

    public void SelectClass(int classID)
    {
        selectedClassID = classID;
        selectedClassName.text = classNames[classID];
        CharacterCustomizer.current.avatars[0].SetWeapon(classWeaponSprites[classID]);
    }

    public void CycleThroughClasses(int direction)
    {
        if (direction == 1)
        {
            selectedClassID = (selectedClassID == classNames.Length-1) ?
                0
                : selectedClassID+1;

        }
        else if (direction == -1)
        {
            selectedClassID = (selectedClassID == 0) ?
                classNames.Length-1
                : selectedClassID-1;
        }
        SelectClass(selectedClassID.Value);
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

    public void OpenCharacterSelectionScreen()
    {

    }
}

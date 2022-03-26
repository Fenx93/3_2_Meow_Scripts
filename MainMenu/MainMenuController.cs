using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static InventorySettings;

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

    [HideInInspector] public Dictionary<CharacterPart, string> selectedItems = new Dictionary<CharacterPart, string>();
    [HideInInspector] public GameModes selectedGameMode;
    [HideInInspector] public Dictionary<string, GameObject> idsToItems = new Dictionary<string, GameObject>();
    [SerializeField] private bool playMusicByDefault = true;

    void Awake()
    {
        current = this;
    }

    void Start()
    {
        print("Main Menu Controller Start called!");
        if(playMusicByDefault)
            AudioController.current.PlayMusic(mainMenuTheme);
        BattlePreparationScreenController.current.SetupClassDescriptionItems();
        BattlePreparationScreenController.current.UpdateClassButtons(classes);
        //PlayerStatsTracker.SetData(1, 0, 75, 0);
        //PlayerStatsTracker.SetData(10, 75*16*30, 75*16*32, 250); - trailer data

        //if (SaveGameController.Instance.SavedGameExists)
        //{
            //Debug.LogWarning("Loading Game!");
            SaveGameController.LoadData();
        //}

        SetCharacterItems();
        PlayerStatsTracker.UpdateUI();
    }

    private void SetCharacterItems()
    {
        var itemParts = new CharacterPart[] { CharacterPart.clothes, CharacterPart.hat };
        var colorParts = new CharacterPart[] { CharacterPart.mainColor, CharacterPart.secondaryColor };

        if (selectedItems.Any())
        {
            foreach (var pair in selectedItems)
            {
                var part = pair.Key;
                if (itemParts.Contains(part))
                {
                    var item = (SpriteTabItem)InventorySettings.Instance.GetItemByID(pair.Key, pair.Value);
                    InventorySettings.SelectItem(item.sprite, part, item.GetID());
                }
                else if (colorParts.Contains(part))
                {
                    var item = (ColorTabItem)InventorySettings.Instance.GetItemByID(pair.Key, pair.Value);
                    InventorySettings.SelectColor(item.color, part, item.GetID());
                }
            }
        }
        else
        {
            //get random unlocked items
            foreach (var part in itemParts)
            {
                var item = (SpriteTabItem)InventorySettings.Instance.GetRandomUnlockedItem(part);
                InventorySettings.SelectItem(item.sprite, part, item.GetID());
            }
            foreach (var part in colorParts)
            {
                var item = (ColorTabItem)InventorySettings.Instance.GetRandomUnlockedItem(part);
                InventorySettings.SelectColor(item.color, part, item.GetID());
            }
        }
    }

    public void SelectClass(int classInteger)
    {
        ScriptableCharacterClass selectedClass = classes[classInteger];
        selectedClassID = selectedClass.CharClass.ToString();
        selectedClassName.text = selectedClass.ClassName;
        CharacterCustomizer.current.avatars[0].SetWeapon(selectedClass.WeaponSprite);

        bool locked = (PlayerStatsTracker.CurrentLvl < selectedClass.UnlocksAtLevel && !unlockClasses);
        var mainMenuUI = MainMenuUI.current;
        if (mainMenuUI != null)
        {
            if (locked)
            {
                mainMenuUI.SetStartGameButton(locked, 
                    LocalisationSystem.GetLocalisedValue("character_class_locked"), selectedClass.UnlocksAtLevel);
            }
            else
            {
                mainMenuUI.SetStartGameButton(locked, LocalisationSystem.GetLocalisedValue("to_battle"));
            }
        }
    }

    public void StartTraining()
    {
        PlayerPrefs.SetInt("IsTraining", 1);
        PlayerPrefs.SetString("SelectedClass", selectedClassID);
        SaveGameController.SaveData();
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }

    public void StartGame()
    {
        if (selectedClassID != null)
        {
            //run game
            PlayerPrefs.SetInt("IsTraining", 0);
            PlayerPrefs.SetString("SelectedClass", selectedClassID);
            SaveGameController.SaveData();
            SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        }
        else
        {
            //do message
            Debug.LogError("No player class selected!");
        }
    }

    public void TestAddMoney()
    {
        PlayerStatsTracker.AddMoney(100);
    }

    public void ItemUnlocked(string id)
    {
        if (idsToItems.Any() && idsToItems.ContainsKey(id))
        {
            var itemButton = idsToItems[id];
            if (itemButton != null)
            {
                var button = itemButton.GetComponent<Button>();
                button.interactable = true;

                var image = itemButton.GetComponentsInChildren<Image>().Last();
                if (image != null)
                {
                    var color = image.color;
                    color.a = 1f;
                    image.color = color;
                }

                var itemButtonImage = itemButton.GetComponent<Image>();
                if (itemButtonImage != null)
                {
                    var tempCol = itemButtonImage.color;
                    tempCol.a = 1f;
                    itemButtonImage.color = tempCol;
                }
            }
        }
        
    }
}

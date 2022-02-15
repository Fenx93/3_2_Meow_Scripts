using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattlePreparationScreenController : MonoBehaviour
{
    public MyDictionary1 selectedActionColors;

    [SerializeField] private Transform classButtonHolder;

    [SerializeField] private GameObject[] actionButtons;

    [SerializeField] private Transform actionDescriptionPanel;

    [SerializeField]
    private TextMeshProUGUI actionNameText,
        energyConsumedText, actionDescriptionText;

    [SerializeField] private Transform classDescriptionPanel;

    private TextMeshProUGUI classDescriptionText;
    private GameObject classIconsHolder, iconDescriptionHolder, iconDescriptionHolder1;

    public static BattlePreparationScreenController current;

    private CombatAction[] _actions;

    void Awake()
    {
        current = this;
    }

    public void SetupClassDescriptionItems()
    {
        classDescriptionText = classDescriptionPanel.GetChild(0).gameObject.GetComponentInChildren<TextMeshProUGUI>();
        var classIconsHolderTransform = classDescriptionPanel.GetChild(1);
        classIconsHolder = classIconsHolderTransform.gameObject;
        iconDescriptionHolder = classIconsHolderTransform.GetChild(0).gameObject;
        iconDescriptionHolder1 = classIconsHolderTransform.GetChild(1).gameObject;
    }

    public void UpdateClassButtons(ScriptableCharacterClass[] classes)
    {
        var buttons = classButtonHolder.GetComponentsInChildren<Button>();
        if (buttons.Length != classes.Length)
            Debug.LogError("Battle Preparation screen error! There are more classes than class buttons(in classHolder).");
        for (int i = 0; i < classes.Length; i++)
        {
            //buttons[i].GetComponentInChildren<TextMeshProUGUI>().text = classes[i].ClassName;
            var classIcon = buttons[i].transform.GetChild(0).gameObject.GetComponent<Image>();
            classIcon.sprite = classes[i].ClassIcon;
            if (classes[i].UnlocksAtLevel > PlayerStatsTracker.CurrentLvl && !MainMenuController.current.unlockClasses)
            {
                var unlocksAtGameObject = buttons[i].transform.GetChild(1);
                unlocksAtGameObject.gameObject.SetActive(true);
                unlocksAtGameObject.gameObject.GetComponent<TextMeshProUGUI>().text = LocalisationSystem.GetLocalisedValue("unlocks_at_level") + " " + classes[i].UnlocksAtLevel;

                classIcon.color = Color.gray;
            }
        }
        SelectClass(0);
    }

    public void SelectClass(int classInteger)
    {
        MainMenuController.current.SelectClass(classInteger);
        AudioController.current.PlayButtonClick();
        UpdateClassDescriptions(classInteger);
        UpdateActionButtons(classInteger);
    }

    private void UpdateClassDescriptions(int classInteger)
    {
        var currentClass = MainMenuController.current.classes[classInteger];

        //get classDescription Text, assign it
        classDescriptionText.text = LocalisationSystem.GetLocalisedValue(currentClass.classDescription);

        classIconsHolder.SetActive(true);

        //get currentClass.icon1 - if not null, get IconDescriptionHolder, assign icon and text to corresponding fields
        bool firstUsed = currentClass.classSpecificIcon != null && currentClass.classSpecificIconDescription != null;
        if (firstUsed)
        {
            iconDescriptionHolder.SetActive(true);
            iconDescriptionHolder.GetComponentInChildren<Image>().sprite = currentClass.classSpecificIcon;
            iconDescriptionHolder.GetComponentInChildren<TextMeshProUGUI>().text = LocalisationSystem.GetLocalisedValue(currentClass.classSpecificIconDescription);
        }
        else
        {
            iconDescriptionHolder.SetActive(false);
        }
        bool secondUsed = currentClass.classSpecificIcon2 != null && currentClass.classSpecificIconDescription2 != null;
        //get currentClass.icon2 - if not null, get IconDescriptionHolder, assign icon and text to corresponding fields
        if (secondUsed)
        {
            iconDescriptionHolder1.SetActive(true);
            iconDescriptionHolder1.GetComponentInChildren<Image>().sprite = currentClass.classSpecificIcon2;
            iconDescriptionHolder1.GetComponentInChildren<TextMeshProUGUI>().text = LocalisationSystem.GetLocalisedValue(currentClass.classSpecificIconDescription2);
        }
        else
        {
            iconDescriptionHolder1.SetActive(false);
        }
        if (!firstUsed && !secondUsed)
        {
            classIconsHolder.SetActive(false);
        }

    }

    private void UpdateActionButtons(int classInteger)
    {
        _actions = MainMenuController.current.classes[classInteger].Actions;
        for (int i = 0; i < _actions.Length; i++)
        {
            actionButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = _actions[i].ToString();
            var energyTextUI = actionButtons[i].transform.GetChild(2).gameObject.GetComponent<TextMeshProUGUI>();
            if (_actions[i].EnergyConsumed != 0)
            {
                energyTextUI.enabled = true;
                energyTextUI.text = _actions[i].EnergyConsumed.ToString();
            }
            else
            {
                energyTextUI.enabled = false;
                energyTextUI.text = "";
            }

            var button = actionButtons[i].GetComponent<Button>();
            ColorBlock cb = button.colors;
            cb.normalColor = selectedActionColors[_actions[i].Classification];
            button.colors = cb;
        }
        SelectedAction(0);
    }

    public void SelectedAction(int actionIndex)
    {
        var action = _actions[actionIndex];
        actionNameText.text = action.ToString();
        energyConsumedText.text = LocalisationSystem.GetLocalisedValue("energy_consumed") +": " + action.EnergyConsumed;
        actionDescriptionText.text = action.Description;//LocalisationSystem.GetLocalisedValue(action.Type.ToString().ToLower() + "_description");
    }
}

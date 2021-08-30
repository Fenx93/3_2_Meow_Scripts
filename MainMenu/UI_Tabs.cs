using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Tab;

public class UI_Tabs : MonoBehaviour
{
    [SerializeField] private Color normalButtonColor, selectedButtonColor, higlightedButtonColor, disabledButtonColor;
    [SerializeField] private TMP_FontAsset font;
    [SerializeField] private Sprite tabsItemBackgroundImage, panelsItemBackgroundImage;
    [SerializeField] private Tab[] tabs;

    private readonly List<Transform> tabPanels = new List<Transform>();
    private readonly List<CharacterPart> tabParts = new List<CharacterPart>();

    private GameObject previousTab;
    private Transform tabsTransform, panelsTransform;

    private void Awake()
    {
        tabsTransform = this.transform.Find("Tabs");
        if (tabsTransform == null)
        {
            Debug.LogError("No Tabs gameobject found!");
        }

        panelsTransform = this.transform.Find("Panels");
        if (panelsTransform == null)
        {
            Debug.LogError("No Panels gameobject found!");
            // Create new tabs holder
        }

        for (int i = 0; i < tabs.Length; i++)
        {
            AddPanel(tabs[i]);
            AddTab(tabs[i].tabName, i);
        }
    }

    private void AddPanel(Tab tab)
    {
        GameObject tabPanel = new GameObject(tab.name + "Items");

        var rect = tabPanel.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.01f, 0.01f);
        rect.anchorMax = new Vector2(0.99f, 0.99f);

        var layoutGroup = tabPanel.AddComponent(typeof(VerticalLayoutGroup)) as VerticalLayoutGroup;
        layoutGroup.childAlignment = TextAnchor.UpperLeft;
        layoutGroup.childControlHeight = true;
        layoutGroup.childControlWidth = true;
        layoutGroup.childScaleHeight = true;
        layoutGroup.childScaleWidth = true;
        layoutGroup.childForceExpandHeight = true;
        layoutGroup.childForceExpandWidth = true;
        layoutGroup.spacing = 5;

        tabPanel.transform.SetParent(panelsTransform);

        tabPanels.Add(tabPanel.transform);
        tabParts.Add(tab.editedCharacterPart);

        rect = tabPanel.GetComponent<RectTransform>();
        rect.offsetMax = new Vector2(0, 0);
        rect.offsetMin = new Vector2(0, 0);

        switch (tab.editedCharacterPart)
        {
            case CharacterPart.mainColor:
            case CharacterPart.secondaryColor:
                FillItems(tab, tabPanel.transform);
                break;
            case CharacterPart.eyes:
            case CharacterPart.nose:
            case CharacterPart.mouth:
                FillItems(tab, tabPanel.transform, true);
                break;
        }

        // disable it
        tabPanel.SetActive(false);
    }

    public void AddTab(string tabname, int index)
    {
        GameObject tabButton = new GameObject(tabname+"TabButton");
        var image = tabButton.AddComponent(typeof(Image)) as Image;
        image.sprite = tabsItemBackgroundImage;
        image.type = Image.Type.Sliced;
        var button = tabButton.AddComponent(typeof(Button)) as Button;
        var c = button.colors;
        c.normalColor = normalButtonColor;
        c.disabledColor = disabledButtonColor;
        c.selectedColor = selectedButtonColor;
        c.highlightedColor = higlightedButtonColor;
        button.colors = c;
        button.onClick.AddListener(() => OpenTab(index));
        tabButton.transform.SetParent(tabsTransform);

        GameObject buttonText = new GameObject("Text");
        var text = buttonText.AddComponent(typeof(TextMeshProUGUI)) as TextMeshProUGUI;
        var rect = buttonText.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.05f, 0.05f);
        rect.anchorMax = new Vector2(0.95f, 0.95f);
        rect.SetParent(tabButton.transform);
        rect = buttonText.GetComponent<RectTransform>();
        rect.offsetMax = new Vector2(0, 0);
        rect.offsetMin = new Vector2(0, 0);

        text.font = font;
        text.text = tabname;
        text.enableAutoSizing = true;
        text.fontSizeMax = 24;
        text.fontSizeMax = 36;
        text.verticalAlignment = VerticalAlignmentOptions.Middle;
        text.alignment = TextAlignmentOptions.Midline;


        if (index == 0)
        {
            button.onClick.Invoke();
            button.Select();
        }
    }


    public void OpenTab(int index)
    {
        //get associated panel, enable it
        if (previousTab != null)
            previousTab.SetActive(false);

        previousTab = tabPanels[index].gameObject;
        previousTab.SetActive(true);

        var n = 0;
        if (MainMenuController.current != null)
        {
            switch (tabParts[index])
            {
                case CharacterPart.mainColor:
                    n = MainMenuController.current.mainColorId;
                    break;
                case CharacterPart.secondaryColor:
                    n = MainMenuController.current.secondaryColorId;
                    break;
                case CharacterPart.eyes:
                    n = MainMenuController.current.eyesId;
                    break;
                case CharacterPart.nose:
                    n = MainMenuController.current.noseId;
                    break;
                case CharacterPart.mouth:
                    n = MainMenuController.current.mouthId;
                    break;
                default:
                    break;
            }
        }
        

        var buttons = previousTab.GetComponentsInChildren<Button>();
        buttons[n].Select();
        //go through n of items, then select the n-th item
    }

    //
    // Ex UI_tab_button
    //

    private GameObject CreatePanel(int i, Transform parent)
    {
        GameObject obj = new GameObject("Panel (" + i + ")");
        var horizontalLayoutGroup = obj.AddComponent(typeof(HorizontalLayoutGroup)) as HorizontalLayoutGroup;
        horizontalLayoutGroup.childControlHeight = true;
        horizontalLayoutGroup.childControlWidth = true;
        horizontalLayoutGroup.childScaleHeight = true;
        horizontalLayoutGroup.childScaleWidth = true;
        horizontalLayoutGroup.spacing = 5;
        obj.transform.SetParent(parent);
        return obj;
    }

    private GameObject CreateItemButton(GameObject obj, int i)
    {
        GameObject itemButton = new GameObject("ItemButton (" + i + ")");
        var image = itemButton.AddComponent(typeof(Image)) as Image;
        image.sprite = panelsItemBackgroundImage;
        image.type = Image.Type.Sliced;
        var button = itemButton.AddComponent(typeof(Button)) as Button;
        var c = button.colors;
        c.normalColor = normalButtonColor;
        c.disabledColor = disabledButtonColor;
        c.selectedColor = selectedButtonColor;
        c.highlightedColor = higlightedButtonColor;
        button.colors = c;
        itemButton.transform.SetParent(obj.transform);
        return itemButton;
    }

    // useSprites sets either items or sprites
    private void FillItems(Tab tab, Transform parent, bool useSprites = false)
    {
        GameObject[] newBackObj = new GameObject[4];
        for (int i = 0; i < 4; i++)
        {
            newBackObj[i] = CreatePanel(i, parent);
        }

        for (int i = 0; i < newBackObj.Length; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                GameObject itemButton = CreateItemButton(newBackObj[i], j);
                var button = itemButton.GetComponent<Button>();

                GameObject item = new GameObject(useSprites ? "Image Item" : "Color Item");
                var image = item.AddComponent(typeof(Image)) as Image;
                var rect = item.GetComponent<RectTransform>();
                rect.anchorMin = new Vector2(0.1f, 0.1f);
                rect.anchorMax = new Vector2(0.9f, 0.9f);

                var index = i * 5 + j;

                bool isEmpty = false;
                if (useSprites)
                {
                    if (tab.sprites.Length > index)
                    {
                        image.sprite = tab.sprites[index];
                        isEmpty = true;
                        button.onClick.AddListener(() => ItemSelected(tab.sprites[index], tab.editedCharacterPart, index));
                    }
                }
                else
                {
                    if (tab.colors.Length > index)
                    {
                        image.sprite = panelsItemBackgroundImage;
                        image.type = Image.Type.Sliced;
                        image.color = tab.colors[index];
                        isEmpty = true;
                        button.onClick.AddListener(() => ItemSelected(tab.colors[index], tab.editedCharacterPart, index));
                    }
                }
                if (!isEmpty)
                {
                    button.interactable = false;
                    image.enabled = false;
                }

                rect.SetParent(itemButton.transform);
                rect.offsetMax = new Vector2(0, 0);
                rect.offsetMin = new Vector2(0, 0);
            }
        }

    }

    private void ItemSelected(Color color, CharacterPart part, int index)
    {
        CharacterCustomizer.current.avatars[0].SetColor(color, part);
        switch (part)
        {
            case CharacterPart.mainColor:
                MainMenuController.current.mainColor = color;
                MainMenuController.current.mainColorId = index;
                break;
            case CharacterPart.secondaryColor:
                MainMenuController.current.secondaryColor = color;
                MainMenuController.current.secondaryColorId = index;
                break;
            default:
                break;
        }
    }

    private void ItemSelected(Sprite sprite, CharacterPart part, int index)
    {
        CharacterCustomizer.current.avatars[0].SetSprite(sprite, part);

        switch (part)
        {
            case CharacterPart.eyes:
                MainMenuController.current.eyes = sprite;
                MainMenuController.current.eyesId = index;
                break;
            case CharacterPart.nose:
                MainMenuController.current.nose = sprite;
                MainMenuController.current.noseId = index;
                break;
            case CharacterPart.mouth:
                MainMenuController.current.mouth = sprite;
                MainMenuController.current.mouthId = index;
                break;
            default:
                break;
        }
    }

}

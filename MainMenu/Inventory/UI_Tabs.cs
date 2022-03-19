using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static InventorySettings;

public class UI_Tabs : MonoBehaviour
{
    [Header("Button Colors")]
    [SerializeField] private Color normalButtonColor;
    [SerializeField] private Color selectedButtonColor, higlightedButtonColor, disabledButtonColor;

    [Header("Font Properties (if autoSize is disabled, fontSizemin is used as default size")]
    [SerializeField] private TMP_FontAsset font;
    [SerializeField] private bool enableAutoSize;
    [SerializeField] private int fontSizeMin, fontSizeMax;
    [SerializeField] private FontStyles fontStyle;
    [SerializeField] private Sprite tabsItemBackgroundImage, panelsItemBackgroundImage;

    [Header("Inventory")]
    [SerializeField] private GameObject paging;

    [SerializeField] private int rows = 4;
    [SerializeField] private int columns = 5;
    [SerializeField] private bool unlockAllItems = false;
    private readonly List<InventoryTab> tabPanels = new List<InventoryTab>();

    private GameObject currentPanel, currentPage;
    private Button currentButton;
    private int currentIndex, currentPageIndex = 0;
    private Transform tabsTransform, panelsTransform;

    private void Awake()
    {
        tabsTransform = transform.Find("Tabs");
        if (tabsTransform == null)
        {
            Debug.LogError("No Tabs gameobject found!");
        }

        panelsTransform = transform.Find("Item Panels");
        if (panelsTransform == null)
        {
            Debug.LogError("No Panels gameobject found!");
            // Create new tabs holder
        }

    }

    private void Start()
    {
        for (int i = 0; i < Tabs.Length; i++)
        {
            AddPanel(Tabs[i]);
            AddTab(Tabs[i].tabName, i);
        }
        currentButton.onClick.Invoke();
    }

    private void AddPanel(Tab tab)
    {
        GameObject tabPanel = new GameObject(tab.name + "Items");

        var rect = tabPanel.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.01f, 0.01f);
        rect.anchorMax = new Vector2(0.99f, 0.99f);

        tabPanel.transform.SetParent(panelsTransform);

        rect = tabPanel.GetComponent<RectTransform>();
        rect.offsetMax = new Vector2(0, 0);
        rect.offsetMin = new Vector2(0, 0);

        switch (tab.editedCharacterPart)
        {
            case CharacterPart.mainColor:
            case CharacterPart.secondaryColor:
                FillItems(tab, tabPanel.transform);
                break;
            case CharacterPart.hat:
            case CharacterPart.clothes:
                FillItems(tab, tabPanel.transform, true);
                break;
        }

        // disable it
        tabPanel.SetActive(false);
    }

    private void AddTab(string tabname, int index)
    {
        //add tab gameobject with selected background image
        GameObject tabButton = new GameObject(tabname+" TabButton");
        var image = tabButton.AddComponent(typeof(Image)) as Image;
        image.sprite = tabsItemBackgroundImage;
        image.type = Image.Type.Sliced;

        //add button to a tab
        var button = tabButton.AddComponent(typeof(Button)) as Button;
        var c = button.colors;
        c.normalColor = normalButtonColor;
        // in order to keep tabs look like "selected" when clicking on something else
        c.disabledColor = selectedButtonColor;
        c.selectedColor = selectedButtonColor;
        c.highlightedColor = higlightedButtonColor;
        button.colors = c;
        button.onClick.AddListener(() => OpenTab(button, index));
        tabButton.transform.SetParent(tabsTransform);

        // Set tab text
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
        text.text = LocalisationSystem.GetLocalisedValue(tabname.ToLower());
        text.fontStyle = fontStyle;
        text.enableAutoSizing = enableAutoSize;
        text.lineSpacing = 30;

        if (enableAutoSize)
        {
            text.fontSizeMax = fontSizeMax;
            text.fontSizeMin = fontSizeMin;
        }
        else
        {
            text.fontSize = fontSizeMin;
        }
        text.verticalAlignment = VerticalAlignmentOptions.Middle;
        text.alignment = TextAlignmentOptions.Midline;

        //Manually select first tab
        if (index == 0)
            currentButton = button;
    }

    public void OpenTab(Button button, int index)
    {
        AudioController.current.PlayButtonClick();

        // Disable previous panel and it's button
        if (currentPanel != null)
            currentPanel.SetActive(false);
        if (currentButton != null)
            currentButton.interactable = true;

        currentIndex = index;
        // Get associated panel, enable it
        var panel = tabPanels[index];
        currentPanel = panel.transform.gameObject;
        currentPanel.SetActive(true);

        button.interactable = false;
        currentButton = button;

        //enable paging based on number of pages
        if (panel.pages != null && panel.pages.Length > 0)
        {
            currentPage = tabPanels[currentIndex].pages[0];
            currentPageIndex = 0;
            bool showPages = panel.pages.Length > 1;
            paging.SetActive(showPages);
            if (showPages)
            {
                UpdatePagingText(panel.pages.Length);
            }
        }

        //int n = 0;
        //if (MainMenuController.current != null)
        //{
        //    // TO-DO: replace these hardcoded values
        //    switch (panel.part)
        //    {
        //        case CharacterPart.mainColor:
        //            n = MainMenuController.current.idsToItems;
        //            break;
        //        case CharacterPart.secondaryColor:
        //            n = MainMenuController.current.secondaryColorId;
        //            break;
        //        case CharacterPart.hat:
        //            n = MainMenuController.current.hatId;
        //            break;
        //        case CharacterPart.clothes:
        //            n = MainMenuController.current.clothesId;
        //            break;
        //        default:
        //            break;
        //    }
        //}

        ////go through n of items, then select the n-th item
        //currentPanel.GetComponentsInChildren<Button>()[n].Select();
    }

    // useSprites sets either items or sprites
    private void FillItems(Tab tab, Transform parent, bool useSprites = false)
    {
        // Sort items by quality 

        int[] map = new[] { (int)ItemQuality.common, (int)ItemQuality.rare, (int)ItemQuality.epic, (int)ItemQuality.legendary };

        var sortedArray = tab.items
          .OrderBy(x => map[(int)(x.quality)])
          .ToArray();

        tab.items = sortedArray;
        // Create Item pages with count = tab.items / (rows * columns)
        float calc = tab.items.Length / (float)(rows * columns);
        int calculatedPageCount = Convert.ToInt32(Math.Ceiling(calc));

        GameObject[] pages = new GameObject[calculatedPageCount];

        for (int i = 0; i < calculatedPageCount; i++)
        {
            GameObject page = new GameObject($"Items Page ({i+1})");
            var rect = page.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.01f, 0.01f);
            rect.anchorMax = new Vector2(0.99f, 0.99f);

            var layoutGroup = page.AddComponent(typeof(VerticalLayoutGroup)) as VerticalLayoutGroup;
            layoutGroup.childAlignment = TextAnchor.UpperLeft;
            layoutGroup.childControlHeight = true;
            layoutGroup.childControlWidth = true;
            layoutGroup.childScaleHeight = true;
            layoutGroup.childScaleWidth = true;
            layoutGroup.childForceExpandHeight = true;
            layoutGroup.childForceExpandWidth = true;
            layoutGroup.spacing = 5;

            page.transform.SetParent(parent);

            rect = page.GetComponent<RectTransform>();
            rect.offsetMax = new Vector2(0, 0);
            rect.offsetMin = new Vector2(0, 0);

            FillPagesWithItems(tab, page.transform, i, useSprites);
            pages[i] = page;
            if (i != 0)
            {
                page.SetActive(false);
            }
        }

        tabPanels.Add(new InventoryTab(parent, tab.editedCharacterPart, pages));
    }

    private void FillPagesWithItems(Tab tab, Transform parent, int pageIndex, bool useSprites = false)
    {
        GameObject[] rowsObject = new GameObject[rows];
        for (int i = 0; i < rows; i++)
        {
            GameObject obj = new GameObject($"Items Row ({i})");
            var horizontalLayoutGroup = obj.AddComponent(typeof(HorizontalLayoutGroup)) as HorizontalLayoutGroup;
            horizontalLayoutGroup.childControlHeight = true;
            horizontalLayoutGroup.childControlWidth = true;
            horizontalLayoutGroup.childScaleHeight = true;
            horizontalLayoutGroup.childScaleWidth = true;
            horizontalLayoutGroup.spacing = 5;
            obj.transform.SetParent(parent);
            rowsObject[i] = obj;
        }

        for (int i = 0; i < rowsObject.Length; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                GameObject itemButton = CreateItemButton(rowsObject[i], j);
                var button = itemButton.GetComponent<Button>();

                GameObject item = new GameObject(useSprites ? "Image Item" : "Color Item");
                var image = item.AddComponent(typeof(Image)) as Image;
                var rect = item.GetComponent<RectTransform>();
                rect.anchorMin = new Vector2(0.2f, 0.1f);
                rect.anchorMax = new Vector2(0.8f, 0.9f);

                // current row * rows + current column + (rows * columns * current page)
                int index = (i * rows) + j + (rowsObject.Length * columns * pageIndex);

                var itemButtonImage = itemButton.GetComponent<Image>();

                bool isEmpty = true;
                if (useSprites)
                {
                    if (tab.items != null && tab.items.Length > index)
                    {
                        var tabItem = (SpriteTabItem) tab.items[index];
                        image.sprite = tabItem.itemIcon;
                        itemButtonImage.color = InventorySettings.itemQualities[tabItem.quality];

                        isEmpty = false;

                        if (!unlockAllItems && tabItem.status == ItemStatus.locked)
                        {
                            var color = image.color;
                            color.a = 0.25f;
                            image.color = color;

                            var tempCol = itemButtonImage.color;
                            tempCol.a = 0.25f;
                            itemButtonImage.color = tempCol;

                            button.interactable = false;
                        }
                        button.onClick.AddListener(() => ItemSelected(tabItem.sprite, tab.editedCharacterPart, tabItem.GetID()));
                        
                        if (tabItem.GetID() != "empty")
                        {
                            MainMenuController.current.idsToItems.Add(tabItem.GetID(), itemButton);
                        }
                    }
                }
                else
                {
                    if (tab.items != null && tab.items.Length > index)
                    {
                        var tabItem = (ColorTabItem) tab.items[index];
                        image.sprite = panelsItemBackgroundImage;
                        image.type = Image.Type.Sliced;
                        image.color = tabItem.color;
                        itemButtonImage.color = InventorySettings.itemQualities[tabItem.quality];

                        isEmpty = false;
                        if (!unlockAllItems && tabItem.status == ItemStatus.locked)
                        {
                            var color = image.color;
                            color.a = 0.25f;
                            image.color = color;

                            var tempCol = itemButtonImage.color;
                            tempCol.a = 0.25f;
                            itemButtonImage.color = tempCol;

                            button.interactable = false;
                        }
                        button.onClick.AddListener(() => ItemSelected(tabItem.color, tab.editedCharacterPart, tabItem.GetID()));
                        if (tabItem.GetID() != "empty")
                        {
                            MainMenuController.current.idsToItems.Add(tabItem.GetID(), itemButton);
                        }
                    }
                }

                if (isEmpty)
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

    private GameObject CreateItemButton(GameObject obj, int i)
    {
        GameObject itemButton = new GameObject($"ItemButton ({i})");
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

    private void ItemSelected(Color color, CharacterPart part, string id)
    {
        PlayItemClickSound();
        CharacterCustomizer.current.avatars[0].SetColor(color, part);
        CharacterCustomizer.current.avatars[1].SetColor(color, part);
        MainMenuController.current.selectedItems[part] = id;
        switch (part)
        {
            case CharacterPart.mainColor:
                CharacterStore.mainColor = color;
                break;
            case CharacterPart.secondaryColor:
                CharacterStore.secondaryColor = color;
                break;
            default:
                break;
        }
    }

    private void ItemSelected(Sprite sprite, CharacterPart part, string id)
    {
        PlayItemClickSound();
        CharacterCustomizer.current.avatars[0].SetSprite(sprite, part);
        CharacterCustomizer.current.avatars[1].SetSprite(sprite, part);
        MainMenuController.current.selectedItems[part] = id;
        switch (part)
        {
            case CharacterPart.hat:
                CharacterStore.hat = sprite;
                break;
            case CharacterPart.clothes:
                CharacterStore.clothes = sprite;
                break;
            default:
                break;
        }
    }
    private void PlayItemClickSound()
    {
        AudioController.current.PlayButtonClick();
    }

    #region Paging

    private void UpdatePagingText(int totalPageNumber)
    {
        paging.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = $"{currentPageIndex + 1}/{totalPageNumber}";
    }

    private void OpenPage(int pageIndex)
    {
        currentPage.SetActive(false);
        currentPage = tabPanels[currentIndex].pages[pageIndex];
        currentPage.SetActive(true);
    }

    public void GoToPage(int pageDirection)
    {
        var pageLength = tabPanels[currentIndex].pages.Length;
        if (currentPageIndex == 0 && pageDirection == -1)
        {
            currentPageIndex = pageLength-1;
        }
        else if (currentPageIndex == pageLength-1 && pageDirection == 1)
        {
            currentPageIndex = 0;
        }
        else
        {
            currentPageIndex += pageDirection;
        }

        OpenPage(currentPageIndex);
        UpdatePagingText(pageLength);
    }

    #endregion
}

public struct InventoryTab {

    public Transform transform;
    public CharacterPart part;
    public GameObject[] pages;

    public InventoryTab(Transform transform, CharacterPart part, GameObject[] pages)
    {
        this.transform = transform;
        this.part = part;
        this.pages = pages;
    }
}

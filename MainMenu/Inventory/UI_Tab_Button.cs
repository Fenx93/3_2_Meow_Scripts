using UnityEngine;
using UnityEngine.UI;
using static InventorySettings;

public class UI_Tab_Button : MonoBehaviour
{
    public Tab _tab;
    
    private Color selectedColor;
    private Sprite selectedSprite;

    private void Awake()
    {
        switch (_tab.editedCharacterPart)
        {
            case CharacterPart.mainColor:
            case CharacterPart.secondaryColor:
                FillItems();
                break;
            case CharacterPart.eyes:
            case CharacterPart.nose:
            case CharacterPart.mouth:
                FillItems(true);
                break;
        }
    }

    private GameObject CreatePanel(int i)
    {
        GameObject obj = new GameObject("Panel (" + i + ")");
        var horizontalLayoutGroup = obj.AddComponent(typeof(HorizontalLayoutGroup)) as HorizontalLayoutGroup;
        horizontalLayoutGroup.childControlHeight = true;
        horizontalLayoutGroup.childControlWidth = true;
        horizontalLayoutGroup.childScaleHeight = true;
        horizontalLayoutGroup.childScaleWidth = true;
        horizontalLayoutGroup.spacing = 5;
        obj.transform.SetParent(this.transform);
        return obj;
    }

    private GameObject CreateItemButton(GameObject obj, int i)
    {
        GameObject itemButton = new GameObject("ItemButton (" + i + ")", typeof(Image));
        var button = itemButton.AddComponent(typeof(Button)) as Button;
        var c = button.colors;
        c.disabledColor = Color.gray;
        c.selectedColor = Color.blue;
        c.highlightedColor = Color.cyan;
        button.colors = c;
        itemButton.transform.SetParent(obj.transform);
        return itemButton;
    }

    // useSprites sets either items or sprites
    private void FillItems(bool useSprites = false)
    {
        GameObject[] newBackObj = new GameObject[4];
        for (int i = 0; i < 4; i++)
        {
            newBackObj[i] = CreatePanel(i);
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

                var itemButtonImage = itemButton.GetComponent<Image>();

                bool isEmpty = true;
                if (useSprites)
                {
                    if (_tab.items.Length > index)
                    {
                        var tabItem = (SpriteTabItem) _tab.items[index];
                        image.sprite = tabItem.sprite;
                        itemButtonImage.color = InventorySettings.itemQualities[tabItem.quality];

                        isEmpty = false;

                        if (tabItem.status == ItemStatus.locked)
                        {
                            var color = image.color;
                            color.a = 0.25f;
                            image.color = color;

                            var tempCol = itemButtonImage.color;
                            tempCol.a = 0.25f;
                            itemButtonImage.color = tempCol;

                            button.interactable = false;
                        }

                        button.onClick.AddListener(() => ItemSelected(tabItem.sprite));
                    }
                }
                else
                {
                    if (_tab.items.Length > index)
                    {
                        var tabItem = (ColorTabItem)_tab.items[index];
                        image.color = tabItem.color;
                        itemButtonImage.color = InventorySettings.itemQualities[tabItem.quality];

                        isEmpty = false;

                        if (tabItem.status == ItemStatus.locked)
                        {
                            var color = tabItem.color;
                            color.a = 0.25f;
                            image.color = color;

                            var tempCol = itemButtonImage.color;
                            tempCol.a = 0.25f;
                            itemButtonImage.color = tempCol;

                            button.interactable = false;
                        }

                        button.onClick.AddListener(() => ItemSelected(tabItem.color));
                    }
                }
                if (isEmpty)
                {
                    button.interactable = false;
                    image.enabled = false;
                }


                rect.SetParent(itemButton.transform);
            }
        }

    }

    private void ItemSelected(Color color)
    {
        CharacterCustomizer.current.avatars[0].SetColor(color, _tab.editedCharacterPart);
        switch (_tab.editedCharacterPart)
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
    
    private void ItemSelected(Sprite sprite)
    {
        CharacterCustomizer.current.avatars[0].SetSprite(sprite, _tab.editedCharacterPart);

        switch (_tab.editedCharacterPart)
        {
            case CharacterPart.eyes:
                CharacterStore.eyes = sprite;
                break;
            case CharacterPart.nose:
                CharacterStore.nose = sprite;
                break;
            case CharacterPart.mouth:
                CharacterStore.mouth = sprite;
                break;
            default:
                break;
        }
    }
}

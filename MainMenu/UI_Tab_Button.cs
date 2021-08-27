using UnityEngine;
using UnityEngine.UI;

public class UI_Tab_Button : MonoBehaviour
{
    public enum CharacterPart { mainColor, secondaryColor, eyes, nose, mouth }
    [SerializeField] private CharacterPart editedCharacterPart;

    [SerializeField] private Color[] colors;
    [SerializeField] private Sprite[] sprites;

    private Color selectedColor;
    private Sprite selectedSprite;

    private void Awake()
    {
        switch (editedCharacterPart)
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

    private void AddTab(string TabName)
    {

    }

    private GameObject CreatePanel(GameObject obj, int i)
    {
        obj = new GameObject("Panel (" + i + ")");
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
            newBackObj[i] = CreatePanel(newBackObj[i], i);
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

                if (useSprites)
                {
                    if (sprites.Length > index)
                    {
                        image.sprite = sprites[index];
                        button.onClick.AddListener(() => ItemSelected(sprites[index]));
                    }
                    else
                    {
                        button.interactable = false;
                        image.enabled = false;
                    }
                }
                else
                {
                    if (colors.Length > index)
                    {
                        image.color = colors[index];
                        button.onClick.AddListener(() => ItemSelected(colors[index]));
                    }
                    else
                    {
                        button.interactable = false;
                        image.enabled = false;
                    }
                }


                rect.SetParent(itemButton.transform);
            }
        }

    }

    private void ItemSelected(Color color)
    {
        CharacterCustomizer.current.avatars[0].SetColor(color, editedCharacterPart);
        switch (editedCharacterPart)
        {
            case CharacterPart.mainColor:
                MainMenuController.current.mainColor = color;
                break;
            case CharacterPart.secondaryColor:
                MainMenuController.current.secondaryColor = color;
                break;
            default:
                break;
        }
    }
    
    private void ItemSelected(Sprite sprite)
    {
        CharacterCustomizer.current.avatars[0].SetSprite(sprite, editedCharacterPart);

        switch (editedCharacterPart)
        {
            case CharacterPart.eyes:
                MainMenuController.current.eyes = sprite;
                break;
            case CharacterPart.nose:
                MainMenuController.current.nose = sprite;
                break;
            case CharacterPart.mouth:
                MainMenuController.current.mouth = sprite;
                break;
            default:
                break;
        }
    }
}
